using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayTaxiwayPoint
    {
        public int Index;
        public String Type;
        public String Orientation;
        public double Latitude;
        public double Longitude;

        public RunwayTaxiwayPoint(XmlNode node)
        {
            Latitude = double.Parse(node.Attributes["lat"]?.InnerText);
            Longitude = double.Parse(node.Attributes["lon"]?.InnerText);

            Type = node.Attributes["type"]?.InnerText;
            Orientation = node.Attributes["orientation"]?.InnerText;

            Index = int.Parse(node.Attributes["index"]?.InnerText);
        }
    }
}
