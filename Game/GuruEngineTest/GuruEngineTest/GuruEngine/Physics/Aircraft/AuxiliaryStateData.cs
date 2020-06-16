using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.ECS;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Aircraft
{
    public class AuxiliaryStateData
    {
        public double TotalTemperature;
        public double Velocity;             // Metres per second
        public double Mach;
        public double Qbar;
        public double AltitudeASL;
        public double DensityRatio;

        public void Update(GameObject parent)
        {
            Mach = Velocity * 3.28084 / parent.atmosphereAltitude.Soundspeed;
            TotalTemperature = parent.atmosphereAltitude.Temperature * (1 + 0.2 * Mach * Mach); // Total Temperature, isentropic flow

            Qbar = 0.5 * parent.atmosphereAltitude.Density * Velocity * Velocity;
            DensityRatio = parent.atmosphereAltitude.Density / parent.atmosphereSeaLevel.Density;
        }
    }
}
