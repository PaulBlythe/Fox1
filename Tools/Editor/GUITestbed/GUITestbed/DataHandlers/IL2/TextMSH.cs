using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.DataHandlers.IL2
{
    public class TextMSH
    {
        enum MeshLoaderMode
        {
            Start,
            Common,
            Lod,
            Hooks,
            Hookloc,
            Materials,
            FaceGroups,
            Vertices,
            UVS,
            Faces,
            ShadowVerts,
            ShadowFaces,
            LodMesh,
            CoCommon,
            CoCommonPart,
            CoCommonType,
            CoVer0,
            CoNeiCnt,
            CoNei,
            CoFaces,
            Done,
            MeshLoaderModes
        };

        public int VertexCount;
        public int FaceCount;
        public VertexPositionNormalTexture[] Verts;
        public short[] indices;

        public List<FaceGroup> FaceGroups = new List<FaceGroup>();
        public CollisionMesh colmesh = new CollisionMesh();
        List<VertexPositionColor> ShadowVerts = new List<VertexPositionColor>();
        List<short> ShadowIndices = new List<short>();
        public List<Lod> Lods = new List<Lod>();
        public List<float> LodDistances = new List<float>();
        public List<Material> Materials = new List<Material>();

        public TextMSH(String filename)
        {
            String dir = Path.GetDirectoryName(filename);
            string line = "";
            int line_number = 0;

            String cpart = "[CoCommon_b0]";
            String cpart2 = "[CoCommon_b0p0]";
            char[] seps = new char[] { ' ', '\a', '\t', '/' };
            MeshLoaderMode mode = MeshLoaderMode.Start;
            using (TextReader reader = File.OpenText(filename))
            {
                string[] parts;
                int count = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    line_number++;
                    if (String.IsNullOrWhiteSpace(line))
                        line = "";
                    if (line.StartsWith("\\"))
                        line = "";
                    if ((line.StartsWith("[NFrames")) || (line.StartsWith("[NBlocks")))
                    {
                        line = line.TrimStart('[');
                        line = line.TrimEnd(']');
                    }

                    if ((line.Length > 0) && (!line.StartsWith("//")) && (!line.StartsWith(";")) && (!line.StartsWith("#")))
                    {
                        bool used = false;
                        String t = line;
                        int g = 0;
                        while (g < t.Length - 1)
                        {
                            if ((t.ElementAt(g) == ' ') && (t.ElementAt(g + 1) == ' '))
                            {
                                t = t.Remove(g, 1);
                            }
                            else
                            {
                                if (t.ElementAt(g) == '\t')
                                {
                                    t = t.Replace('\t', ' ');
                                    g--;
                                }
                                else
                                {
                                    g++;
                                }
                            }
                        }
                        t = t.TrimStart(' ');
                        parts = t.Split(seps, StringSplitOptions.RemoveEmptyEntries);
                        while (!used)
                        {

                            if (line.StartsWith(";"))
                            {
                                mode = MeshLoaderMode.Done;
                                used = true;
                            }
                            if (line.StartsWith("["))
                            {
                                if (mode == MeshLoaderMode.CoFaces)
                                {
                                    colmesh.CurrentPart++;
                                    if (colmesh.CurrentPart == colmesh.Blocks[colmesh.CurrentBlock].NParts)
                                    {
                                        colmesh.CurrentPart = 0;
                                        colmesh.CurrentBlock++;
                                    }
                                    cpart2 = string.Format("[CoCommon_b{0}p{1}]", colmesh.CurrentBlock, colmesh.CurrentPart);
                                    cpart = string.Format("[CoCommon_b{0}]", colmesh.CurrentBlock);

                                }
                                mode = MeshLoaderMode.Start;
                            }
                            switch (mode)
                            {
                                case MeshLoaderMode.Start:
                                    if (line.StartsWith("[CoNei_"))
                                    {
                                        mode = MeshLoaderMode.CoNei;
                                        used = true;
                                    }
                                    if (line.StartsWith("[CoFac_"))
                                    {
                                        mode = MeshLoaderMode.CoFaces;
                                        used = true;
                                    }
                                    if (line.StartsWith("[CoNeiCnt_"))
                                    {
                                        mode = MeshLoaderMode.CoNeiCnt;
                                        used = true;
                                    }
                                    if (line.StartsWith("[CoVer0_"))
                                    {
                                        mode = MeshLoaderMode.CoVer0;
                                        used = true;
                                    }
                                    if (line.StartsWith(cpart2))
                                    {
                                        mode = MeshLoaderMode.CoCommonType;
                                        used = true;
                                    }
                                    if (line.StartsWith(cpart))
                                    {
                                        mode = MeshLoaderMode.CoCommonPart;
                                        used = true;
                                    }
                                    if (line.StartsWith("[CoCommon]"))
                                    {
                                        mode = MeshLoaderMode.CoCommon;
                                        used = true;
                                    }
                                    if (line.StartsWith("[Common]"))
                                    {
                                        mode = MeshLoaderMode.Common;
                                        used = true;
                                    }
                                    if (line.StartsWith("[LOD]"))
                                    {
                                        mode = MeshLoaderMode.Lod;
                                        used = true;
                                    }
                                    if (line.StartsWith("[Hooks]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.Hooks;
                                        used = true;
                                    }
                                    if (line.StartsWith("[HookLoc]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.Hookloc;
                                        used = true;
                                        count = 0;
                                    }
                                    if (line.StartsWith("[Materials]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.Materials;
                                        used = true;
                                    }
                                    if (line.StartsWith("[FaceGroups]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.FaceGroups;
                                        used = true;
                                    }
                                    if (line.StartsWith("[Vertices_Frame0]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.Vertices;
                                        used = true;
                                        count = 0;
                                    }
                                    if (line.StartsWith("[MaterialMapping]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.UVS;
                                        used = true;
                                        count = 0;
                                    }
                                    if (line.StartsWith("[Faces]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.Faces;
                                        used = true;
                                        count = 0;
                                    }
                                    if (line.StartsWith("[ShVertices_Frame0]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.ShadowVerts;
                                        used = true;
                                    }
                                    if ((line.StartsWith("[ShFaces]", StringComparison.OrdinalIgnoreCase)) ||
                                       (line.StartsWith("[Sh_Faces]", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        mode = MeshLoaderMode.ShadowFaces;
                                        used = true;
                                    }
                                    if (line.StartsWith("[sfVertices_Frame0]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.ShadowVerts;
                                        used = true;
                                    }
                                    if (line.StartsWith("[sfFaces]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = MeshLoaderMode.ShadowFaces;
                                        used = true;
                                    }
                                    if ((line.StartsWith("[LOD", StringComparison.OrdinalIgnoreCase)) && !used)
                                    {
                                        Lod l = new Lod(reader, dir);
                                        Lods.Add(l);
                                        used = true;
                                        if (l.Done)
                                        {
                                            reader.Close();
                                            return;
                                        }
                                        if (l.continueance != "")
                                        {
                                            line = l.continueance;
                                            used = false;
                                        }

                                    }
                                    // error catching
                                    if (!used)
                                    {
                                        if (line.StartsWith("[CoCommon_b"))
                                        {

                                            int pp = line.IndexOf('p');
                                            if (pp > 0)
                                            {
                                                if (!line.Equals(cpart2))
                                                {

                                                    mode = MeshLoaderMode.Done;
                                                    line = "";
                                                    colmesh.Blocks.Clear();
                                                    colmesh.NBlocks = 0;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case MeshLoaderMode.Done:
                                    reader.Close();

                                    for (int j = 0; j < colmesh.NBlocks; j++)
                                    {
                                        for (int i = 0; i < colmesh.Blocks[j].NParts; i++)
                                        {
                                            colmesh.Blocks[j].Parts[i].indices = colmesh.Blocks[j].Parts[i].Faces.ToArray();
                                            colmesh.Blocks[j].Parts[i].verts = new VertexPositionColor[colmesh.Blocks[j].Parts[i].Verts.Count];
                                            for (int k = 0; k < colmesh.Blocks[j].Parts[i].Verts.Count; k++)
                                            {
                                                colmesh.Blocks[j].Parts[i].verts[k].Position = colmesh.Blocks[j].Parts[i].Verts[j];
                                                colmesh.Blocks[j].Parts[i].verts[k].Color = Microsoft.Xna.Framework.Color.Red;
                                            }
                                        }
                                    }
                                    return;
                                case MeshLoaderMode.Common:
                                    used = true;
                                    break;
                                case MeshLoaderMode.Lod:
                                    {
                                        string[] nparts = line.Split(' ');
                                        LodDistances.Add(float.Parse(nparts[0], System.Globalization.CultureInfo.InvariantCulture));
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.Hooks:
                                    {

                                        used = true;
                                    }
                                    break;

                                case MeshLoaderMode.Hookloc:

                                    count++;
                                    used = true;
                                    break;

                                case MeshLoaderMode.Materials:
                                    {
                                        Material m = new Material(parts[0], dir);
                                        Materials.Add(m);
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.FaceGroups:
                                    {
                                        if (parts.GetLongLength(0) > 2)
                                        {
                                            FaceGroup fg = new FaceGroup();
                                            fg.Material = int.Parse(parts[0]);
                                            fg.StartVertex = int.Parse(parts[1]);
                                            fg.VertexCount = int.Parse(parts[2]);
                                            fg.StartFace = int.Parse(parts[3]);
                                            fg.FaceCount = int.Parse(parts[4]);
                                            FaceGroups.Add(fg);
                                        }
                                        else
                                        {
                                            VertexCount = int.Parse(parts[0]);
                                            FaceCount = int.Parse(parts[1]);
                                            Verts = new VertexPositionNormalTexture[VertexCount];
                                            indices = new short[FaceCount * 3];
                                        }
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.Vertices:
                                    {

                                        VertexPositionNormalTexture vp = new VertexPositionNormalTexture();
                                        vp.Position = new Vector3(float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                                                                  float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                                                                  float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture));
                                        vp.Normal = new Vector3(float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture),
                                                                float.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture),
                                                                float.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture));
                                        Verts[count] = vp;
                                        count++;
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.UVS:
                                    {
                                        Vector2 v = new Vector2(float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                                                                float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture));
                                        Verts[count].TextureCoordinate = v;
                                        count++;
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.Faces:
                                    {
                                        indices[count++] = short.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                                        indices[count++] = short.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                                        indices[count++] = short.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.ShadowVerts:
                                    {
                                        Vector3 v = new Vector3(float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                                                                float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                                                                float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture));
                                        VertexPositionColor vp = new VertexPositionColor(v, Microsoft.Xna.Framework.Color.Black);
                                        ShadowVerts.Add(vp);
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.ShadowFaces:
                                    {
                                        ShadowIndices.Add(short.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture));
                                        ShadowIndices.Add(short.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture));
                                        ShadowIndices.Add(short.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture));
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.CoCommon:
                                    {
                                        colmesh.CurrentBlock = 0;
                                        colmesh.CurrentPart = 0;
                                        cpart = string.Format("[CoCommon_b{0}]", colmesh.CurrentBlock);
                                        cpart2 = string.Format("[CoCommon_b{0}p{1}]", colmesh.CurrentBlock, colmesh.CurrentPart);
                                        colmesh.NBlocks = int.Parse(parts[1]);
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.CoCommonPart:
                                    {
                                        CollisionMeshBlock cmb = new CollisionMeshBlock();
                                        cmb.NParts = int.Parse(parts[1]);
                                        colmesh.Blocks.Add(cmb);
                                        cpart2 = string.Format("[CoCommon_b{0}p{1}]", colmesh.CurrentBlock, colmesh.CurrentPart);
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.CoCommonType:
                                    {
                                        if (parts[0].Equals("Type"))
                                        {
                                            CollisionMeshPart p = new CollisionMeshPart();
                                            colmesh.Blocks[colmesh.CurrentBlock].Parts.Add(p);

                                            p.Type = parts[1];
                                            used = true;
                                        }
                                        if (parts[0].Equals("NFrames"))
                                        {
                                            colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].NFrames = int.Parse(parts[1]);
                                            used = true;
                                        }
                                        if (parts[0].Equals("Name"))
                                        {
                                            if (parts.Length > 1)
                                                colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].Name = parts[1];
                                            else
                                                colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].Name = "BUG unnamed part";
                                            used = true;
                                        }
                                        if (parts[0].Equals("TypeIntExt"))
                                        {
                                            if (parts[1].Equals("EXTERNAL"))
                                                colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].TypeIntExt = 1;
                                            else if (parts[1].Equals("INTERNAL"))
                                                colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].TypeIntExt = 0;
                                            else
                                                colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].TypeIntExt = int.Parse(parts[1]);
                                            used = true;
                                        }
                                    }
                                    break;
                                case MeshLoaderMode.CoVer0:
                                    {
                                        Vector3 v = new Vector3();
                                        v.X = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                                        v.Y = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                                        v.Z = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                                        colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].Verts.Add(v);
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.CoNeiCnt:
                                    {
                                        colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].NeiCount.Add(int.Parse(parts[0]));
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.CoNei:
                                    {
                                        colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].Neighbours.Add(int.Parse(parts[0]));
                                        used = true;
                                    }
                                    break;
                                case MeshLoaderMode.CoFaces:
                                    {
                                        colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].Faces.Add(short.Parse(parts[0]));
                                        colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].Faces.Add(short.Parse(parts[1]));
                                        colmesh.Blocks[colmesh.CurrentBlock].Parts[colmesh.CurrentPart].Faces.Add(short.Parse(parts[2]));
                                        used = true;
                                    }
                                    break;


                            }
                        }
                    }
                }
                reader.Close();
            }
        }
    }
}

