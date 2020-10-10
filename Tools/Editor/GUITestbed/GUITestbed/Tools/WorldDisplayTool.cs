using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GUITestbed.DataHandlers.Fox1;
using GUITestbed.SerialisableData;
using GUITestbed.Rendering._3D;
using GUITestbed.DataHandlers.Fox1.Objects;
using GUITestbed.Rendering;
using GUITestbed.World;

namespace GUITestbed.Tools
{
    public class WorldDisplayTool : Tool
    {
        public static WorldDisplayTool Instance;

        Matrix Projection;
        SkyDomeSystem sky;
        Location Centre;
        FreeCamera camera;
        public GameTime time;
        MouseState oldmouse;
        bool grabbed = false;
        Vector2 gpos = new Vector2();
        int oldclicks;
        Airport airport = null;
        GroundPlaneSystem ground = null;
        Rectangle displayArea = new Rectangle(256, 30, 1920 - 256 - 320, 950);
        LightManager lights = new LightManager();


        public List<ObjectPack> LoadedPacks = new List<ObjectPack>();
        public List<AirportObjectList> ObjectLists = new List<AirportObjectList>();
        Dictionary<String, List<StaticMesh>> LoadedMeshes = new Dictionary<string, List<StaticMesh>>();
        List<StaticMeshInstance> Instances = new List<StaticMeshInstance>();

        public WorldDisplayTool()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), Game1.Instance.GraphicsDevice.Viewport.AspectRatio, 0.5f, 65000);
            Instance = this;
            time = new GameTime(new TimeSpan(12, 0, 0), new TimeSpan(12, 0, 0));
            camera = new FreeCamera(Vector3.Zero, 0, 0, 0.5f, 65000, Game1.Instance.GraphicsDevice);
            sky = new SkyDomeSystem(Game1.Instance, camera, time, Game1.Instance.GraphicsDevice);
            sky.Initialize();
            sky.LoadContent();
            lights.Load(Game1.Instance.GraphicsDevice, Game1.Instance.Content);
        }

        public override void Update(float dt)
        {
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            if (grabbed)
            {
                if (ms.RightButton == ButtonState.Released)
                {
                    grabbed = false;
                }
                else
                {
                    if (displayArea.Contains(ms.X, ms.Y))
                    {
                        float scalar = dt * 0.01f;
                        float x = ms.X - gpos.X;
                        float y = ms.Y - gpos.Y;
                        camera.Rotate(x * scalar, y * scalar);

                        int clicks = ms.ScrollWheelValue;
                        clicks -= oldclicks;
                        float dy = clicks * 0.5f;
                        camera.Move(new Vector3(0, dy, 0));
                    }
                }

            }
            else
            {
                if (ms.RightButton == ButtonState.Pressed)
                {
                    grabbed = true;
                    gpos.X = ms.X;
                    gpos.Y = ms.Y;
                }
                else
                {
                    if (displayArea.Contains(ms.X, ms.Y))
                    {
                        float scale = 0.5f;
                        if (ks.IsKeyDown(Keys.LeftShift))
                            scale *= 0.5f;
                        if (ks.IsKeyDown(Keys.LeftControl))
                            scale *= 0.5f;

                        int df = oldclicks - ms.ScrollWheelValue;
                        camera.Move(Vector3.Forward * (df * scale));
                    }

                }
            }
            oldclicks = ms.ScrollWheelValue;
            time.TotalGameTime.Add(new TimeSpan(0, 0, 0, 0, (int)(dt * 1000)));
            sky.Update(time);
            camera.Update();
            if (ground != null)
                ground.UpdateLighting(sky.LightIntensity * sky.SunColor * 0.4f, sky.SunColor, sky.GetDirection());
            oldmouse = ms;

        }

        public override void Draw()
        {
            sky.PreDraw(time);
            sky.Draw(camera.View, Projection, camera.Transform.Translation);
            if (ground != null)
                ground.Draw(camera);

            if (airport!=null)
            {
                airport.UpdateLighting(sky);
                airport.Draw(camera);

                Vector4 amb = sky.LightIntensity * sky.SunColor * 0.4f;
                Vector4 sc = sky.SunColor;
                Vector4 sd = sky.GetDirection();

                foreach (StaticMeshInstance smi in Instances)
                {
                    foreach (StaticMesh sm in smi.Meshes)
                    {
                        sm.World = smi.World;
                        sm.Draw(camera, amb, sc, sd);

                        //DebugLineDraw.DrawBoundingBox(sm.Bounds, sm.World, Color.Red);
                    }
                }
            }
            lights.Draw(camera);
            DebugLineDraw.Instance.Draw(camera);
            
        }

        public override void SaveResults(string path)
        {
           
        }

        public void AddAirport(Airport air)
        {
            airport = air;
            Centre = new Location(air.Latitude, air.Longitude, air.Altitude);
            ground = new GroundPlaneSystem((float)(air.Altitude * Constants.FEET_TO_MTR), 1000, 65000);
            ground.Load();
            camera.Position = new Vector3(0, (float)(air.Altitude * Constants.FEET_TO_MTR) + 200, 0);
            
        }

        public void ScanObjects()
        {
            Instances.Clear();
            if (airport != null)
            {
                foreach (AirportObjectList aol in ObjectLists)
                {
                    foreach (RunwaySceneryObject r in aol.Objects)
                    {
                        foreach (ObjectPack ob in LoadedPacks)
                        {
                            if (ob.PackedObjects.ContainsKey(r.Name))
                            {
                                String fn = ob.GetPath(r.Name);
                                if (!LoadedMeshes.ContainsKey(fn))
                                {
                                    DataHandlers.Fox1.Objects.Wavefront w = new DataHandlers.Fox1.Objects.Wavefront(fn);
                                    LoadedMeshes.Add(fn, w.GenerateMeshes(Game1.Instance.GraphicsDevice, Game1.Instance.Content));
                                }
                                StaticMeshInstance smi = new StaticMeshInstance();
                                smi.Meshes = LoadedMeshes[fn];

                                Vector2D p = Cartography.ConvertToLocalised(airport.Latitude, airport.Longitude, r.Latitude, r.Longitude);
                                Vector3 pl = new Vector3((float)p.X, r.Altitude, (float)p.Y);
                                if (r.AltitudeIsAgl)
                                    pl.Y += (float)(airport.Altitude * Constants.FEET_TO_MTR);

                                Matrix scale = Matrix.CreateScale((float)r.scale);
                                Matrix trans = Matrix.CreateTranslation(pl);

                                Matrix rot = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(-(float)r.Heading),
                                                                           MathHelper.ToRadians((float)r.Pitch),
                                                                           MathHelper.ToRadians((float)r.Bank));
                                smi.World = scale * rot * trans;

                                Instances.Add(smi);
                                //System.Console.WriteLine("Found object with Guid " + r.Name.ToString() + "\r\n");
                            }
                        }

                    }
                }
            }
        }
    }
}
