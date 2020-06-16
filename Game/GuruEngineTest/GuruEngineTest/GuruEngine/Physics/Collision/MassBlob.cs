using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision
{
    public class MassBlob
    {
        public float Mass;
        public Vector3 Position;

        public MassBlob(float m, Vector3 p)
        {
            Mass = m;
            Position = p;
        }

        public Matrix GetTensor()
        {
            Matrix inertiaTensor;

            // Calculate some values used below.
            float massFactor = (1.0f / 12.0f) * Mass;
            float widthSquared = 4.0f;
            float heightSquared = 4.0f;
            float depthSquared = 4.0f;

            // Get the inertia tensor.

            inertiaTensor.M11 = massFactor * (heightSquared + depthSquared);
            inertiaTensor.M12 = 0.0f;
            inertiaTensor.M13 = 0.0f;
            inertiaTensor.M14 = 0.0f;

            inertiaTensor.M21 = 0.0f;
            inertiaTensor.M22 = massFactor * (widthSquared + depthSquared);
            inertiaTensor.M23 = 0.0f;
            inertiaTensor.M24 = 0.0f;

            inertiaTensor.M31 = 0.0f;
            inertiaTensor.M32 = 0.0f;
            inertiaTensor.M33 = massFactor * (widthSquared + heightSquared);
            inertiaTensor.M34 = 0.0f;

            inertiaTensor.M41 = 0.0f;
            inertiaTensor.M42 = 0.0f;
            inertiaTensor.M43 = 0.0f;
            inertiaTensor.M44 = 1.0f;

            // Adjust the inertia tensor by the offset.
            return  MathsHelper.TransferAxis(inertiaTensor, Position, Mass);
        }
    }
}
