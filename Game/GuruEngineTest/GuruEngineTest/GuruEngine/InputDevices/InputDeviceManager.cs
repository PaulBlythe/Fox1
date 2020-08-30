using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using GuruEngine.InputDevices.FlightSticks;


namespace GuruEngine.InputDevices
{
    public class InputDeviceManager
    {
        public static InputDeviceManager Instance;
        public PlayerInputMap playerMap;
        public Dictionary<String, InputDevice> Devices = new Dictionary<string, InputDevice>();
        public Dictionary<String, String> ControlToDevice = new Dictionary<string, string>();

        KeyboardState lastState;
        KeyboardState currentState;

        public InputDeviceManager()
        {
            Instance = this;
            playerMap = new PlayerInputMap();
            KeyboardInputDevice kid = new KeyboardInputDevice();
            Devices.Add("Keyboard", kid);
        }

        /// <summary>
        /// Add an input device 
        /// </summary>
        /// <param name="name">Name of the device</param>
        /// <returns>true if available</returns>
        public bool AddDevice(String name)
        {
            switch (name)
            { 
                case "LogitechRudderPedals":
                    {
                        LogitechRudderPedals lrp = new LogitechRudderPedals();
                        Devices.Add(name, lrp);
                        return lrp.Connect();
                    }

                case "X56HOTASStick":
                    {
                        X56HOTASStick st = new X56HOTASStick();
                        Devices.Add(name, st);
                        return st.Connect();
                    }
                case "X56HOTASThrottle":
                    {
                        X56HOTASThrottle st = new X56HOTASThrottle();
                        Devices.Add(name, st);
                        return st.Connect();
                    }
            }
            return false;
        }

        public void Update()
        {
            lastState = currentState;
            currentState = Keyboard.GetState();

            foreach (String s in Devices.Keys)
            {
                Devices[s].Update();
            }
        }

        public bool WasKeyPressed(Keys key)
        {
            if ((!lastState.IsKeyDown(key)) && (currentState.IsKeyDown(key)))
            {
                return true;
            }
            return false;
        }

        public bool WasKeyPressed(char ckey)
        {
            Keys key = convertToKey(ckey);

            if ((!lastState.IsKeyDown(key)) && (currentState.IsKeyDown(key)))
            {
                return true;
            }
            return false;
        }

        public bool GetBouncedButton(String device, String name)
        {
            return Devices[device].DebouncedButtons[name].Down;
        }

        public bool GetButton(String device, String name)
        {
            return Devices[device].Buttons[name];
        }

        public HiHat GetHiHat(String device, String name)
        {
            return Devices[device].HiHats[name];
        }

        public Vector2 GetAxes(String device, String name)
        {
            return Devices[device].Axes[name];
        }

        public int GetPOV(String device, String name)
        {
            return Devices[device].POVs[name];
        }

        public float GetThrottle(String device, String name)
        {
            return Devices[device].Throttles[name];
        }

        public byte GetWheel(String device, String name)
        {
            return Devices[device].Wheels[name];
        }

        public int GetGenericInt(String device, String name)
        {
            return Devices[device].GenericInts[name];
        }

        public float GetGenericFloat(String device, String name)
        {
            return Devices[device].GenericFloats[name];
        }

        public InputDevice GetDevice(string name)
        {
            return Devices[name];
        }

        public void CloseDown()
        {
            foreach (String s in Devices.Keys)
            {
                try
                {
                    Devices[s].CloseDown();
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private Keys convertToKey(char c)
        {
            return (Keys)char.ToUpper(c);
        }

        #region Static methods
        public static float GetFloat(String dev, String name)
        {
            return InputDeviceManager.Instance.GetGenericFloat(dev, name);
        }
        public static InputDevice GetInputDeviceByName(String name)
        {
            return Instance.GetDevice(name);
        }
        public static int GetPOVByName(String dev, String name)
        {
            return Instance.GetPOV(dev, name);
        }
        public static byte GetWheelByName(String dev, String name)
        {
            return Instance.GetWheel(dev, name);
        }
        public static float GetThrottleByName(String dev, String name)
        {
            return Instance.GetThrottle(dev, name);
        }
        public static int GetGenericIntByName(String dev, String name)
        {
            return Instance.GetGenericInt(dev, name);
        }
        public static float GetGenericFloatByName(String dev, String name)
        {
            return Instance.GetGenericFloat(dev, name);
        }
        public static bool GetBouncedButtonByName(String dev, String name)
        {
            return Instance.GetBouncedButton(dev, name);
        }
        public static bool GetButtonByName(String dev, String name)
        {
            return Instance.GetButton(dev, name);
        }
        public static Vector2 GetAxesByName(String dev, String name)
        {
            return Instance.GetAxes(dev, name);
        }
        public static HiHat GetHiHatByName(String dev, String name)
        {
            return Instance.GetHiHat(dev, name);
        }
        public static bool WasKeyPressedByName(char name)
        {
            return Instance.WasKeyPressed(name);
        }
        #endregion

        #region Player input mapping
        public static Vector2 GetPlayerAxes(String control)
        {
            if (Instance.ControlToDevice.ContainsKey(control))
            {
                String device = Instance.ControlToDevice[control];
                return GetAxesByName(device, control);
            }
            return Vector2.Zero;
        }

        public static HiHat GetPlayerHiHat(String control)
        {
            if (Instance.ControlToDevice.ContainsKey(control))
            {
                String device = Instance.ControlToDevice[control];
                return GetHiHatByName(device, control);
            }

            return null;
        }

        public static bool GetPlayerBouncedButton(String control, string name)
        {
            if (Instance.ControlToDevice.ContainsKey(control))
            {
                String device = Instance.ControlToDevice[control];
                return GetBouncedButtonByName(device, name);
            }
            return false;
        }

        public static bool GetPlayerButton(String control)
        {
            if (Instance.ControlToDevice.ContainsKey(control))
            {
                String device = Instance.ControlToDevice[control];
                return GetButtonByName(device, control);
            }
            return false;
        }

        /// <summary>
        /// Check to see if a input value has a mapped device input
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool HasMappedInput(String control)
        {
            return (Instance.ControlToDevice.ContainsKey(control));

        }

        /// <summary>
        /// Gets the type of a mapped input 
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static InputDescriptorType GetPlayerInputType(String control)
        {
            if (Instance.ControlToDevice.ContainsKey(control))
            {
                String d = Instance.ControlToDevice[control];
                return Instance.Devices[d].GetTypeOfControl(control);
            }
            
            return InputDescriptorType.None;
        }

        /// <summary>
        /// This is only used for the keyboard device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="Type"></param>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        public static void RegisterInput(String device, String name, InputDescriptorType Type, Keys key, List<Keys> modifiers)
        {
            KeyboardInputDevice kb = (KeyboardInputDevice)Instance.Devices[device];
            kb.AddKeyHandler(key, modifiers, name, Type);
            Instance.ControlToDevice.Add(name, device);
        }
        #endregion
    }
}
