using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using History.Common.Geographic;

namespace History.RAF.Common
{
    public class RAFGroup
    {
        public String Name;
        public RAFGroupRole Role;
        public List<RAFStation> Stations = new List<RAFStation>();
        public GeographicLocation Location;

        public RAFGroup(RAFGroupRole role, String name, GeographicLocation loc)
        {
            Name = name;
            Role = role;
            Location = loc;
        }
    }
}
