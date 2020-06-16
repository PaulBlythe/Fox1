using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GuruEngine.Algebra;
using GuruEngine.Simulation.Weapons.AAA;
using GuruEngine.Simulation.Weapons.Ammunition;
using GuruEngine.ECS.Components.Mesh;
using GuruEngine.AI;
using GuruEngine.DebugHelpers;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//( Class ShipGunComponent )
//( Group Naval )
//( Type ShipGunComponent )
//( ConnectionList ShipGunFOFComponent GunSights )
//( Connection ShipStateComponent Ship )
//( Parameter String Hook )
//( Parameter String GunType )
//( Parameter Int RoundType )
//( Parameter String Mesh )
//( Parameter String AmmoType )

namespace GuruEngine.ECS.Components.Ships
{
    public enum GunState
    {
        Scanning,
        Loading,
        Firing,
        Aiming,
        Destroyed,
        Init
    }
    public class ShipGunComponent : ECSGameComponent
    {
        String Hook;
        String GunType;
        float RateOfFire;
        ShipStateComponent host;
        List<ShipGunFOFComponent> sights = new List<ShipGunFOFComponent>();
        int RoundType;
        String Mesh;
        String AmmoType;
        float GunnerSkill = 1;
        
        WeaponAAA weapon = null;
        AAARound ammo = null;
        MultiMeshComponent MeshHost;
        int TargetID = 0;

        int debug = 0;

        GunState state = GunState.Init;
        List<BoundingFrustum> fofs = new List<BoundingFrustum>();

        
        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            ShipGunComponent other = new ShipGunComponent();

            other.host = host;
            other.Hook = Hook;
            other.GunType = GunType;
            other.RateOfFire = RateOfFire;
            other.sights = sights;
            other.RoundType = RoundType;
            other.weapon = weapon;
            other.Mesh = Mesh;
            other.ammo = ammo;
            other.AmmoType = AmmoType;
            other.MeshHost = MeshHost;
            return (ECSGameComponent)other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[0])
                {
                    case "GunSights":
                        {
                            string[] mfds = parts[1].Split(',');
                            for (int i = 0; i < mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    sights.Add((ShipGunFOFComponent)Parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::FuelTank:: Unknown list connection request to " + parts[0]);
                }
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

                    case "Ship":
                        {
                            host = (ShipStateComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;




                }
            }
        }

        public override void DisConnect()
        {
            host = null;
            sights.Clear();
            weapon = null;
            Mesh = null;
            ammo = null;
            MeshHost = null;
            fofs.Clear();
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

        }

        public override void Load(ContentManager content)
        {
            WeaponDataBase.Load(GunType);
            AmmunitionDatabase.Load(AmmoType);

            switch (RoundType)
            {
                case 0:             // AAA gun
                    weapon = WeaponDataBase.GetAAAWeapon(GunType.GetHashCode());
                    ammo = AmmunitionDatabase.GetAAARound(AmmoType.GetHashCode());
                    break;
            }
            MeshHost = (MultiMeshComponent)Parent.FindGameComponentByName(Mesh);


        }

        public override void ReConnect(GameObject other)
        {
            ShipGunComponent otherT = (ShipGunComponent)other.FindGameComponentByName(Name);
            otherT.host = host;
            otherT.RateOfFire = RateOfFire;
            otherT.Hook = Hook;
            otherT.sights = sights;
            otherT.GunType = GunType;
            otherT.RoundType = RoundType;
            otherT.weapon = weapon;
            otherT.Mesh = Mesh;
            otherT.ammo = ammo;
            otherT.AmmoType = AmmoType;
            otherT.MeshHost = MeshHost;
            otherT.fofs = fofs;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "GunType":
                    {
                        GunType = Value;

                    }
                    break;
                case "Hook":
                    {
                        Hook = Value;
                    }
                    break;
                case "RateOfFire":
                    {
                        RateOfFire = float.Parse(Value);
                    }
                    break;
                case "RoundType":
                    {
                        RoundType = int.Parse(Value);
                    }
                    break;
                case "Mesh":
                    {
                        Mesh = Value;
                    }
                    break;
                case "AmmoType":
                    {
                        AmmoType = Value;
                    }
                    break;

            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            if (MeshHost.mesh != null)
            {
                if ((fofs.Count == 0) && (MeshHost.IsActive()))
                {
                    float far = weapon.AimMaxDistance;
                    float near = 5;

                    Vector3 start = MeshHost.GetWorldMatrix().Translation;

                    foreach (ShipGunFOFComponent sf in sights)
                    {
                        Matrix mat = Matrix.CreateRotationY(MathHelper.ToRadians(sf.BaseHeading));

                        Vector3 end = new Vector3(0, 0, far);

                        float mid_yaw = MathHelper.ToRadians((sf.YawMax + sf.YawMin) / 2);
                        float mid_pitch = MathHelper.ToRadians((sf.PitchMax + sf.PitchMin) / 2);
                        Matrix tm = Matrix.CreateFromYawPitchRoll(mid_yaw, -mid_pitch, 0);
                        Vector3 target = Vector3.Transform(end, tm);
                        target = Vector3.Transform(target, mat);

                        float x_range = MathHelper.ToRadians(sf.YawMax - sf.YawMin);
                        float y_range = MathHelper.ToRadians(sf.PitchMax - sf.PitchMin);
                        float aspect_ratio = x_range / y_range;

                        Matrix p = Matrix.CreatePerspectiveFieldOfView(x_range, aspect_ratio, near, far);
                        Matrix l = Matrix.CreateLookAt(start, start + target, Vector3.Up);
                        fofs.Add(new BoundingFrustum(l * p));
                        state = GunState.Scanning;
                    }

                }
                else
                {

                    Vector3 forward = MeshHost.GetOriginalWorldMatrix().Forward;
                    Vector3 start = MeshHost.GetWorldMatrix().Translation;

                    Matrix mat = Matrix.CreateRotationY(MathHelper.ToRadians(-90));
                    forward = Vector3.Transform(forward, mat);

                    DebugLineDraw.DrawLine(start, start + forward * 5000, Color.White);



                }
            }

            switch (state)
            {
                case GunState.Scanning:
                    {
                        if (TargetManager.AreAnyTargetsNearby(Parent.IFF, 1, Parent.GetWorldPosition(), weapon.AimMaxDistance))
                        {
                            List<int> targets = TargetManager.GetNearbyTargets(Parent.IFF, 1, Parent.GetWorldPosition());
                            foreach (int t in targets)
                            {
                                AITarget ait = TargetManager.GetAirborneTargetDetails(t);
                                if (CanBear(ait))
                                {
                                    state = GunState.Aiming;
                                    TargetID = t;
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case GunState.Aiming:
                    {
                        if (TargetManager.IsTargetStillValid(TargetID))
                        {
                            if (CanBear(TargetManager.GetAirborneTargetDetails(TargetID)))
                            {
                                debug++;
                                Matrix m = Matrix.CreateRotationZ(MathHelper.ToRadians(debug));

                                //Matrix m = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(-pitch), 0);
                                MeshHost.MatrixAnimate(m);
                            }
                            else
                            {
                                state = GunState.Scanning;
                            }
                        }
                        else
                        {
                            state = GunState.Scanning;
                        }
                    }
                    break;
            }

#if DEBUG

           
            if (fofs.Count>0)
            {
                foreach (BoundingFrustum frustrum in fofs)
                {
                    Vector3[] corners = frustrum.GetCorners();
                    DebugLineDraw.DrawLine(corners[0], corners[1], Color.Yellow);
                    DebugLineDraw.DrawLine(corners[1], corners[2], Color.Yellow);
                    DebugLineDraw.DrawLine(corners[2], corners[3], Color.Yellow);
                    DebugLineDraw.DrawLine(corners[0], corners[3], Color.Yellow);

                    DebugLineDraw.DrawLine(corners[4], corners[5], Color.Yellow);
                    DebugLineDraw.DrawLine(corners[5], corners[6], Color.Yellow);
                    DebugLineDraw.DrawLine(corners[6], corners[7], Color.Yellow);
                    DebugLineDraw.DrawLine(corners[4], corners[7], Color.Yellow);

                    DebugLineDraw.DrawLine(corners[0], corners[4], Color.Yellow);
                    DebugLineDraw.DrawLine(corners[1], corners[5], Color.Yellow);
                    DebugLineDraw.DrawLine(corners[2], corners[6], Color.Yellow);
                    DebugLineDraw.DrawLine(corners[3], corners[7], Color.Yellow);

                    float far = weapon.AimMaxDistance;

                    foreach (ShipGunFOFComponent sf in sights)
                    {
                        Matrix mat = Matrix.CreateRotationY(MathHelper.ToRadians(sf.BaseHeading));
                        Vector3 end = new Vector3(0, 0, far);

                        float mid_yaw = MathHelper.ToRadians((sf.YawMax + sf.YawMin) / 2);
                        float mid_pitch = MathHelper.ToRadians((sf.PitchMax + sf.PitchMin) / 2);
                        Matrix tm = Matrix.CreateFromYawPitchRoll(mid_yaw, -mid_pitch, 0);
                        Vector3 target = Vector3.Transform(end, tm);
                        target = Vector3.Transform(target, mat);

                        Vector3 start = MeshHost.GetWorldMatrix().Translation;
                        DebugLineDraw.DrawLine(start + target, start, Color.Red);
                    }
                }
            }
#endif
        }
        #endregion

        bool CanBear(AITarget target)
        {
            Matrix world = MeshHost.GetWorldMatrix();
           
            Matrix wi = Matrix.Invert(world);
            Vector3 relative_direction = Vector3.Transform(target.Position, wi);

            foreach(BoundingFrustum frustrum in fofs)
                if (frustrum.Contains(target.Position) != ContainmentType.Disjoint)
                    return true;
            
            return false;

        }
    }
}
