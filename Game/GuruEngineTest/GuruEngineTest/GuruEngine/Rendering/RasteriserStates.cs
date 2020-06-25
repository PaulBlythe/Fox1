using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Rendering
{
    public enum RasteriserStates
    {
        Normal,
        NormalNoCull,
        NoDepth,
        NoDepthNoCull,
        Wireframe,
        ShadowMap,
        CullCounterclockwise,
        CullClockwise,
        TotalStates
    }
}
