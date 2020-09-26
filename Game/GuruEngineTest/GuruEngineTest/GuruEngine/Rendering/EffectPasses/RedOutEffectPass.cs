using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Rendering.EffectPasses
{
    public class RedOutEffectPass:RenderEffectPass
    {
        public float Value;

        public RedOutEffectPass()
        {
            Type = EffectPassType.RedOut;
        }
    }
}
