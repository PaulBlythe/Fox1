using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace GuruEngine.GameLogic.SensorData
{
    public class RadarContact
    {
        public float Velocity;              // in mS
        public Quaternion Orientation;
        public Vector3 Position;

        public RadarContact(float velocity, Quaternion orientation, Vector3 position)
        {
            Velocity = velocity;
            Orientation = orientation;
            Position = position;
        }
    }
}
