using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Algebra
{
    public static class DoubleClassExtension
    {

        public const double E = 2.718281828459045235360287471352;

        /// <summary>Represents the logarithm base 10 of e.</summary>
        public const double Log10OfE = 0.4342944819032518276511289189165;

        /// <summary>Represents the logarithm base 2 of e.</summary>
        public const double Log2OfE = 1.4426950408889634073599246810019;

        /// <summary>Represents the natural logarithm of 2.</summary>
        public const double Ln2 = 0.69314718055994530941723212145818f;

        /// <summary>Represents the natural logarithm of 10.</summary>
        public const double Ln10 = 2.3025850929940456840179914546844f;

        /// <summary>Represents one divided by the mathematical constant π.</summary>
        public const double OneOverPi = 0.31830988618379067153776752674503;

        /// <summary>Represents the mathematical constant π.</summary>
        public const double Pi = 3.1415926535897932384626433832795;

        /// <summary>Represents the mathematical constant π divided by two.</summary>
        public const double PiOver2 = 1.5707963267948966192313216916398;

        /// <summary>Represents the mathematical constant π divided by four.</summary>
        public const double PiOver4 = 0.78539816339744830961566084581988;

        /// <summary>Represents the mathematical constant π times two.</summary>
        public const double TwoPi = 6.283185307179586476925286766559;

        public const double EpsilonDSquared = Double.Epsilon * Double.Epsilon;

        /// <summary>
        /// Determines whether a value is zero (regarding a specific tolerance).
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>
        /// <see langword="true"/> if the specified value is zero (within the tolerance); otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// A value is zero if |x| &lt; epsilon.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="epsilon"/> is negative or 0.
        /// </exception>
        public static bool IsZero(double value, double epsilon)
        {
            if (epsilon <= 0.0)
                throw new ArgumentOutOfRangeException("epsilon", "Epsilon value must be greater than 0.");

            return (-epsilon < value) && (value < epsilon);
        }

        /// <summary>
        /// Determines whether a value is zero (regarding the tolerance 
        /// <see cref="EpsilonD"/>).
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>
        /// <see langword="true"/> if the specified value is zero (within the tolerance); otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// A value is zero if |x| &lt; <see cref="EpsilonD"/>.
        /// </remarks>
        public static bool IsZero(double value)
        {
            return (-Double.Epsilon < value) && (value < Double.Epsilon);
        }

        /// <summary>
        /// Clamps the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value which should be clamped.</param>
        /// <param name="min">The min limit.</param>
        /// <param name="max">The max limit.</param>
        /// <returns>
        /// <paramref name="value"/> clamped to the interval
        /// [<paramref name="min"/>, <paramref name="max"/>].
        /// </returns>
        /// <remarks>
        /// Values within the limits are not changed. Values exceeding the limits are cut off.
        /// </remarks>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(max) > 0)
            {
                // min and max are swapped.
                var dummy = max;
                max = min;
                min = dummy;
            }

            if (value.CompareTo(min) < 0)
                value = min;
            else if (value.CompareTo(max) > 0)
                value = max;

            return value;
        }


        /// <summary>
        /// Clamps near-zero values to zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 0 if the value is nearly zero (within the tolerance <see cref="Numeric.EpsilonF"/>) 
        /// or the original value otherwise.
        /// </returns>
        public static double ClampToZero(double value)
        {
            return IsZero(value) ? 0 : value;
        }

        /// <summary>
        /// Clamps near-zero values to zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>
        /// 0 if the value is nearly zero (within the tolerance <paramref name="epsilon"/>) or the
        /// original value otherwise.
        /// </returns>
        public static double ClampToZero(double value, double epsilon)
        {
            return IsZero(value, epsilon) ? 0 : value;
        }

        /// <summary>
        /// Determines whether two values are equal (regarding the tolerance <see cref="EpsilonD"/>).
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>
        /// <see langword="true"/> if the specified values are equal (within the tolerance); otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <strong>Important:</strong> When at least one of the parameters is a 
        /// <see cref="Double.NaN"/> the result is undefined. Such cases should be handled explicitly by
        /// the calling application.
        /// </remarks>
        public static bool AreEqual(double value1, double value2)
        {
            // Infinity values have to be handled carefully because the check with the epsilon tolerance
            // does not work there. Check for equality in case they are infinite:
            if (value1 == value2)
                return true;

            // Scale epsilon proportional the given values.
            double epsilon = Double.Epsilon * (Math.Abs(value1) + Math.Abs(value2) + 1.0);
            double delta = value1 - value2;
            return (-epsilon < delta) && (delta < epsilon);

            // We could also use ... Abs(v1 - v2) <= _epsilonF * Max(Abs(v1), Abs(v2), 1)
        }

        /// <summary>
        /// Determines whether two values are equal (regarding a specific tolerance).
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>
        /// <see langword="true"/> if the specified values are equal (within the tolerance); otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <strong>Important:</strong> When at least one of the parameters is a 
        /// <see cref="Single.NaN"/> the result is undefined. Such cases should be handled explicitly by
        /// the calling application.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="epsilon"/> is negative or 0.
        /// </exception>
        public static bool AreEqual(double value1, double value2, double epsilon)
        {
            if (epsilon <= 0.0f)
                throw new ArgumentOutOfRangeException("epsilon", "Epsilon value must be greater than 0.");

            // Infinity values have to be handled carefully because the check with the epsilon tolerance
            // does not work there. Check for equality in case they are infinite:
            if (value1 == value2)
                return true;

            double delta = value1 - value2;
            return (-epsilon < delta) && (delta < epsilon);
        }

        /// <summary>
        /// Swaps the content of two variables.
        /// </summary>
        /// <typeparam name="T">The type of the objects.</typeparam>
        /// <param name="obj1">First variable.</param>
        /// <param name="obj2">Second variable.</param>
        public static void Swap<T>(ref T obj1, ref T obj2)
        {
            T temp = obj1;
            obj1 = obj2;
            obj2 = temp;
        }

        /// <summary>
        /// Converts an angle value from degrees to radians (double-precision).
        /// </summary>
        /// <param name="degree">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static double ToRadians(double degree)
        {
            return degree * Pi / 180.0;
        }

        /// <summary>
        /// Converts an angle value from radians to degrees (double-precision).
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        public static double ToDegrees(double radians)
        {
            return radians * 180 / Pi;
        }

        /// <summary>
        /// Simulates atmospheric refraction for objects close to the horizon.
        /// </summary>
        /// <param name="elevation">The elevation angle of the object above the horizon.</param>
        /// <returns>
        /// The elevation angle of the object above the horizon after simulating atmospheric refraction.
        /// </returns>
        /// <remarks>
        /// This method does not model variations in atmosphere pressure and temperature.
        /// </remarks>
        public static double Refract(double elevation)
        {
            // See Zimmerman, John C. 1981. Sun-pointing programs and their accuracy.
            // SAND81-0761, Experimental Systems Operation Division 4721, Sandia National Laboratories, Albuquerque, NM.

            //float prestemp;     // temporary pressure/temperature correction
            double refcor;        // temporary refraction correction 
            double tanelev;       // tangent of the solar elevation angle

            if (elevation > ToRadians(85.0))
            {
                // No refraction near zenith. (Algorithm does not work there.)
                refcor = 0.0;
            }
            else
            {
                // Refract.
                tanelev = Math.Tan(elevation);
                if (elevation >= ToRadians(5.0))
                {
                    refcor = 58.1 / tanelev - 0.07 / (Math.Pow(tanelev, 3)) + 0.000086 / (Math.Pow(tanelev, 5));
                }
                else if (elevation >= ToRadians(-0.575))
                {
                    double degElev = ToDegrees(elevation);
                    refcor = 1735.0 + degElev * (-518.2 + degElev * (103.4 + degElev * (-12.79 + degElev * 0.711)));
                }
                else
                {
                    refcor = -20.774 / tanelev;
                }

                //prestemp = (pdat->press * 283.0) / (1013.0 * (273.0 + pdat->temp));
                //refcor *= prestemp / 3600.0;
                refcor /= 3600.0;
            }

            // Refracted solar elevation angle.
            return elevation + ToRadians(refcor);
        }

        /// <summary>
        /// Clamps an angle to the interval [0, 2π].
        /// </summary>
        public static double InRange(double x)
        {
            while (x > TwoPi)
                x -= TwoPi;
            while (x < 0)
                x += TwoPi;
            return x;
        }
    }
}
