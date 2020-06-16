using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.World
{
    public static class Constants
    {
        public const double MTR_TO_DEG_LAT = 0.000008999280057595392;
        public const double DEG_TO_RAD = 0.0174532925199432958;
        public const double RAD_TO_DEG = 57.29577951308232;
        public const double FEET_TO_MTR = 0.3048;
        public const double DEG_TO_MTR_LAT = 111120.0;
        public const double EQU_RAD = 6378137.0;
        public const double FLATTENING = 298.257223563;
        public const double SQUASH = 0.9966471893352525192801545;
        public const double E2 = (1 - SQUASH * SQUASH);
        public const double RA2 = 1.0 / (EQU_RAD * EQU_RAD);
        public const double E4 = E2 * E2;
    }
}
