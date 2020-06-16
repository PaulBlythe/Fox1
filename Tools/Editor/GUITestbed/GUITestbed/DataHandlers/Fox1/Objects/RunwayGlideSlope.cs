using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayGlideSlope
    {
        public double Latitude;
        public double Longitude;
        public float Altitude;
        public double Pitch;
        public String Range;

        public RunwayGlideSlope(XmlNode node)
        {
            Latitude = double.Parse(node.Attributes["lat"]?.InnerText);
            Longitude = double.Parse(node.Attributes["lon"]?.InnerText);
            Pitch = double.Parse(node.Attributes["pitch"]?.InnerText);

            Altitude = Cartography.ReadFloat(node.Attributes["alt"]?.InnerText);

            Range = node.Attributes["range"]?.InnerText;
        }

        
    }
}
