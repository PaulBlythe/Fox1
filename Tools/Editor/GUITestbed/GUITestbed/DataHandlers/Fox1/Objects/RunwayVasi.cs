using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

using GUITestbed.World;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayVasi
    {
        public String End;
        public String Type;
        public String Side;
        public float BiasX;
        public float BiasZ;
        public float Spacing;
        public int Pitch;

        public RunwayVasi(XmlNode node)
        {
            BiasX = Cartography.ReadFloat(node.Attributes["biasX"]?.InnerText);
            BiasZ = Cartography.ReadFloat(node.Attributes["biasZ"]?.InnerText);
            Spacing = Cartography.ReadFloat(node.Attributes["spacing"]?.InnerText);

            Pitch = int.Parse(node.Attributes["pitch"]?.InnerText);

            End = node.Attributes["end"]?.InnerText;
            Type = node.Attributes["type"]?.InnerText;
            Side = node.Attributes["side"]?.InnerText;
        }

    }
}
