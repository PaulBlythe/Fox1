using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

using GUITestbed.Rendering._3D;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class ApronEdgeLight
    {
        public List<List<Vector2D>> Vertices = new List<List<Vector2D>>();

        public ApronEdgeLight(XmlNode node)
        {
            foreach (XmlNode n in node.ChildNodes)
            {
                XmlNodeList l = n.SelectNodes("Vertex");

                List<Vector2D> verts = new List<Vector2D>();

                foreach (XmlNode nn in l)
                {
                    Vector2D v = new Vector2D(0, 0);

                    v.Latitude = double.Parse(nn.Attributes["lat"]?.InnerText);
                    v.Longitude = double.Parse(nn.Attributes["lon"]?.InnerText);
                    verts.Add(v);
                }
                Vertices.Add(verts);
            }
        }
    }
}
