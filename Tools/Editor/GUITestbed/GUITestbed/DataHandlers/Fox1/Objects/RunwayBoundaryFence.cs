using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

using GUITestbed.World;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayBoundaryFence
    {
        public Guid ID;
        public List<Vector2D> Points = new List<Vector2D>();

        public RunwayBoundaryFence(XmlNode node)
        {
            String t = node.Attributes["profile"]?.InnerText;
            ID = new Guid(t);

            XmlNodeList verts = node.SelectNodes("Vertex");
            foreach (XmlNode n in verts)
            {
                double Latitude = double.Parse(n.Attributes["lat"]?.InnerText);
                double Longitude = double.Parse(n.Attributes["lon"]?.InnerText);

                Vector2D v = new Vector2D(Longitude, Latitude);
                Points.Add(v);

            }
        }
    }
}
