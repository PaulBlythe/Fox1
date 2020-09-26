using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Physics.World;

using GuruEngine.ECS.Components.Mesh;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.Physics;
using GuruEngine.DebugHelpers;

//( Class OxygenSupplyComponent )
//( Group AircraftSystems )
//( Type OxygenSupplyComponent )
//( Parameter Float Capacity )
//( Parameter Bool Automatic )

namespace GuruEngine.ECS.Components.AircraftSystems.General
{
    public class OxygenSupplyComponent : ECSGameComponent
    {
        float Capacity = 30;
        public float Contents;
        bool Automatic = false;
        AircraftStateComponent state;

        public OxygenSupplyComponent()
        {
            UpdateStage = 3;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            OxygenSupplyComponent other = new OxygenSupplyComponent();
            other.Capacity = Capacity;
            other.Automatic = Automatic;
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
                        //throw new Exception("GameComponent::FuelTank:: Unknown direct connection request to " + parts[0]);
                        break;
                }
            }
        }

        public override void DisConnect()
        {

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
            state = (AircraftStateComponent)Parent.FindSingleComponentByType<AircraftStateComponent>();
            Contents = Capacity;
        }

        public override void ReConnect(GameObject other)
        {
            OxygenSupplyComponent otherTank = (OxygenSupplyComponent)other.FindGameComponentByName(Name);
            otherTank.Capacity = Capacity;
            otherTank.Automatic = Automatic;
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
                    }
                    break;

                case "Automatic":
                    {
                        Automatic = bool.Parse(Value);
                    }
                    break;
            }
        }

        public override void Update(float dt)
        {
            float altitude = (float)state.GetVar("Altitude", 0.0);
            float alt = (float)(altitude / Constants.fttom);

            if (alt>15000)
            {
                bool supply = false;
                if (Automatic)
                    supply = true;
                else
                {
                    supply = state.GetVar("O2On", false);
                }

                if (supply)
                {
                    float rate = (alt - 15000.0f) / 10000.0f;
                    Contents -= rate * (dt / 60.0f);
                }
            }
            float dc = Contents / Capacity;
            state.SetVar("O2level", dc);
            state.SetVar("O2Altitude", altitude);
        }
        #endregion


    }
}
