using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.AI.Artillery;
using GuruEngine.Simulation.Weapons.AAA;
using GuruEngine.Audio;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.Effects;
using GuruEngine.ECS.Components.Mesh;

//( Class ArtilleryGunnerComponent )
//( Group Artillery )
//( Type ArtilleryGunnerComponent )
//( Connection ParticleEmitterComponent Emitter )
//( Parameter Float YawRate )
//( Parameter Float PitchRate )
//( Parameter Int Crew )
//( Parameter String Hook )

/////////////////////////////////////
//        Mode mapping table       // 
/////////////////////////////////////
// 0    Dormant                    //
// 1    Area supression            //
// 2    Visual aiming              //
/////////////////////////////////////

namespace GuruEngine.ECS.Components.Artillery
{
    public class ArtilleryGunnerComponent : ECSGameComponent
    {
        float YawRate;
        float PitchRate;
        int Crew;
        float time = 0;
        int mode = 0;
        float target_altitude;
        float target_bearing;
        float target_range;
        float target_pitch;
        float fuse_time;
        String Hook;

        float current_pitch = 0;
        float current_yaw = 0;
        WeaponArtillery round = null;

        public ArtilleryComponent gun;
        public ParticleEmitterComponent emitter;
        MultiMeshComponent meshhost;
        Vector3 gun_position;
        Random rand = new Random();

        public ArtilleryGunnerComponent()
        {
            UpdateStage = 4;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            ArtilleryGunnerComponent other = new ArtilleryGunnerComponent();
            other.YawRate = YawRate;
            other.PitchRate = PitchRate;
            other.Crew = Crew;
            other.Hook = Hook;
            other.emitter = emitter;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[0])
                {

                    default:
                        throw new Exception("GameComponent::FlakGunnerComponent:: Unknown list connection request to " + parts[0]);
                }
            }
            else
            {
                string[] objects = parts[2].Split(':');
                switch (parts[0])
                {
                    case "Emitter":
                        {
                            emitter = (ParticleEmitterComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;

                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    default:
                        //throw new Exception("GameComponent::FuelTank:: Unknown direct connection request to " + parts[0]);
                        break;
                }
            }
        }



        public override void DisConnect()
        {
            gun = null;
            round = null;
            emitter = null;
            meshhost = null;
        }

        public override object GetContainedObject(string type)
        {
            return null;
        }

        public override Texture2D GetOffscreenTexture()
        {
            return null;
        }

        public override void HandleEvent(string evt)
        {
            string[] parts = evt.Split(':');
            switch (parts[0])
            {
                case "AttackArea":
                    {
                        target_bearing = float.Parse(parts[1]);
                        target_altitude = float.Parse(parts[2]);
                        target_range = float.Parse(parts[3]);
                        mode = 1;
                    }
                    break;

                case "FireAtWill":
                    {
                        mode = 2;
                    }
                    break;

                case "CeaseFire":
                    {
                        mode = 0;
                    }
                    break;
            }

        }

        public override void Load(ContentManager content)
        {
            gun = (ArtilleryComponent)Parent.FindSingleComponentByType<ArtilleryComponent>();
            gun_position = Parent.GetWorldPosition();


        }

        public override void ReConnect(GameObject other)
        {
            ArtilleryGunnerComponent otherTank = (ArtilleryGunnerComponent)other.FindGameComponentByName(Name);
            otherTank.YawRate = YawRate;
            otherTank.PitchRate = PitchRate;
            otherTank.Crew = Crew;
            otherTank.Hook = Hook;
            otherTank.emitter = (ParticleEmitterComponent)other.FindGameComponentByName(emitter.Name);
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "YawRate":
                    YawRate = float.Parse(Value);
                    break;

                case "PitchRate":
                    PitchRate = float.Parse(Value);
                    break;

                case "Crew":
                    Crew = int.Parse(Value);
                    break;

                case "Hook":
                    Hook = Value;
                    break;
            }
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        public override void Update(float dt)
        {
            time += dt;
            switch (mode)
            {
                case 0:         // dormant
                    if (round == null)
                    {
                        int round_id = gun.Round.GetHashCode();
                        round = WeaponDataBase.GetArtilleryWeapon(round_id);
                        meshhost = Parent.FindHookOwner(Hook);
                    }
                    break;

                case 1:         // start area attack
                    {
                        float aim_range_squared = (target_range * target_range) + (target_altitude * target_altitude);
                        if (aim_range_squared > round.AimMaxDistance * round.AimMaxDistance)
                        {
                            mode = 0;           // bad command, gun cannot fire at target
                            return;
                        }
                        Vector2 r = FlakBallistics.GetPitchAndTime(target_bearing, target_range, target_altitude, round.MuzzleVelocity);
                        target_pitch = r.X;
                        fuse_time = r.Y;
                        mode = 10;
                    }
                    break;

                case 2:         // start visual attack
                    {

                    }
                    break;

                case 10:       // bring gun to target
                    {
                        float pstep = PitchRate * dt;
                        float ystep = YawRate * dt;
                        float dp = target_pitch - current_pitch;
                        float dy = target_bearing - current_yaw;
                        float adp = Math.Abs(dp);
                        float ady = Math.Abs(dy);
                        bool ready = true;
                        if (adp > float.Epsilon)
                        {
                            ready = false;
                            if (adp > pstep)
                            {
                                adp = pstep;
                            }
                            adp *= Math.Sign(dp);
                            current_pitch += adp;
                        }


                        if (ady > float.Epsilon)
                        {
                            float direction = FlakBallistics.GetRotationDirection(target_bearing, current_yaw);
                            ready = false;
                            if (ady > ystep)
                            {
                                ady = ystep;
                            }
                            current_yaw += ady * direction;
                            if (current_yaw > 360)
                                current_yaw -= 360;
                            if (current_yaw < 0)
                                current_yaw += 360;
                        }
                        gun.DoubleVariables["GunPitch"] = -current_pitch;
                        gun.DoubleVariables["GunYaw"] = current_yaw;
                        if (ready)
                        {
                            mode = 11;  // start firing
                            time = 0;
                            AudioManager.PlayDynamicEffectOnceAt(round.Sound, gun_position);

                            Matrix m = meshhost.GetHookMatrix(Hook);
                            for (int i = 0; i < 15; i++)
                            {
                                Vector3 spd = ((2 * (float)rand.NextDouble()) + 1) * m.Down;
                                emitter.AddParticle(m.Translation, spd);
                            }
                        }

                    }
                    break;

                case 11:
                    {
                        if (time >= 0.25)
                        {
                            mode = 12; // reload
                            time = 0;
                            gun.DoubleVariables["Recoil"] = 0;
                        }
                        gun.DoubleVariables["Recoil"] = Math.Sin(time * 4 * MathHelper.Pi);
                    }
                    break;

                case 12:
                    {
                        if (time > gun.FireTime)
                        {
                            time = 0;
                            mode = 11;
                            AudioManager.PlayDynamicEffectOnceAt(round.Sound, gun_position);

                            Matrix m = meshhost.GetHookMatrix(Hook);
                            for (int i = 0; i < 15; i++)
                            {
                                Vector3 spd = ((2 * (float)rand.NextDouble()) + 1) * m.Down;
                                emitter.AddParticle(m.Translation, spd);
                            }
                        }
                    }
                    break;

            }
        }
        #endregion

    }
}
