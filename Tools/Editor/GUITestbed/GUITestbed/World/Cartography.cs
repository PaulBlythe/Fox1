using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.World
{
    public static class Cartography
    {
        /// <summary>
        /// Returns a new Vector2D which is the distance from the reference point in metres
        /// IE. Makes the data point local to the reference point
        /// </summary>
        /// <param name="clat">Reference point latitude</param>
        /// <param name="clon">Reference point longitude</param>
        /// <param name="slat">Data point latitude</param>
        /// <param name="slon">Data point longitude</param>
        /// <returns></returns>
        public static Vector2D ConvertToLocalised(double clat, double clon, double slat, double slon)
        {
            double m_per_deg_lat, m_per_deg_lon, deltaLat, deltaLon;

            m_per_deg_lat = 111132.954 - 559.822 * Math.Cos(MathHelper.ToRadians((float)(2.0 * clat)) + 1.175 * Math.Cos(MathHelper.ToRadians((float)(4.0 * clat))));
            m_per_deg_lon = (3.14159265359 / 180) * 6367449 * Math.Cos(MathHelper.ToRadians((float)clat));

            deltaLat = clat - slat;
            deltaLon = slon - clon;

            return new Vector2D(deltaLon * m_per_deg_lon, deltaLat * m_per_deg_lat);
        }

        public static float ReadFloat(String s)
        {
            String s2 = s.Substring(0, s.Length - 1);
            if (s.EndsWith("F"))
            {
                return float.Parse(s2);
            }
            if (s.EndsWith("M"))
            {
                return (float)(float.Parse(s2) / Constants.FEET_TO_MTR);
            }
            return float.Parse(s);
        }


        /// <summary>
		/// Get open-ended Bezier Spline Control Points.
		/// </summary>
		/// <param name="knots">Input Knot Bezier spline points.</param>
		/// <param name="firstControlPoints">Output First Control points array of knots.Length - 1 length.</param>
		/// <param name="secondControlPoints">Output Second Control points array of knots.Length - 1 length.</param>
		/// <exception cref="ArgumentNullException"><paramref name="knots"/> parameter must be not null.</exception>
		/// <exception cref="ArgumentException"><paramref name="knots"/> array must containg at least two points.</exception>
		public static void GetCurveControlPoints(Vector2D[] knots, out Vector2D[] firstControlPoints, out Vector2D[] secondControlPoints)
        {
            if (knots == null)
                throw new ArgumentNullException("knots");
            int n = knots.Length - 1;
            if (n < 1)
                throw new ArgumentException("At least two knot points required", "knots");
            if (n == 1)
            { // Special case: Bezier curve should be a straight line.
                firstControlPoints = new Vector2D[1];
                firstControlPoints[0] = new Vector2D(0,0);

                // 3P1 = 2P0 + P3
                firstControlPoints[0].X = (2 * knots[0].X + knots[1].X) / 3;
                firstControlPoints[0].Y = (2 * knots[0].Y + knots[1].Y) / 3;

                secondControlPoints = new Vector2D[1];
                secondControlPoints[0] = new Vector2D(0, 0);
                // P2 = 2P1 – P0
                secondControlPoints[0].X = 2 * firstControlPoints[0].X - knots[0].X;
                secondControlPoints[0].Y = 2 * firstControlPoints[0].Y - knots[0].Y;
                return;
            }

            // Calculate first Bezier control points
            // Right hand side vector
            double[] rhs = new double[n];

            // Set right hand side X values
            for (int i = 1; i < n - 1; ++i)
                rhs[i] = 4 * knots[i].X + 2 * knots[i + 1].X;
            rhs[0] = knots[0].X + 2 * knots[1].X;
            rhs[n - 1] = (8 * knots[n - 1].X + knots[n].X) / 2.0;
            // Get first control points X-values
            double[] x = GetFirstControlPoints(rhs);

            // Set right hand side Y values
            for (int i = 1; i < n - 1; ++i)
                rhs[i] = 4 * knots[i].Y + 2 * knots[i + 1].Y;
            rhs[0] = knots[0].Y + 2 * knots[1].Y;
            rhs[n - 1] = (8 * knots[n - 1].Y + knots[n].Y) / 2.0;
            // Get first control points Y-values
            double[] y = GetFirstControlPoints(rhs);

            // Fill output arrays.
            firstControlPoints = new Vector2D[n];
            secondControlPoints = new Vector2D[n];
            for (int i = 0; i < n; ++i)
            {
                // First control point
                firstControlPoints[i] = new Vector2D(x[i], y[i]);
                // Second control point
                if (i < n - 1)
                    secondControlPoints[i] = new Vector2D(2 * knots[i + 1].X - x[i + 1], 2 * knots[i + 1].Y - y[i + 1]);
                else
                    secondControlPoints[i] = new Vector2D((knots[n].X + x[n - 1]) / 2, (knots[n].Y + y[n - 1]) / 2);
            }
        }

        /// <summary>
        /// Solves a tridiagonal system for one of coordinates (x or y) of first Bezier control points.
        /// </summary>
        /// <param name="rhs">Right hand side vector.</param>
        /// <returns>Solution vector.</returns>
        private static double[] GetFirstControlPoints(double[] rhs)
        {
            int n = rhs.Length;
            double[] x = new double[n]; // Solution vector.
            double[] tmp = new double[n]; // Temp workspace.

            double b = 2.0;
            x[0] = rhs[0] / b;
            for (int i = 1; i < n; i++) // Decomposition and forward substitution.
            {
                tmp[i] = 1 / b;
                b = (i < n - 1 ? 4.0 : 3.5) - tmp[i];
                x[i] = (rhs[i] - x[i - 1]) / b;
            }
            for (int i = 1; i < n; i++)
                x[n - i - 1] -= tmp[n - i] * x[n - i]; // Backsubstitution.

            return x;
        }

        public static int LimitInt(double val, int min, int max)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return (int)val;
        }
    }
}
