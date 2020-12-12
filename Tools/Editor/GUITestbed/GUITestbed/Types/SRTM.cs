using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace GUITestbed.Types
{
    public class SRTM
    {
        public int Size;
        public String FileName;

        public SRTM(String Dir, int Latitude, int Longitude)
        {
            FileName = GetFileName(Latitude, Longitude);
            string fp = Path.Combine(Dir, FileName);
            if (!File.Exists(fp))
            {

            }
        }

        private String GetFileName(double Latitude, double Longitude)
        {
            String NS, EW;
            NS = "S";

            if (Latitude > 0)
                NS = "N";
            EW = "E";
            if (Longitude < 0)
                EW = "W";

            return String.Format("{0}{1:00}{2}{3:000}.hgt", NS, Math.Abs(Latitude), EW, Math.Abs(Longitude));
        }
    }
}
