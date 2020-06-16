using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GuruEngine.Assets;

namespace GuruEngine.DebugHelpers
{
    public class DebugRenderer
    {
        private static DebugFlags flags = new DebugFlags();

        private static double display_width = 1920;
        private static double display_region = 20.0;
        private static double display_start = 0;
        private static int oldScrollWheelValue = 0;
        

        static DebugRenderer()
        {
            flags = new DebugFlags();
        }
        public static void Zoom(int dt, float x, float y)
        {
            if (dt == 0)
                return;

            double cx = display_start + (display_region * x / display_width);
            double z = 1;
            if (dt > 0)
            {
                z += (dt / 1000.0);
            }
            else
            {
                z += (dt / 1000.0);
            }
            display_region *= z;
        }
        public static void Render(SpriteBatch batch)
        {
            if (DebugFlags.RenderProfileData)
            {
                MouseState ms = Mouse.GetState();
                int mstep = ms.ScrollWheelValue - oldScrollWheelValue;
                oldScrollWheelValue = ms.ScrollWheelValue;
                DebugRenderer.Zoom(mstep, ms.Position.X, ms.Position.Y);

                ProfileReport report = Profiler.CreateReport();

                int cpuCount = Environment.ProcessorCount;

                batch.Begin();

                Rectangle fs = new Rectangle(0, 0, 1920, 1080);
                batch.Draw(AssetManager.GetWhite(), fs, new Color(0, 0, 0, 128));

                double dt = 0;
                int sy = 10;
                for (int i=0; i<cpuCount; i++)
                {
                    foreach (ProfileReportEntry e in report.Entries)
                    {
                        if (e.Core == i + 1)
                        {
                            int sx = (int)(dt * display_width / display_region);
                            dt += e.ExecutionTime / Stopwatch.Frequency;
                            int ex = (int)(dt * display_width / display_region);
                            batch.Draw(AssetManager.GetWhite(), new Rectangle(10 + sx,sy,(ex-sx)+1,20), new Color(255, 128, 0, 128));
                            batch.DrawString(AssetManager.GetDebugFont(), e.Name, new Vector2(12 + sx, sy), Color.Black);
                        }
                    }
                    sy += 32;
                    
                }

                batch.DrawString(AssetManager.GetDebugFont(), String.Format("Timespan {0}", display_region), Vector2.Zero, Color.White);
                batch.End();

            }
        }
    }
}
