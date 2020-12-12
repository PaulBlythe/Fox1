using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUITestbed.Rendering._3D;
using Microsoft.Xna.Framework;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class Hit
    {
        public double t;
        public double t2;
        public Vector3 normal;
        public Face prim;

        public Hit(Hit other)
        {
            t = other.t;
            t2 = other.t2;
            normal = other.normal;
            prim = other.prim;
        }

        public Hit()
        {
            t = t2 = 0;
            prim = null;
        }
    }
}
