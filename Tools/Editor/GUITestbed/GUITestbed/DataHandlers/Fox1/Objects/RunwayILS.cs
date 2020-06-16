using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayILS
    {
        public double Latitude;
        public double Longitude;
        public float Altitude;
        public double Heading;
        public double Frequency;
        public String End;
        public String Range;
        public double MagneticVariation;
        public String Ident;
        public double Width;
        public String Name;
        public bool BackCourse;

        public RunwayGlideSlope GlideSlope;

        public RunwayILS(XmlNode node)
        {
            BackCourse = ((node.Attributes["backCourse"]?.InnerText) == "YES");

            Latitude = double.Parse(node.Attributes["lat"]?.InnerText);
            Longitude = double.Parse(node.Attributes["lon"]?.InnerText);
            Heading = double.Parse(node.Attributes["heading"]?.InnerText);    
            Width = Cartography.ReadFloat(node.Attributes["width"]?.InnerText);
            Frequency = Cartography.ReadFloat(node.Attributes["frequency"]?.InnerText);
            MagneticVariation = Cartography.ReadFloat(node.Attributes["magvar"]?.InnerText);

            Altitude = Cartography.ReadFloat(node.Attributes["alt"]?.InnerText);

            End = node.Attributes["end"]?.InnerText;
            Range = node.Attributes["range"]?.InnerText;
            Name = node.Attributes["name"]?.InnerText;
            Ident = node.Attributes["ident"]?.InnerText;

            BackCourse = ((node.Attributes["backCourse"]?.InnerText) == "YES");

            GlideSlope = new RunwayGlideSlope(node.SelectSingleNode("GlideSlope"));

        }
        
    }
}
