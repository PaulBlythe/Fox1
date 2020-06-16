using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace History.Common.Geographic
{
    public class GeographicLocation
    {
        public String Name;
        public Double Latitude;
        public Double Longitude;

        public GeographicLocation(String name, Double lat, Double lon)
        {
            Name = name;
            Latitude = lat;
            Longitude = lon;
        }
    }
}
