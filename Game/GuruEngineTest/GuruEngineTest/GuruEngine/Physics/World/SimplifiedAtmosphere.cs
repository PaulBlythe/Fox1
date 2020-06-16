using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Physics.World
{
    public class SimplifiedAtmosphere : AtmosphericModel
    {
        #region Data tables
        public static double[][] pd = new double[][]
        {
            /*  Altitude in feet */ new double[]
            {
                       0,
                   36089,
                   65616,
                  104986,
                  154199,
                  167322,
                  232939,
                  262467,
                  3.28e9
            },
            /*  Pressure in Pa */ new double[]
            {
                  101325,
                 22632.1,
                 5474.89,
                 868.019,
                 110.906,
                 66.9389,
                 3.95642,
                 0.88628,
                 0.00001
            },
            /* Temperature in K */ new double[]
            {
                288.15,
                216.65,
                216.65,
                228.65,
                270.65,
                270.65,
                214.65,
                196.65,
                  2.73
            },
            /* Temperature in C */ new double[]
            {
                 15.00,
                -56.50,
                -56.50,
                -44.50,
                 -2.50,
                 -2.50,
                -58.50,
                -76.50,
                -270.4
            },
            /* Lapse K/M      */ new double[]
            {
                  0.0065,
                       0,
                 -0.0010,
                 -0.0028,
                       0,
                  0.0028,
                  0.0020,
                  0.0,
                  0.0
            }
        };
        #endregion



        public SimplifiedAtmosphere()
        {
        }

        /// <summary>
        /// Update the atmospheric model
        /// </summary>
        /// <param name="h">Height above sea level in metres</param>
        /// <returns></returns>
        public override AtmosphericModelResults Update(double h)
        {
            AtmosphericModelResults res = new AtmosphericModelResults();

            double hgt = 0;
            double p0 = Constants.P0;
            double t0 = Constants.T0;

            int ii = 0;
            for (int i = 0; i <8; i++)
            {
                double xhgt = Double.MaxValue;
                double lapse = pd[4][i];

                // Stratosphere starts at a definite temperature, not a definite height:
                if (ii == 0)
                {
                    xhgt = hgt + (t0 - pd[2][i + 1]) / lapse;
                }
                else if (pd[4][i + 1] != -1)
                {
                    xhgt = pd[0][i + 1];
                }
                if (h <= xhgt)
                {
                    res.Pressure = P_layer(h, hgt, p0, t0, lapse);
                    res.Temperature = T_layer(h, hgt, p0, t0, lapse);
                    res.Soundspeed = Math.Sqrt(2403.0832 * res.Temperature);
                    res.Density = res.Pressure / (1718 * (res.Temperature + 459.7));
                    return res;
                }
                p0 = P_layer(xhgt, hgt, p0, t0, lapse);
                t0 = t0 - lapse * (xhgt - hgt);
                hgt = xhgt;
            }
            return res;
        }


        /// Pressure within a layer, as a function of height.
        /// Physics model:  standard or nonstandard atmosphere,depending on what parameters you pass in.
        /// Height in meters, pressures in pascals.
        /// As always, lapse is positive in the troposphere, and zero in the first part of the stratosphere.
        double P_layer(double height, double href, double Pref, double Tref, double lapse)
        {
            if (lapse >= 0)
            {
                double N = lapse * Constants.Rgas / Constants.mm / Constants.g;
                return Pref * Math.Pow((Tref - lapse * (height - href)) / Tref, (1 / N));
            }
            else
            {
                return Pref * Math.Exp(-Constants.g * Constants.mm / Constants.Rgas / Tref * (height - href));
            }
        }

        /// Temperature within a layer, as a function of height.
        /// Physics model:  standard or nonstandard atmosphere depending on what parameters you pass in.
        /// $hh in meters, pressures in Pa.
        /// As always, $lambda is positive in the troposphere,
        /// and zero in the first part of the stratosphere.
        double T_layer(double hh, double hb, double Pb, double Tb, double lambda)
        {
            return Tb - lambda * (hh - hb);
        }

    }
}
