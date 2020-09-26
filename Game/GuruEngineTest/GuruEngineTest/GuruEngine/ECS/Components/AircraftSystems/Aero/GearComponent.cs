using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Physics.World;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.Physics;
using GuruEngine.DebugHelpers;

//( Class GearComponent )
//( Group Aero )
//( Type GearComponent )
//( Parameter Vector3 Position )
//( Parameter Vector3 Up )
//( Parameter Float Compression )
//( Parameter Float DynamicFriction )
//( Parameter Float StaticFriction )
//( Parameter Bool Castering )
//( Parameter Float SpringCoefficient )
//( Parameter Float DampingCoefficient )
//( Parameter Float BrakingFraction )
//( Parameter Float Rotation )
//( Parameter Float InitialLoad )
//( Parameter Float SpringFactorNotPlaning )
//( Parameter Float SpeedPlaning )
//( Parameter Float ReduceFrictionByExtension )
//( Parameter Float ExtendTime )
//( Parameter String LockCommand )
//( Parameter String BrakeCommand )
//( Parameter String ExtendCommand )
//( Parameter Bool WaterGear )
//( Parameter Float Area )
//( Parameter Float LoadCapacity )
//( Parameter String Name )

namespace GuruEngine.ECS.Components.AircraftSystems.Aero
{
    public class GearComponent : ECSGameComponent
    {
        public Vector3 Position;
        public Vector3 Up = Vector3.Up;
        public float Compression;
        public float DynamicFriction = 0.7f;
        public float StaticFriction = 0.8f;
        public bool Castering = true;                       // If this is true, this wheel cannot be steered                 
        public float SpringCoefficient = 20000;
        public float DampingCoefficient = 5000.0f;
        public float BrakingFraction = 0;
        public float Rotation;
        public float InitialLoad = 0;
        public float SpringFactorNotPlaning = 1;
        public float SpeedPlaning = 0;
        public float ReduceFrictionByExtension = 0;
        public String LockCommand = "";
        public String BrakeCommand = "";
        public String ExtendCommand = "";
        public bool WaterGear = false;
        public float Area = 2;
        public float LoadCapacity = float.MaxValue;
        public String GearName;

        DynamicPhysicsComponent host;
        WorldTransform transform;
        AircraftStateComponent state;
        float oldDepth = 0;

        float Stiffness = 0.06f;
        float Shape = 2.8f;
        float Curvature = 1.03f;
        float WheelSlip = 0.0f;

#if DEBUG
        Vector3 oldforce = Vector3.Zero;
#endif

        public float casterAngle;

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            GearComponent other = new GearComponent();

            other.Position = Position;
            other.Up = Up;
            other.Compression = Compression;
            other.DynamicFriction = DynamicFriction;
            other.StaticFriction = StaticFriction;
            other.Castering = Castering;
            other.SpringCoefficient = SpringCoefficient;
            other.DampingCoefficient = DampingCoefficient;
            other.BrakingFraction = BrakingFraction;
            other.Rotation = Rotation;
            other.InitialLoad = InitialLoad;
            other.SpringFactorNotPlaning = SpringFactorNotPlaning;
            other.SpeedPlaning = SpeedPlaning;
            other.ReduceFrictionByExtension = ReduceFrictionByExtension;
            other.LockCommand = LockCommand;
            other.BrakeCommand = BrakeCommand;
            other.ExtendCommand = ExtendCommand;
            other.WaterGear = WaterGear;
            other.Area = Area;
            other.LoadCapacity = LoadCapacity;
            UpdateStage = 1;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {

                throw new Exception("GameComponent::GearComponent:: Unknown list connection request to " + parts[0]);

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

                    default:
                        throw new Exception("GameComponent::GearComponent:: Unknown direct connection request to " + parts[0]);

                }
            }
        }

        public override void DisConnect()
        {
            host = null;
            transform = null;
            state = null;
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
        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }
        public override void Load(ContentManager content)
        {
            host = (DynamicPhysicsComponent)Parent.FindSingleComponentByType<DynamicPhysicsComponent>();
            state = (AircraftStateComponent)Parent.FindSingleComponentByType<AircraftStateComponent>();

            transform = (WorldTransform)Parent.FindSingleComponentByType<WorldTransform>();
        }

        public override void ReConnect(GameObject tother)
        {
            GearComponent other = (GearComponent)tother.FindGameComponentByName(Name);
            other.Position = Position;
            other.Up = Up;
            other.Compression = Compression;
            other.DynamicFriction = DynamicFriction;
            other.StaticFriction = StaticFriction;
            other.Castering = Castering;
            other.SpringCoefficient = SpringCoefficient;
            other.DampingCoefficient = DampingCoefficient;
            other.BrakingFraction = BrakingFraction;
            other.Rotation = Rotation;
            other.InitialLoad = InitialLoad;
            other.SpringFactorNotPlaning = SpringFactorNotPlaning;
            other.SpeedPlaning = SpeedPlaning;
            other.ReduceFrictionByExtension = ReduceFrictionByExtension;
            other.LockCommand = LockCommand;
            other.BrakeCommand = BrakeCommand;
            other.ExtendCommand = ExtendCommand;
            other.WaterGear = WaterGear;
            other.Area = Area;
            other.LoadCapacity = LoadCapacity;
            other.GearName = GearName;

        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Name":
                    GearName = Value;
                    break;
                case "LockCommand":
                    LockCommand = Value;
                    break;
                case "BrakeCommand":
                    BrakeCommand = Value;
                    break;
                case "ExtendCommand":
                    ExtendCommand = Value;
                    break;
                case "Compression":
                    Compression = float.Parse(Value);
                    break;
                case "SpeedPlaning":
                    SpeedPlaning = (float)(Constants.ktstofps * float.Parse(Value));
                    break;
                case "DynamicFriction":
                    DynamicFriction = float.Parse(Value);
                    break;
                case "StaticFriction":
                    StaticFriction = float.Parse(Value);
                    break;
                case "Castering":
                    Castering = bool.Parse(Value);
                    break;
                case "Area":
                    Area = float.Parse(Value);
                    break;
                case "SpringCoefficient":
                    SpringCoefficient = float.Parse(Value);
                    break;
                case "DampingCoefficient":
                    DampingCoefficient = float.Parse(Value);
                    break;
                case "BrakingFraction":
                    BrakingFraction = float.Parse(Value);
                    break;
                case "Rotation":
                    Rotation = float.Parse(Value);
                    break;
                case "InitialLoad":
                    InitialLoad = float.Parse(Value);
                    break;
                case "LoadCapacity":
                    LoadCapacity = float.Parse(Value);
                    break;
                case "SpringFactorNotPlaning":
                    SpringFactorNotPlaning = float.Parse(Value);
                    break;
                case "ReduceFrictionByExtension":
                    ReduceFrictionByExtension = float.Parse(Value);
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

                case "Up":
                    {
                        Up = new Vector3();
                        string[] parts = Value.Split(',');
                        Up.X = float.Parse(parts[0]);
                        Up.Y = float.Parse(parts[1]);
                        Up.Z = float.Parse(parts[2]);
                    }
                    break;
            }
        }

        public override void Update(float dt)
        {
#if DEBUG
            if (DebugRenderSettings.DrawGears)
            {
                Vector3 p1 = Vector3.Transform(Position, transform.world);
                DebugLineDraw.DrawArrow(p1, Up, Color.Red, 5);
            }
#endif
            if (dt == 0)
                return;

            // If the gear is retracted, just ignore it for now
            float extension = (float)state.GetVar(ExtendCommand, 1.0);
            if (extension == 0)
                return;

            float depth = host.GroundPenetrationDepth(Position);

            Vector3 velocity = host.GetWorldVelocity();
            Vector3 acceleration = host.GetWorldAcceleration();

            float delta_v = velocity.LengthSquared();

            Vector3 direction = Vector3.Zero;
            if (delta_v != 0)
                direction = Vector3.Normalize(velocity);

            // This is a standard gear rather than a float and we are in the water
            // So throw in a large force in opposition to direction of travel
            if ((host.IsOnWater(Position)) && (!WaterGear))
            {
                Vector3 pforce = velocity * velocity * 100 * depth;
                host.AddWorldForce(pforce, Position);
                return;
            }

            // This is a float and we are on land
            // So throw in another force
            if ((host.IsOnGround(Position)) && (WaterGear))
            {
                Vector3 pforce = velocity * velocity * 100 * DynamicFriction;
                host.AddLocalForce(pforce, Position);
                return;
            }

            // Gear are down and in air flow so will produce drag
            float mag = (float)(state.Atmosphere.Density * extension * 0.5f * delta_v * 1.15f * Area);
            Vector3 drag = direction * mag;
            host.AddWorldForce(-drag, Position);

            // if the gear is not in contact with the ground we only have to worry about drag
            if (depth < 0)
            {
                return;
            }

            if (depth > Compression)
                depth = Compression;

            state.SetVar(GearName + "Depth", depth);

            Vector3 force = new Vector3(0, 0, 0);
            float cv = (oldDepth - depth) / dt;                     // Compression velocity
            force.Y = ComputeVerticalStrutForce(depth, cv);
            force.Y -= cv * DampingCoefficient;                     // Spring damping

            if (velocity.Z > 0.0001f)
            {
                WheelSlip = (float)(-Constants.radtodeg * Math.Atan2(velocity.Z, velocity.X));
            }
            force.X = -ComputeSideForceCoefficient() * velocity.X;

            float friction = DynamicFriction;
            if (BrakeCommand != "")
            {
                float db = (float)state.GetVar(BrakeCommand, 0);
                friction = (float)(DynamicFriction * (1.0 - db) + StaticFriction * db);
            }

            float ff = -friction * force.Y;
            float vskid = velocity.Z;

            float fskid;
            if (host.IsOnGround(Position))
            {
                fskid = (float)(CalcFriction(force.Y, vskid) * (friction));
            }
            else
            {
                fskid = (float)(10 * CalcFrictionFluid(force.Y, vskid) * friction);
            }

            fskid = Math.Abs(fskid);
            float cff = Math.Abs(acceleration.Z * host.GetTotalMass());
            if (cff < fskid)
            {
                fskid = cff;
            }
            if (acceleration.Z > 0)
            {
                fskid = -fskid;
            }
            force.Z = fskid;

            host.AddWorldForce(force, Position);

            oldDepth = depth;
            oldforce = force;
        }
        #endregion

        /// <summary>
        /// Friction with gound
        /// </summary>
        /// <param name="wgt"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        float CalcFriction(float wgt, float v)
        {
            const float STOP = 0.1f;
            const float iSTOP = 1.0f / STOP;
            v = Math.Abs(v);
            if (v < STOP) return v * iSTOP * wgt * StaticFriction;
            else return wgt * DynamicFriction;
        }

        /// <summary>
        /// Friction with water
        /// </summary>
        /// <param name="wgt"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        float CalcFrictionFluid(float wgt, float v)
        {
            const float STOP = 0.01f;
            const float iSTOP = 1.0f / STOP;
            v = Math.Abs(v);
            if (v < STOP) return v * iSTOP * wgt * StaticFriction;
            else return wgt * DynamicFriction * v * v * 0.01f;
        }

        /// <summary>
        /// Compute the vertical force on the wheel using square-law damping (per comment in paper AIAA-2000-4303 - see header prologue comments). 
        /// We might consider allowing for both square and linear damping force calculation. 
        /// Also need to possibly give a "rebound damping factor" that differs from the compression case.
        /// </summary>
        public float ComputeVerticalStrutForce(float compressLength, float compressSpeed)
        {
            double springForce = 0;
            double dampForce = 0;

            springForce = compressLength * SpringCoefficient;

            if (compressSpeed >= 0.0)
            {
                dampForce = -compressSpeed * compressSpeed * DampingCoefficient;
            }
            else
            {
                dampForce = compressSpeed * compressSpeed * DampingCoefficient;
            }
            return (float)Math.Max(springForce + dampForce, (double)0.0);
        }

        /// <summary>
        /// Compute the sideforce coefficients using Pacejka's Magic Formula.
        /// 
        ///  y(x) = D sin {C arctan [Bx - E(Bx - arctan Bx)]}
        ///  
        /// Where: B = Stiffness Factor (0.06, here)
        ///        C = Shape Factor (2.8, here)  
        ///        D = Peak Factor (0.8, here)
        ///        E = Curvature Factor (1.03, here) 
        /// </summary>
        /// <param name=""></param>
        float ComputeSideForceCoefficient()
        {
            double StiffSlip = Stiffness * WheelSlip;
            float FCoeff = (float)(0.8f * Math.Sin(Shape * Math.Atan(StiffSlip - Curvature * (StiffSlip - Math.Atan(StiffSlip)))));
            return FCoeff;
        }
    }
}
