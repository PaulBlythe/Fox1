using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Cameras;
using GuruEngine.World.Ephemeris;
using GuruEngine.Algebra;

namespace GuruEngine.World
{
    public class WorldState
    {
        public Matrix View { get { return camera.View; } }
        public Matrix Projection { get { return camera.Projection; } }
        public Matrix ViewInverse {  get { return Matrix.Invert(camera.View); } }

        public Vector4 SunColour;
        public Vector4 SunDirection;
        public float SunElevation;
        public Vector4 AmbientColour;

        public Vector3 CameraPosition;
        public Vector3 OldCameraPosition;
        public Vector3 CameraForward;
        public double CameraLatitude;
        public double CameraLongitude;
        public float GameTimeMultiplier = 1;

        public DateTime GameTime;
        public float TimeStep;
        public AircraftState playerAircraftState;

        private static WorldState Instance;
        public Camera camera;

        private List<WorldItem> worldItems = new List<WorldItem>();
        public List<WorldItem> sortedList = new List<WorldItem>();
        private bool listDirty = false;

        #region Ephemeris data
        double epoch2000Centuries;
        double epoch1990Days;
        double sunEclipticLongitude;

        public double MoonPhaseAngle;
        public double MoonTilt;
        public Vector3D MoonPosition;

        public Vector3D SunPosition;
        public Vector3D SunDirectionRefracted;

        Matrix33D EquatorialToGeographic;
        Matrix33D EclipticToEquatorial;

        Matrix44D EquatorialToWorld;
        Matrix44D equatorialToWorldNoPrecession;

        public double LST;
        #endregion

        public WorldState()
        {
            Instance = this;

            camera = new QuaternionCamera(Renderer.GetAspectRatio());
            GameTime = new DateTime(1940, 9, 2, 12, 0, 0);
            CameraLatitude = 50;
            CameraLongitude = -2;
            playerAircraftState = new AircraftState();
        }

        public void Update(float timeStep)
        {
            OldCameraPosition = CameraPosition;
            TimeStep = timeStep * GameTimeMultiplier;
            GameTime = GameTime.AddSeconds(TimeStep);
            GameTime = GameTime.AddMilliseconds(TimeStep * 1000.0f);
            CameraPosition = camera.Position;
            CameraForward = camera.Forward;
            

            if (listDirty)
            {
                lock (worldItems)
                {
                    lock (sortedList)
                    {
                        sortedList = worldItems.OrderBy(o => o.UpdatePass).ToList();
                        listDirty = false;
                    }
                }
            }
            UpdateWorld();
        }

        public void UpdateWorld()
        {
            #region Update Ephemeris
            epoch2000Centuries = Ephemeris.Ephemeris.ToEpoch2000Centuries(GameTime.ToUniversalTime(), true);
            epoch1990Days = Ephemeris.Ephemeris.ToEpoch1990Days(GameTime.ToUniversalTime(), false);
            
            // GMST = Greenwich mean sidereal time in radians.
            double gmst = 4.894961 + 230121.675315 * Ephemeris.Ephemeris.ToEpoch2000Centuries(GameTime.ToUniversalTime(), false);
            EquatorialToGeographic = Matrix33D.CreateRotationZ(-gmst);

            // To transform from ecliptic to equatorial, we rotate by the obliquity of the ecliptic.
            double e = 0.409093 - 0.000227 * epoch2000Centuries;
            EclipticToEquatorial = Matrix33D.CreateRotationX(e);

            Matrix33D Ry = Matrix33D.CreateRotationY(-0.00972 * epoch2000Centuries);
            Matrix33D Rz = Matrix33D.CreateRotationZ(0.01118 * epoch2000Centuries);
            Matrix33D precession = Rz * Ry * Rz;

            // Latitude rotation
            var rLat = Matrix33D.CreateRotationY(DoubleClassExtension.ToRadians(CameraLatitude) - DoubleClassExtension.PiOver2);

            // Longitude rotation
            // LMST = Local mean sidereal time in radians.
            double lmst = gmst + DoubleClassExtension.ToRadians(CameraLongitude);
            var rLong = Matrix33D.CreateRotationZ(-lmst);

            // Earth radius at the equator. (We assume a perfect sphere. We do not support geodetic 
            // systems with imperfect earth spheres.)
            const double earthRadius = 6378.137 * 1000;
            var equatorialToHorizontalTranslation = new Vector3D(0, -earthRadius - CameraPosition.Y, 0);

            // Switching of the coordinate axes between Equatorial (z up) and Horizontal (y up).
            var axisSwitch = new Matrix33D(0, 1, 0,
                                           0, 0, 1,
                                           1, 0, 0);

            EquatorialToWorld = new Matrix44D(axisSwitch * rLat * rLong * precession, equatorialToHorizontalTranslation);

            equatorialToWorldNoPrecession = new Matrix44D(axisSwitch * rLat * rLong, equatorialToHorizontalTranslation);

            ComputeSunPosition();
            ComputeMoonPosition();

            double utcnow = GameTime.ToUniversalTime().TimeOfDay.TotalHours;
            //The sidereal time is measured by the rotation of the Earth, with respect to the stars (rather than relative to the Sun).
            //Local sidereal time is the right ascension (RA, an equatorial coordinate) of a star on the observers meridian.
            //In other words, he sidereal time is a direct indication of whether a celestial object of known right ascension is observable at that instant.
            LST = 100.46f + 0.985647f * epoch1990Days + utcnow * 15.0f;

            //4a. As RA and DEC are in degree, we convert this one in degrees as well
            int modulo = (int)Math.Floor(LST) / 360;
            LST = (LST - (360 * modulo)) / 15.0f;
            LST *= 360 / 24.0;

            #endregion

            lock (sortedList)
            {
                foreach (WorldItem w in sortedList)
                {
                    w.Update(this);
                }
            }
        }

        public void AddWorldItem(WorldItem w)
        {
            listDirty = true;
            lock (worldItems)
            {
                worldItems.Add(w);
            }
        }

        public static WorldState GetWorldState()
        {
            return WorldState.Instance;
        }

        public static float GetWorldTimestep()
        {
            return WorldState.Instance.TimeStep;
        }

        public static AircraftState GetPlayerAircraftState()
        {
            return Instance.playerAircraftState;
        }

        public static void ChangeTimeStepMultiplier(float value)
        {
            Instance.GameTimeMultiplier *= value;
        }

        void ComputeSunPosition()
        {
            // See http://en.wikipedia.org/wiki/Position_of_the_Sun. (But these formulas seem to be a bit
            // more precise.)

            double meanAnomaly = 6.24 + 628.302 * epoch2000Centuries;

            // Ecliptic longitude.
            sunEclipticLongitude = 4.895048 + 628.331951 * epoch2000Centuries + (0.033417 - 0.000084 * epoch2000Centuries) * Math.Sin(meanAnomaly) + 0.000351 * Math.Sin(2.0 * meanAnomaly);

            // Distance from earth in astronomical units.
            double geocentricDistance = 1.000140 - (0.016708 - 0.000042 * epoch2000Centuries) * Math.Cos(meanAnomaly) - 0.000141 * Math.Cos(2.0 * meanAnomaly);

            // Sun position.
            Vector3D sunPositionEcliptic = Vector3D.ToCartesian(geocentricDistance, 0, sunEclipticLongitude);
            Vector3D sunPositionEquatorial = EclipticToEquatorial * sunPositionEcliptic;

            // Note: The sun formula is already corrected by precession.
            SunPosition = equatorialToWorldNoPrecession.TransformDirection(sunPositionEquatorial);
            Vector3D sunDirection = SunPosition;

            // Convert from astronomical units to meters.
            const double au = 149597870700; // 1 au = 149,597,870,700 m
            SunPosition *= au;
            SunDirection = -SunPosition.ToVector4();
            SunDirection.Normalize();

            // Account for atmospheric refraction.
            double elevation = Math.Asin(sunDirection.Y);
            SunElevation = (float)elevation;
            elevation = DoubleClassExtension.Refract(elevation);
            sunDirection.Y = Math.Sin(elevation);
            sunDirection.Normalize();
            SunDirectionRefracted = sunDirection;
        }

        void ComputeMoonPosition()
        {
            double lp = 3.8104 + 8399.7091 * epoch2000Centuries;
            double m = 6.2300 + 628.3019 * epoch2000Centuries;
            double f = 1.6280 + 8433.4663 * epoch2000Centuries;
            double mp = 2.3554 + 8328.6911 * epoch2000Centuries;
            double d = 5.1985 + 7771.3772 * epoch2000Centuries;

            double longitude =
                lp
                + 0.1098 * Math.Sin(mp)
                + 0.0222 * Math.Sin(2 * d - mp)
                + 0.0115 * Math.Sin(2 * d)
                + 0.0037 * Math.Sin(2 * mp)
                - 0.0032 * Math.Sin(m)
                - 0.0020 * Math.Sin(2 * f)
                + 0.0010 * Math.Sin(2 * d - 2 * mp)
                + 0.0010 * Math.Sin(2 * d - m - mp)
                + 0.0009 * Math.Sin(2 * d + mp)
                + 0.0008 * Math.Sin(2 * d - m)
                + 0.0007 * Math.Sin(mp - m)
                - 0.0006 * Math.Sin(d)
                - 0.0005 * Math.Sin(m + mp);

            double latitude =
                +0.0895 * Math.Sin(f)
                + 0.0049 * Math.Sin(mp + f)
                + 0.0048 * Math.Sin(mp - f)
                + 0.0030 * Math.Sin(2 * d - f)
                + 0.0010 * Math.Sin(2 * d + f - mp)
                + 0.0008 * Math.Sin(2 * d - f - mp)
                + 0.0006 * Math.Sin(2 * d + f);

            longitude = DoubleClassExtension.InRange(longitude);
            sunEclipticLongitude = DoubleClassExtension.InRange(sunEclipticLongitude);

            double moonage = epoch1990Days % 29.530588853;

            moonage = (moonage / 29.530588853);     // 0-1 with 0 and 1 both full moon
            moonage = (moonage - 0.5) * 2;          // -1 to 1 with abs(age) == 1 full moon
            if (moonage<0)
            {
                // waxing
                MoonPhaseAngle = MathHelper.Lerp(1, 0.1f, (float)-moonage);
            }
            else
            {
                // waning
                MoonPhaseAngle = MathHelper.Lerp(0.1f, 1, (float)moonage);
            }


            MoonPhaseAngle = MoonPhaseAngle * MathHelper.PiOver2;



            MoonTilt = Math.Abs(longitude - sunEclipticLongitude);
            MoonPhaseAngle = DoubleClassExtension.InRange(MoonPhaseAngle);

            double pip =
                +0.016593
                + 0.000904 * Math.Cos(mp)
                + 0.000166 * Math.Cos(2 * d - mp)
                + 0.000137 * Math.Cos(2 * d)
                + 0.000049 * Math.Cos(2 * mp)
                + 0.000015 * Math.Cos(2 * d + mp)
                + 0.000009 * Math.Cos(2 * d - m);

            double dMoon = 1.0 / pip; // Earth radii

            // Moon position in Cartesian coordinates of the ecliptic coordinates system.
            Vector3D moonPositionEcliptic = Vector3D.ToCartesian(dMoon, latitude, longitude);

            // Moon position in Cartesian coordinates of the equatorial coordinates system.
            Vector3D moonPositionEquatorial = EclipticToEquatorial * moonPositionEcliptic;

            // To [m].
            moonPositionEquatorial *= 6378.137 * 1000;

            // Note: The moon formula is already corrected by precession.
            MoonPosition = equatorialToWorldNoPrecession.TransformPosition(moonPositionEquatorial);
        }
    }
}
