using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Player.Records
{
    public class AircraftCertificationRecord
    {
        public String Aircraft;
        public DateTime When;
        public String Unit;

        public AircraftCertificationRecord(String a, String u, DateTime w)
        {
            Aircraft = a;
            When = w;
            Unit = u;
        }
    }
}
