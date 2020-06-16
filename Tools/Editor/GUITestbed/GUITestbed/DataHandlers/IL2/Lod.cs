using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.DataHandlers.IL2
{
    public class Lod
    {
        public List<Material> Materials = new List<Material>();
        public List<FaceGroup> FaceGroups = new List<FaceGroup>();
        public List<VertexPositionColor> ShadowVerts = new List<VertexPositionColor>();
        public List<short> ShadowIndices = new List<short>();
        public List<List<VertexPositionNormalTexture>> animation_frames = new List<List<VertexPositionNormalTexture>>();
        public List<List<VertexPositionColor>> animation_shadow_frames = new List<List<VertexPositionColor>>();

        public short NumFrames;
        public int VertexCount;
        public int FaceCount;

        public VertexPositionNormalTexture[] Verts;
        public short[] Indices;
        public VertexPositionColor[] ShadowArray;
        public short[] ShadowIndicesArray;

        public bool Done;
        public bool Animated;
        public String continueance;

        #region Constructors
        public Lod()
        {
            Animated = false;
            NumFrames = 0;
        }

        public Lod(TextReader reader, String dir)
        {
            Done = false;
            continueance = "";
            ReadMaterials(reader, dir);
            ReadFaceGroups(reader);
            ReadVertices(reader);
            ReadUVs(reader);
            ReadFaces(reader);
            if (ReadShadowVerts(reader))
            {
                ReadShadowFaces(reader);
                ShadowArray = ShadowVerts.ToArray();
                ShadowIndicesArray = ShadowIndices.ToArray();
            }

        }
        #endregion

        #region Loaders
        private void ReadShadowFaces(TextReader reader)
        {
            string line;
            string[] parts;

            line = reader.ReadLine();
            while (!line.Contains("ShFaces"))
                line = reader.ReadLine();

            while (true)
            {
                line = reader.ReadLine();
                if (String.IsNullOrWhiteSpace(line))
                    return;
                parts = line.Split(' ');
                if ((line.Length == 0) || (line.StartsWith("//")))
                {
                    return;
                }
                ShadowIndices.Add(short.Parse(parts[0]));
                ShadowIndices.Add(short.Parse(parts[1]));
                ShadowIndices.Add(short.Parse(parts[2]));
            }
        }

        private bool ReadShadowVerts(TextReader reader)
        {
            string line;
            string[] parts;


            line = reader.ReadLine();
            if (line == null)
            {
                Done = true;
                return false;
            }


            if ((line.StartsWith("//")) || (line.Length < 1))
            {
                line = reader.ReadLine();
            }
            while (String.IsNullOrWhiteSpace(line))
            {
                line = reader.ReadLine();
                if (line == null)
                {
                    Done = true;
                    return false;
                }
            }

            if (line.StartsWith(";"))
            {
                Done = true;
                return false;
            }
            if (line.StartsWith("[CoCommon"))
            {
                //Done = true;
                continueance = line;
                return false;
            }
            if (!line.Contains("ShVertices"))       // no shadow verts for this lod
            {
                if (line.StartsWith("[LOD"))
                {
                    continueance = line;
                    return false;
                }
                return false;
            }
            while (true)
            {
                line = reader.ReadLine();
                line = line.Replace('\t', ' ');
                parts = line.Split(' ');
                if ((line.Length == 0) || (line.StartsWith("//")))
                {
                    return true;
                }
                Vector3 v = new Vector3(float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                                        float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                                        float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture));
                ShadowVerts.Add(new VertexPositionColor(v, Color.Black));
            }
        }

        private void ReadMaterials(TextReader reader, String dir)
        {
            while (true)
            {
                if (reader.Peek() == '[')
                    return;

                String line = reader.ReadLine();
                if (String.IsNullOrWhiteSpace(line))
                    return;

                String[] parts = line.Split(' ');
                Material m = new Material(parts[0], dir);
                Materials.Add(m);
            }
        }

        private void ReadFaceGroups(TextReader reader)
        {
            String line = "";
            char[] seperators = new char[] { ' ', '\t' };
            string[] parts;

            while (!line.StartsWith("[LOD"))
                line = reader.ReadLine();

            line = reader.ReadLine();

            parts = line.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            VertexCount = int.Parse(parts[0]);
            FaceCount = int.Parse(parts[1]);
            Verts = new VertexPositionNormalTexture[VertexCount];
            Indices = new short[FaceCount * 3];

            while (true)
            {
                line = reader.ReadLine();
                if (line.Length == 0)
                    return;

                parts = line.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

                FaceGroup f = new FaceGroup();
                f.Material = int.Parse(parts[0]);
                f.StartVertex = int.Parse(parts[1]);
                f.VertexCount = int.Parse(parts[2]);
                f.StartFace = int.Parse(parts[3]);
                f.FaceCount = int.Parse(parts[4]);
                FaceGroups.Add(f);

            }
        }

        private void ReadVertices(TextReader reader)
        {
            string line = null;
            string[] parts;
            char[] seps = new char[] { ' ', '\a', '\t' };

            while (String.IsNullOrWhiteSpace(line))
                line = reader.ReadLine();

            if (line.StartsWith("//"))
                line = reader.ReadLine();



            for (int i = 0; i < VertexCount; i++)
            {
                line = reader.ReadLine();
                if (line.Length < 1)
                    line = reader.ReadLine();
                if (line.StartsWith("//"))
                    line = reader.ReadLine();

                parts = line.Split(seps, StringSplitOptions.RemoveEmptyEntries);
                Vector3 v = new Vector3(float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                                        float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                                        float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture));
                Vector3 n = new Vector3(float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture),
                                        float.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture),
                                        float.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture));
                VertexPositionNormalTexture vn = new VertexPositionNormalTexture(v, n, Vector2.Zero);
                Verts[i] = vn;
            }
            reader.ReadLine();
        }

        private void ReadUVs(TextReader reader)
        {
            string line;
            string[] parts;

            line = reader.ReadLine();
            while (!line.Contains("MaterialMapping]"))
                line = reader.ReadLine();


            for (int i = 0; i < VertexCount; i++)
            {
                line = reader.ReadLine();
                while (String.IsNullOrWhiteSpace(line))
                    line = reader.ReadLine();
                if (line.StartsWith("//"))
                    line = reader.ReadLine();

                parts = line.Split(' ');
                Vector2 v = new Vector2(float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                                        float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture));
                Verts[i].TextureCoordinate = v;
            }
            reader.ReadLine();
        }

        private void ReadFaces(TextReader reader)
        {
            int i = 0;
            string line;
            string[] parts;

            line = reader.ReadLine();
            while (!line.Contains("Faces]"))
                line = reader.ReadLine();
            int j = 0;
            while (j < FaceCount)
            {
                line = reader.ReadLine();
                while (String.IsNullOrWhiteSpace(line))
                    line = reader.ReadLine();


                if (!line.StartsWith("//"))
                {
                    parts = line.Split(' ');
                    Indices[i++] = short.Parse(parts[0]);
                    Indices[i++] = short.Parse(parts[1]);
                    Indices[i++] = short.Parse(parts[2]);
                    j++;
                }
            }
            reader.ReadLine();
        }
        #endregion

    }

}
