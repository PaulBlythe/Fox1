using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class PhotonEmitter
    {
        public Vector3 EmittedColor;
        public Vector3 V1;
        public Vector3 V2;
        public Vector3 V3;
        public Vector3 V4;
        public Vector3 Normal;
        public Vector3 Centre;
        Random rand = new Random();


        public double getArea()
        {
            return
                AreaOfTriangle((V1 - V2).Length(), (V1 - V3).Length(), (V2 - V3).Length()) +
                AreaOfTriangle((V3 - V4).Length(), (V1 - V4).Length(), (V1 - V3).Length());

        }

        float AreaOfTriangle(float a, float b, float c)
        {
            float s = (a + b + c) / (float)2;
            return (float)Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        public Vector3 RandomPoint()
        {
            Vector3 a = V1;
            Vector3 b = V2;
            Vector3 c = V3;
            Vector3 d = V4;

            float s = (float)rand.NextDouble();
            float t = (float)rand.NextDouble();

            Vector3 answer = s * t * a + s * (1 - t) * b + (1 - s) * t * d + (1 - s) * (1 - t) * c;
            return answer;
        }

        
    }
}
