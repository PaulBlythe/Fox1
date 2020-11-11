using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.World.Aircraft
{
    public static class AircraftNames
    {
        public static String GetAircraftName(AircraftTypes type)
        {
            switch (type)
            {
                case AircraftTypes.SpitfireMkI:
                    return "Spitfire 1";
                case AircraftTypes.SpitfireMkII:
                    return "Spitfire 2";
                case AircraftTypes.SpitfireMkIII:
                    return "Spitfire 3";
                case AircraftTypes.SpitfireMkIV:
                    return "Spitfire 4";
                case AircraftTypes.SpitfireMkV:
                    return "Spitfire 5";
                case AircraftTypes.SpitfireMkVI:
                    return "Spitfire 6";
                case AircraftTypes.SpitfireMkVII:
                    return "Spitfire 7";
                case AircraftTypes.SpitfireMkVIII:
                    return "Spitfire 8";
                case AircraftTypes.SpitfireMkIX:
                    return "Spitfire 9";
                case AircraftTypes.SpitfireMkX:
                    return "Spitfire 10";
               


            }
            return "Unknown aircraft";
        }
    }
}
