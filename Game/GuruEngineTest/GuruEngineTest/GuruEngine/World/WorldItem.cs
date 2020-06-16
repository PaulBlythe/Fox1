using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace GuruEngine.World
{
    public abstract class WorldItem
    {
        public int UpdatePass;

        public abstract void Update(WorldState state);

    }
}
