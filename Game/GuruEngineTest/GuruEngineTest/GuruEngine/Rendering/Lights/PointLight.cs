using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.Rendering.Lights
{
    public class PointLight
    {
        public Vector3 Position;
        public Color Colour;
        public float Radius;
        public float Intensity;
        public int ID;

        public PointLight(Vector3 pos, Color color, float rad, float intensity)
        {
            Position = pos;
            Colour = color;
            Radius = rad;
            Intensity = intensity;
        }
    }
}
