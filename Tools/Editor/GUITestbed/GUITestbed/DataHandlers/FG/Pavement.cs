using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.DataHandlers.FG
{
    public class Pavement
    {
        public int SurfaceType;
        public double Roughness;
        public double TextureRotation;
        public String Name;

        public List<LineNode> Nodes = new List<LineNode>();
        public List<PointSequence> chains = new List<PointSequence>();

        public Pavement(string definition)
        {
            definition = Parser.Skip(definition);
            definition = Parser.GetInt(definition, out SurfaceType);
            definition = Parser.GetDouble(definition, out Roughness);
            definition = Parser.GetDouble(definition, out TextureRotation);
            Name = definition;
        }

        //public void ConvertToChain(double clat, double clon, float altitude)
        //{
        //    chains = APTHelper.LineNodeListToChain(Nodes);
        //    foreach (PointSequence p in chains)
        //    {
        //        p.Verts = APTHelper.PointSequenceToVertices(p, clat, clon, altitude);
        //
        //        TriangleNet.Geometry.InputGeometry Geom = new TriangleNet.Geometry.InputGeometry();
        //        foreach (Vector3 v in p.Verts)
        //        {
        //            Geom.AddPoint(v.X * 100, v.Z * 100, 0, 0);
        //        }
        //        for (int i = 0; i < p.Verts.Count; i++)
        //        {
        //            int np = (i + 1) % p.Verts.Count;
        //            Geom.AddSegment(i, np, 0);
        //        }
        //        TriangleNet.Mesh mesh = new Mesh();
        //        //mesh.Behavior.UseBoundaryMarkers = true;
        //        //mesh.Behavior.Algorithm = TriangulationAlgorithm.Incremental;
        //        try
        //        {
        //            mesh.Triangulate(Geom);
        //
        //            List<VertexPositionNormalTexture> polyVerts = new List<VertexPositionNormalTexture>();
        //            List<short> Indices = new List<short>();
        //
        //            short ind = 0;
        //            foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
        //            {
        //                for (int i = 0; i < 3; i++)
        //                {
        //                    Vector3 position = new Vector3((float)tri.GetVertex(i).X / 100.0f, altitude, (float)tri.GetVertex(i).Y / 100.0f);
        //
        //                    float u = (float)((position.X - (mesh.Bounds.Xmin / 100.0f)) / 10.0);
        //                    float v = (float)((position.Z - (mesh.Bounds.Ymin / 100.0f)) / 10.0);
        //
        //                    Vector2 UV = new Vector2(u, v);
        //                    VertexPositionNormalTexture vpn = new VertexPositionNormalTexture(position, Vector3.UnitX, UV);
        //                    polyVerts.Add(vpn);
        //                    Indices.Add(ind);
        //                    ind++;
        //                }
        //            }
        //            Texture2D tex;
        //            switch (SurfaceType)
        //            {
        //                case 1:
        //                    tex = Game1.Instance.RunwayTextures["pavement"];
        //                    break;
        //                case 2:
        //                    tex = Game1.Instance.RunwayTextures["pavement"];
        //                    break;
        //                case 3:
        //                    tex = Game1.Instance.RunwayTextures["grass"];
        //                    break;
        //                case 4:
        //                    tex = Game1.Instance.RunwayTextures["dirt"];
        //                    break;
        //                case 5:
        //                    tex = Game1.Instance.RunwayTextures["gravel"];
        //                    break;
        //                case 12:
        //                    tex = Game1.Instance.RunwayTextures["lake"];
        //                    break;
        //                case 14:
        //                    tex = Game1.Instance.RunwayTextures["snow"];
        //                    break;
        //
        //                default:
        //                    tex = Game1.Instance.White;
        //                    break;
        //            }
        //            DrawableObject d = new DrawableObject(polyVerts, Indices, tex);
        //            draws.Add(d);
        //        }
        //        catch (Exception e)
        //        {
        //            //Console.WriteLine(e);
        //            //System.IO.TextWriter writeFile = new StreamWriter(@"C:\Research\Triangle.NET\Data\" + this.GetHashCode().ToString() + ".poly");
        //            //writeFile.WriteLine(String.Format("{0} 2 0 0", p.Verts.Count));
        //            //for (int i=0; i<p.Verts.Count; i++)
        //            //{
        //            //    writeFile.WriteLine(String.Format("{0}  {1} {2}", i+1, p.Verts[i].X,p.Verts[i].Z));
        //            //}
        //            //writeFile.WriteLine(String.Format("{0} 0", p.Verts.Count + 1));
        //            //for (int i = 0; i < p.Verts.Count; i++)
        //            //{
        //            //    writeFile.WriteLine(String.Format("{0}  {1} {2}", i + 1, i + 1, i + 2));
        //            //}
        //            //writeFile.WriteLine(String.Format("{0}  {1} {2}", p.Verts.Count + 1, 1, p.Verts.Count + 1));
        //            //writeFile.WriteLine("0");
        //            //writeFile.Close();
        //
        //        }
        //
        //    }
        //    Console.WriteLine("Pavement has " + chains.Count.ToString() + " chains");
        //}
        //
        
    }
}
