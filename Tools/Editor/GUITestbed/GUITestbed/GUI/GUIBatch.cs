using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI
{
    public class GUIBatch
    {
        GraphicsDevice Device;
        Effect Shader;
        BlendState blend;
        RasterizerState stat = new RasterizerState();

        List<GUIVertex> Vertices = new List<GUIVertex>();

        float time = 0;
        Vector3 lightpos = new Vector3(1 ,- 0.314184f , 1.072981f);

        Dictionary<String, List<Vector2>> circleCache = new Dictionary<string, List<Vector2>>();

        public GUIBatch(GraphicsDevice device, Effect shader)
        {
            Device = device;
            Shader = shader;
            stat.CullMode = CullMode.None;
        }

        public void Begin(BlendState bs)
        {
            blend = bs;
            Vertices.Clear();

            Shader.Parameters["LightDirection2"].SetValue(lightpos);
        }

        public void End()
        {
            Device.BlendState = blend;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = stat;
            GUIVertex[] v = Vertices.ToArray();

            foreach (EffectPass pass in Shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawUserPrimitives<GUIVertex>(PrimitiveType.TriangleList, v, 0, v.Length / 3);
            }
        }

        public void FillRectangle(Rectangle r, Color c)
        {
            Vector4 rv = new Vector4(r.X, r.Y, r.Width, r.Height);

            Vector3 tl = new Vector3(r.X, r.Y, -0.5f);
            Vector3 tr = new Vector3(r.X + r.Width, r.Y, -0.5f);
            Vector3 bl = new Vector3(r.X, r.Y + r.Height, -0.5f);
            Vector3 br = new Vector3(r.X + r.Width, r.Y + r.Height, -0.5f);

            GUIVertex v1 = new GUIVertex(tl, c, rv, new Vector2(0, 0));
            GUIVertex v2 = new GUIVertex(tr, c, rv, new Vector2(1, 0));
            GUIVertex v3 = new GUIVertex(br, c, rv, new Vector2(1, 1));
            GUIVertex v4 = new GUIVertex(bl, c, rv, new Vector2(0, 1));

            Vertices.Add(v1);
            Vertices.Add(v2);
            Vertices.Add(v3);

            Vertices.Add(v1);
            Vertices.Add(v3);
            Vertices.Add(v4);


        }

        public void FillRectangle2(Rectangle r, Color c)
        {
            Vector4 rv = new Vector4(r.X, r.Y, r.Width, r.Height);

            Vector3 tl = new Vector3(r.X, r.Y, -0.5f);
            Vector3 tr = new Vector3(r.X + r.Width, r.Y, -0.5f);
            Vector3 bl = new Vector3(r.X, r.Y + r.Height, -0.5f);
            Vector3 br = new Vector3(r.X + r.Width, r.Y + r.Height, -0.5f);

            GUIVertex v1 = new GUIVertex(tl, c, rv, new Vector2(0, 1));
            GUIVertex v2 = new GUIVertex(tr, c, rv, new Vector2(1, 1));
            GUIVertex v3 = new GUIVertex(br, c, rv, new Vector2(1, -1));
            GUIVertex v4 = new GUIVertex(bl, c, rv, new Vector2(0, -1));

            Vertices.Add(v1);
            Vertices.Add(v2);
            Vertices.Add(v3);

            Vertices.Add(v1);
            Vertices.Add(v3);
            Vertices.Add(v4);


        }


        private List<Vector2> CreateCircle(double radius, int sides)
        {
            // Look for a cached version of this circle
            String circleKey = radius + "x" + sides;
            if (circleCache.ContainsKey(circleKey))
            {
                return circleCache[circleKey];
            }

            List<Vector2> vectors = new List<Vector2>();

            const double max = 2.0 * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta))));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(new Vector2((float)(radius * Math.Cos(0)), (float)(radius * Math.Sin(0))));

            // Cache this circle so that it can be quickly drawn next time
            circleCache.Add(circleKey, vectors);

            return vectors;
        }

        private List<Vector2> CreateArc(float radius, int sides, float startingAngle, float radians)
        {
            List<Vector2> points = new List<Vector2>();
            points.AddRange(CreateCircle(radius, sides));
            points.RemoveAt(points.Count - 1); // remove the last point because it's a duplicate of the first

            // The circle starts at (radius, 0)
            double curAngle = 0.0;
            double anglePerSide = MathHelper.TwoPi / sides;

            // "Rotate" to the starting point
            while ((curAngle + (anglePerSide / 2.0)) < startingAngle)
            {
                curAngle += anglePerSide;

                // move the first point to the end
                points.Add(points[0]);
                points.RemoveAt(0);
            }

            // Add the first point, just in case we make a full circle
            points.Add(points[0]);

            // Now remove the points at the end of the circle to create the arc
            int sidesInArc = (int)((radians / anglePerSide) + 0.5);
            points.RemoveRange(sidesInArc + 1, points.Count - sidesInArc - 1);

            return points;
        }

        public void FillArc(Vector2 center, float radius, int sides, float startingAngle, float radians, Color color)
        {
            List<Vector2> arc = CreateArc(radius, sides, startingAngle, radians);
            List<GUIVertex> carc = new List<GUIVertex>();
            Vector4 rv = new Vector4(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            foreach (Vector2 v in arc)
            {
                Vector2 v3 = v + center;
                GUIVertex gv = new GUIVertex();
                gv.Color = color;
                gv.Region = rv;
                gv.TextureCoordinates = new Vector2();
                gv.TextureCoordinates.X = Math.Abs(v3.X - center.X)  / (float)(radius - 1);
                gv.TextureCoordinates.Y = Math.Abs(v3.Y - center.Y)  / (float)(radius - 1);
                gv.Position = new Vector3(v3.X, v3.Y, -0.5f);
                carc.Add(gv);
            }
            GUIVertex gvc = new GUIVertex();
            gvc.Color = color;
            gvc.Region = rv;
            gvc.TextureCoordinates = new Vector2(0,0);
            gvc.Position = new Vector3(center.X, center.Y, -0.5f);

            int p1 = 0;
            int p2 = 1;
            while (p2<carc.Count)
            {
                Vertices.Add(gvc);
                Vertices.Add(carc[p2]);
                Vertices.Add(carc[p1]);

                p1++;
                p2++;
            }


        }
    }
}
