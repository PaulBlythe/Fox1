using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectEditor.Layouts
{
    public class Region
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public Vector Centre;

        public Region(double x, double y, double w, double h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            Centre = new Vector(x + (Width / 2), y + (Height / 2));
        }

        public void Move(Vector v)
        {
            X += v.X;
            Y += v.Y;
            Centre.X += v.X;
            Centre.Y += v.Y;
        }

        public Vector GetCenter()
        {
            return Centre;
        }

        public double DistanceSquared(Region other)
        {
            double res = 0;
            double dx = X - other.X;
            double dy = Y - other.Y;
            res = (dx * dx) + (dy * dy);
            return res;
        }

        public Vector Direction(Region other)
        {
            double distance = Math.Sqrt(DistanceSquared(other));
            if (distance < 1)
                distance = 1;
            double dx = X - other.X;
            double dy = Y - other.Y;
            Vector res = new Vector(dx, dy);
            res /= distance;
            return res;
        }

        public bool Intersects(Region rectangle)
        {
            if ((rectangle.Y >= this.Y && rectangle.Y - rectangle.Height <= this.Y && rectangle.X <= this.X && rectangle.X + rectangle.Width >= this.X) ||
                (rectangle.Y >= this.Y && rectangle.Y - rectangle.Height <= this.Y && rectangle.X >= this.X && rectangle.X <= this.X + this.Width) ||
                (rectangle.Y <= this.Y && rectangle.Y >= this.Y - this.Height && rectangle.X <= this.X && rectangle.X + rectangle.Width >= this.X) ||
                (rectangle.Y <= this.Y && rectangle.Y >= this.Y - this.Height && rectangle.X >= this.X && rectangle.X <= this.X + this.Width))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
