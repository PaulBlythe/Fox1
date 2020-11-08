using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GUITestbed.Rendering._3D;

namespace GUITestbed.Scenes
{
    public class ObjectInstance
    {
        public Matrix World;
        public int Object;
        public RenderTarget2D LightMap;
    }

    public class SpotLight
    {
        public Vector3 Direction;
        public Vector3 Position;
        public float ConeAngle;
    }

    public class LightBox
    {
        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 Axis;
        public float Width;
        public float Depth;
        public BoundingBox Bounds;

        public void Init()
        {
            Vector3 dx = Axis * Width * 0.5f;
            Vector3 dz = Vector3.Cross(Axis, -Direction) * Depth * 0.5f;
            Vector3 min = Position - dx - dz;
            Vector3 max = Position + dx + dz;
            min.Y = -2;
            Bounds = new BoundingBox(min, max);
        }
    }

    public class TranslateAnimator
    {
        public int Object;
        public Vector3 BasePosition;
        public Vector3 LowPosition;
        public Vector3 HighPosition;
        public Vector3 Rotation;
        public float MinValue;
        public float MaxValue;
        public String Key;
        public RenderTarget2D LightMap;
    }

    public class Scene
    {
        public List<String> Objects = new List<string>();
        public List<ObjectInstance> Instances = new List<ObjectInstance>();
        public List<LightBox> LightBoxes = new List<LightBox>();
        public List<SpotLight> SpotLights = new List<SpotLight>();
        public List<TranslateAnimator> tanims = new List<TranslateAnimator>();

        public List<Wavefront> Meshes = new List<Wavefront>();

        public Scene(String file)
        {
            char[] splits = new char[] { ' ', ',' };
            string line;
            System.IO.StreamReader reader = new System.IO.StreamReader(file);
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                if (line.StartsWith("#"))
                {
                    
                }else if (line.StartsWith("object"))
                {
                    Objects.Add(parts[1]);
                }else if (line.StartsWith("instance"))
                {
                    int obj = int.Parse(parts[1]);

                    float x = float.Parse(parts[2]);
                    float y = float.Parse(parts[3]);
                    float z = float.Parse(parts[4]);

                    float h = MathHelper.ToRadians(float.Parse(parts[5]));
                    float p = MathHelper.ToRadians(float.Parse(parts[6]));
                    float r = MathHelper.ToRadians(float.Parse(parts[7]));

                    Matrix w = Matrix.CreateFromYawPitchRoll(h, p, r) * Matrix.CreateTranslation(x, y, z);
                    ObjectInstance oi = new ObjectInstance();
                    oi.Object = obj;
                    oi.World = w;

                    Instances.Add(oi);
                }else if (line.StartsWith("lightbox")){

                    LightBox lb = new LightBox();

                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    lb.Position = new Vector3(x, y, z);

                    x = float.Parse(parts[4]);
                    y = float.Parse(parts[5]);
                    z = float.Parse(parts[6]);
                    lb.Direction = new Vector3(x, y, z);

                    lb.Width = float.Parse(parts[7]);
                    lb.Depth = float.Parse(parts[8]);

                    x = float.Parse(parts[9]);
                    y = float.Parse(parts[10]);
                    z = float.Parse(parts[11]);
                    lb.Axis = new Vector3(x, y, z);
                    lb.Init();
                    LightBoxes.Add(lb);
                }else if (line.StartsWith("tinstance"))
                {
                    TranslateAnimator ta = new TranslateAnimator();
                    ta.Object = int.Parse(parts[1]);

                    float x = float.Parse(parts[2]);
                    float y = float.Parse(parts[3]);
                    float z = float.Parse(parts[4]);
                    ta.BasePosition = new Vector3(x, y, z);

                    float h = MathHelper.ToRadians(float.Parse(parts[5]));
                    float p = MathHelper.ToRadians(float.Parse(parts[6]));
                    float r = MathHelper.ToRadians(float.Parse(parts[7]));
                    ta.Rotation = new Vector3(h, p, r);

                    ta.Key = parts[8];
                    ta.MinValue = float.Parse(parts[9]);
                    ta.MaxValue = float.Parse(parts[10]);

                    x = float.Parse(parts[11]);
                    y = float.Parse(parts[12]);
                    z = float.Parse(parts[13]);
                    ta.LowPosition = new Vector3(x, y, z);

                    x = float.Parse(parts[14]);
                    y = float.Parse(parts[15]);
                    z = float.Parse(parts[16]);
                    ta.HighPosition = new Vector3(x, y, z);

                    tanims.Add(ta);
                }else if (line.StartsWith("spotlight"))
                {
                    SpotLight sp = new SpotLight();

                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    sp.Position = new Vector3(x, y, z);

                    x = float.Parse(parts[4]);
                    y = float.Parse(parts[5]);
                    z = float.Parse(parts[6]);
                    sp.Direction = new Vector3(x, y, z);
                    sp.ConeAngle = MathHelper.ToRadians(float.Parse(parts[7]));

                    SpotLights.Add(sp);
                }
            }
            reader.Close();
           
            foreach (String s in Objects)
            {
                String o = Path.Combine(FilePaths.DataPath, s);
                Wavefront w = new Wavefront(o);
                Meshes.Add(w);
            }
        }

        public Wavefront GetMesh(int i)
        {
            if (i<Instances.Count)
            {
                return Meshes[Instances[i].Object];
            }
            i -= Instances.Count;
            return Meshes[tanims[i].Object];
        }

        public Matrix GetWorld(int i)
        {
            if (i < Instances.Count)
            {
                return Instances[i].World;
            }
            i -= Instances.Count;

            TranslateAnimator ta = tanims[i];
            return Matrix.CreateFromYawPitchRoll(ta.Rotation.X, ta.Rotation.Y, ta.Rotation.Z) * Matrix.CreateTranslation(ta.BasePosition);
        }

        public int GetCount()
        {
            return Instances.Count + tanims.Count;
        }

        public RenderTarget2D GetLightMap(int i)
        {
            if (i<Instances.Count)
            {
                return Instances[i].LightMap;
            }
            i -= Instances.Count;
            return tanims[i].LightMap;
        }
    }
}
