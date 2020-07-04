using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using GuruEngine.Maths;

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
        float WindDirectionDegrees = 25;
        #endregion

        public WeatherManager()
        {
            Instance = this;

            CirrusCloudHeight = 1000;
            CirrusCloudAmount = 100.0f;
            WindSpeed = 5;
            Matrix wd = Matrix.CreateRotationY(MathHelper.ToDegrees(WindDirectionDegrees));
            WindDirection = Vector3.Transform(Vector3.Forward, wd);
        }

        void UpdateWindDirection(float degrees)
        {
            WindDirectionDegrees = degrees;
            WindDirectionDegrees = MathUtils.TrimAngleDegrees(WindDirectionDegrees);
            Matrix wd = Matrix.CreateRotationY(MathHelper.ToDegrees(WindDirectionDegrees));
            WindDirection = Vector3.Transform(Vector3.Forward, wd);
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

        public static float GetWindDirectionDegrees()
        {
            return Instance.WindDirectionDegrees;
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

        #region Setters
        public static void SetWindDirection(float degrees)
        {
            Instance.UpdateWindDirection(degrees);
        }
        public static void SetWindSpeed(float speed)
        {
            Instance.WindSpeed = Math.Max(speed, 0);
        }
        #endregion
    }
}
