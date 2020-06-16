using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Simulation.Components.Radar.CrossSections
{
    public class SphereRadarCrossSection:RadarCrossSectionDefinition
    {
        public override float CalculateRCS(int angle)
        {
            if (angle < 90)
            {
                float da = angle / 90.0f;
                return Scalar * smoothLerp(Nose * 16, RightBeam * 16, da);

            }
            else if (angle < 180)
            {
                float da = (angle - 90.0f) / 90.0f;
                return Scalar * smoothLerp(RightBeam * 16, Tail * 16, da);

            }
            else if (angle < 270)
            {
                float da = (angle - 180.0f) / 90.0f;
                return Scalar * smoothLerp(Tail * 16, LeftBeam * 16, da);

            }
            float d = (angle - 270.0f) / 90.0f;
            return Scalar * smoothLerp(LeftBeam * 16, Nose * 16, d);
        }
    }
}
