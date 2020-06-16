using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GuruEngine.Data.Code;

namespace GuruEngine.GameLogic.Missions
{
    public class MissionData
    {
        public List<Airfield> airfields = new List<Airfield>();
        public List<Squadron> squadrons = new List<Squadron>();

        enum LoadingModes
        {
            Header,
            Airfields,
            Squadrons,
            Total
        }

        public MissionData(String missionName)
        {
            LoadingModes mode = LoadingModes.Header;

            using (TextReader reader = File.OpenText(missionName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    switch (mode)
                    {
                        case LoadingModes.Header:
                            {
                                if (line.StartsWith("[AIRFIELDS]"))
                                    mode = LoadingModes.Airfields;
                                if (line.StartsWith("[SQUADRONS]"))
                                    mode = LoadingModes.Squadrons;
                            }
                            break;

                        case LoadingModes.Airfields:
                            {
                                if (line.StartsWith("[/AIRFIELDS]"))
                                    mode = LoadingModes.Header;
                                else
                                {
                                    Airfield ar = new Airfield();
                                    ar.Load(line);
                                    airfields.Add(ar);
                                }
                            }
                            break;
                        case LoadingModes.Squadrons:
                            {
                                if (line.StartsWith("[/SQUADRONS]"))
                                    mode = LoadingModes.Header;
                                else
                                {
                                    Squadron ar = new Squadron();
                                    ar.Load(line);
                                    squadrons.Add(ar);
                                }
                            }
                            break;
                    }
                }
            }
        }

    }
}
