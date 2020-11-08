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
    }
}
