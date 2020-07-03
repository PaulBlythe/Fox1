using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.FG
{
    public class Windsock
    {
        public double Latitude;
        public double Longitude;
        public bool Lit;
        public String Name;

        public Windsock(String definition)
        {
            definition = Parser.Skip(definition);
            definition = Parser.GetDouble(definition, out Latitude);
            definition = Parser.GetDouble(definition, out Longitude);
            definition = Parser.GetBool(definition, out Lit);
            definition = definition.Substring(1);
            Name = definition;

        }
    }
}
