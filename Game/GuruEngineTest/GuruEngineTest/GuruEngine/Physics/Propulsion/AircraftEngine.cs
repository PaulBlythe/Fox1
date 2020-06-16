using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.ECS;
using GuruEngine.ECS.Components.AircraftSystems.General;

namespace GuruEngine.Physics.Propulsion
{
    public abstract class AircraftEngine
    {
        public abstract void Update(GameObject parent, EngineComponent comp, float dt);
        public abstract void SetDefinition(String filename);
        public abstract void HandleEvent(String evt);
        public abstract float GetThrust();
        public abstract float GetPowerProduced();
    }
}
