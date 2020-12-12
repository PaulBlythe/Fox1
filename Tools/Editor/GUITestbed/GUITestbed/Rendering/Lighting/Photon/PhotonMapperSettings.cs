using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public enum RenderModes
    {
        RENDER_UNDISTRIBUTED,
        RENDER_ABSORBED,
        RENDER_RADIANCE,
        RENDER_FORM_FACTORS,
        RENDER_LIGHTS,
        RENDER_MATERIALS,
    }

    public static class PhotonMapperSettings
    {
        public static RenderModes render_mode = RenderModes.RENDER_FORM_FACTORS;
        public static bool interpolate = true;
        public static bool intersect_backfacing = true;
        public static bool gather_indirect = true;
        public static float ambient_light = 0.2f;
        public static int num_shadow_samples = 3;
        public static int num_bounces = 4;
        public static int num_photons_to_shoot = 10000;
    }
}
