using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Physics.World;

//( Class FlapComponent )
//( Group Aero )
//( Type FlapComponent )
//( Parameter Float Start )
//( Parameter Float End )
//( Parameter Float Lift )
//( Parameter Float Drag )
//( Parameter String Control )

namespace GuruEngine.ECS.Components.AircraftSystems.Aero
{
    public class FlapComponent : ECSGameComponent
    {
        public float Start;
        public float End;
        public float Lift;
        public float Drag;
        public String Control;

        public AircraftStateComponent host;
        public float Incidence = 0;

        public FlapComponent()
        {
            UpdateStage = 1;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            FlapComponent other = new FlapComponent();
            other.Start = Start;
            other.End = End;
            other.Lift = Lift;
            other.Drag = Drag;
            other.Control = Control;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {

                throw new Exception("GameComponent::HStabComponent:: Unknown list connection request to " + parts[0]);

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
                        throw new Exception("GameComponent::HStabComponent:: Unknown direct connection request to " + parts[0]);

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
            host = (AircraftStateComponent)Parent.FindSingleComponentByType<AircraftStateComponent>();
        }

        public override void ReConnect(GameObject tother)
        {
            FlapComponent other = (FlapComponent)tother.FindGameComponentByName(Name);
            other.Start = Start;
            other.End = End;
            other.Lift = Lift;
            other.Drag = Drag;
            other.Control = Control;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Start":
                    Start = float.Parse(Value);
                    break; 
                case "End":
                    End = float.Parse(Value);
                    break;
                case "Lift":
                    Lift = float.Parse(Value);
                    break;
                case "Drag":
                    Drag = float.Parse(Value);
                    break;
                case "Control":
                    Control = Value;
                    break;
                
            }
        }

        public override void Update(float dt)
        {
            double inc = host.GetVar(Control, 0);
            Incidence = (float)inc;
        }
        #endregion
    }
}
