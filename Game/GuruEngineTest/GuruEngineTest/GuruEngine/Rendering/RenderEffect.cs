using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuruEngine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GuruEngineTest.GuruEngine.Rendering
{
    public class RenderEffect
    {
        public Effect effect;
        public Dictionary<String, EffectParameter> Parameters = new Dictionary<String, EffectParameter>();

    }
}
