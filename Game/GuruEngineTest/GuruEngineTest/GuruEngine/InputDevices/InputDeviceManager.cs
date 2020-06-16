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

        public Dictionary<String, InputDevice> Devices = new Dictionary<string, InputDevice>();
        KeyboardState lastState;
        KeyboardState currentState;
        public PlayerInputMap playerMap;


        public InputDeviceManager()
        {
            Instance = this;
            playerMap = new PlayerInputMap();
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
        public static Vector2 GetPlayerAxes(PlayerInputValue control)
        {
            switch (control)
            {
                case PlayerInputValue.MFDMasterInput:
                    {
                        if (Instance.playerMap.MFDMasterInput.Device != "")
                        {
                            return GetAxesByName(Instance.playerMap.MFDMasterInput.Device, Instance.playerMap.MFDMasterInput.Input);
                        }
                    }
                    break;
            }
            return Vector2.Zero;
        }

        public static HiHat GetPlayerHiHat(PlayerInputValue control)
        {
            switch (control)
            {
                case PlayerInputValue.MFDMasterSelectDown:
                    return GetHiHatByName(Instance.playerMap.MFDMasterSelectDown.Device, Instance.playerMap.MFDMasterSelectDown.Input);
                case PlayerInputValue.MFDMasterSelectUp:
                    return GetHiHatByName(Instance.playerMap.MFDMasterSelectUp.Device, Instance.playerMap.MFDMasterSelectUp.Input);
                case PlayerInputValue.MFDSlaveSelectUp:
                    return GetHiHatByName(Instance.playerMap.MFDSlaveSelectUp.Device, Instance.playerMap.MFDSlaveSelectUp.Input);
                case PlayerInputValue.MFDSlaveSelectDown:
                    return GetHiHatByName(Instance.playerMap.MFDSlaveSelectDown.Device, Instance.playerMap.MFDSlaveSelectDown.Input);
            }
            return null;
        }

        public static bool GetPlayerBouncedButton(PlayerInputValue control, string name)
        {
            switch (control)
            {
                case PlayerInputValue.MFDMasterInput:
                    return Instance.GetBouncedButton(Instance.playerMap.MFDMasterInput.Device, name);
                case PlayerInputValue.MFDSlaveInput:
                    return Instance.GetBouncedButton(Instance.playerMap.MFDSlaveInput.Device, name);
            }
            return false;
        }

        /// <summary>
        /// Check to see if a input value has a mapped device input
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool HasMappedInput(PlayerInputValue control)
        {
           switch (control)
            {
                case PlayerInputValue.MFDMasterInput:
                    {
                        return (Instance.playerMap.MFDMasterInput.Device != "");
                    }
                case PlayerInputValue.MFDMasterSelectDown:
                    {
                        return (Instance.playerMap.MFDMasterSelectDown.Device != "");
                    }
                case PlayerInputValue.MFDMasterSelectUp:
                    {
                        return (Instance.playerMap.MFDMasterSelectUp.Device != "");
                    }
                case PlayerInputValue.MFDSlaveInput:
                    {
                        return (Instance.playerMap.MFDSlaveInput.Device != "");
                    }
                case PlayerInputValue.MFDSlaveSelectDown:
                    {
                        return (Instance.playerMap.MFDSlaveSelectDown.Device != "");
                    }
                case PlayerInputValue.MFDSlaveSelectUp:
                    {
                        return (Instance.playerMap.MFDSlaveSelectUp.Device != "");
                    }
            }
            return false;
        }

        /// <summary>
        /// Gets the type of a mapped input 
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static InputDescriptorType GetPlayerInputType(PlayerInputValue control)
        {
            switch (control)
            {
                case PlayerInputValue.MFDMasterInput:
                    return Instance.playerMap.MFDMasterInput.Type;
                case PlayerInputValue.MFDMasterSelectDown:
                    return Instance.playerMap.MFDMasterSelectDown.Type;
                case PlayerInputValue.MFDMasterSelectUp:
                    return Instance.playerMap.MFDMasterSelectUp.Type;
                case PlayerInputValue.MFDSlaveInput:
                    return Instance.playerMap.MFDSlaveInput.Type;
                case PlayerInputValue.MFDSlaveSelectDown:
                    return Instance.playerMap.MFDSlaveSelectDown.Type;
                case PlayerInputValue.MFDSlaveSelectUp:
                    return Instance.playerMap.MFDSlaveSelectUp.Type;
            }
            return InputDescriptorType.None;
        }
        #endregion
    }
}
