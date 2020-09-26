using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Physics.World;

//( Class StallComponent )
//( Group Aero )
//( Type StallComponent )
//( Parameter Float AOA )
//( Parameter Float Width )
//( Parameter Float Peak )


namespace GuruEngine.ECS.Components.AircraftSystems.Aero
{
    public class StallComponent:ECSGameComponent
    {
        public float AOA;
        public float Width;
        public float Peak;

        public StallComponent()
        {
            UpdateStage = 1;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            StallComponent other = new StallComponent();
            other.AOA = AOA;
            other.Width = Width;
            other.Peak = Peak;
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

        public override void Load(ContentManager content)
        {

        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        public override void ReConnect(GameObject tother)
        {
            StallComponent other = (StallComponent)tother.FindGameComponentByName(Name);
            other.AOA = AOA;
            other.Width = Width;
            other.Peak = Peak;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "AOA":
                    AOA = (float)(Constants.degtorad * float.Parse(Value));
                    break;
                case "Width":
                    Width = (float)(Constants.degtorad * float.Parse(Value));
                    break;
                case "Peak":
                    Peak = float.Parse(Value);
                    break;
                
            }
        }

        public override void Update(float dt)
        {

        }
        #endregion
    }
}
