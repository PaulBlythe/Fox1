using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.ECS;
using GuruEngine.InputDevices;

//( Class InputEventComponent )
//( Type InputEventComponent )
//( Group Input )
//( Parameter String Button )
//( Parameter String Event )
//( Parameter String Type )
//( Parameter String Modifier )
//( Parameter Float Min )
//( Parameter Float Max )

namespace GuruEngine.ECS.Components.Input
{
    public class InputEventComponent : ECSGameComponent
    {
        public String Button;
        public String Event;
        public String Type;
        public String Modifier;
        public float Minimum;
        public float Maximum;

        private bool state = false;

        #region ECS game component interface
        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Event": Event = Value; break;
                case "Button": Button = Value; break;
                case "Type": Type = Value; break;
                case "Modifier": Modifier = Value; break;
                case "Min": Minimum = float.Parse(Value); break;
                case "Max": Maximum = float.Parse(Value); break;
            }
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <returns></returns>
        public override ECSGameComponent Clone()
        {
            InputEventComponent res = new InputEventComponent();
            res.Button = Button;
            res.Event = Event;
            res.Modifier = Modifier;
            res.Type = Type;
            res.Minimum = Minimum;
            res.Maximum = Maximum;
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
                string[] objects = parts[2].Split(':');
                switch (parts[1])
                {
                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;


                    default:
                        throw new Exception("GameComponent::InputEventComponent:: Unknown direct connection request to " + parts[0]);
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
            PlayerInputMap.RegisterInput(Event, Button, Type, Modifier);
        }

        public override void ReConnect(GameObject other)
        {
            InputEventComponent otherI = (InputEventComponent)other.FindGameComponentByName(Name);
            otherI.Event = Event;
            otherI.Button = Button;
            otherI.Modifier = Modifier;
            otherI.Type = Type;
            otherI.Minimum = Minimum;
            otherI.Maximum = Maximum;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void Update(float dt)
        {

        }
        #endregion
    }
}
