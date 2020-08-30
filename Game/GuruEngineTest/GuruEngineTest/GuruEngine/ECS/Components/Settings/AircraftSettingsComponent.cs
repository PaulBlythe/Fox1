using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.ECS.Components.Settings
{
    public class AircraftSettingsComponent:ECSGameComponent
    {
        public Dictionary<String, float> Variables = new Dictionary<string, float>();


        public AircraftSettingsComponent()
        {
            UpdateStage = 4;
        }


        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            AircraftSettingsComponent other = new AircraftSettingsComponent();
            other.Variables = Variables;
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
            String[] parts = evt.Split(':');


        }

        public override void Load(ContentManager content)
        {
           
        }

        public override void ReConnect(GameObject otherObject)
        {
            AircraftSettingsComponent other = (AircraftSettingsComponent)otherObject.FindGameComponentByName(Name);
            other.Variables = Variables;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            Variables.Add(Name, float.Parse(Value));
        }

        public override void Update(float dt)
        {
        }
        #endregion

    }
}
