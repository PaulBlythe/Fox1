using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.ECS;
using GuruEngine.Physics.World;
using GuruEngine.World;
using GuruEngine.ECS.Components.Physics;

//( Class PropellerComponent )
//( Group Flight )
//( Type Propellor )
//( Parameter Vector3 Position )
//( Parameter Float Mass )
//( Parameter Float Moment )
//( Parameter Float Radius )
//( Parameter Float CruiseSpeed )
//( Parameter Float CruiseRPM )
//( Parameter Float CruisePower )
//( Parameter Float CruiseAlt )
//( Parameter Float MinRPM )
//( Parameter Float MaxRPM )
//( Parameter Float GearRatio )
//( Parameter Int Number )
//( Parameter Bool VariablePitch )

namespace GuruEngine.ECS.Components.AircraftSystems.Thrusters
{
    public class PropellerComponent:ECSGameComponent
    {
        AircraftStateComponent State;

        Vector3 Position;
        float Mass;
        float Moment;
        float Radius;
        float CruiseSpeed;
        float CruiseRPM;
        float CruisePower;
        float CruiseAlt;
        float MinRPM;
        float MaxRPM;
        float GearRatio;
        String Engine;
        String FeatherCommand;
        int Number;
        bool VariablePitch = false;


        float lambdaPeak;
        float beta;
        float etaC;
        float j0;
        float baseJ0;
        float f0;
        float omega;

        DynamicPhysicsComponent host;

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            PropellerComponent other = new PropellerComponent();
            other.State = State;
            other.Position = Position;
            other.Mass = Mass;
            other.Moment = Moment;
            other.Radius = Radius;
            other.CruiseSpeed = CruiseSpeed;
            other.CruiseRPM = CruiseRPM;
            other.CruisePower = CruisePower;
            other.CruiseAlt = CruiseAlt;
            other.MinRPM = MinRPM;
            other.MaxRPM = MaxRPM;
            other.GearRatio = GearRatio;
            other.FeatherCommand = FeatherCommand;
            other.Engine = Engine;
            other.Number = Number;
            other.VariablePitch = VariablePitch;
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

                    default:
                        throw new Exception("GameComponent::PropellorComponent:: Unknown direct connection request to " + parts[0]);

                }
            }
        }

        public override void DisConnect()
        {
            State = null;
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
            State = (AircraftStateComponent)Parent.FindSingleComponentByType<AircraftStateComponent>();

            lambdaPeak = (float)Math.Pow(5.0, -1.0 / 4.0);
            beta = (float)(1.0f / (Math.Pow(5.0f, -1.0f / 4.0f) - Math.Pow(5.0f, -5.0f / 4.0f)));

            etaC = 0.85f; // make this settable?

            j0 = CruiseSpeed / (CruiseRPM * lambdaPeak);
            baseJ0 = j0;

            float V2 = CruiseSpeed * CruiseSpeed + (Radius * CruiseRPM) * (Radius * CruiseRPM);

            AtmosphericModelResults res = GuruEngine.World.World.GetAtmos().Update(CruiseAlt);

            f0 = (float)(2 * etaC * CruisePower / (res.Density * CruiseSpeed * V2));

            host = (DynamicPhysicsComponent)Parent.FindSingleComponentByType<DynamicPhysicsComponent>();
        }

        public override void ReConnect(GameObject otherT)
        {
            PropellerComponent other = (PropellerComponent)otherT.FindGameComponentByName(Name);

            if (State != null)
                other.State = (AircraftStateComponent)otherT.FindGameComponentByName(State.Name);

            other.State = State;
            other.Position = Position;
            other.Mass = Mass;
            other.Moment = Moment;
            other.Radius = Radius;
            other.CruiseSpeed = CruiseSpeed;
            other.CruiseRPM = CruiseRPM;
            other.CruisePower = CruisePower;
            other.CruiseAlt = CruiseAlt;
            other.MinRPM = MinRPM;
            other.MaxRPM = MaxRPM;
            other.GearRatio = GearRatio;
            other.FeatherCommand = FeatherCommand;
            other.Engine = Engine;
            other.Number = Number;
            other.VariablePitch = VariablePitch;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "GearRatio":
                    GearRatio = float.Parse(Value);
                    break;
                case "Position":
                    Position = new Vector3();
                    string[] parts = Value.Split(',');
                    Position.X = float.Parse(parts[0]);
                    Position.Y = float.Parse(parts[1]);
                    Position.Z = float.Parse(parts[2]);
                    break;
                case "Mass":
                    Mass = float.Parse(Value) * Constants.LBS2KG;
                    break;
                case "Moment":
                    Moment = float.Parse(Value);
                    break;
                case "Radius":
                    Radius = float.Parse(Value);
                    break;
                case "CruiseSpeed":
                    CruiseSpeed = float.Parse(Value) * Constants.KTS2MPS;
                    break;
                case "CruiseRPM":
                    CruiseRPM = float.Parse(Value) * Constants.RPM2RAD;
                    break;
                case "CruisePower":
                    CruisePower = float.Parse(Value);
                    break;
                case "CruiseAlt":
                    CruiseAlt = (float)(float.Parse(Value) * Constants.fttom);
                    break;
                case "MinRPM":
                    MinRPM = float.Parse(Value);
                    break;
                case "MaxRPM":
                    MaxRPM = float.Parse(Value);
                    break;
                case "Engine":
                    Engine = Value;
                    break;
                case "Feather":
                    FeatherCommand = Value;
                    break;
            }
        }

        public override void Update(float dt)
        {
            // Feather the prop
            if (State.GetVar(FeatherCommand,0) > 0)
            {
                omega *= 0.95f;
                if (omega < 0.1)
                    omega = 0;

                State.SetVar(String.Format("Propellor{0}RPM", Number), omega / Constants.RPM2RAD);
                return;
            }

            float density = (float)State.Atmosphere.Density;
            omega = GearRatio * (float)State.GetVar(Engine, 0);           // engine rpm
            float speed = (float)State.GetVar("AirSpeed", 0);             // airspeed 

            if (VariablePitch)
            {
                float proppitch = (float)State.GetVar(String.Format("Propellor{0}Pitch", Number), 0);
                j0 = (float)(baseJ0 * Math.Pow(2, 2 - 4 * proppitch));
            }
            float tipspd = Radius * omega;
            float V2 = speed * speed + tipspd * tipspd;

            // Sanify
            if (speed < 0) speed = 0;
            if (omega < 0.001) omega = 0.001f;

            float J = speed / omega;        // Advance ratio
            float lambda = J / j0;          // Unitless scalar advance ratio

            // There's an undefined point at lambda == 1.
            if (lambda == 1.0f) lambda = 0.9999f;

            float l4 = lambda * lambda;
            l4 = l4 * l4;   // lambda^4
            float gamma = (etaC * beta / j0) * (1 - l4); // thrust/torque ratio

            // Compute a thrust coefficient, with clamping at very low lambdas (fast propeller / slow aircraft).
            float tc = (1 - lambda) / (1 - lambdaPeak);

            float thrust = 0.5f * density * V2 * f0 * tc;
            float torque = thrust / gamma;
            if (lambda > 1)
            {
                // This is the negative thrust / windmilling regime.  
                // Throw out the efficiency graph approach and instead simply extrapolate the existing linear thrust coefficient and a
                // torque coefficient that crosses the axis at a preset windmilling speed.  
                // The tau0 value is an analytically calculated (i.e. don't mess with it) value for a torque coefficient at lamda==1.
                float tau0 = (0.25f * j0) / (etaC * beta * (1 - lambdaPeak));
                float lambdaWM = 1.2f; // lambda of zero torque (windmilling)
                torque = tau0 - tau0 * (lambda - 1) / (lambdaWM - 1);
                torque *= 0.5f * density * V2 * f0;
            }
            host.AddLocalForce(new Vector3(0, 0, thrust), Position);
            host.AddTorque(torque);

            // Link to animator
            State.SetVar(String.Format("Propellor{0}RPM", Number), omega / Constants.RPM2RAD);

        }
        #endregion

        public float GetRPM()
        {
            return GearRatio * (float)State.GetVar(Engine, 0); 
        }

        public float GetGearRatio()
        {
            return GearRatio;
        }

        public float GetPowerRequired()
        {
            return 0;
        }

    }
}
