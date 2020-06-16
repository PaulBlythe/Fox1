using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUITestbed.DataHandlers.Mapping.Types;
using Microsoft.Xna.Framework;

namespace GUITestbed.DataHandlers.Mapping.Projections
{
    public class MercatorProjection : Projection
    {
        public override Vector2 GetDisplayScale(Region display_region, int width, int height)
        {
            Vector2 result = new Vector2();

            double top_width = GetMetresPerDegreeLongitude(display_region.MinY);
            double bot_width = GetMetresPerDegreeLongitude(display_region.MaxY);
            double wm = Math.Max(top_width, bot_width);

            result.X = (float)(wm / width );

            double hi = (display_region.MaxY - display_region.MinY) * 111320.0;
            result.Y = (float)(hi / height);

            return result;
        }

        public override double GetMetresPerDegreeLongitude(double Latitude)
        {
            double rad = Math.PI / 180.0;
            return 40075000.0 * Math.Cos(rad * Latitude) / 360.0;
        }

        public override double GetNewLatitude(double start_latitude, double metres)
        {
            return start_latitude + (metres / 111320.0);
        }

        /// <summary>
        /// Takes longitude, latitude and projects it into pixel space
        /// </summary>
        /// <param name="x">longitude</param>
        /// <param name="y">latitude</param>
        /// <returns></returns>
        public override Vector2 Project(DoublePoint origin, double x, double y, Vector2 scale)
        {
            Vector2 res = new Vector2(0, 0);

            double dy = y - origin.Y;               // in degrees
            dy *= 111320.0;                         // in metres
            res.Y = (float)(dy / scale.Y);

            double dx = x - origin.X;               // in degrees
            dx *= GetMetresPerDegreeLongitude(x);   // in metres
            res.X = (float)(dx / scale.X);

            return res;
        }
    }
}
