using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using HidLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.InputDevices.FlightSticks
{
    public class LogitechRudderPedals : InputDevice
    {
        private HidDevice[] deviceList;
        private HidDevice RudderPedals;
        private HidFastReadDevice fastStick;

        Thread updateThread;
        volatile bool scan = true;

        public override bool Connect()
        {
            deviceList = HidDevices.Enumerate().ToArray();
            for (int i = 0; i < deviceList.Length; i++)
            {
                if (deviceList[i].Description.StartsWith("Flight Rudder Pedals"))
                {
                    RudderPedals = deviceList[i];

                    HidFastReadEnumerator enumerator = new HidFastReadEnumerator();
                    fastStick = (HidFastReadDevice)enumerator.GetDevice(RudderPedals.DevicePath);


                    fastStick.OpenDevice();
                    fastStick.MonitorDeviceEvents = false;
                    fastStick.Inserted += Device_Inserted;
                    fastStick.Removed += Device_Removed;
                    fastStick.FastReadReport(RhinoReadProcess);

                    GenericInts.Add("LEFTBRAKE", 0);
                    GenericInts.Add("RIGHTBRAKE", 0);
                    GenericFloats.Add("RUDDER", 0);

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
            //updateThread.Abort();
            //updateThread.Join();
            //fastStick.Dispose();
            //fastStick = null;

        }
        private void Device_Inserted()
        {
        }
        private void Device_Removed()
        {
        }

        private static void timer_elapsed(LogitechRudderPedals parent)
        {
            bool com = parent.scan;
            while (com)
            {
                //try
                //{
                    com = parent.scan;
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
                        com = false;
                    }
                    Thread.Sleep(8);
                //}catch (ThreadAbortException)
                //{
                //    com = false;
                //}finally{
                //    com = false;
                //}
            }
            parent.fastStick.Dispose();
        }


        public void RhinoReadProcess(HidReport report)
        {
            ReadHandler(report);
        }
        /// <summary>
        /// Byte 0 7 bits left brake
        /// Byte 0 1 bit right brake
        /// Byte 1 6 bits right brake
        /// Byte 1 2 bits rudder
        /// Bit  2 8 bits rudder
        /// </summary>
        /// <param name="report"></param>
        private void ReadHandler(HidReport report)
        {
            int pov = (report.Data[0] & 0x7f);
            GenericInts["LEFTBRAKE"] = pov;

            pov = (report.Data[0] & 0x80) >> 7;
            pov += (report.Data[1] & 0x3f) << 1;
            GenericInts["RIGHTBRAKE"] = pov;

            pov = (report.Data[1] & 0xC0) >> 6;
            pov += (report.Data[2]) << 2;

            float val = ((pov / 512.0f) - 0.5f) * 2.0f;
            GenericFloats["RUDDER"] = val;
  
        }
        public override bool Update()
        {

            return false;
        }
    
    }
}
