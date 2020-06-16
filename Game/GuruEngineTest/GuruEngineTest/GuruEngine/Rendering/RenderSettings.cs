﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Rendering
{
    public enum Skies
    {
        SimpleScattered,
        Traced,
    };
    public enum FixedFilterSize
    {
        Filter2x2,
        Filter3x3,
        Filter5x5,
        Filter7x7
    }
    public class RenderSettings
    {

        private static readonly int[] KernelSizes = { 2, 3, 5, 7 };

        public Skies SkyType = Skies.Traced;
        public bool HDREnabled = false;
        public bool RenderShadows = true;
        public bool CascadeShadowMaps = false;
        public int ShadowMapSize = 2048;
        public int ShadowMapCascades = 4;
        public bool StabilizeCascades = true;
        public float SplitDistance0 = 0.005f;
        public float SplitDistance1 = 0.015f;
        public float SplitDistance2 = 0.5f;
        public float SplitDistance3 = 1.0f;
        public FixedFilterSize FixedFilterSize = FixedFilterSize.Filter2x2;

        public int FixedFilterKernelSize
        {
            get { return KernelSizes[(int)FixedFilterSize]; }
        }
    }
}