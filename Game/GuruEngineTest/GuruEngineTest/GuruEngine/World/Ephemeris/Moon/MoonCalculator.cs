using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace GuruEngine.World.Ephemeris
{
    public class MoonCalculator
    {
        private const double rad = Math.PI / 180.0;
        private const double J2000 = 2451545;
        private const double dayMs = 86400000;
        private const double J1970 = 2440588;
        private const double e = rad * 23.4397; // obliquity of the Earth

        /// <summary>
        /// Gets the moon position
        /// </summary>
        /// <param name="dt">Date time </param>
        /// <param name="lat">Latitude</param>
        /// <param name="lng">Longitude</param>
        /// <returns>
        /// X == Azimuth
        /// Y == Altitude
        /// Z == Distance
        /// W == Parallactic Angle 
        /// </returns>
        public static Vector4 GetMoonPosition(DateTime dt, double lat, double lng)
        {
            double lw = rad * -lng;
            double phi = rad * lat;
            double d = JulianDays(dt);

            Vector3 c = MoonCoords(d);
            double H = SiderealTime(d, lw) - c.X;
            double h = Altitude(H, phi, c.Y);
            // formula 14.1 of "Astronomical Algorithms" 2nd edition by Jean Meeus (Willmann-Bell, Richmond) 1998.
            double pa = Math.Atan2(Math.Sin(H), Math.Tan(phi) * Math.Cos(c.Y) - Math.Sin(c.Y) * Math.Cos(H));

            h += AstroRefraction(h); // altitude correction for refraction
            return new Vector4((float)Azimuth(H, phi, c.Y), (float)h, c.Z, (float) pa );
        }


        public static double JulianDays(DateTime dt)
        {
            dt = dt.Kind == DateTimeKind.Local ? dt.ToUniversalTime() : dt;
            return ToJulianDate(dt) - J2000;
        }

        public static double ToJulianDate(DateTime dt)
        {
            dt = dt.Kind == DateTimeKind.Local ? dt.ToUniversalTime() : dt;
            return (dt - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / dayMs - 0.5 + J1970;
        }
        public static double Azimuth(double H, double phi, double dec)
        {
            return Math.Atan2(Math.Sin(H), Math.Cos(H) * Math.Sin(phi) - Math.Tan(dec) * Math.Cos(phi));
        }

        public static double SiderealTime(double d, double lw)
        {
            return rad * (280.16 + 360.9856235 * d) - lw;
        }

        public static double AstroRefraction(double h)
        {
            if (h < 0) // the following formula works for positive altitudes only.
            {
                h = 0; // if h = -0.08901179 a div/0 would occur.
            }

            // formula 16.4 of "Astronomical Algorithms" 2nd edition by Jean Meeus (Willmann-Bell, Richmond) 1998.
            // 1.02 / tan(h + 10.26 / (h + 5.10)) h in degrees, result in arc minutes -> converted to rad:
            return 0.0002967 / Math.Tan(h + 0.00312536 / (h + 0.08901179));
        }

        public static double Altitude(double H, double phi, double dec)
        {
            return Math.Asin(Math.Sin(phi) * Math.Sin(dec) + Math.Cos(phi) * Math.Cos(dec) * Math.Cos(H));
        }

        /// <summary>
        /// geocentric ecliptic coordinates of the moon
        /// </summary>
        /// <param name="d"></param>
        /// <returns>
        /// X == Ascension
        /// Y == Declination
        /// Z == Distance in km
        /// </returns>
        public static Vector3 MoonCoords(double d) 
        {
            double L = rad * (218.316 + 13.176396 * d); // ecliptic longitude
            double M = rad * (134.963 + 13.064993 * d); // mean anomaly
            double F = rad * (93.272 + 13.229350 * d);  // mean distance

            double l = L + rad * 6.289 * Math.Sin(M); // longitude
            double b = rad * 5.128 * Math.Sin(F);     // latitude
            double dt = 385001 - 20905 * Math.Cos(M);  // distance to the moon in km

            return new Vector3((float)RightAscension(l, b), (float)Declination(l, b), (float)dt);
        }

        public static double RightAscension(double l, double b)
        {
            return Math.Atan2(Math.Sin(l) * Math.Cos(e) - Math.Tan(b) * Math.Sin(e), Math.Cos(l));
        }

        public static double Declination(double l, double b)
        {
            return Math.Asin(Math.Sin(b) * Math.Cos(e) + Math.Cos(b) * Math.Sin(e) * Math.Sin(l));
        }

        /// <summary>
        /// Return an angle in the range of 0 -> 2PI Radians
        /// </summary>
        /// <returns>Angle in the range 0 - 2PI</returns>
        /// <param name="_angle">Angle to convert</param>
        private static double Mod2Pi(double _angle)
        {
            double b = _angle / (2.0f * Math.PI);
            double a = (2.0f * Math.PI) * (b - Math.Floor((float)b));
            if (a < 0) a = (2.0f * Math.PI) + a;
            return a;
        }

        public static Vector3 GetSunDirection(DateTime when)
        {
            double a = when.DayOfYear / 365.0;
            a = a * Math.PI * 2;
            double xe = Math.Cos(a);
            double ye = Math.Sin(a);

            ////1. Days elapsed since J2000 (1st january 2000 at 12:00)
            //DateTime epoch = new DateTime(2000, 1, 1, 12, 0, 0);
            //TimeSpan j2000TS = when.ToUniversalTime() - epoch;
            //double j2000 = j2000TS.TotalDays;
            //
            ////2. Centuries since J2000
            //double cJ2000 = j2000 / 36525.0f;
            //
            //double inclinationE = (0.00005f - 46.94f * cJ2000 / 3600.0f) * rad;
            //double longNodeE = (-11.26064f - 18228.25f * cJ2000 / 3600.0f) *rad;
            //double longPeriE = (102.94719f + 1198.28f * cJ2000 / 3600.0f) * rad;
            //double meanDistE = 1.00000011f - 0.00000005f * cJ2000;
            //double eccenctricityE = 0.01671022f - 0.00003804f * cJ2000;
            //double meanLongE = Mod2Pi((100.46435f + 129597740.63f * cJ2000 / 3600.0f) * rad);
            //
            ////Position of Earth in its orbit
            //double me = Mod2Pi(meanLongE - longPeriE);
            //double ve = TrueAnomaly(me, eccenctricityE);
            //double pEarthOrbit = meanDistE * (1 - eccenctricityE * eccenctricityE) / (1 + eccenctricityE * Math.Cos(ve));


            //Heliocentric rectangular coordinates of Earth
            //double xe = pEarthOrbit * Math.Cos(ve + longPeriE);
            //double ye = pEarthOrbit * Math.Sin(ve + longPeriE);
            double ze = 0.0f;

            return new Vector3((float)xe, (float) ze, (float)ye);
        }


        /// <summary>
        /// Trues the anomaly.
        /// To find what's this function is about...didn't understand it
        /// </summary>
        /// <returns>The anomaly</returns>
        /// <param name="M">M.</param>
        /// <param name="e">E.</param>
        private static double TrueAnomaly(double M, double e)
        {
            double V;

            // initial approximation of eccentric anomaly
            var E = M + e * Math.Sin(M) * (1.0f + e * Math.Cos(M));

            // convert eccentric anomaly to true anomaly
            V = 2f * Math.Atan(Math.Sqrt((1f + e) / (1f - e)) * Math.Tan(0.5f * E));

            if (V < 0) V = V + (2f * Math.PI);

            return V;
        }
    }
}
