using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using GuruEngine.Physics.World;
using GuruEngine.ECS;
using GuruEngine.Maths;
using GuruEngine.World.PaintSchemes;
using GuruEngine.World.Terrain;
using GuruEngine.Physics.World.Geodetic;
using GuruEngine.ECS.Components.World;
using GuruEngine.AI;
using GuruEngine.Physics.Collision;
using GuruEngine.AI.Scripting;
using GuruEngine.World.Weather;


namespace GuruEngine.World
{
    public class World
    {
        public String Name;
        public GameObjectManager gameObjectManager;
        public AtmosphericModel atmosphericModel;
        public TerrainManager terrainManager;
        public TargetManager targetManager;
        public BulletManager bulletManager;
        public ScriptManager scriptManager;
        public WeatherManager weatherManager;

#if DEBUG
        DebugHelpers.DebugMessageQueue dmq = new DebugHelpers.DebugMessageQueue();
#endif

        //public SoundEffectManager noise;

        public static World Instance;

        public GlobalCoordinates cameraCoordinates;

        public World(String name)
        {
            Name = name;
            Instance = this;
            gameObjectManager = new GameObjectManager();
            atmosphericModel = new SimplifiedAtmosphere();      // TODO allow swap to more accurate atmospheroc model
            terrainManager = new TerrainManager();
            targetManager = new TargetManager();
            bulletManager = new BulletManager();
            scriptManager = new ScriptManager();
            weatherManager = new WeatherManager();
        }

        public void Initialise(ContentManager Content)
        {
            double gx = 0;
            double gy = 0;

            String file = Path.Combine(FilePaths.DataPath, "Scenes");
            file = Path.Combine(file, Name);
            if (File.Exists(file))
            {
                string line = null;
                TextReader reader = new StreamReader(file);
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(' ');
                    switch (parts[0])
                    {
                        case "location":
                            {
                                line = reader.ReadLine();
                                string[] parts2 = line.Split(' ');
                                gx = double.Parse(parts2[0]);
                                gy = double.Parse(parts2[1]);
                            }
                            break;
                        case "objects":
                            {
                                int nobjs = int.Parse(parts[1]);
                                for (int i = 0; i < nobjs; i++)
                                {
                                    line = reader.ReadLine();
                                    gameObjectManager.LoadGameObject(Settings.GetInstance().GameObjectDirectory + line);

                                }
                            }
                            break;
                        case "instances":
                            {
                                int nobjs = int.Parse(parts[1]);
                                for (int i = 0; i < nobjs; i++)
                                {
                                    line = reader.ReadLine();
                                    string[] parts2 = line.Split(' ');

                                    float x = float.Parse(parts2[1]);
                                    float y = float.Parse(parts2[2]);
                                    float z = float.Parse(parts2[3]);

                                    int year = int.Parse(parts2[6]);
                                    bool doTail = bool.Parse(parts2[7]);
                                    int iff = int.Parse(parts2[9]);

                                    GameObject obj = gameObjectManager.CreateInstanceAt(parts2[0], new Vector3(x, y, z));

                                    switch (parts2[10])
                                    {
                                        case "1":
                                            obj.ApplyPaintScheme(new BritishPaintScheme(parts2[4], parts2[5], year, doTail, parts2[8]));
                                            break;
                                        case "2":
                                            obj.ApplyPaintScheme(new GermanPaintScheme(parts2[4], parts2[5], year, doTail, parts2[8]));
                                            break;
                                    }
                                    obj.SetIFF(iff);

                                }
                            }
                            break;
                    }
                }
            }



            gameObjectManager.LoadContent(Content);
            //noise.LoadPacks();

            cameraCoordinates = new GlobalCoordinates(new Angle(gx),new Angle(gy));

            terrainManager.Init((int)gx, (int)gy);

        }

        public void Update(float dt)
        {
            for (int i=0; i<5; i++)
            {
                gameObjectManager.Update(i, dt);
            }
            terrainManager.Update();
            bulletManager.Update(dt);

#if DEBUG
            DebugHelpers.DebugMessageQueue.Instance.Update(dt);
#endif

            // MouseState ms = Mouse.GetState();

            //Parallel.ForEach(instruments, (s) =>
            //{
            //    Vector2 mp = new Vector2(ms.X - s.screenRegion.X, ms.Y - s.screenRegion.Y);
            //    s.manager.Update(mp, ms.LeftButton == ButtonState.Pressed);
            //});
            //noise.Update(dt);
        }

        public void Unload()
        {
            gameObjectManager.Destroy();
            terrainManager.Destroy();

            //noise.Destroy();
        }

        public void RenderOffscreenRenderTargets()
        {
            gameObjectManager.RenderOffscreenRenderTargets();
        }

        //public void RegisterInstrumentManager(InstrumentManager inst, Rectangle rect)
        //{
        //    instruments.Add(new InstrumentManagerRecord(inst, rect));
        //}

        public GameObject CreateEmptyGameObjectAt(String Name, Vector3 pos)
        {
            GameObject go = new GameObject();
            WorldTransform wt = new WorldTransform();
            wt.LocalPosition = pos;
            go.Components.Add("WorldTransform_1",wt);
            gameObjectManager.AddGameObject(go, Name);

            return go;
        }

        public static AtmosphericModel GetAtmos()
        {
            return Instance.atmosphericModel;
        }

        #region WGS 84 methods
        public static bool InverseWGS84(double lat1, double lon1, double lat2, double lon2, out double az1, out double az2, out double s)
        {
            double a = WorldConstants.EQU_RAD;
            double rf = WorldConstants.FLATTENING;
            int iter = 0;
            double testv = 1.0E-10;
            double f = (rf > 0 ? 1.0 / rf : 0.0);
            double b = a * (1.0 - f);

            double phi1 = MathConstants.DEG_TO_RAD * lat1;
            double lam1 = MathConstants.DEG_TO_RAD * lon1;
            double phi2 = MathConstants.DEG_TO_RAD * lat2;
            double lam2 = MathConstants.DEG_TO_RAD * lon2;

            double sinphi1 = Math.Sin(phi1);
            double cosphi1 = Math.Cos(phi1);
            double sinphi2 = Math.Sin(phi2);
            double cosphi2 = Math.Cos(phi2);

            if ((Math.Abs(lat1 - lat2) < testv) && ((Math.Abs(lon1 - lon2) < testv) || Math.Abs(lat1 - 90) < testv))
            {
                // Two points are identical
                az1 = az2 = s = 0;
                return true;
            }
            else if (Math.Abs(cosphi1) < testv)
            {
                // Initial point is polar
                InverseWGS84(lat2, lon2, lat1, lon1, out az1, out az2, out s);
                b = az1;
                az1 = az2;
                az2 = b;
                return true;
            }
            else if (Math.Abs(cosphi2) < testv)
            {
                // Terminal point is polar
                double lon3 = lon1 + 180;
                InverseWGS84(lat1, lon1, lat1, lon3, out az1, out az2, out s);

                s *= 0.5;
                az2 = az1 + 180;
                if (az2 > 360)
                    az2 -= 360;
                return true;
            }
            else if ((Math.Abs(Math.Abs(lon1 - lon2) - 180) < testv) && (Math.Abs(lat1 + lat2) < testv))
            {
                // Geodetic passes through the pole
                double s1, s2;
                InverseWGS84(lat1, lon1, lat1, lon2, out az1, out az2, out s1);
                InverseWGS84(lat2, lon2, lat1, lon2, out az1, out az2, out s2);
                az2 = az1;
                s = s1 + s2;
                return true;

            }else
            {
                double dlam = lam2 - lam1;
                double dlams = dlam;
                double temp = (1.0 - f) * sinphi1 / cosphi1;
                double cosu1 = 1.0 / Math.Sqrt(1 + temp * temp);
                double sinu1 = temp * cosu1;
                temp = (1.0 - f) * sinphi2 / cosphi2;
                double cosu2 = 1.0 / Math.Sqrt(1 + temp * temp);
                double sinu2 = temp * cosu2;

                double cos2saz;
                double sdlams;
                double cdlams;
                double sig;
                double c2sigm;
                double sinsig;
                double cossig;

                do
                {
                    sdlams = Math.Sin(dlams);
                    cdlams = Math.Cos(dlams);
                    sinsig = Math.Sqrt(cosu2 * cosu2 * sdlams * sdlams + (cosu1 * sinu2 - sinu1 * cosu2 * cdlams) * (cosu1 * sinu2 - sinu1 * cosu2 * cdlams));
                    cossig = sinu1 * sinu2 + cosu1 * cosu2 * cdlams;
                    sig = Math.Atan2(sinsig, cossig);
                    double sinaz = cosu1 * cosu2 * sdlams / sinsig;
                    cos2saz = 1.0 - sinaz * sinaz;
                    c2sigm = ((sinu1 == 0) || (sinu2 == 0)) ? cossig : cossig - 2 * sinu1 * sinu2 / cos2saz;
                    double tc = f * cos2saz * (4.0 + f * (4.0 - 3.0 * cos2saz)) / 16.0;
                    temp = dlams;
                    dlams = dlam + (1.0 - tc) * f * sinaz * (sig + tc * sinsig * (c2sigm + tc * cossig * (-1.0 + 2.0 * c2sigm * c2sigm)));
                    iter++;
                    if ((Math.Abs(dlams) > MathHelper.Pi) && (iter > 50))
                    {
                        az1 = az2 = s = 0;
                        return false;
                    }

                } while (Math.Abs(temp - dlams) > testv);

                double us = cos2saz * (a * a - b * b) / (b * b);
                double rnumer = -(cosu1 * sdlams);
                double denom = sinu1 * cosu2 - cosu1 * sinu2 * cdlams;
                az2 = MathConstants.RAD_TO_DEG * Math.Atan2(rnumer, denom);
                if (Math.Abs(az2) < testv)
                    az2 = 0;
                if (az2 < 0)
                    az2 += 360;

                rnumer = cosu2 * sdlams;
                denom = cosu1 * sinu2 - sinu1 * cosu2 * cdlams;
                az1 = MathConstants.RAD_TO_DEG * Math.Atan2(rnumer, denom);
                if (Math.Abs(az1) < testv)
                    az1 = 0;
                if (az1 < 0)
                    az1 += 360;

                double ta = 1.0 + us * (4096.0 + us * (320 - 175 * us)) / 16384.0;
                double tb = us * (256.0 + us * (-128.0 + us * (74.0 - 97.0 * us))) / 1024.0;

                s = b * ta * (sig - tb * sinsig * (c2sigm + tb * (cossig * (-1.0 + 2.0 * c2sigm * c2sigm) - tb * c2sigm * (-3.0 + 4.0 * sinsig * sinsig) * (-3.0 + 4.0 * c2sigm * c2sigm) / 6.0) / 4.0));
                return true;
            }

        }
        #endregion
    }
}
