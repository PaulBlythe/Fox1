using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Data.Code
{
    public class Squadron
    {
        public String Name;
        public int Airframes;
        public int ServiceableAirframes;
        public int Pilots;
        public String AircraftCode;

        public void Load(String line)
        {
            String[] parts = line.Split(',');
            Name = parts[0];
            Airframes = int.Parse(parts[1]);
            ServiceableAirframes = int.Parse(parts[2]);
            Pilots = int.Parse(parts[3]);
            AircraftCode = parts[4];

        }
    }
}
