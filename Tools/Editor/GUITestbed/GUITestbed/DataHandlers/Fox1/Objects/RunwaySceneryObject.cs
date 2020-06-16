using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwaySceneryObject
    {
        public double Latitude;
        public double Longitude;
        public float Altitude;
        public bool AltitudeIsAgl;
        public double Pitch;            // in degrees
        public double Bank;             // in degrees
        public double Heading;          // in degrees
        public Guid Name;
        public double scale = 1.00;
        public bool isWindsock = false;

        public double PoleHeight;
        public double SockLength;
        public bool Lighted;

        public RunwaySceneryObject(XmlNode node)
        {
            Altitude = Cartography.ReadFloat(node.Attributes["alt"]?.InnerText);
            Latitude = double.Parse(node.Attributes["lat"]?.InnerText);
            Longitude = double.Parse(node.Attributes["lon"]?.InnerText);
            Heading = double.Parse(node.Attributes["heading"]?.InnerText);
            Pitch = double.Parse(node.Attributes["pitch"]?.InnerText);
            Bank = double.Parse(node.Attributes["bank"]?.InnerText);

            AltitudeIsAgl = ((node.Attributes["altitudeIsAgl"]?.InnerText) == "TRUE");

            if (node.ChildNodes[0].LocalName == "LibraryObject")
            {
                String g = node.ChildNodes[0].Attributes["name"]?.InnerText;

                if (g.Contains("{"))
                    Name = new Guid(g);
                else
                {
                    String m1 = g.Substring(0, 8).ToLower();
                    String m2 = g.Substring(8, 4).ToLower();
                    String m3 = g.Substring(12, 4).ToLower();
                    String m4 = g.Substring(16, 2).ToLower();
                    String m5 = g.Substring(18, 2).ToLower();
                    String m6 = g.Substring(20, 2).ToLower();
                    String m7 = g.Substring(22, 2).ToLower();
                    String m8 = g.Substring(24, 2).ToLower();
                    String m9 = g.Substring(26, 2).ToLower();
                    String m10 = g.Substring(28, 2).ToLower();
                    String m11 = g.Substring(30, 2).ToLower();

                    Name = new Guid(m1 + m3 + m2 + m7 + m6 + m5 + m4 + m11 + m10 + m9 + m8);
                }

                g = node.ChildNodes[0].Attributes["scale"]?.InnerText;
                scale = double.Parse(g);
            }
            else
            {
                if (node.ChildNodes[0].LocalName == "Windsock")
                {
                    isWindsock = true;
                    PoleHeight = double.Parse(node.ChildNodes[0].Attributes["poleHeight"]?.InnerText);
                    SockLength = double.Parse(node.ChildNodes[0].Attributes["sockLength"]?.InnerText);
                    Lighted = ((node.ChildNodes[0].Attributes["lighted"]?.InnerText) == "YES");
                }
            }

        }

    }
}
