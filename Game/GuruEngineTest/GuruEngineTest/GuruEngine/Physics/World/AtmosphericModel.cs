using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Physics.World
{
    public struct AtmosphericModelResults
    {
        public double Pressure;
        public double Temperature;
        public double Soundspeed;           // feet per second
        public double Density;
    }
    public abstract class AtmosphericModel
    {
        public abstract AtmosphericModelResults Update(double height);
    }
}
