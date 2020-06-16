using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using GUITestbed.DataHandlers.Mapping.ColourMapping;
using GUITestbed.DataHandlers.Mapping.Types;

namespace GUITestbed.DataHandlers.Mapping.Projections
{
    public class FlatProjection : Projection
    {
        double FlatScale;

        /// <summary>
        /// Input is metres per pixel
        /// </summary>
        /// <param name="scale"></param>
        public FlatProjection(double scale)
        {
            FlatScale = (1852 * 60) / scale;
        }

        public override Vector2 GetDisplayScale(Region display_region, int width, int height)
        {
            double dx = display_region.MaxX - display_region.MinX;      // in degrees
            double dy = display_region.MaxY - display_region.MinY;      // in degrees
            dx *= FlatScale;                                            // in metres
            dy *= FlatScale;                                            // in metres

            Vector2 result = new Vector2();
            result.X = (float)(dx / width);
            result.Y = (float)(dy / height);
            return result;

        }

        public override double GetMetresPerDegreeLongitude(double Latitude)
        {
            return FlatScale;
        }

        public override double GetNewLatitude(double start_latitude, double metres)
        {
            return start_latitude + metres / FlatScale;
        }

        public override Vector2 Project(DoublePoint centre, double x, double y, Vector2 scale)
        {
            Vector2 result = new Vector2();

            double dx = (x - centre.X) * FlatScale;
            double dy = (y - centre.Y) * FlatScale;

            result.X = (float)(dx / scale.X);
            result.Y = (float)(dy / scale.Y);
            return result;
        }
    }
}
