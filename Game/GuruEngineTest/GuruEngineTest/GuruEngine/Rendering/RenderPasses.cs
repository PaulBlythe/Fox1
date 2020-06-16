using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Rendering
{
    public static class RenderPasses
    {
        public const int Sky = 0;
        public const int Ephemeris = 1;
        public const int Terrain = 2;
        public const int Geometry = 3;
        public const int SortedGeometry = 4;
        public const int Particles = 5;
        public const int Transparent = 6;
        public const int TotalPasses = 7;
    }
}
