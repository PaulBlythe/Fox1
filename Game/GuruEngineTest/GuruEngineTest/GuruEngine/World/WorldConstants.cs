using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GuruEngine.World
{
    public static class WorldConstants
    {
        public const double EQU_RAD = 6378137.0;
        public const double FLATTENING = 298.257223563;
        public const double SQUASH = 0.9966471893352525192801545;
        public const double E2 = (1 - SQUASH * SQUASH);
        public const double RA2 = 1.0 / (EQU_RAD * EQU_RAD);
        public const double E4 = E2 * E2;
       

    }
}
