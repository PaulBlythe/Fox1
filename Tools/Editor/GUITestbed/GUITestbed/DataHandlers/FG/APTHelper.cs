using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.FG
{
    public static class APTHelper
    {

        public static List<PointSequence> LineNodeListToChain(List<LineNode> inList)
        {
            PointSequence result = new PointSequence();

            List<PointSequence> chains = new List<PointSequence>();

            Vector2D lo_pt = new Vector2D(0, 0);
            Vector2D hi_pt = new Vector2D(0, 0);

            int next, orig, prev;

            int cur = 0;
            while (cur != inList.Count)
            {
                bool has_lo = inList[cur].IsCurved();
                if (has_lo)
                {
                    lo_pt = Vector2D.Reciprocal(inList[cur].pt, inList[cur].ctrl);
                }
                next = orig = prev = cur;
                next++;
                while ((next != inList.Count) &&
                       (inList[next].pt == inList[cur].pt) &&
                       (!inList[prev].IsCurved() || !inList[next].IsCurved()))
                {
                    prev = next;
                    next++;
                }
                next--;
                cur = next;

                bool has_hi = inList[cur].IsCurved();
                if (has_hi)
                {
                    hi_pt = inList[cur].ctrl;
                }

                Node b = new Node();
                b.pt = inList[cur].pt;
                b.set_split((has_hi != has_lo) || (cur != orig));

                if (has_lo)
                    b.SetControlLo(lo_pt);
                if (has_hi)
                    b.SetControlHi(hi_pt);

                b.linetype = inList[cur].LineType;
                b.lighting = inList[cur].LightingType;

                result.Points.Add(b);

                if (inList[cur].IsEnd())
                {
                    result.Closed = false;
                    chains.Add(result);
                    result = new PointSequence();
                }

                if (inList[cur].IsClose())
                {
                    result.Closed = true;
                    chains.Add(result);
                    result = new PointSequence();
                }
                cur++;
            }

            return chains;
        }

        public static List<Vector3> PointSequenceToVertices(PointSequence points, double clat, double clon, float altitude)
        {
            Segment2 s = new Segment2();
            BezierVector2D b = new BezierVector2D();

            List<Vector3> verts = new List<Vector3>();
            int nSides = points.Points.Count;
            if (!points.Closed)
                nSides--;

            for (int i = 0; i < nSides; i++)
            {
                if (points.GetSide(i, out s, out b))
                {
                    // this segment is a bezier

                    // convert to metres relative to centre of mesh
                    b.pt1 = Cartography.ConvertToLocalised(clat, clon, b.pt1.Y, b.pt1.X);
                    b.pt2 = Cartography.ConvertToLocalised(clat, clon, b.pt2.Y, b.pt2.X);
                    b.c1 = Cartography.ConvertToLocalised(clat, clon, b.c1.Y, b.c1.X);
                    b.c2 = Cartography.ConvertToLocalised(clat, clon, b.c2.Y, b.c2.X);

                    double pixels_approx = Math.Sqrt(new Vector2D(b.pt1, b.c1).SquareLength()) +
                                           Math.Sqrt(new Vector2D(b.c1, b.c2).SquareLength()) +
                                           Math.Sqrt(new Vector2D(b.c2, b.pt2).SquareLength());

                    int point_count = Cartography.LimitInt(pixels_approx / 2, 5, 100);
                    int ns = 1;
                    if (i == 0)
                        ns = 0;
                    for (int n = ns; n <= point_count; n++)
                    {
                        Vector2D p = b.CurvePoint((double)n / (double)point_count);
                        Vector3 v = new Vector3((float)p.X, altitude, (float)p.Y);
                        verts.Add(v);
                    }

                }
                else
                {
                    // this is a straight line
                    Vector2D start = Cartography.ConvertToLocalised(clat, clon, s.p1.Y, s.p1.X);
                    Vector2D end = Cartography.ConvertToLocalised(clat, clon, s.p2.Y, s.p2.X);
                    Vector3 v = new Vector3((float)start.X, altitude, (float)start.Y);
                    if (i == 0)
                        verts.Add(v);

                    v = new Vector3((float)end.X, altitude, (float)end.Y);
                    verts.Add(v);
                }
            }

            return verts;
        }
    }
}
