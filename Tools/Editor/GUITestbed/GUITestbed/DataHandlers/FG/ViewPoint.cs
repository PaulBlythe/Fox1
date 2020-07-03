using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.FG
{
    public class ViewPoint
    {
        public double Latitude;
        public double Longitude;
        public int Height;          // in feet

        public ViewPoint(String definition)
        {
            definition = Parser.Skip(definition);
            definition = Parser.GetDouble(definition, out Latitude);
            definition = Parser.GetDouble(definition, out Longitude);
            Parser.GetInt(definition, out Height);

        }

        public ViewPoint()
        {

        }
    }
}