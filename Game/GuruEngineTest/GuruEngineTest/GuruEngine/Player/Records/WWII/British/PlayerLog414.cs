using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuruEngine.Player.Records.Common;
using GuruEngine.Characters;

namespace GuruEngine.Player.Records.WWII.British
{
    public class PlayerLog414:PilotsLog
    {
        public PlayerLog414()
        {
            PilotName = "Paul Blythe";
            aircraftCertifications.Add(new Certification(World.Aircraft.AircraftTypes.SpitfireMkI, new DateTime(1939, 4, 21), "W/Cmdr F Brown", Rank.MakeCode(RankCodes.OF1, 10, false), "No 327 fs", true));
            aircraftCertifications.Add(new Certification(World.Aircraft.AircraftTypes.SpitfireMkI, new DateTime(1939, 5, 29), "W/Cmdr F Brown", Rank.MakeCode(RankCodes.OF1, 10, false), "No 327 fs", false));
            aircraftCertifications.Add(new Certification(World.Aircraft.AircraftTypes.SpitfireMkIV, new DateTime(1941, 8, 13), "W/Cmdr G Evans", Rank.MakeCode(RankCodes.OF1, 10, false), "No 12 otu", true));

        }

    }
}
