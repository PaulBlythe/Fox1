using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.World
{
    public class Vector2D
    {
        private double x, y;


        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2D(Vector2D other)
        {
            x = other.X;
            y = other.Y;

        }

        public Vector2D(Vector2D p1, Vector2D p2)
        {
            x = p2.x - p1.x;
            y = p2.y - p1.y;
        }

        #region Properties

        public double Latitude
        {
            get { return y; }
            set { y = value; }
        }

        public double Longitude
        {
            get { return x; }
            set { x = value; }
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        #endregion

        #region Object
        public override bool Equals(object obj)
        {
            if (obj is Vector2D)
            {
                Vector2D v = (Vector2D)obj;
                if (v.x == x && v.y == y)
                    return obj.GetType().Equals(this.GetType());
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{{X={0}, Y={1}}}", x, y);
        }
        #endregion

        public double Length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public double SquareLength()
        {
            return (x * x) + (y * y);
        }

        public double SquareDistance(Vector2D other)
        {
            double tx = x - other.X;
            double ty = y - other.Y;

            return (tx * tx) + (ty * ty);
        }
        public void Normalise()
        {
            double l = Length();
            x /= l;
            y /= l;
        }

        public static Vector2D operator +(Vector2D u, Vector2D v)
        {
            return new Vector2D(u.x + v.x, u.y + v.y);
        }

        public static Vector2D operator -(Vector2D u, Vector2D v)
        {
            return new Vector2D(u.x - v.x, u.y - v.y);
        }

        public static Vector2D operator *(Vector2D u, double a)
        {
            return new Vector2D(a * u.x, a * u.y);
        }

        public static Vector2D operator /(Vector2D u, double a)
        {
            return new Vector2D(u.x / a, u.y / a);
        }

        public static Vector2D operator -(Vector2D u)
        {
            return new Vector2D(-u.x, -u.y);
        }

        public static double GetAngle(Vector2D c)
        {
            return GetAngle(c, new Vector2D(0, 0));
        }

        public static double GetAngle(Vector2D c, Vector2D f)
        {
            return (180 * (1 + Math.Atan2((c.Y - f.Y), (c.X - f.X)) / Math.PI));
        }

        public static double Distance(Vector2D a, Vector2D b)
        {
            return Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2));
        }

        public static Vector2D Rotate(Vector2D point, Vector2D origin, double angle)
        {
            double X = origin.X + ((point.X - origin.X) * Math.Cos(angle) - (point.Y - origin.Y) * Math.Sin(angle));
            double Y = origin.Y + ((point.X - origin.X) * Math.Sin(angle) + (point.Y - origin.Y) * Math.Cos(angle));
            return new Vector2D(X, Y);
        }

        public static Vector2D GetPoint(Vector2D origin, double length, double angle)
        {
            return new Vector2D(origin.X + length * Math.Cos(angle), origin.Y + length * Math.Sin(angle));
        }

        public static Vector2D VectorLLToMeters(Vector2D cnt, Vector2D v)
        {
            Vector2D result = new Vector2D(v);
            result.X *= (Constants.DEG_TO_MTR_LAT * Math.Cos(cnt.Y) * Constants.DEG_TO_RAD);
            result.Y *= Constants.DEG_TO_MTR_LAT;
            return result;
        }

        public static Vector2D Reciprocal(Vector2D pt, Vector2D ctrl)
        {
            return pt + new Vector2D(ctrl, pt);
        }
    }
}
