using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuruEngine.Characters;

namespace GuruEngine.Player.Records.Campaign
{
    public class PlayerCampaignRecord
    {
        public CampaignPeriods Period;
        public Rank rank;
        public List<AircraftCertificationRecord> Certificates = new List<AircraftCertificationRecord>();


        public void AddCertificate(String Aircraft, String Unit, DateTime dateTime)
        {
            Certificates.Add(new AircraftCertificationRecord(Aircraft, Unit, dateTime));
        }
    }
}
