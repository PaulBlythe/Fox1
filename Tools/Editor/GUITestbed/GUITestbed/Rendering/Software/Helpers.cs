using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GUITestbed.Rendering.Software
{
    public class Helpers
    {
        public static BoundingBox GetBoundingBox( Vector3[] points)
        {
            float xmin = float.MaxValue;
            float ymin = float.MaxValue;
            float zmin = float.MaxValue;

            float xmax = float.MinValue;
            float ymax = float.MinValue;
            float zmax = float.MinValue;

            for (int i=0; i<points.Length; i++)
            {
                if (points[i].X < xmin)
                    xmin = points[i].X;
                if (points[i].Y < ymin)
                    ymin = points[i].Y;
                if (points[i].Z < zmin)
                    zmin = points[i].Z;

                if (points[i].X > xmax)
                    xmax = points[i].X;
                if (points[i].Y > ymax)
                    ymax = points[i].Y;
                if (points[i].Z > zmax)
                    zmax = points[i].Z;
            }

            return new BoundingBox(new Vector3(xmin, ymin, zmin), new Vector3(xmax, ymax, zmax));
        }
    }
}
