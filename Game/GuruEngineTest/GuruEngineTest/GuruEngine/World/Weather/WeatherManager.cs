using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.World.Weather
{
    public class WeatherManager
    {
        public static WeatherManager Instance;

        #region Output variables
        float CirrusCloudHeight;
        float CirrusCloudAmount;
        Vector3 WindDirection;
        float WindSpeed;            // in metres per second
        #endregion

        public WeatherManager()
        {
            Instance = this;

            CirrusCloudHeight = 1000;
            CirrusCloudAmount = 100.0f;
            WindSpeed = 5;
            WindDirection = new Vector3(1, 0, 0);
        }


        #region Getters
        public static float GetCirrusHeight()
        {
            return Instance.CirrusCloudHeight;
        }

        public static float GetCirrusAmount()
        {
            return Instance.CirrusCloudAmount;
        }

        public static Vector3 GetWindDirection()
        {
            return Instance.WindDirection;
        }

        public static float GetWindSpeed()
        {
            return Instance.WindSpeed;
        }
        #endregion
    }
}
