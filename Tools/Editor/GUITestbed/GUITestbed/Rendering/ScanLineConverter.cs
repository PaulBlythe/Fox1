using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GUITestbed;

namespace GUITestbed.Rendering
{
    public class Edge
    {
        public Vector2 Pt1;
        public Vector2 Pt2;

        public float ymin;
        public float ymax;

        public Edge(Vector2 p1, Vector2 p2)
        {
            Pt1 = p1;
            Pt2 = p2;
            ymin = Math.Min(p1.Y, p2.Y);
            ymax = Math.Max(p1.Y, p2.Y);

            if (Pt2.Y <Pt1.Y)
            {
                Vector2 t = Pt1;
                Pt1 = Pt2;
                Pt2 = t;
            }
        }
        public bool Contains(float y)
        {
            return ((y >= ymin) && (y <= ymax));
        }

        public Vector2 Lerp(int y)
        {
            float m = (Pt1.Y - Pt2.Y) / (Pt1.X - Pt2.X);
            float dy = Pt1.Y - y;
            float dx = dy / m;
            return new Vector2(Pt1.X - dx, y);
        }
    }

    public class ScanLineConverter
    {
        public static void ScanConvertPolygon(SpriteBatch batch, Color color, List<Edge>edges)
        {
            float ymin = float.MaxValue;
            float ymax = float.MinValue;
            foreach (Edge e in edges)
            {
                ymin = Math.Min(ymin, e.ymin);
                ymax = Math.Max(ymax, e.ymax);
            }
            for (int y = (int)ymin; y<=(int)ymax; y++)
            {
                List<Vector2> xps = new List<Vector2>();
                foreach (Edge e in edges)
                {
                    if (e.Contains(y))
                    {
                        xps.Add(e.Lerp(y));
                    }
                }
                List<Vector2> SortedList = xps.OrderBy(o => o.X).ToList();
                while (SortedList.Count>1)
                {
                    batch.DrawLine(SortedList[0], SortedList[1], color);
                    SortedList.RemoveAt(0);
                    SortedList.RemoveAt(0);
                }
            }

        }
    }
}
