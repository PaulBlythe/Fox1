using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.World
{
    public class BezierVector2D
    {
        public Vector2D pt1;
        public Vector2D pt2;
        public Vector2D c1;
        public Vector2D c2;

        public Vector2D CurvePoint(double t)
        {
            double nt = 1.0 - t;
            double w0 = nt * nt * nt;
            double w1 = 3.0 * nt * nt * t;
            double w2 = 3.0 * nt * t * t;
            double w3 = t * t * t;

            return new Vector2D(w0 * pt1.X + w1 * c1.X + w2 * c2.X + w3 * pt2.X, w0 * pt1.Y + w1 * c1.Y + w2 * c2.Y + w3 * pt2.Y);
        }
    }
}