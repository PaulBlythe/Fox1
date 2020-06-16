using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Rendering.Lights;

namespace  GuruEngine.Rendering
{
    public class LightManager
    {

        public List<PointLight> pointLights = new List<PointLight>();
        public List<DirectionalLight> directionLights = new List<DirectionalLight>();

        public LightManager()
        {
        }

        public void AddPointLight(Vector3 pos, Color color, float rad, float intensity)
        {
            PointLight pl = new PointLight(pos, color, rad, intensity);
            pointLights.Add(pl);
        }

        public void AddDirectionalLight(Vector3 dir, Color col, bool isSun, bool isMoon)
        {
            DirectionalLight dl = new DirectionalLight(dir, col, isSun, isMoon);
            directionLights.Add(dl);
        }

        public void Reset()
        {
            pointLights.Clear();
            directionLights.Clear();
        }

        
    }
}
