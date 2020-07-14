using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.DebugHelpers
{
    public static class DebugRenderSettings
    {
        public static bool RenderCollisionMeshes = false;
        public static bool RenderMoonTexture = false;
        public static bool RenderSSAOTexture = false;
        public static bool RenderClock = false;
        public static bool RenderShadowMap = false;
        public static bool RenderDepthMap = false;
        public static bool RenderHooks = false;
        public static bool RenderTurrets = false;
        public static bool RenderAimPoints = false;
        public static bool RenderBullets = false;

        #region Physics debugging 
        public static bool DrawFuelTanks = false;
        public static bool DrawGears = false;
        public static bool DrawForces = false;
        public static bool DrawMasses = false;
        #endregion
    }
}
