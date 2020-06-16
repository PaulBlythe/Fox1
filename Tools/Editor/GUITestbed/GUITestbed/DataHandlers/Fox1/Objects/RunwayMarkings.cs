using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayMarkings
    {
        public bool AlternateThreshold;
        public bool AlternateTouchdown;
        public bool AlternateFixedDistance;
        public bool AlternatePrecision;
        public bool LeadingZeroIdent;
        public bool NoThresholdEndArrows;
        public bool Edges;
        public bool Threshold;
        public bool FixedDistance;
        public bool Touchdown;
        public bool Dashes;
        public bool Ident;
        public bool Precision;
        public bool EdgePavement;
        public bool SingleEnd;
        public bool PrimaryClosed;
        public bool SecondaryClosed;
        public bool PrimaryStol;
        public bool SecondaryStol;

        public RunwayMarkings(XmlNode node)
        {
            AlternateThreshold = ((node.Attributes["alternateThreshold"]?.InnerText) == "YES");
            AlternateTouchdown = ((node.Attributes["alternateTouchdown"]?.InnerText) == "YES");
            AlternateFixedDistance = ((node.Attributes["alternateFixedDistance"]?.InnerText) == "YES");
            AlternatePrecision = ((node.Attributes["alternatePrecision"]?.InnerText) == "YES");
            LeadingZeroIdent = ((node.Attributes["leadingZeroIdent"]?.InnerText) == "YES");
            NoThresholdEndArrows = ((node.Attributes["noThresholdEndArrows"]?.InnerText) == "YES");
            Edges = ((node.Attributes["edges"]?.InnerText) == "YES");
            Threshold = ((node.Attributes["threshold"]?.InnerText) == "YES");
            FixedDistance = ((node.Attributes["fixedDistance"]?.InnerText) == "YES");
            Touchdown = ((node.Attributes["touchdown"]?.InnerText) == "YES");
            Dashes = ((node.Attributes["dashes"]?.InnerText) == "YES");
            Ident = ((node.Attributes["ident"]?.InnerText) == "YES");
            Precision = ((node.Attributes["precision"]?.InnerText) == "YES");
            EdgePavement = ((node.Attributes["edgePavement"]?.InnerText) == "YES");
            SingleEnd = ((node.Attributes["singleEnd"]?.InnerText) == "YES");
            PrimaryClosed = ((node.Attributes["primaryClosed"]?.InnerText) == "YES");
            SecondaryClosed = ((node.Attributes["secondaryClosed"]?.InnerText) == "YES");
            PrimaryStol = ((node.Attributes["primaryStol"]?.InnerText) == "YES");
            SecondaryStol = ((node.Attributes["secondaryStol"]?.InnerText) == "YES");
        }
    }
}
