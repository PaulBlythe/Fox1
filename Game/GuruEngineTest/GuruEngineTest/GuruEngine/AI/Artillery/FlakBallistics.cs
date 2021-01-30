using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.AI.Artillery
{
    public static class FlakBallistics
    {

        /// <summary>
        /// Calculate the required orientation to fire a round and the flight time
        /// This is a simple calculation that does not take into account round drag
        /// It is just for animating a AAA gun
        /// </summary>
        /// <param name="Bearing">Degrees</param>
        /// <param name="Range">Metres</param>
        /// <param name="Altitude">Metres</param>
        /// <param name="Velocity">Muzzle velocity in metres per second</param>
        /// <returns>
        /// Vector2
        /// {
        ///     X == Pitch degrees
        ///     Y == Time  seconds
        /// }
        /// </returns>
        public static Vector2 GetPitchAndTime(float Bearing, float Range, float Altitude, float Velocity)
        {
            Vector2 result = new Vector2();
            float travel_range = (float)Math.Sqrt(Range * Range + Altitude * Altitude);
            float estimated_time = travel_range / Velocity;
            float gravity_drop = 0.5f * 9.81f * estimated_time * estimated_time;
            float aim_altitude = Altitude + gravity_drop;
            result.X = MathHelper.ToDegrees((float)Math.Atan(aim_altitude / Range));
            result.Y = (travel_range + gravity_drop) / Velocity;

            return result;
        }

        /// <summary>
        /// Get rotation direction
        /// </summary>
        /// <param name="target">angle in degrees</param>
        /// <param name="cur">angle in degrees</param>
        /// <returns></returns>
        public static float GetRotationDirection(float target, float cur)
        {
            float res = 1;
            target = LimitAngle(target);

            float left = LimitAngle(cur - 90);
            left = target - left;
            float right = LimitAngle(cur + 90) ;
            right = target - right;
            if (Math.Abs(left) < Math.Abs(right))
                res = -1;

             return res;
        }

        public static float LimitAngle(float ang)
        {
            if (ang < 0)
                ang += 360;
            if (ang > 360)
                ang -= 360;
            return ang;
        }
    }
}
