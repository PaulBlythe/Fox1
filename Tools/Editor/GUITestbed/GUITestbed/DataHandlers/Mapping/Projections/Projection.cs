using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using GUITestbed.DataHandlers.Mapping.Types;

namespace GUITestbed.DataHandlers.Mapping.Projections
{
    public abstract class Projection
    {
        public abstract double GetMetresPerDegreeLongitude(double Latitude);
        public abstract double GetNewLatitude(double start_latitude, double metres);
        public abstract Vector2 Project(DoublePoint centre, double x, double y, Vector2 scale);
        public abstract Vector2 GetDisplayScale(Region display_region, int width, int height);
    }
}
