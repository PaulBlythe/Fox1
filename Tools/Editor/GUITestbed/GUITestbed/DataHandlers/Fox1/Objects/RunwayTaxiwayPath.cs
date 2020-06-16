using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using GUITestbed.Rendering._3D;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayTaxiwayPath
    {
        public String Type;
        public int Start;
        public int End;
        public float Width;
        public float WeightLimit;
        public bool DrawSurface;
        public bool DrawDetail;
        public String Surface;
        public String Name;
        public bool CenterLine;
        public bool CenterLineLighted;

        public Vector3[] Corners = new Vector3[4];
        public Vector3[] Centres = new Vector3[2];

        public RunwayTaxiwayPath(XmlNode node)
        {
            Surface = node.Attributes["surface"]?.InnerText;
            Type = node.Attributes["type"]?.InnerText;
            Name = node.Attributes["name"]?.InnerText;

            Width = Cartography.ReadFloat(node.Attributes["width"]?.InnerText);
            WeightLimit = Cartography.ReadFloat(node.Attributes["weightLimit"]?.InnerText);

            CenterLineLighted = ((node.Attributes["centerLineLighted"]?.InnerText) == "YES");
            CenterLine = ((node.Attributes["centerLine"]?.InnerText) == "YES");
            DrawSurface = ((node.Attributes["drawSurface"]?.InnerText) == "YES");
            DrawDetail = ((node.Attributes["drawDetail"]?.InnerText) == "YES");

            Start = int.Parse(node.Attributes["start"]?.InnerText);
            End = int.Parse(node.Attributes["end"]?.InnerText);


        }

        
    }
}
