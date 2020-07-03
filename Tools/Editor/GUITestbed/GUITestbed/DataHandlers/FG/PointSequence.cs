using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.FG
{
    public class PointSequence
    {
        public List<Node> Points = new List<Node>();
        public List<Vector3> Verts = new List<Vector3>();

        public bool Closed = false;

        public bool GetSide(int n, out Segment2 s, out BezierVector2D bez)
        {
            s = null;
            bez = null;

            BezierVector2D b = new BezierVector2D();

            int n1 = n;
            int n2 = (n + 1) % Points.Count;

            b.pt1 = Points[n1].pt;
            b.pt2 = Points[n2].pt;
            b.c1 = b.pt1;
            b.c2 = b.pt2;

            Vector2D hi = null;
            Vector2D lo = null;
            if (!Points[n1].GetControlHandleHi(out hi))
            {
                b.c1 = b.pt1;
            }
            else
            {
                b.c1 = hi;
            }
            if (!Points[n2].GetControlHandleLo(out lo))
            {
                b.c2 = b.pt2;
            }
            else
            {
                b.c2 = lo;
            }

            if ((hi == null) && (lo == null))
            {
                s = new Segment2();
                s.p1 = b.pt1;
                s.p2 = b.pt2;
                return false;
            }
            bez = b;
            return true;
        }
    }
}
