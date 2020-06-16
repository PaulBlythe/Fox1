using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.Rendering.Lights
{
    public class DirectionalLight
    {
        public Vector3 Direction;
        public Color Colour;
        public int ID;
        public bool Sun;
        public bool Moon;

        public DirectionalLight(Vector3 dir, Color col, bool isSun, bool isMoon)
        {
            Direction = dir;
            Colour = col;
            Sun = isSun;
            Moon = isMoon;
        }

    }
}
