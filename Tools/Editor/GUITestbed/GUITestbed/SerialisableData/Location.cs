using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

using GUITestbed.World;

namespace GUITestbed.SerialisableData
{
    public class Location
    {
        public double Latitude;
        public double Longitude;
        public float Altitude;

        public Location(XmlNode node)
        {
            Latitude = Double.Parse(node.Attributes["lat"]?.InnerText);
            Longitude = Double.Parse(node.Attributes["lon"]?.InnerText);
            String s = node.Attributes["alt"]?.InnerText;
            Altitude = Cartography.ReadFloat(s);
        }

        public Location(double la, double lo, float al)
        {
            Latitude = la;
            Longitude = lo;
            Altitude = al;
        }
    }
}
