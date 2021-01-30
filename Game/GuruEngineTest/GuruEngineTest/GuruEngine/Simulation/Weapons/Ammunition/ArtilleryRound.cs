using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Simulation.Weapons.Ammunition
{
    public class ArtilleryRound
    {
        public float LiveTime;
        public float Mass;
        public float Power;
        public float Caliber;
        public bool Fused;
        public int FuseType;
        public bool Explodes;
        public float ExplosionRadius;

        //==========================================
        //== fuse types                           ==
        //==========================================
        //==  0 ==  Impact only                   ==
        //==  1 ==  Timed                         ==
        //==  2 ==  Altitude                      ==
        //==  3 ==  Proximity                     ==
        //==========================================
    }
}
