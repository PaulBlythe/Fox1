using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Simulation.Components.Radar.CrossSections
{
    public class ModernAirframeRadarCrossSection : RadarCrossSectionDefinition
    {
        public override float CalculateRCS(int angle)
        {
            float res = 0;
            if (angle <= 15)
            {
                float da = angle / 15.0f;
                res = smoothLerp(10, 12, da) * Nose;
            }
            else if (angle < 45)
            {
                float r1 = 12 * Nose;
                float r2 = 8 * RightBeam;
                float da = (angle - 15) / 30.0f;
                res = smoothLerp(r1, r2, da);

            }
            else if (angle < 90)
            {
                float da = (angle - 45) / 45.0f;
                res = smoothLerp(8, 16, da) * RightBeam;

            }
            else if (angle < 135)
            {
                float r1 = 16 * RightBeam;
                float r2 = 8 * Tail;
                float da = (angle - 90) / 45.0f;
                res = smoothLerp(r1, r2, da);

            }
            else if (angle < 180)
            {
                float da = (angle - 135) / 45.0f;
                res = smoothLerp(8, 12, da) * Tail;

            }
            else if (angle < 225)
            {
                float r1 = 12 * Tail;
                float r2 = 8 * LeftBeam;
                float da = (angle - 180) / 45.0f;
                res = smoothLerp(r1, r2, da);

            }
            else if (angle < 270)
            {
                float da = (angle - 225) / 45.0f;
                res = smoothLerp(8, 16, da) * LeftBeam;

            }
            else if (angle < 315)
            {
                float r1 = 16 * LeftBeam;
                float r2 = 8 * LeftBeam;
                float da = (angle - 270) / 45.0f;
                res = smoothLerp(r1, r2, da);

            }
            else if (angle < 345)
            {
                float r1 = 8 * LeftBeam;
                float r2 = 12 * Nose;
                float da = (angle - 315) / 30.0f;
                res = smoothLerp(r1, r2, da);
            }
            else
            {
                float da = (angle - 345) / 15.0f;
                res = smoothLerp(12, 10, da) * Nose;
            }
            return Scalar * res;
        }
    }
}
