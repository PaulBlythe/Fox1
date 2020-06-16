using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.ECS.Components.Generic;
using GuruEngine.Physics.Propulsion;
using GuruEngine.Physics.Propulsion.Engines;

//( Class EngineComponent )
//( Group Propulsion )
//( Type Engine )
//( Connection CollisionMeshComponent Collision )
//( Connection FuelManagementComponent Fuel )
//( Parameter Int Type )
//( Parameter String Definition ) 
//( Parameter String SoundEffectPack )

namespace GuruEngine.ECS.Components.AircraftSystems.General
{
    public class EngineComponent:ECSGameComponent
    {
        public List<FuelTank> ConnectedTanks = new List<FuelTank>();
        public String BaseSoundID;
        public FuelManagementComponent Fuel;
        public AircraftEngine Engine;
        float ThrottleSetting;

        public EngineComponent()
        {
            ThrottleSetting = 0;
            Engine = null;
            Fuel = null;
            UpdateStage = 4;
        }

        #region ECSGameComponent methods
        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                
                case "Type":
                    {
                        int type = int.Parse(Value);
                        switch (type)
                        {
                            case 0:
                                Engine = new Turbine();
                                break;
                            default:
                                throw new Exception("EngineComponent::SetParameter() Unknown engine type " + type.ToString());
                        }
                    }
                    break;

                case "Definition":
                    {
                        Engine.SetDefinition(Value);
                    }
                    break;
            }
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <returns></returns>
        public override ECSGameComponent Clone()
        {
            EngineComponent res = new EngineComponent();
            res.Engine = Engine;
            return res;
        }

        /// <summary>
        /// Connect up the component
        /// Only connects to root
        /// </summary>
        /// <param name="components"></param>
        /// <param name="isList"></param>
        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[1])
                {
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
                    case "CollisionMeshComponent":
                        {

                        }
                        break;
                    case "Fuel":
                        {
                            Fuel = (FuelManagementComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;


                    default:
                        throw new Exception("GameComponent::EngineComponent:: Unknown direct connection request to " + parts[0]);
                }
            }

        }

        public override void DisConnect()
        {
            Engine = null;
            Fuel = null;
        }

        /// <summary>
        /// The only contained object is the radar cross srction
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
            switch (evt)
            {
                case "EngineStart":
                    {
                        Engine.HandleEvent(evt);
                    }
                    break;
            }
        }

        public override void Load(ContentManager content)
        {
            
        }

        public override void ReConnect(GameObject other)
        {
            EngineComponent otherEngine = (EngineComponent)other.FindGameComponentByName(Name);
            otherEngine.Engine = Engine;
            FuelManagementComponent om = (FuelManagementComponent)other.FindGameComponentByName(Fuel.Name);
            otherEngine.Fuel = om;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void Update(float dt)
        {
            Engine.Update(Parent, this, dt);
        }
        #endregion

        #region EngineComponent methods
        public float GetThrottleSetting()
        {
            return ThrottleSetting;
        }
        #endregion
    }
}
