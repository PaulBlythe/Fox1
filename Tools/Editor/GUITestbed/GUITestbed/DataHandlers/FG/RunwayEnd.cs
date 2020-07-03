using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.FG
{
    public class RunwayEnd
    {
        public String Name;
        public double Latitude;
        public double Longitude;
        public double DisplacedLength;
        public double OverrunLength;
        public int RunwayMarkings;
        public int ApproachLighting;
        public bool TDZLighting;
        public int RunwayEndIdentifierLights;
        public bool AttachedLightObject = false;

        public RunwayEnd()
        {
        }

        public string Set(string definition)
        {
            definition = Parser.GetString(definition, out Name);
            definition = Parser.GetDouble(definition, out Latitude);
            definition = Parser.GetDouble(definition, out Longitude);
            definition = Parser.GetDouble(definition, out DisplacedLength);
            definition = Parser.GetDouble(definition, out OverrunLength);
            definition = Parser.GetInt(definition, out RunwayMarkings);
            definition = Parser.GetInt(definition, out ApproachLighting);
            definition = Parser.GetBool(definition, out TDZLighting);
            definition = Parser.GetInt(definition, out RunwayEndIdentifierLights);
            return definition;
        }
    }
}
