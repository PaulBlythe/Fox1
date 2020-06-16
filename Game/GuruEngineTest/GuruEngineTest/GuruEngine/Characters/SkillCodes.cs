using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Characters
{
    public enum SkillCodes
    {
        Pilot,
        AerialGunnery,                  // gunnery while in an aircraft shooting at aircraft
        Bombadier,
        Navigator,
        AAAGunner,                      // gunnery while on ground shooting at aircraft
        Gunner,                         // gunnery shooting at ground vehicles from ground
        Strafing,                       // gunnery shooting at ground target from the air

    }
}
