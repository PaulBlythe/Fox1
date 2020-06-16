using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Helpers;

//( Class PlayerOptionsComponent )
//( Group Options )
//( Type PlayerOptionsComponent )

namespace GuruEngine.ECS.Components.Configuration
{
    public class PlayerOptionsComponent : ECSGameComponent
    {
        Dictionary<String, float> Values = new Dictionary<string, float>();
        Dictionary<String, float> Minimums = new Dictionary<string, float>();
        Dictionary<String, float> Maximums = new Dictionary<string, float>();

        #region ECSGameComponent methods
        public override void SetParameter(string Name, string Value)
        {
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <returns></returns>
        public override ECSGameComponent Clone()
        {
            PlayerOptionsComponent res = new PlayerOptionsComponent();
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
            }
            else
            {
                switch (parts[0])
                {
                    case "Root":
                        {
                            string[] objects = parts[1].Split(':');
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
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

        public override void ReConnect(GameObject other)
        {
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void Update(float dt)
        {
        }
        #endregion

        #region PlayerOptionsComponent interface
        /// <summary>
        /// Add a variable
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="defaultvalue">Default value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        public void RegisterVariable(String name, float defaultvalue, float min, float max)
        {
            if (Values.ContainsKey(name))
                return;

            Values.Add(name, defaultvalue);
            Minimums.Add(name, min);
            Maximums.Add(name, max);
        }

        public void SetValue(String name, float value)
        {
            if (Values.ContainsKey(name))
            {
                if (value < Minimums[name])
                    value = Minimums[name];
                if (value > Maximums[name])
                    value = Maximums[name];

                Values[name] = value;
            }
        }

        public float GetValue(String name)
        {
            if (Values.ContainsKey(name))
                return Values[name];
            return 0;
        }

        #endregion
    }
}
