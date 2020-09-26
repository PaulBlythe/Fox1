using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Physics.Aircraft;

//( Class HookListComponent )
//( Type HookListComponent )
//( Parameter String Entry )

namespace GuruEngine.ECS.Components.Mesh
{
    public class HookListComponent: ECSGameComponent
    {
        public Dictionary<String, Hook> Hooks = new Dictionary<string, Hook>();

        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            HookListComponent other = new HookListComponent();
            other.Hooks = Hooks;
            return (ECSGameComponent)other;
        }

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
                            string[] objects = parts[2].Split(':');
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;
                    
                    default:
                        throw new Exception("GameComponent::OnAPG68v5:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            Hooks.Clear();
            Parent = null;
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

        public override void ReConnect(GameObject other)
        {
            HookListComponent otherHLC = (HookListComponent)other.FindGameComponentByName(Name);
            otherHLC.Hooks = Hooks;
            otherHLC.Parent = other;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Entry")
            {
                string[] parts = Value.Split(',');
                Hook h = new Hook();
                h.Load(parts);
                Hooks.Add(h.Name, h);
            }
        }

        public override void Update(float dt)
        {
        }
        #endregion

        #region public methods
        public Hook FindHook(String name)
        {
            return Hooks[name];
        }

        public bool HasHook(String id)
        {
            return Hooks.ContainsKey(id);
        }
        #endregion
    }
}
