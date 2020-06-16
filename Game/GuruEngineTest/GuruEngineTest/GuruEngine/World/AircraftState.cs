using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.World.Payloads;

namespace GuruEngine.World
{
    /// <summary>
    /// This thing holds the aircraft's dynamic variables
    /// </summary>
    public class AircraftState
    {
        public String AircraftName;
        public String CallSign;

        public List<Weapon> Stations = new List<Weapon>();
    }
}
