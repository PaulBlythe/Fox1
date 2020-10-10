using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GUITestbed.Tools;

namespace GUITestbed.Rendering._3D
{
    public class WavefrontPoint
    {
        public int Vertex;
        public int Normal;
        public int Texture;

        public WavefrontPoint(String s)
        {
            char[] splits = new char[] { '/' };

            string[] parts = s.Split(splits);
            Vertex = int.Parse(parts[0]) - 1;
            Texture = int.Parse(parts[1]) - 1;
            Normal = int.Parse(parts[2]) - 1;

        }
    }

    public class WavefrontTriangle
    {
        public WavefrontPoint p1;
        public WavefrontPoint p2;
        public WavefrontPoint p3;
    }

    public class WavefrontFaceGroup
    {
        public String Material;
        public List<WavefrontTriangle> Triangles = new List<WavefrontTriangle>();
        public List<VertexPositionNormalTexture> verts = new List<VertexPositionNormalTexture>();
        public BasicEffect Effect;
        public Texture2D Texture;
    }

    public class WavefrontMaterial
    {
        public String TexName;
        public float Ns;
        public float d;
        public int Illum;
        public Vector3 diffuse;
        public Vector3 ambient;
        public Vector3 specular;
    }

    public class Wavefront
    {
        List<WavefrontFaceGroup> facegroups = new List<WavefrontFaceGroup>();
        List<Vector3> Positions = new List<Vector3>();
        List<Vector3> Normals = new List<Vector3>();
        List<Vector2> TexCoords = new List<Vector2>();
        Dictionary<String, WavefrontMaterial> Materials = new Dictionary<string, WavefrontMaterial>();
        public BoundingBox boundingBox;

        public Wavefront(String file)
        {
            char[] splits = new char[] { ' ', ',' };
            string line;
            StreamReader reader = new System.IO.StreamReader(file);
            WavefrontFaceGroup fg = null;

            float xmin = float.MaxValue;
            float xmax = float.MinValue;
            float ymin = float.MaxValue;
            float ymax = float.MinValue;
            float zmin = float.MaxValue;
            float zmax = float.MinValue;


            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    switch (parts[0])
                    {
                        case "mtllib":
                            LoadMaterials(parts[1], file);
                            break;

                        case "v":
                            {
                                Vector3 v = new Vector3();
                                v.X = float.Parse(parts[1]);
                                v.Y = float.Parse(parts[2]);
                                v.Z = float.Parse(parts[3]);
                                Positions.Add(v);

                                if (v.X < xmin)
                                    xmin = v.X;
                                if (v.Y < ymin)
                                    ymin = v.Y;
                                if (v.Z < zmin)
                                    zmin = v.Z;

                                if (v.X > xmax)
                                    xmax = v.X;
                                if (v.Y > ymax)
                                    ymax = v.Y;
                                if (v.Z > zmax)
                                    zmax = v.Z;


                            }
                            break;

                        case "vt":
                            {
                                Vector2 v = new Vector2();
                                v.X = float.Parse(parts[1]);
                                v.Y = float.Parse(parts[2]);
                                TexCoords.Add(v);
                            }
                            break;

                        case "vn":
                            {
                                Vector3 v = new Vector3();
                                v.X = float.Parse(parts[1]);
                                v.Y = float.Parse(parts[2]);
                                v.Z = float.Parse(parts[3]);
                                Normals.Add(v);
                            }
                            break;

                        case "usemtl":
                            fg = new WavefrontFaceGroup();
                            fg.Material = parts[1];
                            fg.Effect = new BasicEffect(Game1.Instance.GraphicsDevice);
                            facegroups.Add(fg);
                            break;

                        case "f":
                            if (parts.Length == 4)
                            {
                                WavefrontTriangle t1 = new WavefrontTriangle();
                                t1.p1 = new WavefrontPoint(parts[1]);
                                t1.p2 = new WavefrontPoint(parts[2]);
                                t1.p3 = new WavefrontPoint(parts[3]);
                                fg.Triangles.Add(t1);
                            }
                            else
                            {
                                WavefrontTriangle t1 = new WavefrontTriangle();
                                t1.p1 = new WavefrontPoint(parts[1]);
                                t1.p2 = new WavefrontPoint(parts[2]);
                                t1.p3 = new WavefrontPoint(parts[3]);
                                fg.Triangles.Add(t1);

                                WavefrontTriangle t2 = new WavefrontTriangle();
                                t2.p1 = new WavefrontPoint(parts[1]);
                                t2.p2 = new WavefrontPoint(parts[3]);
                                t2.p3 = new WavefrontPoint(parts[4]);
                                fg.Triangles.Add(t2);
                            }
                            break;
                    }
                }
            }

            foreach (WavefrontFaceGroup w in facegroups)
            {
                foreach (WavefrontTriangle wt in w.Triangles)
                {
                    VertexPositionNormalTexture vpt = new VertexPositionNormalTexture();
                    vpt.Position = Positions[wt.p1.Vertex];
                    vpt.Normal = Normals[wt.p1.Normal];
                    vpt.TextureCoordinate = TexCoords[wt.p1.Texture];
                    w.verts.Add(vpt);

                    vpt = new VertexPositionNormalTexture();
                    vpt.Position = Positions[wt.p2.Vertex ];
                    vpt.Normal = Normals[wt.p2.Normal];
                    vpt.TextureCoordinate = TexCoords[wt.p2.Texture];
                    w.verts.Add(vpt);

                    vpt = new VertexPositionNormalTexture();
                    vpt.Position = Positions[wt.p3.Vertex];
                    vpt.Normal = Normals[wt.p3.Normal];
                    vpt.TextureCoordinate = TexCoords[wt.p3.Texture];
                    w.verts.Add(vpt);
                }

                w.Effect.VertexColorEnabled = false;
                w.Effect.DiffuseColor = Materials[w.Material].diffuse;
                w.Effect.SpecularColor = Materials[w.Material].specular;
                w.Effect.AmbientLightColor = Materials[w.Material].ambient;
                w.Effect.SpecularPower = Materials[w.Material].Ns;
                w.Effect.TextureEnabled = true;

                String dir = Path.GetDirectoryName(file);
                String ft = Path.Combine(dir, Materials[w.Material].TexName);
                w.Texture = TextureLoader.Load(ft);

                boundingBox = new BoundingBox(new Vector3(xmin, ymin, zmin), new Vector3(xmax, ymax, zmax));
            }

            
        }

        public void Draw(Matrix Projection, Matrix View, Matrix World)
        {
            foreach (WavefrontFaceGroup w in facegroups)
            {
                w.Effect.Projection = Projection;
                w.Effect.World = World;
                w.Effect.View = View;
                w.Effect.Texture = w.Texture;

                foreach (EffectPass pass in w.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Game1.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, w.verts.ToArray(), 0, w.verts.Count / 3);
                }
            }
        }

        public void Draw(Effect fx, Matrix Projection, Matrix View, Matrix World)
        {
            foreach (WavefrontFaceGroup w in facegroups)
            {
                fx.Parameters["Projection"].SetValue(Projection);
                fx.Parameters["World"].SetValue(World);
                fx.Parameters["View"].SetValue(View);
                fx.Parameters["ModelTexture"].SetValue(w.Texture);

                foreach (EffectPass pass in fx.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Game1.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, w.verts.ToArray(), 0, w.verts.Count / 3);
                }
            }
        }

        public void Draw(Effect fx)
        {
            foreach (WavefrontFaceGroup w in facegroups)
            {
                foreach (EffectPass pass in fx.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Game1.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, w.verts.ToArray(), 0, w.verts.Count / 3);
                }
            }
        }

        public void LoadMaterials(String f, String p)
        {
            String dir = Path.GetDirectoryName(p);
            String ml = Path.Combine(dir, f);
            char[] splits = new char[] { ' ', ',' ,'\t'};
            string line;
            StreamReader reader = new System.IO.StreamReader(ml);
            WavefrontMaterial mat = null;

            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    switch (parts[0])
                    {
                        case "newmtl":
                            {
                                mat = new WavefrontMaterial();
                                Materials.Add(parts[1], mat);
                            }
                            break;

                        case "Ns":
                            {
                                mat.Ns = float.Parse(parts[1]);
                            }
                            break;

                        case "d":
                            {
                                mat.d = float.Parse(parts[1]);
                            }
                            break;

                        case "illum":
                            {
                                mat.Illum = int.Parse(parts[1]);
                            }
                            break;

                        case "Kd":
                            {
                                Vector3 v = new Vector3();
                                v.X = float.Parse(parts[1]);
                                v.Y = float.Parse(parts[2]);
                                v.Z = float.Parse(parts[3]);

                                mat.diffuse = v;
                            }
                            break;

                        case "Ka":
                            {
                                Vector3 v = new Vector3();
                                v.X = float.Parse(parts[1]);
                                v.Y = float.Parse(parts[2]);
                                v.Z = float.Parse(parts[3]);

                                mat.ambient = v;
                            }
                            break;

                        case "Ks":
                            {
                                Vector3 v = new Vector3();
                                v.X = float.Parse(parts[1]);
                                v.Y = float.Parse(parts[2]);
                                v.Z = float.Parse(parts[3]);

                                mat.specular = v;
                            }
                            break;

                        case "map_Kd":
                            mat.TexName = parts[1];
                            break;

                    }
                }
            }
        }
    }
}
