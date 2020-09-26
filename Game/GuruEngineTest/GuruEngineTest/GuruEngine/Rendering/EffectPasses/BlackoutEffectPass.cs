using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Rendering.EffectPasses
{
    public class BlackoutEffectPass : RenderEffectPass
    {
        public float Value;

        public BlackoutEffectPass()
        {
            Type = EffectPassType.BlackOut;
        }
    }
}
