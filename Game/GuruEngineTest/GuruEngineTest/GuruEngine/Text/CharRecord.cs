using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Text
{
    class CharRecord
    {
        public int ID;

        public int X;
        public int Y;
        public int Width;
        public int Height;

        public float xoffset;
        public float yoffset;
        public float xadvance;

        public CharRecord(int id, int x, int y, int w, int h, float xo, float yo, float xa)
        {
            ID = id;
            X = x;
            Y = y;
            Width = w;
            Height = h;
            xoffset = xo;
            yoffset = yo;
            xadvance = xa;
        }
    }
}
