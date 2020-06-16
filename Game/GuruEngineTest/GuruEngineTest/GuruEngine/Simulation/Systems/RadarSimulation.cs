using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GuruEngine.Simulation.Systems
{
    public class RadarSimulation
    {
        /// <summary>
        /// The energy received by a target painted by a radar
        /// </summary>
        /// <param name="InitialPower">Peak transmission power in watts</param>
        /// <param name="Gain">Dimensionless</param>
        /// <param name="PropogationFactor">1 = vacuum, Dimensionless</param>
        /// <param name="Range">In metres</param>
        /// <returns></returns>
        public static float GetReceivedPower(float InitialPower, float Gain, float PropogationFactor, float Range)
        {
            return (InitialPower * Gain * PropogationFactor) / (4 * MathHelper.Pi * Range * Range);
        }

        /// <summary>
        /// The amount of energy received from a radar pulse reflected by a target
        /// </summary>
        /// <param name="InitialPower">Peak transmission power in watts</param>
        /// <param name="Gain">Dimensionless</param>
        /// <param name="Aperture">Square metres</param>
        /// <param name="RadarCrossSection">In metres</param>
        /// <param name="Range">Range in metres</param>
        /// <param name="PropogationFactor">1 = vacuum, Dimensionless</param>
        /// <returns></returns>
        public static float GetReturn(float InitialPower, float Gain, float Aperture, float RadarCrossSection, float Range, float PropogationFactor)
        {
            float res = (float)(InitialPower * Gain * Aperture * RadarCrossSection * Math.Pow(PropogationFactor, 4));
            float div = (float)((4 * MathHelper.Pi) * (4 * MathHelper.Pi) * Math.Pow(Range, 4));

            if (div < float.Epsilon)
                return InitialPower;

            return res / div;

        }
    }
}
