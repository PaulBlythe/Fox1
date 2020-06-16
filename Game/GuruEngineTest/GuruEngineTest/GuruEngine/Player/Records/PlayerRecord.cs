using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.Player.Records.Campaign;

namespace GuruEngine.Core.Player.Records
{
    public class PlayerRecord
    {
        public PlayerCampaignRecord CurrentCampaign = null;
        public List<PlayerCampaignRecord> CompletedCampaigns = new List<PlayerCampaignRecord>();

    }
}
