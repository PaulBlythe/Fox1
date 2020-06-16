using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using GuruEngine.Physics.World;
using GuruEngine.DebugHelpers;
using GuruEngine.World;

namespace GuruEngine.Physics.Collision
{
    public class BulletRecord
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public float Age = 0;
        public float MaxAge;
        public Vector3 OldPosition;
        public float Mass;
        public float Drag;
        public float Kaliber;
        public bool IsTracer;

        public bool Update(float dt)
        {
            OldPosition = Position;

            AtmosphericModel atmos = GuruEngine.World.World.GetAtmos();
            AtmosphericModelResults res = atmos.Update(Position.Y);
            float velocity = Velocity.Length();
            float drag = (float)((res.Density * Velocity.LengthSquared() * Drag * Kaliber) / (2 * Mass));
            velocity -= drag * dt;
            Vector3 d = Velocity;
            d.Normalize();
            Velocity = d * velocity;
            Velocity.Y -= 9.81f * dt;

            Position += Velocity * dt;

            Age += dt;
            
#if DEBUG
            if (DebugRenderSettings.RenderBullets)
            {
                DebugLineDraw.DrawLine(Position, OldPosition, Color.Orange);
            }
#endif

            if (Age >= MaxAge)
                return true;

            return false;
        }

    }
}
