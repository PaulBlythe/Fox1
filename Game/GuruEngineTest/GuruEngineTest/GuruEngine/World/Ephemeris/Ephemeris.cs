using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using GuruEngine.Algebra;

namespace GuruEngine.World.Ephemeris
{
    public static class Ephemeris
    {
        /// <summary>
        /// Converts the specified <see cref="DateTimeOffset"/> to the number of days since 
        /// January 1, 1990.
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/>.</param>
        /// <param name="terrestrialTime">
        /// Specifying terrestrial time means you want atomic clock time, not corrected by leap seconds 
        /// to account for slowing of the earth's rotation, as opposed to GMT which does account for 
        /// leap seconds.
        /// </param>
        /// <returns>The fractional number of days elapsed since January 1, 1990.</returns>
        /// <remarks>
        /// This method converts a date to days elapsed since January 1, 1990 on the Julian calendar. 
        /// Used for internal astronomical calculations. Since this number is smaller than that returned 
        /// by <see cref="ToJulianDate"/>, it is of higher precision.
        /// </remarks>
        public static double ToEpoch1990Days(DateTimeOffset dateTimeOffset, bool terrestrialTime)
        {
            double julianDate = ToJulianDate(dateTimeOffset, terrestrialTime);
            return julianDate - 2447891.5;
        }

        /// <summary>
        /// Converts the specified <see cref="DateTimeOffset"/> to the number of centuries since 
        /// January 1, 2000.
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/>.</param>
        /// <param name="terrestrialTime">
        /// Specifying terrestrial time means you want atomic clock time, not corrected by leap seconds 
        /// to account for slowing of the earth's rotation, as opposed to GMT which does account for 
        /// leap seconds.
        /// </param>
        /// <returns>The fractional number of centuries elapsed since January 1, 2000.</returns>
        /// <remarks>
        /// This method converts a date to centuries and fraction since January 1, 2000. Used for
        /// internal astronomical calculations. Since this number is smaller than that returned by
        /// <see cref="ToJulianDate"/>, it is of higher precision.
        /// </remarks>
        public static double ToEpoch2000Centuries(DateTimeOffset dateTimeOffset, bool terrestrialTime)
        {
            double julianDate = ToJulianDate(dateTimeOffset, terrestrialTime);
            return (julianDate - 2451545.0) / 36525.0;  // A Julian year is 365.25 days.
        }

        /// <summary>
        /// Converts the specified <see cref="DateTimeOffset"/> to the Julian Date.
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/>.</param>
        /// <param name="terrestrialTime">
        /// Specifying terrestrial time means you want atomic clock time, not corrected by leap seconds 
        /// to account for slowing of the earth's rotation, as opposed to GMT which does account for 
        /// leap seconds.
        /// </param>
        /// <returns>
        /// Days and fractions since noon Universal Time on January 1, 4713 BCE on the Julian calendar.
        /// </returns>
        /// <remarks>
        /// Julian Dates are used for astronomical calculations (such as our own ephemeris model) and 
        /// represent days and fractions since noon Universal Time on January 1, 4713 BCE on the Julian 
        /// calendar. Note that due to precision limitations of 64-bit doubles, the resolution of the 
        /// date returned may be as low as within 8 hours.
        /// </remarks>
        public static double ToJulianDate(DateTimeOffset dateTimeOffset, bool terrestrialTime)
        {
            // See http://de.wikipedia.org/wiki/Julianisches_Datum.

            // Convert to GMT/UTC.
            var dateTime = dateTimeOffset.UtcDateTime;
            double h = dateTime.Hour + (dateTime.Minute + dateTime.Second / 60.0 + dateTime.Millisecond / 60.0 / 1000.0) / 60.0;
            double d = dateTime.Day + (h / 24.0);

            double y, m;
            if (dateTime.Month < 3)
            {
                y = dateTime.Year - 1;
                m = dateTime.Month + 12;
            }
            else
            {
                y = dateTime.Year;
                m = dateTime.Month;
            }

            double result = 1720996.5 - Math.Floor(y / 100.0) + Math.Floor(y / 400.0) + Math.Floor(365.25 * y) + Math.Floor(30.6001 * (m + 1)) + d;

            // UTC is an approximation (within 0.9 seconds) for UT1. 
            // Terrestrial time (http://en.wikipedia.org/wiki/Terrestrial_Time) is ahead of UT1 by 
            // deltaT (http://en.wikipedia.org/wiki/DeltaT) which is a number which depends on date, 
            // earth mass, melting ice, etc. deltaT = 65 s is accurate enough for us.
            if (terrestrialTime)
                result += 65.0 / 60.0 / 60.0 / 24.0;

            return result;
        }

        private static readonly Spectrum spectrum = new Spectrum();
        public static readonly Matrix33 XYZToRGB = new Matrix33(3.240479f, -1.53715f, -0.49853f,-0.969256f, 1.875991f, 0.041556f, 0.055648f, -0.204043f, 1.057311f);

        /// <summary>
        /// Gets the extraterrestrial sunlight intensity based on NASA data.
        /// </summary>
        /// <value>The sunlight intensity outside the earth's atmosphere in [lux].</value>
        public static Vector3 ExtraterrestrialSunlight
        {
            get
            {
                if (!_extraterrestrialSunlight.HasValue)
                {
                    spectrum.SetSolarSpectrum();
                    Vector3 sunlightXyz = spectrum.ToXYZ();
                    Vector3 sunlightRgb = XYZToRGB * sunlightXyz;
                    _extraterrestrialSunlight = sunlightRgb;
                }

                return _extraterrestrialSunlight.Value;
            }
        }
        private static Vector3? _extraterrestrialSunlight;


        
    }
}
