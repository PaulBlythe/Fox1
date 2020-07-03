using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.FG
{
    public class RadioFrequency
    {
        /// <summary>
        /// Type can be :
        /// AWOS              ATC recorded
        /// Unicom            Unicom (US), CTAF (US), Radio (UK)
        /// CLD               Clearance delivery
        /// GND               Ground
        /// TWR               Tower
        /// APP               Approach
        /// DEP               Departure
        /// </summary>
        public String Type;

        /// <summary>
        /// Frequency in MHz
        /// </summary>
        public double Frequency;

        public string Name;

        public RadioFrequency(String definition)
        {
            int typecode;
            definition = Parser.GetInt(definition, out typecode);
            switch (typecode)
            {
                case 50:
                    Type = "AWOS";
                    break;
                case 51:
                    Type = "Unicom";
                    break;
                case 52:
                    Type = "CLD";
                    break;
                case 53:
                    Type = "GND";
                    break;
                case 54:
                    Type = "TWR";
                    break;
                case 55:
                    Type = "APP";
                    break;
                case 56:
                    Type = "DEP";
                    break;

            }

            definition = Parser.GetDouble(definition, out Frequency);
            Frequency /= 100.0;

            definition = definition.Substring(1);
            Name = definition;
        }

    }
}
