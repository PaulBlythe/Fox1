using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering.Objects;
using GuruEngine.Rendering.VertexDeclarations;

namespace GuruEngine.Geometry
{
    public static class Geometry
    {
        /// <summary>
        /// Tapered cylinder along the Z axis
        /// </summary>
        /// <param name="sp">Start position</param>
        /// <param name="start_radius">In metres</param>
        /// <param name="end_radius">In metres</param>
        /// <param name="length">In metres</param>
        /// <param name="segments">Number of points per ridge</param>
        /// <param name="ridges">Number of ridges along the length</param>
        /// <returns></returns>
        public static TriangleMesh GenerateTaperedCylinder(Vector3 sp, float start_radius, float end_radius, float length, int segments, int ridges)
        {
            TriangleMesh result = new TriangleMesh();

            List<VertexPositionNormalTexture> Circles = new List<VertexPositionNormalTexture>();
            List<short> inds = new List<short>();

            float angle_step = (2 * MathHelper.Pi) / (segments - 1);
            float current_radius = start_radius;

            Vector3 cp = sp;
            float radius_step = (start_radius - end_radius) / ridges;

            float texx = 0;
            for (int i=0; i<=ridges; i++)
            {
                float angle = 0;
                
                for (int j=0; j<segments; j++)
                {
                    Matrix m = Matrix.CreateRotationZ(angle);

                    Vector3 n = Vector3.Transform(Vector3.Up, m);
                    Vector3 p = cp + (n * current_radius);
                    Vector2 t = new Vector2(texx, angle / MathHelper.TwoPi);

                    VertexPositionNormalTexture v = new VertexPositionNormalTexture();
                    v.Position = p;
                    v.Normal = n;
                    v.TextureCoordinate = t;
                    Circles.Add(v);

                    angle += angle_step;
                }
                cp.Z += (length / ridges);
                current_radius -= radius_step;
                texx += 1.0f / (ridges);
            }
            result.Vertices = Circles.ToArray();

            int sv = 0;
            for (int i=0; i<ridges; i++)
            {
                for (int j=0; j<segments; j++)
                {
                    if (j == segments - 1)
                    {
                        inds.Add((short)(sv + j));
                        inds.Add((short)sv);
                        inds.Add((short)(sv + segments));

                        inds.Add((short)(sv + j));
                        inds.Add((short)(sv + j + segments));
                        inds.Add((short)(sv + segments));
                    }
                    else
                    {
                        inds.Add((short)(sv + j));
                        inds.Add((short)(sv + j + 1));
                        inds.Add((short)(sv + j + 1 + segments));

                        inds.Add((short)(sv + j));
                        inds.Add((short)(sv + j + segments));
                        inds.Add((short)(sv + j + 1 + segments));
                    }


                }
                sv += segments;
            }
            result.Indices = inds.ToArray();

            return result;
        }

        /// <summary>
        /// Vertical post 
        /// </summary>
        /// <param name="height">in metres</param>
        /// <param name="side">thickness</param>
        /// <returns></returns>
        public static TriangleMesh GeneratePost(float height, float side)
        {
            TriangleMesh result = new TriangleMesh();

            Vector3 top1 = new Vector3(-side, height, -side);
            Vector3 top2 = new Vector3(-side, height,  side);
            Vector3 top3 = new Vector3( side, height,  side);
            Vector3 top4 = new Vector3( side, height, -side);


            Vector3 b1 = new Vector3(-side, 0, -side);
            Vector3 b2 = new Vector3(-side, 0,  side);
            Vector3 b3 = new Vector3( side, 0,  side);
            Vector3 b4 = new Vector3( side, 0, -side);

            List<VertexPositionNormalTexture> verts = new List<VertexPositionNormalTexture>();
            List<short> inds = new List<short>();

            // Top
            VertexPositionNormalTexture v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Up;
            v.Position = top1;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Up;
            v.Position = top2;
            v.TextureCoordinate = new Vector2(0, 1);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Up;
            v.Position = top3;
            v.TextureCoordinate = new Vector2(1, 1);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Up;
            v.Position = top4;
            v.TextureCoordinate = new Vector2(1, 0);
            verts.Add(v);

            inds.Add(0); inds.Add(1); inds.Add(2);
            inds.Add(0); inds.Add(2); inds.Add(3);

            // Bottom
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Down;
            v.Position = b1;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Down;
            v.Position = b2;
            v.TextureCoordinate = new Vector2(0, 1);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Down;
            v.Position = b3;
            v.TextureCoordinate = new Vector2(1, 1);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Down;
            v.Position = b4;
            v.TextureCoordinate = new Vector2(1, 0);
            verts.Add(v);

            inds.Add(0 + 4); inds.Add(1 + 4); inds.Add(2 + 4);
            inds.Add(0 + 4); inds.Add(2 + 4); inds.Add(3 + 4);

            // Left
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Left;
            v.Position = top1;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Left;
            v.Position = b1;
            v.TextureCoordinate = new Vector2(0, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Left;
            v.Position = b2;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Left;
            v.Position = top2;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);

            inds.Add(0 + 8); inds.Add(1 + 8); inds.Add(2 + 8);
            inds.Add(0 + 8); inds.Add(2 + 8); inds.Add(3 + 8);

            // Right
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Right;
            v.Position = top3;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Right;
            v.Position = b3;
            v.TextureCoordinate = new Vector2(0, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Right;
            v.Position = b4;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Right;
            v.Position = top4;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);

            inds.Add(0 + 12); inds.Add(1 + 12); inds.Add(2 + 12);
            inds.Add(0 + 12); inds.Add(2 + 12); inds.Add(3 + 12);


            // Back
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Backward;
            v.Position = top1;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Backward;
            v.Position = b1;
            v.TextureCoordinate = new Vector2(0, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Backward;
            v.Position = b4;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Backward;
            v.Position = top4;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);

            inds.Add(0 + 16); inds.Add(1 + 16); inds.Add(2 + 16);
            inds.Add(0 + 16); inds.Add(2 + 16); inds.Add(3 + 16);

            // Front
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Forward;
            v.Position = top2;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Forward;
            v.Position = b2;
            v.TextureCoordinate = new Vector2(0, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Forward;
            v.Position = b3;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Forward;
            v.Position = top3;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);

            inds.Add(0 + 20); inds.Add(1 + 20); inds.Add(2 + 20);
            inds.Add(0 + 20); inds.Add(2 + 20); inds.Add(3 + 20);


            result.Vertices = verts.ToArray();
            result.Indices = inds.ToArray();
            return result;
        }

        /// <summary>
        /// Generate a post centred on 0,0,0
        /// </summary>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static TriangleMesh GenerateCentredPost(float length, float width)
        {
            TriangleMesh result = new TriangleMesh();
            float height = length / 2;
            float side = width / 2;

            Vector3 top1 = new Vector3(-side, height, -side);
            Vector3 top2 = new Vector3(-side, height, side);
            Vector3 top3 = new Vector3(side, height, side);
            Vector3 top4 = new Vector3(side, height, -side);


            Vector3 b1 = new Vector3(-side, -height, -side);
            Vector3 b2 = new Vector3(-side, -height, side);
            Vector3 b3 = new Vector3( side, -height, side);
            Vector3 b4 = new Vector3( side, -height, -side);

            List<VertexPositionNormalTexture> verts = new List<VertexPositionNormalTexture>();
            List<short> inds = new List<short>();

            // Top
            VertexPositionNormalTexture v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Up;
            v.Position = top1;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Up;
            v.Position = top2;
            v.TextureCoordinate = new Vector2(0, 1);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Up;
            v.Position = top3;
            v.TextureCoordinate = new Vector2(1, 1);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Up;
            v.Position = top4;
            v.TextureCoordinate = new Vector2(1, 0);
            verts.Add(v);

            inds.Add(0); inds.Add(1); inds.Add(2);
            inds.Add(0); inds.Add(2); inds.Add(3);

            // Bottom
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Down;
            v.Position = b1;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Down;
            v.Position = b2;
            v.TextureCoordinate = new Vector2(0, 1);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Down;
            v.Position = b3;
            v.TextureCoordinate = new Vector2(1, 1);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Down;
            v.Position = b4;
            v.TextureCoordinate = new Vector2(1, 0);
            verts.Add(v);

            inds.Add(0 + 4); inds.Add(1 + 4); inds.Add(2 + 4);
            inds.Add(0 + 4); inds.Add(2 + 4); inds.Add(3 + 4);

            // Left
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Left;
            v.Position = top1;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Left;
            v.Position = b1;
            v.TextureCoordinate = new Vector2(0, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Left;
            v.Position = b2;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Left;
            v.Position = top2;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);

            inds.Add(0 + 8); inds.Add(1 + 8); inds.Add(2 + 8);
            inds.Add(0 + 8); inds.Add(2 + 8); inds.Add(3 + 8);

            // Right
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Right;
            v.Position = top3;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Right;
            v.Position = b3;
            v.TextureCoordinate = new Vector2(0, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Right;
            v.Position = b4;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Right;
            v.Position = top4;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);

            inds.Add(0 + 12); inds.Add(1 + 12); inds.Add(2 + 12);
            inds.Add(0 + 12); inds.Add(2 + 12); inds.Add(3 + 12);


            // Back
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Backward;
            v.Position = top1;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Backward;
            v.Position = b1;
            v.TextureCoordinate = new Vector2(0, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Backward;
            v.Position = b4;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Backward;
            v.Position = top4;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);

            inds.Add(0 + 16); inds.Add(1 + 16); inds.Add(2 + 16);
            inds.Add(0 + 16); inds.Add(2 + 16); inds.Add(3 + 16);

            // Front
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Forward;
            v.Position = top2;
            v.TextureCoordinate = new Vector2(0, 0);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Forward;
            v.Position = b2;
            v.TextureCoordinate = new Vector2(0, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Forward;
            v.Position = b3;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);
            v = new VertexPositionNormalTexture();
            v.Normal = Vector3.Forward;
            v.Position = top3;
            v.TextureCoordinate = new Vector2(side, height);
            verts.Add(v);

            inds.Add(0 + 20); inds.Add(1 + 20); inds.Add(2 + 20);
            inds.Add(0 + 20); inds.Add(2 + 20); inds.Add(3 + 20);


            result.Vertices = verts.ToArray();
            result.Indices = inds.ToArray();
            return result;
        }


        public static TriangleMesh GeneratePlane(Vector3 centre, float scale, float texturescale)
        {
            TriangleMesh t = new TriangleMesh();

            t.Vertices = new VertexPositionNormalTexture[4];
            t.Indices = new short[6];

            t.Vertices[0] = new VertexPositionNormalTexture();
            t.Vertices[0].Position = new Vector3(-scale, 0, -scale);
            t.Vertices[0].Normal = Vector3.Up;
            t.Vertices[0].TextureCoordinate = new Vector2(-scale / texturescale, -scale / texturescale);

            t.Vertices[1] = new VertexPositionNormalTexture();
            t.Vertices[1].Position = new Vector3(-scale, 0, scale);
            t.Vertices[1].Normal = Vector3.Up;
            t.Vertices[1].TextureCoordinate = new Vector2(-scale / texturescale, scale / texturescale);

            t.Vertices[2] = new VertexPositionNormalTexture();
            t.Vertices[2].Position = new Vector3(scale, 0, -scale);
            t.Vertices[2].Normal = Vector3.Up;
            t.Vertices[2].TextureCoordinate = new Vector2(scale / texturescale, -scale / texturescale);

            t.Vertices[3] = new VertexPositionNormalTexture();
            t.Vertices[3].Position = new Vector3(scale, 0, scale);
            t.Vertices[3].Normal = Vector3.Up;
            t.Vertices[3].TextureCoordinate = new Vector2(scale / texturescale, scale / texturescale);

            t.Indices[0] = 0;
            t.Indices[1] = 1;
            t.Indices[2] = 3;

            t.Indices[3] = 0;
            t.Indices[4] = 3;
            t.Indices[5] = 2;

            return t;
        }

        /// <summary>
        /// Create a cone aligned along the Right vector
        /// </summary>
        /// <param name="length"></param>
        /// <param name="steps"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static TriangleMesh GenerateCone(float length, float steps, float radius)
        {
            TriangleMesh mesh = new TriangleMesh();
            List<VertexPositionNormalTexture> Circle = new List<VertexPositionNormalTexture>();
            List<short> inds = new List<short>();

            VertexPositionNormalTexture centre = new VertexPositionNormalTexture();
            centre.Position = new Vector3(0, 0, 0);
            centre.TextureCoordinate = new Vector2(0.5f, 0.5f);
            centre.Normal = Vector3.Up;
            Circle.Add(centre);

            float angle = 0;
            float angle_step = (2 * MathHelper.Pi) / (steps - 1);
            for (int j = 0; j < steps; j++)
            {
                Matrix m = Matrix.CreateRotationZ(angle);

                Vector3 n = Vector3.Transform(Vector3.Right, m);
                Vector3 p = (n * radius);
                p.Z += length;
                Vector2 t = new Vector2(1, angle / MathHelper.TwoPi);

                VertexPositionNormalTexture v = new VertexPositionNormalTexture();
                v.Position = p;
                v.Normal = n;
                v.TextureCoordinate = t;
                Circle.Add(v);

                angle += angle_step;
            }
            mesh.Vertices = Circle.ToArray();
            int p1 = 1;
            for (int i=0; i<steps; i++)
            {
                inds.Add(0);
                inds.Add((short)p1);
                if (i == steps - 1)
                    inds.Add((short)1);
                else
                    inds.Add((short)(p1 + 1));
                p1++;
            }
            mesh.Indices = inds.ToArray();

            return mesh;
        }
    }
}
