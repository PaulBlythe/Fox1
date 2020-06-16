using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.InputDevices
{

    /// <summary>
    /// This is a helper class for other scenes.
    /// It monitors a device and reports any inputs that a user does.
    /// Normally this will only be used in none time critical code sections
    /// </summary>
    public class InputDeviceMonitor
    {
        List<bool> Buttonsv = new List<bool>();
        List<Vector2> Axesv = new List<Vector2>();
        List<HiHat> HiHatsv = new List<HiHat>();
        List<int> POVsv = new List<int>();
        List<byte> Wheelsv = new List<byte>();
        List<float> Throttlesv = new List<float>();
        List<int> GenericIntsv = new List<int>();
        List<float> GenericFloatsv = new List<float>();
        List<bool> DebouncedButtonsv = new List<bool>();


        List<string> Buttons = new List<string>();
        List<string> Axes = new List<string>();
        List<string> HiHats = new List<string>();
        List<string> POVs = new List<string>();
        List<string> Wheels = new List<string>();
        List<string> Throttles = new List<string>();
        List<string> GenericInts = new List<string>();
        List<String> GenericFloats = new List<string>();
        List<string> DebouncedButtons = new List<string>();

        String name;
        InputDevice host;

        public List<String> ChangedInputs = new List<string>();


        public InputDeviceMonitor(String DeviceName)
        {
            name = DeviceName;
            host = InputDeviceManager.GetInputDeviceByName(name);
            while (host.IsLocked())
            {
                Thread.Sleep(1);
            }
            host.Lock();
            foreach (string s in host.Buttons.Keys)
            {
                Buttons.Add(s);
                Buttonsv.Add(false);
            }
            foreach (string s in host.Axes.Keys)
            {
                Axes.Add(s);
                Axesv.Add(new Vector2());
            }
            foreach (string s in host.HiHats.Keys)
            {
                HiHats.Add(s);
                HiHatsv.Add(new HiHat());
            }
            foreach (string s in host.POVs.Keys)
            {
                POVs.Add(s);
                POVsv.Add(InputDeviceManager.GetPOVByName(name, s));
            }
            foreach (string s in host.Wheels.Keys)
            {
                Wheels.Add(s);
                Wheelsv.Add(InputDeviceManager.GetWheelByName(name, s));
            }
            foreach (string s in host.Throttles.Keys)
            {
                Throttles.Add(s);
                Throttlesv.Add(InputDeviceManager.GetThrottleByName(name, s));
            }
            foreach (string s in host.GenericInts.Keys)
            {
                GenericInts.Add(s);
                GenericIntsv.Add(InputDeviceManager.GetGenericIntByName(name, s));
            }
            foreach (string s in host.GenericFloats.Keys)
            {
                GenericFloats.Add(s);
                GenericFloatsv.Add(InputDeviceManager.GetGenericFloatByName(name, s));
            }
            foreach (string s in host.DebouncedButtons.Keys)
            {
                DebouncedButtons.Add(s);
                DebouncedButtonsv.Add(InputDeviceManager.GetBouncedButtonByName(name, s));
            }
            host.Unlock();
        }

        public void Update()
        {
            ChangedInputs.Clear();

            for (int i=0; i<Buttons.Count; i++)
            {
                bool b = InputDeviceManager.GetButtonByName(name, Buttons[i]);
                if (b != Buttonsv[i])
                {
                    ChangedInputs.Add(Buttons[i]);
                    Buttonsv[i] = b;
                }
            }

            for (int i = 0; i < Axes.Count; i++)
            {
                Vector2 b = InputDeviceManager.GetAxesByName(name, Axes[i]);
                Vector2 diff = b - Axesv[i];
                if (diff.Length()>0.05)
                {
                    ChangedInputs.Add(Axes[i]);
                    Axesv[i] = b;
                }
            }

            for (int i = 0; i < HiHats.Count; i++)
            {
                HiHat b = InputDeviceManager.GetHiHatByName(name, HiHats[i]);
                if (!b.Is(HiHatsv[i]))
                {
                    String src = HiHats[i];
                    if (b.Up != HiHatsv[i].Up)
                        src += "UP";
                    else if (b.Down != HiHatsv[i].Down)
                        src += "DOWN";
                    else if (b.Left != HiHatsv[i].Left)
                        src += "LEFT";
                    else if (b.Right != HiHatsv[i].Right)
                        src += "RIGHT";

                    ChangedInputs.Add(src);
                    HiHatsv[i].Set(b);
                }
            }

            for (int i = 0; i < POVs.Count; i++)
            {
                int b = InputDeviceManager.GetPOVByName(name, POVs[i]);
                if (b != POVsv[i])
                {
                    ChangedInputs.Add(POVs[i]);
                    POVsv[i] = b;
                }
            }

            for (int i = 0; i < Wheels.Count; i++)
            {
                byte b = InputDeviceManager.GetWheelByName(name, Wheels[i]);

                int db = b - Wheelsv[i];

                if (Math.Abs(db) >3)
                {
                    ChangedInputs.Add(Wheels[i]);
                    Wheelsv[i] = b;
                }
            }

            for (int i = 0; i < Throttles.Count; i++)
            {
                float b = InputDeviceManager.GetThrottleByName(name, Throttles[i]);
                float diff = Math.Abs(b - Throttlesv[i]);
                if (diff > 0.05)
                {
                    ChangedInputs.Add(Throttles[i]);
                    Throttlesv[i] = b;
                }
            }

            for (int i = 0; i < GenericInts.Count; i++)
            {
                int b =InputDeviceManager.GetGenericIntByName(name, GenericInts[i]);
                if (b != GenericIntsv[i])
                {
                    ChangedInputs.Add(GenericInts[i]);
                    GenericIntsv[i] = b;
                }
            }

            for (int i = 0; i < GenericFloats.Count; i++)
            {
                float b =InputDeviceManager.GetGenericFloatByName(name, GenericFloats[i]);
                if (b != GenericFloatsv[i])
                {
                    ChangedInputs.Add(GenericFloats[i]);
                    GenericFloatsv[i] = b;
                }
            }

            for (int i = 0; i < DebouncedButtons.Count; i++)
            {
                bool b = InputDeviceManager.GetBouncedButtonByName(name, DebouncedButtons[i]);
                if (b != DebouncedButtonsv[i])
                {
                    ChangedInputs.Add(DebouncedButtons[i]);
                    DebouncedButtonsv[i] = b;
                }
            }
        }

    }
}
