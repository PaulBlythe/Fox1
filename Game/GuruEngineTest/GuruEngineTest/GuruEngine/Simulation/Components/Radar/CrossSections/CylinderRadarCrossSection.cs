using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Simulation.Components.Radar.CrossSections
{
    public class CylinderRadarCrossSection : RadarCrossSectionDefinition
    {
        public override float CalculateRCS(int angle)
        {
            int section = angle / 45;

            switch (section)
            {
                case 0:
                    {
                        float da = angle / 45.0f;
                        return Scalar * smoothLerp(16 * Nose, 6 * RightBeam, da);
                    }
                case 1:
                    {
                        float da = (angle - 45) / 45.0f;
                        return Scalar * smoothLerp(6 * RightBeam, 18 * RightBeam, da);
                    }
                case 2:
                    {
                        float da = (angle - 90) / 45.0f;
                        return Scalar * smoothLerp(18 * RightBeam, 6 * RightBeam, da);
                    }
                case 3:
                    {
                        float da = (angle - 135) / 45.0f;
                        return Scalar * smoothLerp(6 * RightBeam, 16 * Tail, da);
                    }
                case 4:
                    {
                        float da = (angle - 180) / 45.0f;
                        return Scalar * smoothLerp(16 * Tail, 6 * LeftBeam, da);
                    }
                case 5:
                    {
                        float da = (angle - 225) / 45.0f;
                        return Scalar * smoothLerp(6 * LeftBeam, 18 * LeftBeam, da);
                    }
                case 6:
                    {
                        float da = (angle - 270) / 45.0f;
                        return Scalar * smoothLerp(18 * LeftBeam, 6 * LeftBeam, da);
                    }
                default:
                    {
                        float da = (angle - 315) / 45.0f;
                        return Scalar * smoothLerp(6 * LeftBeam, 16 * Nose, da);
                    }

            }
        }
    }
}
