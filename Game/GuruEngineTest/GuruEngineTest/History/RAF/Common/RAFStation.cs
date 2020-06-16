using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using History.Common.Geographic;

namespace History.RAF.Common
{
    public class RAFStation
    {
        public String Name;
        public String CallSign;
        public RAFStationTypes Type;
        public List<RAFWing> Wings = new List<RAFWing>();
        public GeographicLocation Location;

        public RAFStation(String name, String sign, RAFStationTypes type, GeographicLocation loc)
        {
            Name = name;
            CallSign = sign;
            Type = type;
            Location = loc;
        }

    }
}
