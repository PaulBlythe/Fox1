using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Physics.World
{
    public class Constants
    {
        public static double M_PI = 3.1415926535897932384626433832795;

        public const double radtodeg = 57.29578;
        public const double degtorad = 1.745329E-2;
        public const double hptoftlbssec = 550.0;
        public const double psftoinhg = 0.014138;
        public const double psftopa = 47.88;
        public const double fpstokts = 0.592484;
        public const double ktstofps = 1.68781;
        public const double inchtoft = 0.08333333;
        public const double in3tom3 = 1.638706E-5;
        public const double m3toft3 = 1.0 / (0.3048 * 0.3048 * 0.3048);
        public const double inhgtopa = 3386.38;
        public const double fttom = 0.3048;
        public const double mtoft = 3.2808399;
        public const double Reng = 1716.0;
        public const double SHRatio = 1.40;
        public const double fpstokmh = 1.09728;
        public const float KTS2MPS = 1852.0f / 3600.0f;
        public const float MPS2KTS = 3600.0f / 1852.0f;
        public const float KMH2MPS = 1 / 3.6f;
        public const float RPM2RAD = (float)(3.1415926535897932384626433832795f / 30.0f);
        public const float LBS2KG = 0.45359237f;

        public const double SutherlandConstant = 198.72;          // deg Rankine
        public const double Beta = 2.269690E-08;                  // slug/(sec ft R^0.5)

        public const double slugtolb = 32.174049;
        public const double lbtoslug = 1.0 / slugtolb;
        public const double kgtolb = 2.20462;
        public const double kgtoslug = 0.06852168;
        public const double DEFAULT_TOLERANCE = 0.001;

        // Earth defaults
        public const double RotationRate = 0.00007292115;
        public const double GM = 14.07644180E15;     // WGS84 value
        public const double RadiusReference = 20925650.00;        // Equatorial radius (WGS84)
        public const double C2_0 = -4.84165371736E-04; // WGS84 value for the C2,0 coefficient
        public const double J2 = 1.0826266836E-03;   // WGS84 value for J2
        public const double a = 20925646.3255;      // WGS84 semimajor axis length in feet 
        public const double b = 20855486.5951;      // WGS84 semiminor axis length in feet
        public const double earthPosAngle = 0.0;

        public const int ePhi = 0;          // Roll
        public const int eTht = 1;          // Pitch
        public const int ePsi = 2;          // Heading

        public const int eNorth = 0;
        public const int eEast = 1;
        public const int eDown = 2;

        public const int eDrag = 0;
        public const int eLift = 2;
        public const int eSide = 1;

        // Engine defaults
        public const int MAX_BOOST_SPEEDS = 3;


        public const double R_air = 287.3;                  // Gas constant for air J/Kg/K
        public const double rho_fuel = 800;                 // estimate
        public const double calorific_value_fuel = 47.3e6;
        public const double Cp_air = 1005;                  // Specific heat (constant pressure) J/Kg/K
        public const double Cp_fuel = 1700;

        public const double mm = 0.0289644;		            // [kg/mole] molar mass of air (dry?)
        public const double Rgas = 8.31432;		            // [J/K/mole] gas constant
        public const double g = 9.80665;		            // [m/s/s] acceleration of gravity

        public const double P0 = 101325.0; 		            // [pascals] ISA sea-level pressure
        public const double T0 = 15.0 + 273.15;	            // [K] ISA sea-level temperature
        public const double lam0 = 0.0065;		            // [K/m] ISA troposphere lapse rate
    }
}
