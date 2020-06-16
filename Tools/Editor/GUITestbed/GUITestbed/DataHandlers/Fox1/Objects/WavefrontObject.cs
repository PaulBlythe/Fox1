using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using GUITestbed.Rendering._3D;

namespace GUITestbed.DataHandlers.Fox1.Objects
{

    public class wVertex
    {
        public int Position;
        public int Normal;
        public int Texture;
    }

    public class wTriangle
    {
        public wVertex p1;
        public wVertex p2;
        public wVertex p3;
    }

    public class wMaterial
    {
        public String Name;
        public Vector3 Ka;
        public Vector3 Kd;
        public Vector3 Ks;
        public float Ns = 10;
        public float Ni = 1;
        public float Alpha = 1;

        // illum 0:             a constant color illumination model, using the Kd for the material
        // illum 1:             a diffuse illumination model using Lambertian shading, 
        //                          taking into account Ka, Kd, the intensity and position of each light source and the angle at which it strikes the surface.
        // illum 2:             a diffuse and specular illumination model using Lambertian shading and Blinn's interpretation of Phong's specular illumination model,
        //                          taking into account Ka, Kd, Ks, and the intensity and position of each light source and the angle at which it strikes the surface.
        public int Illum = 2;
        public String Texture;
        public String AOTexture = null;
        public String PBRTexture = null;
        public CullMode cullMode = CullMode.None;

    }

    public class wGroup
    {
        public String material;
        public List<wTriangle> triangles = new List<wTriangle>();
    }


    public class Wavefront
    {
        String MaterialLibrary = "";
        String Name = "";
        String Directory;

        public List<Vector3> Positions = new List<Vector3>();
        List<Vector3> Normals = new List<Vector3>();
        public List<Vector2> Textures = new List<Vector2>();
        public List<wTriangle> Faces = new List<wTriangle>();
        public List<wGroup> Groups = new List<wGroup>();
        Dictionary<String, wMaterial> Materials = new Dictionary<string, wMaterial>();

        public Wavefront(String filename)
        {
            Name = Path.GetFileNameWithoutExtension(filename);
            Directory = Path.GetDirectoryName(filename);

            char[] splits = new char[] { ' ', '\t' };
            string line = null;
            TextReader readFile = new StreamReader(filename);
            bool generate_normals = true;
            while (true)
            {
                line = readFile.ReadLine();
                if (line == null)
                {
                    break;
                }
                string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    switch (parts[0])
                    {
                        case "#":           // comments
                            break;

                        case "mtllib":
                            MaterialLibrary = parts[1];
                            break;

                        case "usemtl":
                            {
                                wGroup g = new wGroup();
                                g.material = parts[1];
                                Groups.Add(g);
                            }
                            break;

                        case "v":
                            {
                                Vector3 v = new Vector3();
                                v.X = float.Parse(parts[1]);
                                v.Y = float.Parse(parts[2]);
                                v.Z = float.Parse(parts[3]);

                                Positions.Add(v);
                            }
                            break;

                        case "vn":
                            {
                                Vector3 v = new Vector3();
                                v.X = float.Parse(parts[1]);
                                v.Y = float.Parse(parts[2]);
                                v.Z = float.Parse(parts[3]);
                                generate_normals = false;
                                Normals.Add(v);
                            }
                            break;

                        case "vt":
                            {
                                Vector2 v = new Vector2();
                                v.X = float.Parse(parts[1]);
                                v.Y = 1.0f - float.Parse(parts[2]);

                                Textures.Add(v);
                            }
                            break;

                        case "f":
                            {
                                wTriangle v = new wTriangle();
                                v.p1 = ConvertVertex(parts[1]);
                                v.p2 = ConvertVertex(parts[2]);
                                v.p3 = ConvertVertex(parts[3]);
                                Groups[Groups.Count - 1].triangles.Add(v);
                            }
                            break;
                    }
                }
            }
            readFile.Close();
            readFile = null;

            #region Generate normals
            // If it is missing normals , then we have to generate them 
            if (generate_normals)
            {
                Normals.Capacity = Positions.Count;
                int[] Counts = new int[Positions.Count];

                for (int i = 0; i < Positions.Count; i++)
                {
                    Normals.Add(new Vector3(0, 0, 0));
                    Counts[i] = 0;
                }

                foreach (wGroup g in Groups)
                {
                    foreach (wTriangle wt in g.triangles)
                    {
                        Vector3 v1 = Positions[wt.p1.Position];
                        Vector3 v2 = Positions[wt.p2.Position];
                        Vector3 v3 = Positions[wt.p3.Position];

                        Vector3 d1 = v1 - v2;
                        Vector3 d2 = v2 - v3;

                        d1.Normalize();
                        d2.Normalize();

                        Vector3 Normal = Vector3.Cross(d1, d2);
                        Normal.Normalize();

                        Counts[wt.p1.Position]++;
                        Counts[wt.p2.Position]++;
                        Counts[wt.p3.Position]++;

                        Normals[wt.p1.Position] += Normal;
                        Normals[wt.p2.Position] += Normal;
                        Normals[wt.p3.Position] += Normal;

                        wt.p1.Normal = wt.p1.Position;
                        wt.p2.Normal = wt.p2.Position;
                        wt.p3.Normal = wt.p3.Position;

                    }
                }
                for (int i = 0; i < Normals.Count; i++)
                {
                    Normals[i] /= (float)Counts[i];
                }
            }
            #endregion

            #region Load material library
            String ml = Path.Combine(Directory, MaterialLibrary);
            if (File.Exists(ml))
            {
                TextReader rFile = new StreamReader(ml);
                wMaterial current = null;

                while (true)
                {
                    line = rFile.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        switch (parts[0])
                        {
                            case "newmtl":
                                {
                                    wMaterial w = new wMaterial();
                                    w.Name = parts[1];
                                    Materials.Add(w.Name, w);
                                    current = w;
                                }
                                break;

                            case "Ns":
                                {
                                    current.Ns = float.Parse(parts[1]);
                                }
                                break;

                            case "d":
                                {
                                    current.Alpha = float.Parse(parts[1]);
                                }
                                break;

                            case "illum":
                                {
                                    current.Illum = int.Parse(parts[1]);
                                }
                                break;

                            case "map_Kd":
                                {
                                    current.Texture = parts[1];
                                }
                                break;

                            case "Kd":
                                {
                                    current.Kd = new Vector3();
                                    current.Kd.X = float.Parse(parts[1]);
                                    current.Kd.Y = float.Parse(parts[2]);
                                    current.Kd.Z = float.Parse(parts[3]);
                                }
                                break;

                            case "Ka":
                                {
                                    current.Ka = new Vector3();
                                    current.Ka.X = float.Parse(parts[1]);
                                    current.Ka.Y = float.Parse(parts[2]);
                                    current.Ka.Z = float.Parse(parts[3]);

                                    if (current.Ka.LengthSquared() == 0)
                                    {
                                        current.Ka.X = current.Ka.Y = current.Ka.Z = 0.25f;
                                    }
                                }
                                break;

                            case "Ks":
                                {
                                    current.Ks = new Vector3();
                                    current.Ks.X = float.Parse(parts[1]);
                                    current.Ks.Y = float.Parse(parts[2]);
                                    current.Ks.Z = float.Parse(parts[3]);
                                }
                                break;
                        }
                    }

                }
                rFile.Close();
            }
            #endregion


        }

        wVertex ConvertVertex(string p)
        {
            wVertex w = new wVertex();
            string[] parts = p.Split('/');

            w.Position = int.Parse(parts[0]) - 1;
            if (parts.Length > 1)
                w.Texture = int.Parse(parts[1]) - 1;
            else
                w.Texture = w.Position;

            if (parts.Length > 2)
                w.Normal = int.Parse(parts[2]) - 1;
            else
                w.Normal = w.Position;

            return w;
        }


        public List<StaticMesh> GenerateMeshes(GraphicsDevice device, ContentManager content)
        {
            List<StaticMesh> meshes = new List<StaticMesh>();

            Matrix m = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(-90), 0);
            for (int i = 0; i < Positions.Count; i++)
                Positions[i] = Vector3.Transform(Positions[i], m);

            for (int i = 0; i < Normals.Count; i++)
                Normals[i] = Vector3.Transform(Normals[i], m);

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (Vector3 v in Positions)
            {
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            } 
            for (int i = 0; i < Positions.Count; i++)
                Positions[i] += new Vector3(0, -min.Y, 0);

            foreach (wGroup w in Groups)
            {
                StaticMesh sm = new StaticMesh();
                wMaterial mat = Materials[w.material];

                sm.Colour = new Vector4(mat.Kd.X, mat.Kd.Y, mat.Kd.Z, 1);
                sm.Shininess = mat.Ns;
                sm.Specular = new Vector4(mat.Ks.X, mat.Ks.Y, mat.Ks.Z, 1);
                sm.cullMode = mat.cullMode;

                switch (mat.Illum)
                {
                    case 0:
                    case 2:
                        {
                            sm.effect = ShaderManager.GetEffect("Shaders/SpecularDiffuseFog");

                            List<VertexPositionNormalTexture> vl = new List<VertexPositionNormalTexture>();
                            foreach (wTriangle tri in w.triangles)
                            {
                                VertexPositionNormalTexture v = new VertexPositionNormalTexture();
                                v.Position = Positions[tri.p1.Position];
                                v.Normal = Normals[tri.p1.Normal];
                                v.TextureCoordinate = Textures[tri.p1.Texture];

                                vl.Add(v);

                                v = new VertexPositionNormalTexture();
                                v.Position = Positions[tri.p2.Position];
                                v.Normal = Normals[tri.p2.Normal];
                                v.TextureCoordinate = Textures[tri.p2.Texture];

                                vl.Add(v);

                                v = new VertexPositionNormalTexture();
                                v.Position = Positions[tri.p3.Position];
                                v.Normal = Normals[tri.p3.Normal];
                                v.TextureCoordinate = Textures[tri.p3.Texture];

                                vl.Add(v);
                            }
                            VertexPositionNormalTexture[] data = vl.ToArray();
                            sm.verts = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vl.Count, BufferUsage.None);
                            sm.verts.SetData<VertexPositionNormalTexture>(data);
                            sm.nverts = vl.Count;

                            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                            foreach(Vector3 v in Positions)
                            {
                                min = Vector3.Min(min, v);
                                max = Vector3.Max(max, v);
                            }
                            sm.Bounds = new BoundingBox(min, max);

                            if (mat.Texture != null)
                            {
                                if (mat.Texture.EndsWith(".DDS"))
                                    mat.Texture = mat.Texture.Replace(".DDS", ".png");

                                String tp = Path.Combine(Directory, mat.Texture);
                                if (!File.Exists(tp))
                                {
                                    String np = mat.Texture.ToLower();
                                    if (File.Exists(Path.Combine(Directory, np)))
                                    {
                                        tp = Path.Combine(Directory, np);
                                    }
                                    
                                }
                                if (File.Exists(tp))
                                {
                                    using (var stream = new System.IO.FileStream(tp, FileMode.Open))
                                    {
                                        sm.texture = Texture2D.FromStream(device, stream);
                                    }
                                }
                                else
                                {
                                    Color[] fill = new Color[] { Color.White, Color.White, Color.White, Color.White };
                                    sm.texture = new Texture2D(device, 2, 2);
                                    sm.texture.SetData<Color>(fill);
                                }
                            }
                            else
                            {
                                Color[] fill = new Color[] { Color.White, Color.White, Color.White, Color.White };
                                sm.texture = new Texture2D(device, 2, 2);
                                sm.texture.SetData<Color>(fill);

                            }

                            sm.Colour = new Vector4(mat.Kd.X, mat.Kd.Y, mat.Kd.Z, mat.Alpha);
                            sm.Ambient = new Vector4(mat.Ka.X, mat.Ka.Y, mat.Ka.Z, mat.Alpha);
                            sm.Specular = new Vector4(mat.Ks.X, mat.Ks.Y, mat.Ks.Z, mat.Alpha);
                            sm.Shininess = mat.Ns * 10;

                            sm.World = Matrix.Identity;
                            meshes.Add(sm);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                
            }

            return meshes;
        }
    }
}
 