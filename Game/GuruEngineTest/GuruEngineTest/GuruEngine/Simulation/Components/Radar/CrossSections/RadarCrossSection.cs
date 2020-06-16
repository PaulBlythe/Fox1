using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GuruEngine.Simulation.Components.Radar.CrossSections
{
    public abstract class RadarCrossSectionDefinition
    {
        public float Nose;
        public float RightBeam;
        public float Tail;
        public float LeftBeam;
        public float Scalar;

        public abstract float CalculateRCS(int angle);

        public float smoothLerp(float va, float vb, float t)
        {
            t = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
            return (t * vb) + ((1 - t) * va);
        }

        public static RadarCrossSectionDefinition Load(String path)
        {
            RadarCrossSectionDefinition res = null;

            using (BinaryReader b = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                int type = b.ReadInt32();
                switch (type)
                {
                    case 0:
                        {
                            res = new ModernAirframeRadarCrossSection();
                        }
                        break;

                    case 1:
                        {
                            res = new CylinderRadarCrossSection();
                        }
                        break;
                    case 2:
                        {
                            res = new SphereRadarCrossSection();
                        }
                        break;

                }
                res.Nose = b.ReadSingle();
                res.Tail = b.ReadSingle();
                res.RightBeam = b.ReadSingle();
                res.LeftBeam = b.ReadSingle();
                res.Scalar = b.ReadSingle();
            }

            return res;
        }
    }
}
