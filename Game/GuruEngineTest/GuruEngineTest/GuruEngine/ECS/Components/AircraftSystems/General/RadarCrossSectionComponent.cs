using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Simulation.Components.Radar.CrossSections;
using GuruEngine;

//( Class RadarCrossSectionComponent )
//( Group Avionics )
//( Type RadarCrossSection )
//( Parameter String Definition )

namespace GuruEngine.ECS.Components.AircraftSystems.General
{
    public class RadarCrossSectionComponent : ECSGameComponent
    {
        GameObject parent = null;
        RadarCrossSectionDefinition rcs = null;

        public RadarCrossSectionComponent()
        {
            UpdateStage = 3;
        }

        /// <summary>
        /// Set the one and only parameter
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Definition":
                    {
                        String path = GuruEngine.Settings.GetInstance().GameRootDirectory + @"\RadarCrossSections\" + Value + ".rad";
                        rcs = RadarCrossSectionDefinition.Load(path);
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
            RadarCrossSectionComponent res = new RadarCrossSectionComponent();
            res.rcs = rcs;
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
            }else
            {
                switch (parts[0])
                {
                    case "Root":
                        {
                            string[] objects = parts[2].Split(':');
                            parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;
                    
                    default:
                        throw new Exception("GameComponent::RadarCrossSectionComponent:: Unknown direct connection request to " + parts[0]);
                }
            }

        }

        public override void DisConnect()
        {
        }

        /// <summary>
        /// The only contained object is the radar cross srction
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override object GetContainedObject(string type)
        {
            switch (type)
            {
                case "RadarCrossSection":
                    {
                        return rcs;
                    }
            }
            return null;
        }

        public override Texture2D GetOffscreenTexture()
        {
            return null;
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        public override void HandleEvent(string evt)
        {
        }

        public override void Load(ContentManager content)
        {
        }

        public override void ReConnect(GameObject other)
        {
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void Update(float dt)
        {
        }

    }
}
