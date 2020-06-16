using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.GameLogic.Navigation
{
    public class NavPoint
    {
        public double Latitude;
        public double Longitude;
        public float Bearing;
        public float Range;

        public NavPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public void UpdateBearingAndRange()
        {
            throw new NotImplementedException();
        }
    }
}
