using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class RayPrimitive
    {
        Material material;
        List<Photon> photons = new List<Photon>();
        double intensity;

        Material getMaterial()
        {
            return material;
        }

        public void addPhoton(Photon p)
        {
            photons.Add(p);
        }

        public List<Photon> getPhotons()
        {
            return photons;
        }

        public void resetPhotons()
        {
            photons.Clear();
        }

        public double getIntensity()
        {
            return intensity;
        }

        public void setIntensity(double d)
        {
            if (d < 0.0)
            {
                d = 0.0;
            }
            if (d > 1.0)
            {
                d = 1.0;
            }
            intensity = d;
        }
    }
}
