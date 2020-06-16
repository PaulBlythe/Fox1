using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Maths;

namespace GuruEngine.Simulation.Components.Radar.Airbourne
{
    public class AirbourneRadarTarget
    {
        public int UID;
        public int IFF;             // 0 Unknown , 1 friend, 2 foe
        public bool Jamming;
        public bool Emiting;
        public Vector3[] LocalPosition = new Vector3[4];
        public float Velocity;
        public float AngleOffTheNose;
        public Quaternion Orientation;
    }
}
