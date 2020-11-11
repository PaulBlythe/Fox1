using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.World.Aircraft;
using GuruEngine.Characters;

namespace GuruEngine.Player.Records.Common
{
    public class Certification
    {
        public AircraftTypes AircraftType;
        public DateTime Date;
        public String CertifyingOfficer;
        public int Rank;
        public String Location;
        public bool DayOnly;


        public Certification(AircraftTypes type, DateTime date, String officer, int rank, String location, bool day)
        {
            AircraftType = type;
            Date = date;
            CertifyingOfficer = officer;
            Rank = rank;
            Location = location;
            DayOnly = day;
        }
    }
}
