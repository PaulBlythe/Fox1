using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GUITestbed.Rendering._3D;
using GUITestbed.World;
using GUITestbed.Rendering;

using TriangleNet;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayPavement
    {
        public String Surface;
        public bool DrawSurface;
        public bool DrawDetail;
        public List<Vector2D> Vertices = new List<Vector2D>();
        public List<short> Indices = new List<short>();

        List<VertexPositionNormalTexture> tempverts = new List<VertexPositionNormalTexture>();

        public RunwayPavement(XmlNode node)
        {
            Surface = node.Attributes["surface"]?.InnerText;
            DrawSurface = ((node.Attributes["drawSurface"]?.InnerText) == "YES");
            DrawDetail = ((node.Attributes["drawDetail"]?.InnerText) == "YES");

            XmlNodeList verts = node.SelectNodes("Vertex");
            foreach (XmlNode node2 in verts)
            {
                Vector2D v = new Vector2D(0, 0);

                v.Y = double.Parse(node2.Attributes["lat"]?.InnerText);
                v.X = double.Parse(node2.Attributes["lon"]?.InnerText);

                Vertices.Add(v);
            }
        }

        public RunwayObject Build(Vector2D airport_centre, float altitude)
        {
            List<Vector2D> tverts = new List<Vector2D>();
            foreach (Vector2D v in Vertices)
            {
                Vector2D v2 = Cartography.ConvertToLocalised(airport_centre.Y, airport_centre.X, v.Y, v.X);
                tverts.Add(v2);
            }


            TriangleNet.Geometry.InputGeometry Geom = new TriangleNet.Geometry.InputGeometry();
            TriangleNet.Mesh mesh = new Mesh();
            foreach (Vector2D v in tverts)
            {
                Geom.AddPoint(v.X, v.Y, 0, 0);
            }
            for (int i = 0; i < tverts.Count; i++)
            {
                int np = (i + 1) % tverts.Count;
                Geom.AddSegment(i, np, 0);
            }

            tempverts.Clear();
            Indices.Clear();
            mesh.Triangulate(Geom);

            short ind = 0;
            foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 position = new Vector3((float)tri.GetVertex(i).X , altitude, (float)tri.GetVertex(i).Y);

                    float u = (float)((position.X - (mesh.Bounds.Xmin)) / 10.0);
                    float v = (float)((position.Z - (mesh.Bounds.Ymin)) / 10.0);

                    Vector2 UV = new Vector2(u, v);
                    VertexPositionNormalTexture vpn = new VertexPositionNormalTexture(position, Vector3.Up, UV);
                    tempverts.Add(vpn);
                    Indices.Add(ind);
                    ind++;
                }
            }

            RunwayObject ro = new RunwayObject();
            ro.Verts = tempverts.ToArray();
            ro.Indices = Indices.ToArray();

            switch (Surface)
            {
                case "CONCRETE":
                    ro.texture = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/concrete");
                    ro.rs.DepthBias = 0;// 1;
                    break;

                case "ASPHALT":
                    ro.texture = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/asphalt");
                    ro.rs.DepthBias = 0;
                    break;

                case "DIRT":
                    ro.texture = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/dirt");
                    ro.rs.DepthBias = 0;
                    break;

                case "CEMENT":
                    ro.texture = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/cement");
                    ro.rs.DepthBias = 0;// 5;
                    break;

                default:
                    throw new Exception("Blastpad missing surface type");
            }
            ro.fx = ShaderManager.GetEffect("Shaders/DiffuseFog");

            return ro;
        }

        
    }
}
