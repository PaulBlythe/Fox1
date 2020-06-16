using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayApproachLights
    {
        public String End;
        public String System;
        public int Strobes;
        public bool Reil;                   // Runway end identifier lights
        public bool Touchdown;
        public bool EndLights;

        public RunwayApproachLights(XmlNode node)
        {
            Reil = ((node.Attributes["reil"]?.InnerText) == "TRUE");
            Touchdown = ((node.Attributes["touchdown"]?.InnerText) == "TRUE");
            EndLights = ((node.Attributes["endLights"]?.InnerText) == "TRUE");

            End = node.Attributes["end"]?.InnerText;
            System = node.Attributes["system"]?.InnerText;

            Strobes = int.Parse(node.Attributes["strobes"]?.InnerText);
        }
    }
}
