using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.ECS.Components.Mesh;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.Physics;
using GuruEngine.DebugHelpers;

//( Class FuelTankComponent )
//( Group Propulsion )
//( Type FuelTank )
//( Parameter Float Capacity )
//( Parameter Vector3 Position )
//( Connection CollisionMeshComponent Collision )
//( ConnectionList FuelTank Engines )

namespace GuruEngine.ECS.Components.AircraftSystems.General
{
    public class FuelTank : ECSGameComponent
    {
        Vector3 Position;
        float Capacity;                                // in KG's
        float Contents;
        float Mass;
        public bool ConnectedToEngine;                 // allows for pilots to manage fuel tank usage
        CollisionMeshComponent Collision;
        List<EngineComponent> Engines = new List<EngineComponent>();
        WorldTransform Transform;
        DynamicPhysicsComponent Physics = null;

        float Damage;
        bool DumpingFuel;
        int TankIndex = -1;

        const float LbsPerTonne = 2204.622622f;
        const float MaxDumpRate = (LbsPerTonne / 60.0f);

        /// <summary>
        /// Capacity is in lbs
        /// </summary>
        public FuelTank()
        {
            UpdateStage = 1;
            Collision = null;
            Position = Vector3.Zero;
            Capacity = 0;
            Contents = 0;
            DumpingFuel = false;
            Damage = 0;
            ConnectedToEngine = true;
            Transform = null;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            FuelTank other = new FuelTank();
            other.Capacity = Capacity;
            other.Contents = Contents;
            other.Collision = Collision;
            other.Position = Position;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[1])
                {
                    case "Engines":
                        {
                            string[] mfds = parts[1].Split(',');
                            for (int i = 0; i < mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    Engines.Add((EngineComponent)Parent.FindGameComponentByName(objs[0]));
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
                    case "Collision":
                        {
                            Collision = (CollisionMeshComponent)Parent.FindGameComponentByName(objects[0]);
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

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        public override void DisConnect()
        {
            Transform = null;
            Collision = null;
            Engines.Clear();
            Contents = 0;
            Physics = null;
        }

        public override object GetContainedObject(string type)
        {
            switch (type)
            {
                case "CollisionMeshComponent":
                    return Collision;
            }
            return null;
        }

        public override Texture2D GetOffscreenTexture()
        {
            return null;
        }

        public override void HandleEvent(string evt)
        {
            String[] parts = evt.Split(':');

            switch (parts[0])
            {
                case "DumpFuel":
                    {
                        DumpingFuel = bool.Parse(parts[1]);
                    }
                    break;

                case "Damage":
                    {
                        Damage += float.Parse(parts[1]);
                    }
                    break;
            }
        }

        public override void Load(ContentManager content)
        {
             
        }

        public override void ReConnect(GameObject other)
        {
            FuelTank otherTank = (FuelTank)other.FindGameComponentByName(Name);
            foreach (ECSGameComponent gc in Engines)
            {
                ECSGameComponent ogc = other.FindGameComponentByName(gc.Name);
                otherTank.Engines.Add((EngineComponent)ogc);
            }
            if (Collision != null)
                otherTank.Collision = (CollisionMeshComponent)other.FindGameComponentByName(Collision.Name);
            
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Capacity":
                    {
                        Capacity = float.Parse(Value);
                        Contents = Capacity;
                    }
                    break;

                case "Position":
                    {
                        Position = new Vector3();
                        string[] parts = Value.Split(',');
                        Position.X = float.Parse(parts[0]);
                        Position.Y = float.Parse(parts[1]);
                        Position.Z = float.Parse(parts[2]);
                    }
                    break;
            }
        }

        public override void Update(float dt)
        {
            if (Physics == null)
            {
                Physics = (DynamicPhysicsComponent)Parent.FindSingleComponentByType<DynamicPhysicsComponent>();
                TankIndex = Physics.AddPointMass(Position, Capacity);
                Transform = (WorldTransform)Parent.FindSingleComponentByType<WorldTransform>();
            }


            float leakamount = 0;
            float p = Contents / Capacity;
            
            if (DumpingFuel)
            {
                leakamount += p * MaxDumpRate * dt;
            }
            if (Damage > 0.1)
            {
                leakamount += p * MaxDumpRate * Damage * dt;
            }
            if (leakamount>0)
            {
                Contents -= leakamount;
            }

            if (Contents<0)
            {
                Contents = 0;
            }
            Physics.UpdatePointMass(TankIndex, Capacity);

#if DEBUG
            if (DebugRenderSettings.DrawFuelTanks)
            {
                Vector3 p1 = Transform.GetLocalPosition() + Position;
                DebugLineDraw.DrawArrow(p1, Vector3.Down, Color.DarkBlue, 4);
            }
#endif

        }
        #endregion

        #region Fuel tank specific methods

        /// <summary>
        /// Called by the engines when burning fuel
        /// </summary>
        /// <param name="amount"></param>
        public float BurnFuel(float amount)
        {
            if (Contents >= amount)
            {
                Contents -= amount;
                amount = 0;
            }
            else
            {
                amount = Contents;
                Contents = 0;
            }
            Mass = Contents / 9.81f;
            return amount;
        }

        public bool HasFuel()
        {
            return (Contents > 0);
        }

        public bool IsValid()
        {
            return  (HasFuel() && ConnectedToEngine);
        }

        public float Fill(float amount)
        {
            float overage = 0.0f;

            Contents += amount;

            if (Contents > Capacity)
            {
                overage = Contents - Capacity;
                Contents = Capacity;
            }
            Mass = Contents / 9.81f;
            return overage;
        }

        #endregion
    }
}
