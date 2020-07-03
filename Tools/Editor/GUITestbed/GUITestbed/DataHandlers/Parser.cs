using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers
{
    public static class Parser
    {
        public static String GetInt(string Input, out int value)
        {
            value = 0;
            String remainder = "";

            // strip off leading spaces
            while (Input.StartsWith(" "))
                Input = Input.Substring(1);

            while ((!Input.StartsWith(" ")) && (Input.Length > 0))
            {
                remainder += Input[0];
                Input = Input.Substring(1);
                value = int.Parse(remainder);
            }

            return Input;
        }

        public static String GetDouble(string Input, out double value)
        {
            value = 0;
            String remainder = "";

            // strip off leading spaces
            while (Input.StartsWith(" "))
                Input = Input.Substring(1);

            while (!Input.StartsWith(" "))
            {
                remainder += Input[0];
                Input = Input.Substring(1);

            }
            value = double.Parse(remainder);

            return Input;
        }

        public static String GetBool(string Input, out bool value)
        {
            String remainder = "";
            value = false;
            // strip off leading spaces
            while (Input.StartsWith(" "))
                Input = Input.Substring(1);

            while (!Input.StartsWith(" "))
            {
                remainder += Input[0];
                Input = Input.Substring(1);
                value = remainder.Equals("1");
            }

            return Input;
        }

        public static String Skip(string Input)
        {
            // strip off leading spaces
            while (Input.StartsWith(" "))
                Input = Input.Substring(1);

            while (!Input.StartsWith(" "))
            {
                Input = Input.Substring(1);
            }

            return Input;
        }

        public static String GetString(string Input, out String value)
        {
            String remainder = "";
            value = "";
            // strip off leading spaces
            while (Input.StartsWith(" "))
                Input = Input.Substring(1);

            if (Input.Length > 0)
            {
                while ((!Input.StartsWith(" ")) && (Input.Length > 0))
                {
                    remainder += Input[0];
                    Input = Input.Substring(1);

                }
                value = remainder;
            }
            return Input;
        }
    }
}
