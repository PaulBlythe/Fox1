using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.FG
{
    public class Sign
    {
        public double Latitude;
        public double Longitude;
        public double Heading;
        public int Type;
        public int Size;
        public string CodeString;

        public Sign(string definition)
        {
            definition = Parser.Skip(definition);
            definition = Parser.GetDouble(definition, out Latitude);
            definition = Parser.GetDouble(definition, out Longitude);
            definition = Parser.GetDouble(definition, out Heading);
            definition = Parser.GetInt(definition, out Type);
            definition = Parser.GetInt(definition, out Size);
            definition = definition.Substring(1);
            CodeString = definition;
        }
    }
}