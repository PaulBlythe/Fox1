using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.Maths
{
    public class Intersections
    {
        /// <summary>
        /// Given a object with a speed we will find what direction it must head (in normalized velocity form)
        /// to intercept the given target with a given velocity from its current position
        /// </summary>
        /// <returns>The normal vector the object or bullet must head in to intercept</returns>
        public static Vector2 QuadricIntercept(Vector2 obj_position, float obj_speed, Vector2 target_position, Vector2 target_velocity)
        {
            float tvx = target_velocity.X;
            float tvy = target_velocity.Y;
            float pdx = target_position.X - obj_position.X;
            float pdy = target_position.Y - obj_position.Y;

            float d = pdx * pdx + pdy * pdy;
            float s = (tvx * tvx + tvy * tvy) - obj_speed * obj_speed;
            float q = (tvx * pdx + tvy * pdy);
            float disc = (q * q) - s * d; // simplify get rid of the fluff
            float disclen = (float)Math.Sqrt(disc);

            float t = (-q + disclen) / s;
            float t2 = (-q - disclen) / s;

            if (t < 0.0f)
                t = t2;

            Vector2 aimpoint = Vector2.Zero;
            if (t > 0.0f)
            {
                aimpoint.X = t * tvx + target_position.X;
                aimpoint.Y = t * tvy + target_position.Y;
            }
            return aimpoint; // returns Vector2.Zero if no positive future time to fire exists
        }

        /// <summary>
        /// Given a object with a speed we will find what direction it must head (in normalized velocity form)
        /// to intercept the given target with a given velocity from its current position
        /// </summary>
        /// <returns>The normal vector the object or bullet must head in to intercept</returns>
        public static Vector3 QuadricIntercept(Vector3 obj_position, float obj_speed, Vector3 target_position, Vector3 target_velocity)
        {
            float tvx = target_velocity.X;
            float tvy = target_velocity.Y;
            float tvz = target_velocity.Z;

            float pdx = target_position.X - obj_position.X;
            float pdy = target_position.Y - obj_position.Y;
            float pdz = target_position.Z - obj_position.Z;

            float d = pdx * pdx + pdy * pdy + pdz * pdz;
            float s = (tvx * tvx + tvy * tvy  + tvz * tvz) - obj_speed * obj_speed;
            float q = (tvx * pdx + tvy * pdy + tvz * pdz);
            float disc = (q * q) - s * d; 
            float disclen = (float)Math.Sqrt(disc);

            float t = (-q + disclen) / s;
            float t2 = (-q - disclen) / s;

            if (t < 0.0f)
                t = t2;

            Vector3 aimpoint = Vector3.Zero;
            if (t > 0.0f)
            {
                aimpoint.X = t * tvx + target_position.X;
                aimpoint.Y = t * tvy + target_position.Y;
                aimpoint.Z = t * tvz + target_position.Z;
            }
            return aimpoint; // returns Vector2.Zero if no positive future time to fire exists
        }
    }
}
