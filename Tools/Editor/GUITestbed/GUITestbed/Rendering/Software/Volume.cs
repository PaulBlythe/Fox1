using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace GUITestbed.Rendering.Software
{
    public class Volume
    {
        public BoundingBox Bounds;
        public Vector3[] Points = new Vector3[12];


        public Volume(Vector3 centre, Vector2 size, float Height, Matrix Orientation)
        {
            Vector3 tbl = centre + new Vector3(-size.X, 0, size.Y);
            Vector3 tfl = centre + new Vector3(-size.X, 0, -size.Y);
            Vector3 tbr = centre + new Vector3(size.X, 0, size.Y);
            Vector3 tfr = centre + new Vector3(size.X, 0, -size.Y);

            Points[0] = Vector3.Transform(tbl, Orientation);
            Points[1] = Vector3.Transform(tbr, Orientation);
            Points[2] = Vector3.Transform(tfr, Orientation);

            Points[3] = Vector3.Transform(tbl, Orientation);
            Points[4] = Vector3.Transform(tfr, Orientation);
            Points[5] = Vector3.Transform(tfl, Orientation);

            tbl.Y -= Height;
            tbr.Y -= Height;
            tfl.Y -= Height;
            tfr.Y -= Height;

            Points[6] = Vector3.Transform(tbl, Orientation);
            Points[7] = Vector3.Transform(tbr, Orientation);
            Points[8] = Vector3.Transform(tfr, Orientation);

            Points[9] = Vector3.Transform(tbl, Orientation);
            Points[10] = Vector3.Transform(tfr, Orientation);
            Points[11] = Vector3.Transform(tfl, Orientation);

            Bounds = Helpers.GetBoundingBox(Points);

        }


    }
}
