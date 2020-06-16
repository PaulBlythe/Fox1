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
//( Connection InputEventComponent Target )
//( Comment "OnUp    Sends the event when the button is released" )
//( Comment "OnDown  Sends the event when the button is depressed" )
//( Comment "OneShot Sends the event when the button state changes" )


namespace GuruEngine.ECS.Components.Input
{
    public class InputEventComponent:ECSGameComponent
    {
        public String Device = "Keyboard";
        public String Button;
        public String Event;
        public String Type;
        public ECSGameComponent Target = null;

        private bool state = false;

        #region ECS game component interface
        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Event": Event = Value; break;
                case "Button": Button = Value; break;
                case "Type": Type = Value; break;
            }
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <returns></returns>
        public override ECSGameComponent Clone()
        {
            InputEventComponent res = new InputEventComponent();
            res.Device = Device;
            res.Button = Button;
            res.Event = Event;
            res.Target = Target;
            res.Type = Type;
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
                    case "Target":
                        {
                            Target = Parent.FindGameComponentByName(objects[0]);
                        }
                        break;

                    default:
                        throw new Exception("GameComponent::InputEventComponent:: Unknown direct connection request to " + parts[0]);
                }
            }

        }

        public override void DisConnect()
        {
            Target = null;
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
            InputEventComponent otherI = (InputEventComponent)other.FindGameComponentByName(Name);
            otherI.Device = Device;
            otherI.Event = Event;
            otherI.Button = Button;
            otherI.Target = other.FindGameComponentByName(Target.Name);
            otherI.Type = Type;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void Update(float dt)
        {
            bool pressed = false;
            if (Device == "Keyboard")
                pressed = InputDeviceManager.WasKeyPressedByName(Button[0]);
            else
                pressed = InputDeviceManager.GetButtonByName(Device, Button);

            if (pressed == state)
                return;
            state = pressed;
            switch (Type)
            {

                case "OnUp":
                    {
                        if (!pressed)
                            Target.HandleEvent(Event);
                    }
                    break;

                case "OnDown":
                    {
                        if (pressed)
                            Target.HandleEvent(Event);
                    }
                    break;

                case "OneShot":
                    {
                        Target.HandleEvent(Event);
                    }
                    break;
            }
        }
        #endregion
    }
}
