using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace GuruEngine.Cameras
{
    public abstract class Camera
    {
        public Matrix View;
        public Matrix Projection;
        public Vector3 Position;
        public Vector3 Forward;
        public Vector3 Right;

        public abstract void Update(float gt);

    }
}
