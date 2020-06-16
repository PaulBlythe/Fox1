using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.Physics
{
    public class PhysicsMass
    {
        public float Mass;
        public Vector3 Position;
        public bool IsStatic;

        public PhysicsMass(float m, Vector3 v, bool stat)
        {
            Mass = m;
            Position = v;
            IsStatic = stat;
        }
    }
}
