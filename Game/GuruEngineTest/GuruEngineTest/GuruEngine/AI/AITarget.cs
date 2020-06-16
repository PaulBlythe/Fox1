using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.AI
{
    public class AITarget
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 OldPosition;
        public Vector3 OldVelocity;
        public int IFF;
        public int TargetID;
    }
}
