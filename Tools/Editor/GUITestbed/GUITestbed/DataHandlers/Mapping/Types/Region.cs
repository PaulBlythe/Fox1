using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.Mapping.Types
{
    public class Region
    {
        public double MinX;
        public double MinY;
        public double MaxX;
        public double MaxY;

        public Region()
        {
            MinX = MaxX = MinY = MaxY = 0;
        }

        public Region(BinaryReader b)
        {
            MinX = b.ReadDouble();
            MinY = b.ReadDouble();
            MaxX = b.ReadDouble();
            MaxY = b.ReadDouble();
        }

        public bool Intersects(Region other)
        {

            if (other.MinX > MaxX)
                return false;
            if (other.MinY > MaxY)
                return false;
            if (other.MaxX < MinX)
                return false;
            if (other.MaxY < MinY)
                return false;

            return true;
        }

        public bool Contains(double x, double y)
        {
            if (x < MinX)
                return false;
            if (x > MaxX)
                return false;
            if (y < MinY)
                return false;
            if (y > MaxY)
                return false;
            return true;
        }
    }
}
