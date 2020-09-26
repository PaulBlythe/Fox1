using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.ECS.Components.Cockpit
{
    public class NightMaterialListComponent : ECSGameComponent
    {
        public List<String> Variables = new List<string>();


        public NightMaterialListComponent()
        {
            UpdateStage = 4;
        }


        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            NightMaterialListComponent other = new NightMaterialListComponent();
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

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {

        }

        public override void ReConnect(GameObject otherObject)
        {
            NightMaterialListComponent other = (NightMaterialListComponent)otherObject.FindGameComponentByName(Name);
            other.Variables = Variables;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            Variables.Add(Value);
        }

        public override void Update(float dt)
        {
        }
        #endregion

    }
}
