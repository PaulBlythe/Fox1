using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class Photon
    {
        public Vector3 position;
        public Vector3 direction_from;
        public Vector3 energy;
        public int bounce;

        public Photon(Vector3 pos, Vector3 dir, Vector3 erg, int b)
        {
            position = pos;
            direction_from = dir;
            energy = erg;
            bounce = b;
        }
    }
}
