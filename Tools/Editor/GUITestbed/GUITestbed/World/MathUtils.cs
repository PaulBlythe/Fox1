using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.World
{
    public static class MathUtils
    {
        public static double Limit(double t, double v1, double v2)
        {
            if (t < v1)
                return v1;
            if (t > v2)
                return v2;
            return t;
        }

        public static double TableLookup2D(double rowKey, double colKey, double[] column, double[] row, double[,] data)
        {
            double rFactor, cFactor, col1temp, col2temp, Value;
            int r = 1;
            int c = 1;

            while (r > 1 && row[r - 1] > rowKey) { r--; }
            while (r < row.Length - 1 && row[r] < rowKey) { r++; }

            while (c > 1 && column[c - 1] > colKey) { c--; }
            while (c < column.Length - 1 && column[c] < colKey) { c++; }


            rFactor = (rowKey - row[r - 1]) / (row[r] - row[r - 1]);
            cFactor = (colKey - column[c - 1]) / (column[c] - column[c - 1]);

            if (rFactor > 1.0) rFactor = 1.0;
            else if (rFactor < 0.0) rFactor = 0.0;

            if (cFactor > 1.0) cFactor = 1.0;
            else if (cFactor < 0.0) cFactor = 0.0;

            col1temp = rFactor * (data[r, c - 1] - data[r - 1, c - 1]) + data[r - 1, c - 1];
            col2temp = rFactor * (data[r, c] - data[r - 1, c]) + data[r - 1, c];

            Value = col1temp + cFactor * (col2temp - col1temp);

            return Value;

        }

        public static double TableLookup1D(double colkey, double[] column, double[] data)
        {
            double cFactor, col1temp, Value;
            int c = 1;


            while (c > 1 && column[c - 1] > colkey) { c--; }
            while (c < column.Length - 1 && column[c] < colkey) { c++; }

            cFactor = (colkey - column[c - 1]) / (column[c] - column[c - 1]);

            if (cFactor > 1.0) cFactor = 1.0;
            else if (cFactor < 0.0) cFactor = 0.0;

            col1temp = cFactor * (data[c] - data[c - 1]) + data[c - 1];

            Value = col1temp;

            return Value;
        }

        /// <summary>
        /// Converts from degrees Rankine to degrees Kelvin.
        /// </summary>
        /// <param name="rankine">The temperature in degrees Rankine</param>
        /// <returns>The temperature in Kelvin.</returns>
        public static double RankineToKelvin(double rankine)
        {
            return rankine / 1.8;
        }

        public static double CelsiusToFahrenheit(double celsius)
        {
            return celsius * 1.8 + 32.0;
        }

        public static double FahrenheitToCelsius(double fahrenheit)
        {
            return (fahrenheit - 32.0) / 1.8;
        }

        public static double KelvinToFahrenheit(double kelvin)
        {
            return 1.8 * kelvin - 459.4;
        }

        /// <summary>
        /// Converts from degrees Rankine to degrees Celsius.
        /// </summary>
        /// <param name="rankine">The temperature in degrees Rankine.</param>
        /// <returns>The temperature in Celsius.</returns>
        public static double RankineToCelsius(double rankine)
        {
            return (rankine - 491.67) / 1.8;
        }

        public static double Lerp(double v1, double v2, double t)
        {
            return v1 + ((v2 - v1) * t);
        }

        public static double square_signed(double value)
        {
            if (value < 0)
                return value * value * -1;
            else
                return value * value;
        }

        public static double Lag(double input, double output, double time, double dt)
        {
            if (input == output)
                return output;

            double rate = 1.0 / time;
            double ThisDt = Math.Abs((input - output) / rate);
            if (dt < ThisDt)
            {
                ThisDt = dt;
                if (output < input)
                    output += ThisDt * rate;
                else
                    output -= ThisDt * rate;
            }
            else
                // Handle this case separate to make sure the termination condition
                // is met even in inexact arithmetics ...
                output = input;

            return output;

        }

        public static double Range(double x, double r)
        {
            return x - (r * Math.Floor(x / r));
        }

        public static float Cvt(float paramFloat1, float Minumum, float Maximum, float Start, float Finish)
        {
            float y = Math.Min(Math.Max(paramFloat1, Minumum), Maximum);
            return Start + ((Finish - Start) * (y - Minumum) / (Maximum - Minumum));
        }

        public static float HorizonDistance(float elevation)
        {
            const float er = 6378137f;
            return (float)Math.Sqrt((2 * elevation * er) + (elevation * elevation));
        }


        // Find the points where the two circles intersect.
        public static int FindCircleCircleIntersections(
            float cx0, float cy0, float radius0,
            float cx1, float cy1, float radius1,
            out Vector2 intersection1, out Vector2 intersection2)
        {
            // Find the distance between the centers.
            float dx = cx0 - cx1;
            float dy = cy0 - cy1;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            // See how many solutions there are.
            if (dist > radius0 + radius1)
            {
                // No solutions, the circles are too far apart.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (dist < Math.Abs(radius0 - radius1))
            {
                // No solutions, one circle contains the other.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if ((dist == 0) && (radius0 == radius1))
            {
                // No solutions, the circles coincide.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else
            {
                // Find a and h.
                double a = (radius0 * radius0 -
                    radius1 * radius1 + dist * dist) / (2 * dist);
                double h = Math.Sqrt(radius0 * radius0 - a * a);

                // Find P2.
                double cx2 = cx0 + a * (cx1 - cx0) / dist;
                double cy2 = cy0 + a * (cy1 - cy0) / dist;

                // Get the points P3.
                intersection1 = new Vector2(
                    (float)(cx2 + h * (cy1 - cy0) / dist),
                    (float)(cy2 - h * (cx1 - cx0) / dist));
                intersection2 = new Vector2(
                    (float)(cx2 - h * (cy1 - cy0) / dist),
                    (float)(cy2 + h * (cx1 - cx0) / dist));

                // See if we have 1 or 2 solutions.
                if (dist == radius0 + radius1) return 1;
                return 2;
            }
        }


        
    }
}
