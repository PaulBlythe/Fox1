using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace GuruEngine.Data.Code
{
    public class Airfield
    {
        public double Latitude;
        public double Longitude;
        public String Name;

        public void Load(String line)
        {
            String[] parts = line.Split(',');
            Name = parts[0];
            Latitude = double.Parse(parts[1]);
            Longitude = double.Parse(parts[2]);

        }
    }
}
