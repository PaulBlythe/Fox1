using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using HidLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.InputDevices.FlightSticks
{
    public class X56HOTASStick : InputDevice
    {
        private HidDevice[] deviceList;
        private HidDevice RhinoStick;
        private HidFastReadDevice fastStick;

        private HiHat h1 = new HiHat();
        private HiHat h2 = new HiHat();

        Thread updateThread;
        volatile bool scan = true;

        public override bool Connect()
        {
            deviceList = HidDevices.Enumerate().ToArray();
            for (int i = 0; i < deviceList.Length; i++)
            {
                if (deviceList[i].Description.StartsWith("X56 H.O.T.A.S. Stick (HID)"))
                {
                    RhinoStick = deviceList[i];

                    HidFastReadEnumerator enumerator = new HidFastReadEnumerator();
                    fastStick = (HidFastReadDevice)enumerator.GetDevice(RhinoStick.DevicePath);

                    
                    fastStick.OpenDevice();
                    fastStick.MonitorDeviceEvents = false;
                    fastStick.Inserted += Device_Inserted;
                    fastStick.Removed += Device_Removed;
                    fastStick.FastReadReport(RhinoReadProcess);

                    Buttons.Add("ABUTTON", false);
                    Buttons.Add("BBUTTON", false);
                    Buttons.Add("FIRE", false);
                    Buttons.Add("PINKY", false);

                    DebouncedButtons.Add("THUMBSTICK", new DebouncedButton());

                    HiHats.Add("H1", h1);
                    HiHats.Add("H2", h2);

                    POVs.Add("POV1", 0);

                    Axes.Add("FLIGHTSTICK", new Microsoft.Xna.Framework.Vector2(0, 0));
                    Axes.Add("THUMBSTICK", new Microsoft.Xna.Framework.Vector2(0, 0));

                    updateThread = new Thread(() => timer_elapsed(this));
                    updateThread.IsBackground = true;
                    updateThread.Start();
                    
                    return true;
                }
            }
            return false;
        }
        public override void CloseDown()
        {
            scan = false;
        }
        private void Device_Inserted()
        {
        }
        private void Device_Removed()
        {
        }

        private static void timer_elapsed(X56HOTASStick parent)
        {
            bool con = parent.scan;
            while (con)
            {
                //try
                //{
                    con = parent.scan;
                    if (parent.fastStick != null)
                    {

                        HidDeviceData dat = parent.fastStick.FastRead();
                        if (dat.Status == HidDeviceData.ReadStatus.Success)
                        {
                            HidReport report = new HidReport(parent.fastStick.Capabilities.InputReportByteLength, dat);
                            parent.RhinoReadProcess(report);
                        }
                    }else
                    {
                        con = false;
                    }
                    Thread.Sleep(8);
                //}catch(ThreadAbortException)
                //{
                //    con = false;
                //    return;
                //}finally {
                //    con = false;
                //}
            }
            parent.fastStick.Dispose();
        }


        public void RhinoReadProcess(HidReport report)
        {
            ReadHandler(report);
        }
        /// <summary>
        /// Byte 0 - 3 Joystick position 16 bit centre 8000
        /// Byte 4 - 5  Joystick twist position 12 bit centre 800
        /// Byte 5 top 4 bits POV 8 directions
        /// Byte 6 
        /// Bit  1 = Fire
        /// Bit  2 = A button
        /// Bit  4 = B button
        /// Bit  8 = Thumbstick depressed
        /// Bit  16 = 
        /// Bit  32 = Pinky
        /// Bit  64 = H1 Up
        /// Bit 128 = H1 right
        /// Byte 7
        /// Bit  1 = H1 down
        /// Bit  2 = H1 left
        /// Bit  4 = H2 up
        /// Bit  8 = H2 right
        /// Bit 16 = H2 down
        /// Bit 32 = H2 left
        /// Bit 64 = 
        /// Bit 128 =
        /// Byte 9 and 10 Thumstick position (8 bit centre 80)
        /// </summary>
        /// <param name="report"></param>
        private void ReadHandler(HidReport report)
        {
            int pov = (report.Data[5] & 0xF0) >> 4;
            POVs["POV1"] = pov;

            bool pressed = (report.Data[6] & 1) != 0;
            Buttons["FIRE"] = pressed;

            pressed = (report.Data[6] & 2) != 0;
            Buttons["ABUTTON"] = pressed;

            pressed = (report.Data[6] & 4) != 0;
            Buttons["BBUTTON"] = pressed;

            pressed = (report.Data[6] & 8) != 0;
            DebouncedButtons["THUMBSTICK"].Down = pressed;

            pressed = (report.Data[6] & 32) != 0;
            Buttons["PINKY"] = pressed;

            pressed = (report.Data[6] & 64) != 0;
            HiHats["H1"].Up = pressed;
            pressed = (report.Data[6] & 128) != 0;
            HiHats["H1"].Right = pressed;
            pressed = (report.Data[7] & 1) != 0;
            HiHats["H1"].Down = pressed;
            pressed = (report.Data[7] & 2) != 0;
            HiHats["H1"].Left = pressed;

            pressed = (report.Data[7] & 4) != 0;
            HiHats["H2"].Up = pressed;
            pressed = (report.Data[7] & 8) != 0;
            HiHats["H2"].Right = pressed;
            pressed = (report.Data[7] & 16) != 0;
            HiHats["H2"].Down = pressed;
            pressed = (report.Data[7] & 32) != 0;
            HiHats["H2"].Left = pressed;


            ushort sx = (ushort)((ushort)(report.Data[0]) + (ushort)(256 * ((ushort)report.Data[1])));
            ushort sy = (ushort)((ushort)(report.Data[2]) + (ushort)(256 * ((ushort)report.Data[3])));
            float x = sx - 32768;
            float y = sy - 32768;
            Vector2 v2 = new Vector2(x / 32768.0f, y / 32768.0f);
            Axes["FLIGHTSTICK"] = v2;

            x = (report.Data[9] - 128.0f) / 128.0f;
            y = (report.Data[10] - 128.0f) / 128.0f;
            Axes["THUMBSTICK"] = new Vector2(x, y);

        }
        public override bool Update()
        {
            return false;
        }
    }
}
