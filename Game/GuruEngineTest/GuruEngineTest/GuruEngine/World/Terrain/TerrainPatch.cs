using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.Physics.World.Geodetic;

namespace GuruEngine.World.Terrain
{
    /// <summary>
    /// A terrain patch is 1 minute of latitude and longitude
    /// </summary>
    public abstract class TerrainPatch
    {
        public bool isVisible = true;
        public GlobalCoordinates Position;

        public abstract void Update();
        public abstract void Destroy();
    }
}
