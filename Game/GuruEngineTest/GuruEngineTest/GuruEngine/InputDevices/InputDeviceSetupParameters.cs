using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;

namespace GuruEngine.InputDevices
{
    public class InputDeviceSetupParameters
    {
        public Dictionary<String, Vector2> ScrollPositions = new Dictionary<string, Vector2>();
        public Dictionary<String, Rectangle> HiLightRegions = new Dictionary<string, Rectangle>();
        public Vector2 StartPos;

#if DEBUG
        /// <summary>
        /// Helper function just for development
        /// </summary>
        /// <param name="devname"></param>
        public void Serialize(String devname)
        {
            String filename = GuruEngine.FilePaths.DevicesPath;
            filename = Path.Combine(filename,devname);
            filename = Path.Combine(filename,"layout.txt");

            TextWriter writeFile = new StreamWriter(filename);
            writeFile.WriteLine(String.Format("ScrollPositions {0}", ScrollPositions.Count));
            foreach (String s in ScrollPositions.Keys)
            {
                Vector2 pos = ScrollPositions[s];
                writeFile.WriteLine(String.Format("{0} {1} {2}", s,pos.X,pos.Y));
            }
            writeFile.WriteLine(String.Format("HiLightRegions {0}", HiLightRegions.Count));
            foreach (String s in HiLightRegions.Keys)
            {
                Rectangle pos = HiLightRegions[s];
                writeFile.WriteLine(String.Format("{0} {1} {2} {3} {4}", s, pos.X,pos.Y,pos.Width,pos.Height));
            }

            writeFile.WriteLine(String.Format("ScreenPosition {0} {1}", StartPos.X, StartPos.Y));
            writeFile.Close();
        }

        public void DeSerialise(String devname)
        {
            ScrollPositions.Clear();
            HiLightRegions.Clear();

            String fn = Settings.GetInstance().GameRootDirectory;
            fn += "/Devices/";
            fn += devname;
            fn += "/layout.txt";

            TextReader r = new StreamReader(fn);
            String line;
            String[] parts;

            line = r.ReadLine();
            parts = line.Split(' ');
            int nScroll = int.Parse(parts[1]);
            for (int i=0; i< nScroll; i++)
            {
                line = r.ReadLine();
                parts = line.Split(' ');
                Vector2 p = new Vector2();
                p.X = float.Parse(parts[1]);
                p.Y = float.Parse(parts[2]);
                ScrollPositions.Add(parts[0], p);
            }

            line = r.ReadLine();
            parts = line.Split(' ');
            int nRegions = int.Parse(parts[1]);
            for (int i = 0; i < nRegions; i++)
            {
                line = r.ReadLine();
                parts = line.Split(' ');
                Rectangle p = new Rectangle();
                p.X = int.Parse(parts[1]);
                p.Y = int.Parse(parts[2]);
                p.Width = int.Parse(parts[3]);
                p.Height = int.Parse(parts[4]);
                HiLightRegions.Add(parts[0], p);
            }

            StartPos = new Vector2();
            line = r.ReadLine();
            parts = line.Split(' ');
            StartPos.X = float.Parse(parts[1]);
            StartPos.Y = float.Parse(parts[2]);

            r.Close();

        }
#endif
    }
}
