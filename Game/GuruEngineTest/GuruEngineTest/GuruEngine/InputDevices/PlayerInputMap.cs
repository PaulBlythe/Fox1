using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace GuruEngine.InputDevices
{
    public class PlayerInputMap
    {
        public static PlayerInputMap Instance;

        public PlayerInputMap()
        {
            Instance = this;
        }

        /// <summary>
        /// Called by an InputEventComponent to register an input
        /// Default is always the keyboard
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Key"></param>
        /// <param name="Modifier">Shift,Alt,Ctrl,None or combinations</param>
        /// <param name="Type">Button,BouncedButton,HiHat,etc</param>
        public static void RegisterInput(String ID, String Key, String Type, String Modifier)
        {
            Keys key = (Keys)Enum.Parse(typeof(Keys), Key, true);
            List<Keys> modifiers = new List<Keys>();

            if (Modifier.Contains("LeftAlt"))
                modifiers.Add(Keys.LeftAlt);

            if (Modifier.Contains("LeftCtrl"))
                modifiers.Add(Keys.LeftControl);

            if (Modifier.Contains("LeftShift"))
                modifiers.Add(Keys.LeftShift);

            if (Modifier.Contains("RightAlt"))
                modifiers.Add(Keys.RightAlt);

            if (Modifier.Contains("RightCtrl"))
                modifiers.Add(Keys.RightControl);

            if (Modifier.Contains("RightShift"))
                modifiers.Add(Keys.RightShift);

            InputDescriptorType t = InputDescriptorType.None;
            switch (Type)
            {
                case "HiHat":
                    t = InputDescriptorType.HiHat;
                    break;

                case "Button":
                    t = InputDescriptorType.Button;
                    break;

                case "Dial":
                    t = InputDescriptorType.Dial;
                    break;

                case "Stick":
                    t = InputDescriptorType.Stick;
                    break;
                case "BouncedButton":
                    t = InputDescriptorType.BouncedButton;
                    break;
                case "Toggle":
                    t = InputDescriptorType.Toggle;
                    break;
            }

            InputDeviceManager.RegisterInput("Keyboard", ID, t, key, modifiers);

        }
    }
}
