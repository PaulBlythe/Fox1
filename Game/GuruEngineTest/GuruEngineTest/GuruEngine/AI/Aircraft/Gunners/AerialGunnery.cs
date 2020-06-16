using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using GuruEngine.DebugHelpers;

using GuruEngine.AI;

namespace GuruEngine.AI.Aircraft.Gunners
{
    public static class AerialGunnery
    {
        public static Vector3 Gravity = new Vector3(0, -9.81f, 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">From target manager</param>
        /// <param name="velocity">Aircraft velocity</param>
        /// <param name="muzzlevelocity">From BulletPropertiesComponent</param>
        /// <param name="dt">Time step</param>
        /// <param name="inWorld">Inverted local world</param>
        /// <returns></returns>
        public static Vector3 GetAimDirection(AITarget target, Vector3 velocity, float muzzlevelocity, float dt, Matrix inWorld, out Vector3 aimpoint)
        {
            // First work out the time it will take to get to the targets current position
            Vector3 relative_current_position = Vector3.Transform(target.Position, inWorld);
            float bullet_velocity = velocity.Length() + muzzlevelocity;
            float time1 = relative_current_position.Length() / bullet_velocity;

            // work out instantaneous acceleration and velocity for target
            Vector3 ivel = (target.Position - target.OldPosition) / dt;
            Vector3 iaccel = (target.Velocity - target.OldVelocity) / dt;

            // move instantaneous values into local world space
            ivel = Vector3.Transform(ivel, inWorld);
            iaccel = Vector3.Transform(iaccel, inWorld);

            // move target to where it will be after time1
            Vector3 predict1 = relative_current_position + (ivel * time1) + (0.5f * iaccel * time1 * time1);

            // work out time a bullet would take to go there 
            float time2 = predict1.Length() / bullet_velocity;

            // work out how far the bullet will go in time2
            float dist1 = bullet_velocity * time2;

            // how far is the bullet from the predicted position
            float dist2 = predict1.Length() - dist1;

            // adjust the time based on this distance to get our time estimation.
            float time3 = time2 + (dist2 / bullet_velocity);

            // work out bullet drop due to gravity
            float drop = 0.5f * 9.81f * time3 * time3;

            // Adjust time for bullet drop
            time3 += drop / bullet_velocity;

            // work out the target position using time3
            Vector3 predict2 = relative_current_position + (ivel * time3) + (0.5f * iaccel * time3 * time3);

            drop = 0.5f * 9.81f * time3 * time3;

            // move aim point up to compensate 
            predict2.Y += drop;
            predict2.Normalize();

            ivel = (target.Position - target.OldPosition) / dt;
            iaccel = (target.Velocity - target.OldVelocity) / dt;
            Vector3 dp = target.Position + (ivel * time3) + (0.5f * iaccel * time3 * time3);
            dp.Y += drop;
            aimpoint = dp;
#if DEBUG
            if (DebugRenderSettings.RenderAimPoints)
            {
                
                DebugLineDraw.DrawTarget(dp, Color.Red, 1);
            }
#endif
            
            return predict2;
        }
    }
}
