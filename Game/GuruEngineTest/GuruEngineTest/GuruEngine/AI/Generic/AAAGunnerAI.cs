using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.Core.AI.Generic
{
    /// TODO include gravity and drag in aiming calculations
    public class AAAGunnerAI
    {
        public static AAAGunnerAI Instance;
        public Random rand = new Random();

        public AAAGunnerAI()
        {
            Instance = this;
        }

        /// <summary>
        /// Calculate time on target using gunner skill.
        /// Note ignores drag and gravity      
        /// </summary>
        /// <param name="muzzlevelocity">Metres per second</param>
        /// <param name="distance">Metres</param>
        /// <param name="gunnerskill">0-1</param>
        /// <returns>time</returns>
        public static float GetEstimatedTOT(float muzzlevelocity, float distance, float gunnerskill)
        {
            float ttt = distance / muzzlevelocity;
            ttt += (float)(Instance.rand.NextDouble() * 2 * (1-gunnerskill));
            return ttt;
        }

        /// <summary>
        /// Calculate lead for a gunner using gunner skill
        /// </summary>
        /// <param name="targetposition">In world coordinates</param>
        /// <param name="targetvelocity">In metres per second</param>
        /// <param name="gunnerskill">0-1</param>
        /// <param name="dt">Estimated time to target</param>
        /// <returns>aim position</returns>
        public static Vector3 GetAimPosition(Vector3 targetposition, Vector3 targetvelocity, float gunnerskill, float dt)
        {
            float aimoffset = 1 + (float)(Instance.rand.NextDouble() * (1 - gunnerskill));
            return  targetposition + (targetvelocity * aimoffset * dt);
        }
    }
}
