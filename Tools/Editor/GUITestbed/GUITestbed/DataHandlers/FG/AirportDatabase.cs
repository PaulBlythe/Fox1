using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.FG
{
    public class AirportDatabase
    {
        public List<String> ICAOS = new List<string>();
        public List<String> Names = new List<string>();

        String source;

        public AirportDatabase(String s)
        {
            string line;
            source = s;

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(s);
            while ((line = file.ReadLine()) != null)
            {
                string[] splits = line.Split(' ');
                switch (splits[0])
                {
                    case "1":
                    case "16":
                    case "17":
                        {
                            int Type;
                            Double Elevation;
                            bool HasControlTower;
                            String ICAO;
                            String Name;

                            line = Parser.GetInt(line, out Type);
                            line = Parser.GetDouble(line, out Elevation);
                            line = Parser.GetBool(line, out HasControlTower);
                            line = Parser.Skip(line);
                            line = Parser.GetString(line, out ICAO);
                            line = line.Substring(1);
                            Name = line;

                            Names.Add(Name);
                            ICAOS.Add(ICAO);
                        }
                        break;
                }
            }
        }

        public int FindName(String s)
        {
            for (int i=0; i<Names.Count; i++)
            {
                if (Names[i].Contains(s))
                    return i;
            }
            return -1;
        }

        public int FindICAO(String s)
        {
            for (int i = 0; i < ICAOS.Count; i++)
            {
                if (ICAOS[i].Contains(s))
                    return i;
            }
            return -1;
        }
    }
}
