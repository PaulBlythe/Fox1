using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GuruEngine.InputDevices
{
    public class KeyboardInputDevice : InputDevice
    {
        KeyboardState lastState;
        KeyboardState newState;

        List<KeyboardButton> activeButtons = new List<KeyboardButton>();
        List<KeyboardButton> activeToggles = new List<KeyboardButton>();

        public override void CloseDown()
        {

        }

        public override bool Connect()
        {
            return true;
        }

        public override bool Update()
        {
            lastState = newState;
            newState = Keyboard.GetState();

            foreach (KeyboardButton kb in activeButtons)
            {
                bool pressed = true;
                foreach (Keys k in kb.keys)
                {
                    if (newState.IsKeyUp(k))
                        pressed = false;
                }
                Buttons[kb.ID] = pressed;
            }
            foreach (KeyboardButton kb in activeToggles)
            {
                bool pressed = true;
                bool released = false;
                foreach (Keys k in kb.keys)
                {
                    if (lastState.IsKeyUp(k))
                        pressed = false;
                }
                if (pressed)     // all the keys where down last frame   
                {
                   
                    foreach (Keys k in kb.keys)
                    {
                        if (newState.IsKeyUp(k))
                            released = true;
                    }
                }

                Buttons[kb.ID] = pressed && released;
            }
            return true;
        }

        public void AddKeyHandler(Keys key, List<Keys> modifiers, String id, InputDescriptorType type)
        {
            switch (type)
            {
                case InputDescriptorType.Button:
                    {
                        Buttons.Add(id, false);
                        KeyboardButton kb = new KeyboardButton();
                        kb.Down = false;
                        kb.ID = id;
                        kb.keys.Add(key);
                        foreach (Keys k in modifiers)
                            kb.keys.Add(k);
                        activeButtons.Add(kb);
                    }
                    break;

                case InputDescriptorType.Toggle:
                    {
                        Buttons.Add(id, false);
                        KeyboardButton kb = new KeyboardButton();
                        kb.Down = false;
                        kb.ID = id;
                        kb.keys.Add(key);
                        foreach (Keys k in modifiers)
                            kb.keys.Add(k);
                        activeToggles.Add(kb);
                    }
                    break;
            }
        }

    }
}
