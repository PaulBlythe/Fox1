using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Helpers;
using GuruEngine.AI.Aircraft.Gunners;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.Weapons.Bullets;
using GuruEngine.ECS.Components.Effects;
using GuruEngine.ECS.Components.Mesh;
using GuruEngine.DebugHelpers;
using GuruEngine.AI;
using GuruEngine.Audio;
using GuruEngine.Physics.Collision;
using GuruEngine.Rendering;

//( Class TurretComponent )
//( Group Aircraft )
//( Type TurretComponent )
//( Connection BulletPropertiesComponent Bullets )
//( Connection ParticleEmitterComponent Emitter )
//( Parameter Float MinYaw )
//( Parameter Float MaxYaw )
//( Parameter Float MinPitch )
//( Parameter Float MaxPitch )
//( Parameter String Hook )
//( Parameter Int Rounds )
//( Parameter Float AimRange )
//( Parameter Float ROF )
//( Parameter String Animate )
//( Parameter String Sound )

namespace GuruEngine.ECS.Components.AircraftSystems.General
{
    public enum TurretState
    {
        Unmanned,
        Scanning,
        Tracking,
        Shooting,
        Destroyed,
        OutOfAmmo
    }

    public class TurretComponent : ECSGameComponent
    {
        public float MinYaw;
        public float MaxYaw;
        public float MinPitch;
        public float MaxPitch;
        public float AimRange;
        public float ROF;
        public string Hook;
        public string Animate;
        public string Sound;
        public int Rounds;
        public BulletPropertiesComponent bulletproperties;
        public ParticleEmitterComponent emitter;

        MultiMeshComponent meshhost;
        MultiMeshComponent meshAnimate;
        BoundingFrustum viewFrustrum;
        WorldTransform baseTransform;

        float pitch = 0;
        float yaw = 0;
        float firetime = 0;
        float firerate = 0;
        int trackedTarget = -1;
        int sound_id = 0;

        TurretState state = TurretState.Scanning;

        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            TurretComponent other = new TurretComponent();
            other.MinYaw = MinYaw;
            other.MaxYaw = MaxYaw;
            other.MinPitch = MinPitch;
            other.MaxPitch = MaxPitch;
            other.Hook = Hook;
            other.Rounds = Rounds;
            other.AimRange = AimRange;
            other.bulletproperties = bulletproperties;
            other.ROF = ROF;
            other.Animate = Animate;
            other.emitter = emitter;
            other.Sound = Sound;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {

            }
            else
            {
                string[] objects = parts[2].Split(':');
                switch (parts[0])
                {
                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    case "Bullets":
                        {
                            bulletproperties = (BulletPropertiesComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    case "Emitter":
                        {
                            emitter = (ParticleEmitterComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::TurretComponent:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            Parent = null;
            bulletproperties = null;
            UpdateStage = 99;
            emitter = null;
        }

        public override object GetContainedObject(string type)
        {
            if (type == "Bullets")
                return bulletproperties;

            return null;
        }

        public override Texture2D GetOffscreenTexture()
        {
            return null;
        }

        public override void HandleEvent(string evt)
        {
        }

        public override void Load(ContentManager content)
        {
            firerate = 1.0f / (ROF / 60.0f);
            UpdateStage = 3;

            baseTransform = (WorldTransform) Parent.FindGameComponentByName("WorldTransform_1");
            sound_id = AudioManager.AddSoundEffect(Sound);

        }

        public override void ReConnect(GameObject pother)
        {
            TurretComponent other = (TurretComponent)pother.FindGameComponentByName(Name);
            other.MinYaw = MinYaw;
            other.MaxYaw = MaxYaw;
            other.MinPitch = MinPitch;
            other.MaxPitch = MaxPitch;
            other.Hook = Hook;
            other.Rounds = Rounds;
            other.AimRange = AimRange;
            other.ROF = ROF;
            other.Animate = Animate;
            other.bulletproperties = (BulletPropertiesComponent)pother.FindGameComponentByName(bulletproperties.Name);
            other.emitter = (ParticleEmitterComponent)pother.FindGameComponentByName(emitter.Name);
            other.Parent = pother;
            other.Sound = Sound;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Sound":
                    Sound = Value;
                    break;
                case "Animate":
                    Animate = Value;
                    break;
                case "ROF":
                    ROF = float.Parse(Value);
                    break;
                case "AimRange":
                    AimRange = float.Parse(Value);
                    break;
                case "MinYaw":
                    MinYaw = float.Parse(Value);
                    break;
                case "MaxYaw":
                    MaxYaw = float.Parse(Value);
                    break;
                case "MinPitch":
                    MinPitch = float.Parse(Value);
                    break;
                case "MaxPitch":
                    MaxPitch = float.Parse(Value);
                    break;
                case "Hook":
                    Hook = Value;
                    break;
                case "Rounds":
                    Rounds = int.Parse(Value);
                    break; 
            }
        }

        public override void Update(float dt)
        {
            if (meshhost == null)
            {
                meshAnimate = (MultiMeshComponent)Parent.FindGameComponentByName(Animate);

                meshhost = Parent.FindHookOwner(Hook);
                Matrix m = meshhost.GetOriginalHookMatrix(Hook);
                Vector3 p1 = m.Translation;
                Vector3 p2 = m.Translation + (1000.0f * m.Forward);

                float x_range = MathHelper.ToRadians(MaxYaw - MinYaw);
                float y_range = MathHelper.ToRadians(MaxPitch - MinPitch);
                float aspect_ratio = x_range / y_range;

                Matrix p = Matrix.CreatePerspectiveFieldOfView(x_range, aspect_ratio, 5, 1000);
                Matrix l = Matrix.CreateLookAt(p1, p2, Vector3.Up);
                viewFrustrum = new BoundingFrustum(l * p);
            }
            firetime -= dt;

            switch (state)
            {
                case TurretState.Scanning:
                    {
                        if (TargetManager.AreAnyTargetsNearby(Parent.IFF, 1, Parent.GetWorldPosition(), 1000))
                        {
                            List<int> targets = TargetManager.GetNearbyTargets(Parent.IFF, 1, Parent.GetWorldPosition());
                            foreach (int t in targets)
                            {
                                AITarget ait = TargetManager.GetAirborneTargetDetails(t);
                                Matrix m = meshhost.GetHookMatrix(Hook);
                                Vector3 targetpos = Vector3.Transform(ait.Position, Matrix.Invert(m));
                                if (viewFrustrum.Contains(targetpos) != ContainmentType.Disjoint)
                                {
                                    state = TurretState.Tracking;
                                    trackedTarget = ait.TargetID;
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case TurretState.Tracking:
                    {
                        Matrix m = meshhost.GetHookMatrix(Hook);
                        AITarget target = TargetManager.GetAirborneTargetDetails(trackedTarget);
                        float distance2 = (target.Position - m.Translation).LengthSquared();
                        if (distance2 > AimRange * AimRange)
                        {
                            state = TurretState.Scanning;
                        }
                        else
                        {
                            Matrix ww = Matrix.Invert(m);
                            Vector3 targetpos = Vector3.Transform(target.Position, ww);
                            if (viewFrustrum.Contains(targetpos) != ContainmentType.Disjoint)
                            {
                                Vector3 aimpoint;
                                Vector3 aimdirection = AerialGunnery.GetAimDirection(target, Vector3.Zero, bulletproperties.speed, dt, ww, out aimpoint);

                                float target_yaw = (float)Math.Atan(aimdirection.X / aimdirection.Z);
                                float target_pitch = (float)Math.Atan(aimdirection.Z / aimdirection.Z);
                                target_yaw = MathHelper.ToDegrees(target_yaw);
                                target_pitch = MathHelper.ToDegrees(target_pitch);

                                bool shoot = true;
                                if (Math.Abs(target_pitch - pitch) < 1)
                                {
                                    pitch = target_pitch;
                                }
                                else
                                {
                                    if (pitch > target_pitch)
                                        pitch -= 1;
                                    else
                                        pitch += 1;

                                    shoot = false;
                                }
                                if (Math.Abs(target_yaw - yaw) < 1)
                                {
                                    yaw = target_yaw;
                                }
                                else
                                {
                                    if (yaw > target_yaw)
                                        yaw -= 1;
                                    else
                                        yaw += 1;

                                    shoot = false;
                                }
                                if (shoot)
                                {
                                    if (firetime<=0)
                                    {
#if DEBUG
                                        if (DebugRenderSettings.RenderTurrets)
                                        {
                                            DebugLineDraw.DrawLine(m.Translation, target.Position, Color.White);
                                        }
#endif
                                        emitter.AddParticle(m.Translation, Vector3.Zero);
                                        AudioManager.PlayDynamicEffectOnceAt(sound_id, m.Translation);
                                        firetime = firerate;

                                        BulletRecord br = new BulletRecord();
                                        br.Position = m.Translation;
                                        Vector3 v1 = (aimpoint - m.Translation);
                                        v1.Normalize();
                                        br.Velocity = (baseTransform.Velocity + bulletproperties.speed) * v1; 
                                        br.MaxAge = bulletproperties.timeLife;
                                        br.Kaliber = bulletproperties.kaliber;
                                        br.Mass = bulletproperties.mass;
                                        br.IsTracer = (Rounds & 3) == 0;
                                        Rounds--;
                                        BulletManager.Instance.AddBullet(br);

                                        Renderer.AddPointLight(m.Translation, Color.Orange, 5.0f, 2.0f);

                                    }
                                    
                                }

                            }
                            else
                            {
                                state = TurretState.Scanning;
                            }

                            // Z == yaw
                            // Y == pitch
                            Matrix animation = Matrix.CreateRotationZ(MathHelper.ToRadians(yaw));
                            meshAnimate.MatrixAnimate(animation);
                        }
                    }
                    break;
            }

#if DEBUG
            if (DebugRenderSettings.RenderTurrets)
            {
                meshhost = Parent.FindHookOwner(Hook);

                if (meshhost != null)
                {
                    Matrix m = meshhost.GetHookMatrix(Hook);
                    Vector3 p1 = m.Translation;
                    Vector3 p2 = m.Translation + (5.0f * m.Down);

                    if (state== TurretState.Scanning)
                        DebugLineDraw.DrawLine(p1, p2, Color.Yellow);
                    else
                        DebugLineDraw.DrawLine(p1, p2, Color.Purple);

                    DebugLineDraw.DrawFrustrum(viewFrustrum, m);
                }
            }
#endif
        }

        #endregion

    }
}
