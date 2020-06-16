using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using HidLibrary;

namespace GuruEngine.InputDevices.FlightSticks
{
    public class X56HOTASThrottle : InputDevice
    {
        private HidDevice[] deviceList;
        private HidDevice RhinoStick;
        private HidFastReadDevice fastStick;
        private HiHat h1 = new HiHat();
        private HiHat h2 = new HiHat();

        Thread updateThread;
        public volatile bool scan = true;

        public override bool Connect()
        {
            deviceList = HidDevices.Enumerate().ToArray();
            for (int i = 0; i < deviceList.Length; i++)
            {
                if (deviceList[i].Description.StartsWith("X56 H.O.T.A.S. Throttle (HID)"))
                {
                    RhinoStick = deviceList[i];

                    HidFastReadEnumerator enumerator = new HidFastReadEnumerator();
                    fastStick = (HidFastReadDevice)enumerator.GetDevice(RhinoStick.DevicePath);

                    fastStick.OpenDevice();
                    fastStick.MonitorDeviceEvents = false;
                    fastStick.Inserted += Device_Inserted;
                    fastStick.Removed += Device_Removed;
                    fastStick.ReadReport(RhinoReadProcess);

                    Buttons.Add("EBUTTON", false);
                    Buttons.Add("IBUTTON", false);
                    Buttons.Add("HBUTTON", false);
                    Buttons.Add("SW1", false);
                    Buttons.Add("SW2", false);
                    Buttons.Add("SW3", false);
                    Buttons.Add("SW4", false);
                    Buttons.Add("SW5", false);
                    Buttons.Add("SW6", false);
                    Buttons.Add("TGL1UP", false);
                    Buttons.Add("TGL1DOWN", false);
                    Buttons.Add("TGL2UP", false);
                    Buttons.Add("TGL2DOWN", false);
                    Buttons.Add("TGL3UP", false);
                    Buttons.Add("TGL3DOWN", false);
                    Buttons.Add("TGL4UP", false);
                    Buttons.Add("TGL4DOWN", false);
                    Buttons.Add("K1UP", false);
                    Buttons.Add("K1DOWN", false);
                    Buttons.Add("CLICKWHEELUP", false);
                    Buttons.Add("CLICKWHEELDOWN", false);
                    Buttons.Add("SLD", false);

                    DebouncedButtons.Add("THUMBSTICK", new DebouncedButton());

                    HiHats.Add("H3", h1);
                    HiHats.Add("H4", h2);
                    
                    Axes.Add("THUMBSTICK", new Microsoft.Xna.Framework.Vector2(0, 0));

                    Wheels.Add("F", 0);
                    Wheels.Add("G", 0);
                    Wheels.Add("RTY4", 0);
                    Wheels.Add("RTY3", 0);

                    Throttles.Add("Left", 0);
                    Throttles.Add("Right", 0);

                    updateThread = new Thread(() => timer_elapsed(this));
                    updateThread.IsBackground = true;
                    updateThread.Start();

                    return true;
                }
            }
            return false;
        }
        private void Device_Inserted()
        {
        }
        private void Device_Removed()
        {
        }
        private static void timer_elapsed(X56HOTASThrottle parent)
        {
            bool con = parent.scan;

            while (con)
            {
                //try
                //{
                    con = parent.scan;
                    if (parent.fastStick != null)
                    {
                        HidDeviceData dat = parent.fastStick.FastRead(6);
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
                //}finally
                //{
                //    con = false;
                //    Thread.ResetAbort();
                //}
            }
            parent.fastStick.Dispose();
            parent.fastStick = null;
        }
        private void RhinoReadProcess(HidReport report)
        {
            ReadHandler(report);
        }
        /// <summary>
        /// Byte 0 Left throttle 0 = forward 
        /// Byte 1 bottom 4 bits left throttle
        ///        top 4 bits right throttle low nibble
        /// Byte 2 bottom 4 bits right throttle
        /// 
        /// Bit 16 E button
        /// Bit 128 I button
        /// 
        /// Byte 3
        /// Bit  1 H button
        /// Bit  2 Sw1 
        /// Bit  4 Sw2 
        /// Bit  8 Sw3 
        /// Bit 16 Sw4 
        /// Bit 32 Sw5 
        /// Bit 64 Sw6 
        /// Bit 128 tgl 1 up
        /// 
        /// Byte 4
        /// Bit  1 tgl 1 down
        /// Bit  2 tgl 2 up
        /// Bit  4 tgl 2 down
        /// Bit  8 tgl 3 up
        /// Bit 16 tgl 3 down
        /// Bit 32 tgl 4 up 
        /// Bit 64 tgl 4 down
        /// Bit 128 H3 up
        /// 
        /// Byte 5 
        /// Bit  1 H3 forward 
        /// Bit  2 H3 down
        /// Bit  4 H3 back
        /// Bit  8 H4 up 
        /// Bit 16 H4 forward
        /// Bit 32 H4 down
        /// Bit 64 h4 back
        /// 
        /// Bit 128 K1 up
        /// 
        /// Byte 6 
        /// Bit  1 K1 down
        /// Bit  2 Click wheel up
        /// Bit  4 Click wheel down 
        /// Bit  8 Thumbstick button
        /// Bit 16 SLD forward
        /// 
        /// Byte 7 Wheel F 
        /// Byte 8 Thumbstick X
        /// Byte 9 Wheel g
        /// Byte 10 Thumbstick Y
        /// Byte 11 RTY 4
        /// Byte 12 RTY 3
        /// 
        /// </summary>
        /// <param name="report"></param>
        private void ReadHandler(HidReport report)
        {
            bool pressed = (report.Data[2] & 16) != 0;
            Buttons["EBUTTON"] = pressed;
            pressed = (report.Data[2] & 128) != 0;
            Buttons["IBUTTON"] = pressed;
            pressed = (report.Data[3] & 1) != 0;
            Buttons["HBUTTON"] = pressed;

            pressed = (report.Data[6] & 2) != 0;
            Buttons["CLICKWHEELUP"] = pressed;
            pressed = (report.Data[6] & 4) != 0;
            Buttons["CLICKWHEELDOWN"] = pressed;

            pressed = (report.Data[3] & 2) != 0;
            Buttons["SW1"] = pressed;
            pressed = (report.Data[3] & 4) != 0;
            Buttons["SW2"] = pressed;
            pressed = (report.Data[3] & 8) != 0;
            Buttons["SW3"] = pressed;
            pressed = (report.Data[3] & 16) != 0;
            Buttons["SW4"] = pressed;
            pressed = (report.Data[3] & 32) != 0;
            Buttons["SW5"] = pressed;
            pressed = (report.Data[3] & 64) != 0;
            Buttons["SW6"] = pressed;

            pressed = (report.Data[3] & 128) != 0;
            Buttons["TGL1UP"] = pressed;
            pressed = (report.Data[4] & 1) != 0;
            Buttons["TGL1DOWN"] = pressed;
            pressed = (report.Data[4] & 2) != 0;
            Buttons["TGL2UP"] = pressed;
            pressed = (report.Data[4] & 4) != 0;
            Buttons["TGL2DOWN"] = pressed;
            pressed = (report.Data[4] & 8) != 0;
            Buttons["TGL3UP"] = pressed;
            pressed = (report.Data[4] & 16) != 0;
            Buttons["TGL3DOWN"] = pressed;
            pressed = (report.Data[4] & 32) != 0;
            Buttons["TGL4UP"] = pressed;
            pressed = (report.Data[4] & 64) != 0;
            Buttons["TGL4DOWN"] = pressed;

            pressed = (report.Data[5] & 128) != 0;
            Buttons["K1UP"] = pressed;
            pressed = (report.Data[6] & 1) != 0;
            Buttons["K1DOWN"] = pressed;

            pressed = (report.Data[4] & 128) != 0;
            HiHats["H3"].Up = pressed;
            pressed = (report.Data[5] & 1) != 0;
            HiHats["H3"].Right = pressed;
            pressed = (report.Data[5] & 2) != 0;
            HiHats["H3"].Down = pressed;
            pressed = (report.Data[5] & 4) != 0;
            HiHats["H3"].Left = pressed;

            pressed = (report.Data[5] & 8) != 0;
            HiHats["H4"].Up = pressed;
            pressed = (report.Data[5] & 16) != 0;
            HiHats["H4"].Right = pressed;
            pressed = (report.Data[5] & 32) != 0;
            HiHats["H4"].Down = pressed;
            pressed = (report.Data[5] & 64) != 0;
            HiHats["H4"].Left = pressed;

            pressed = (report.Data[6] & 16) != 0;
            Buttons["SLD"] = pressed;
            pressed = (report.Data[6] & 8) != 0;
            DebouncedButtons["THUMBSTICK"].Down = pressed;

            float x = (report.Data[8] - 128.0f) / 128.0f;
            float y = (report.Data[10] - 128.0f) / 128.0f;
            Axes["THUMBSTICK"] = new Vector2(x, y);

            Wheels["F"] = report.Data[7];
            Wheels["G"] = report.Data[9];
            Wheels["RTY4"] = report.Data[11];
            Wheels["RTY3"] = report.Data[12];
            
            int t2 = (report.Data[0] & 255);
            int t3 = (report.Data[1] & 3);
            int t4 = t2 + (256 * t3);
            int t5 = 1024 - t4;
            float t2f = t5 / 1024.0f;
            Throttles["Left"] = t2f;

            t2 = (report.Data[1] & 255); 
            t3 = (report.Data[2] & 15);
            t4 = t2 + (256 * t3);
            t5 = 4096 - t4;
            t2f = t5 / 4096.0f;
            Throttles["Right"] = t2f;

        }

        public override bool Update()
        {
            return false;
        }

        public override void CloseDown()
        {
            scan = false;
            //updateThread.Abort();
            //fastStick.Dispose();
            //fastStick = null;
        }
    }
}

