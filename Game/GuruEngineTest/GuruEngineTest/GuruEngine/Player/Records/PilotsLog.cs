using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuruEngine.Player.Records.Common;

namespace GuruEngine.Player.Records
{
    public abstract class PilotsLog
    {
        public List<Certification> aircraftCertifications = new List<Certification>();
        public String PilotName;


    }
}
