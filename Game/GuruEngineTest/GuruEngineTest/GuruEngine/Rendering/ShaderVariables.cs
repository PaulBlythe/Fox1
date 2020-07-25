using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Rendering
{
    public enum ShaderVariables
    {
        World,
        WorldInverseTranspose,
        WorldViewProjection,
        View,
        Projection,
        ViewInverse,

        SunDirection,
        MoonDirection,
        ViewVector,

        Texture01,
        Texture02,
        Texture03,
        Texture04,
        Texture05,
        Texture06,
        Texture07,
        Texture08,

        DiffuseIntensity,
        SpecularIntensity,
        AmbientIntensity,

        AmbientColour,
        MaterialColour,
        SpecularColor,
        SunColour,

        Shininess,
        AlphaCut,
        TestAlpha,

        EnvironmentMap,

        Time,

        Shadows,

        Lit,                // lit by the sun
        MoonLit,            // lit by the moon

        WindSpeed,
        WindDirection,

        TOTAL
    }
}
