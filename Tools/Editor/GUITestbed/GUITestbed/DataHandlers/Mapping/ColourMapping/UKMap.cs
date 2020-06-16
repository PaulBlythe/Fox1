using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.DataHandlers.Mapping.ColourMapping
{
    public class UKMap : ColourMap
    {
        Dictionary<String, ColourRecord> Records = new Dictionary<string, ColourRecord>();

        public UKMap()
        {
            Records.Add("Highway", new ColourRecord(Color.Red, 2));
            Records.Add("Major Highway", new ColourRecord(Color.Red, 4));
            Records.Add("Unknown", new ColourRecord(Color.Red, 1));
            Records.Add("Secondary Highway", new ColourRecord(Color.DarkRed, 1));
            Records.Add("Road", new ColourRecord(Color.Red, 1));
            Records.Add("Ferry Route", new ColourRecord(Color.Yellow, 2));
            Records.Add("Beltway", new ColourRecord(Color.Red, 2));
            Records.Add("Bypass", new ColourRecord(Color.Red, 2));
            Records.Add("Track", new ColourRecord(Color.DarkRed, 1));
            Records.Add("Coastline", new ColourRecord(Color.SeaGreen, 1));

            Records.Add("Land", new ColourRecord(Color.Brown, 1));
            Records.Add("ocean", new ColourRecord(Color.Blue, 1));
            Records.Add("sea", new ColourRecord(Color.Blue, 1));
            Records.Add("channel", new ColourRecord(Color.Blue, 1));
            Records.Add("bay", new ColourRecord(Color.Blue, 1));
            Records.Add("gulf", new ColourRecord(Color.Blue, 1));
            Records.Add("fjord", new ColourRecord(Color.Blue, 1));
            Records.Add("strait", new ColourRecord(Color.Blue, 1));
            Records.Add("lagoon", new ColourRecord(Color.Blue, 1));
            Records.Add("generic", new ColourRecord(Color.Blue, 1));
            Records.Add("sound", new ColourRecord(Color.Blue, 1));
            Records.Add("reef", new ColourRecord(Color.DarkSlateBlue, 1));

            Records.Add("River", new ColourRecord(Color.DarkBlue, 1));
            Records.Add("International boundary (verify)", new ColourRecord(Color.Black, 2));
            Records.Add("Minor island", new ColourRecord(Color.Khaki, 2));

            Records.Add("city", new ColourRecord(Color.Black, 6));
        }

        public override ColourRecord GetColour(string type)
        {
            if (Records.ContainsKey(type))
            {
                return Records[type];
            }
            return Records["Road"];
        }

        public override int GetSize(string type)
        {
            if (Records.ContainsKey(type))
            {
                return Records[type].Width;
            }
            return 2;
        }
    }
}
