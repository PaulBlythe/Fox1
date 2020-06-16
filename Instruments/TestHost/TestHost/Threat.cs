using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHost
{
    public class Threat
    {
        public float Bearing;
        public int Type;
        public float ThreatLevel;
        public bool Identified = true;

        public String Format()
        {
            String res = String.Format("b:{0},t:{1},l:{2},i:{3}", Bearing, Type, ThreatLevel, Identified);
            return res;
        }
    }
}
