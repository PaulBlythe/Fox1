using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Rendering.EffectPasses
{
    public class HypoxiaEffectPass : RenderEffectPass
    {
        public float BlurLevel;
        public float BlackoutLevel;

        public HypoxiaEffectPass()
        {
            Type = EffectPassType.Hypoxia;
        }
    }
}
