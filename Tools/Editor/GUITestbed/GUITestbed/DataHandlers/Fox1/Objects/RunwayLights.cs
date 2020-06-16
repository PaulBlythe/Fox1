using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayLights
    {
        public String Center;
        public String Edge;
        public bool CenterRed;

        public RunwayLights(XmlNode node)
        {
            Center = node.Attributes["center"]?.InnerText;
            Edge = node.Attributes["edge"]?.InnerText;
            CenterRed = ((node.Attributes["centerRed"]?.InnerText) == "TRUE");
        }
    }
}
