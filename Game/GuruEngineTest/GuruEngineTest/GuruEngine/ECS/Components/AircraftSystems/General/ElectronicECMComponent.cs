using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//( Class ElectronicECMComponent )
//( Type ECM )

namespace GuruEngine.ECS.Components.AircraftSystems.General
{
    public class ElectronicECMComponent :ECSGameComponent
    {
        bool Active = false;

        public ElectronicECMComponent()
        {

        }

        #region ECSGameComponent methods
        public override void Connect(string components, bool isList)
        {
            
        }

        public override void SetParameter(string Name, string Value)
        {
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

        public override void RenderOffscreenRenderTargets()
        {
           
        }

        public override void Update(float dt)
        {
            
        }

        public override ECSGameComponent Clone()
        {
            return new ElectronicECMComponent();
        }

        public override void ReConnect(GameObject other)
        {
        }
        #endregion

    }
}
