using System;
using System.Collections.Generic;
using System.IO;
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
using GUITestbed.Scenes;
using GUITestbed.Rendering.Software;
using Wavefront = GUITestbed.Rendering._3D.Wavefront;

namespace GUITestbed.Tools
{
    public class SceneViewTool : Tool
    {
        public static SceneViewTool Instance;
        Matrix Projection;
        SkyDomeSystem sky;
        FreeCamera camera;
        public GameTime time;
        MouseState oldmouse;
        bool grabbed = false;
        Vector2 gpos = new Vector2();
        int oldclicks;
        GroundPlaneSystem ground = null;
        Scene current = null;
        RasterizerState rasterState;

        Dictionary<String, float> SceneValues = new Dictionary<string, float>();
        Dictionary<string, Vector2> SceneRanges = new Dictionary<string, Vector2>();
        Dictionary<string, float> SceneDirections = new Dictionary<string, float>();

        BasicEffect debug_effect;
        bool isLit = false;

        List<Volume> Volumes = new List<Volume>();

        RenderTarget2D[] lightmeshes;
        Effect box_light_effect;
        Effect lit_effect;

        Rectangle displayArea = new Rectangle(256, 30, 1920 - 256 - 320, 950);

        public SceneViewTool()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), Game1.Instance.GraphicsDevice.Viewport.AspectRatio, 1.0f, 65000);
            camera = new FreeCamera(new Vector3(0,3.85f,-5.8f), MathHelper.ToRadians(180), 0, 0.5f, 65000, Game1.Instance.GraphicsDevice);
            sky = new SkyDomeSystem(Game1.Instance, camera, time, Game1.Instance.GraphicsDevice);
            sky.Initialize();
            sky.LoadContent();
            time = new GameTime(new TimeSpan(12, 0, 0), new TimeSpan(12, 0, 0));
            ground = new GroundPlaneSystem(0.0f, 1000, 65000);
            ground.Load();
            Instance = this;

            rasterState = new RasterizerState();
            rasterState.MultiSampleAntiAlias = true;
            rasterState.ScissorTestEnable = false;
            rasterState.FillMode = FillMode.Solid;
            rasterState.CullMode = CullMode.None;
            rasterState.DepthBias = 0;
            rasterState.SlopeScaleDepthBias = 0;

            debug_effect = new BasicEffect(Game1.Instance.GraphicsDevice);
            debug_effect.VertexColorEnabled = false;
            debug_effect.AmbientLightColor = Vector3.Zero;
            debug_effect.DiffuseColor = Vector3.UnitX;
            debug_effect.TextureEnabled = false;

            box_light_effect = Game1.Instance.Content.Load<Effect>(@"Shaders\BoxLight");
            lit_effect = Game1.Instance.Content.Load<Effect>(@"Shaders\LitSceneObject");

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
                        camera.Move(Vector3.Forward * (df * scale * 0.01f));
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

            if (current!=null)
            {
                for (int i = 0; i < current.tanims.Count; i++)
                {
                    TranslateAnimator ta = current.tanims[i];
                    SceneValues[ta.Key] += SceneDirections[ta.Key] * dt;
                    if (SceneValues[ta.Key] >= ta.MaxValue)
                    {
                        SceneValues[ta.Key] = ta.MaxValue;
                        SceneDirections[ta.Key] *= -1;
                    }
                    if (SceneValues[ta.Key] <= ta.MinValue)
                    {
                        SceneValues[ta.Key] = ta.MinValue;
                        SceneDirections[ta.Key] *= -1;
                    }
                }
            }
        }

        public override void Draw()
        {
            sky.PreDraw(time);
            sky.Draw(camera.View, Projection, camera.Transform.Translation);
            if (ground != null)
                ground.Draw(camera);

            Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.Instance.GraphicsDevice.RasterizerState = rasterState;


            if (current!=null)
            {
                if (isLit)
                {
                    int index = 0;
                   
                    for (int i = 0; i < current.Instances.Count; i++)
                    {
                        SetupLights(index);
                        ObjectInstance oi = current.Instances[i];
                        current.Meshes[oi.Object].Draw(lit_effect, Projection, camera.View, current.Instances[i].World);
                        index++;
                    }
                    for (int i = 0; i < current.tanims.Count; i++)
                    {
                        SetupLights(index);
                        TranslateAnimator ta = current.tanims[i];
                        float d = SceneValues[ta.Key] / (ta.MaxValue - ta.MinValue);
                        Vector3 pos = Vector3.Lerp(ta.LowPosition, ta.HighPosition, d);
                        Matrix w = Matrix.CreateFromYawPitchRoll(ta.Rotation.X, ta.Rotation.Y, ta.Rotation.Z) * Matrix.CreateTranslation(pos);

                        current.Meshes[ta.Object].Draw(lit_effect, Projection, camera.View, w);
                        index++;
                    }
                    DebugLineDraw.DrawBoundingBox(current.LightBoxes[0].Bounds, Matrix.Identity, Color.Red);
                }
                else
                {
                    for (int i = 0; i < current.Instances.Count; i++)
                    {
                        ObjectInstance oi = current.Instances[i];
                        current.Meshes[oi.Object].Draw(Projection, camera.View, current.Instances[i].World);
                    }

                    for (int i = 0; i < current.tanims.Count; i++)
                    {
                        TranslateAnimator ta = current.tanims[i];
                        float d = SceneValues[ta.Key] / (ta.MaxValue - ta.MinValue);
                        Vector3 pos = Vector3.Lerp(ta.LowPosition, ta.HighPosition, d);
                        Matrix w = Matrix.CreateFromYawPitchRoll(ta.Rotation.X, ta.Rotation.Y, ta.Rotation.Z)
                                 * Matrix.CreateTranslation(pos);

                        current.Meshes[ta.Object].Draw(Projection, camera.View, w);
                    }
                }
            }
            camera.Projection = Projection;
            DebugLineDraw.Instance.Draw(camera);
        }

        public override void SaveResults(string path)
        {

        }

        #region Create a light volume

        public const int LightTextureSize = 256;
        public const int LightMeshSize = 4096;
        public const int MeshesPerLine = (LightMeshSize / LightTextureSize);
        public const int MeshesPerFile = (MeshesPerLine * MeshesPerLine);
        public const float CellSize = (2.0f / (float)MeshesPerLine);

        public void CreateLightVolume1()
        {
            if (current == null)
                return;

            int total_meshes = current.Instances.Count;
            total_meshes += current.tanims.Count;
            int textures = (total_meshes / MeshesPerFile) + 1;
            lightmeshes = new RenderTarget2D[textures];
            for (int i=0; i<textures; i++)
            {
                lightmeshes[i] = new RenderTarget2D(Game1.Instance.GraphicsDevice, 4096, 4096, false, SurfaceFormat.Color, DepthFormat.None,0,RenderTargetUsage.PreserveContents);
                Game1.Instance.GraphicsDevice.SetRenderTarget(lightmeshes[i]);
                Game1.Instance.GraphicsDevice.Clear(Color.Blue);
                Game1.Instance.GraphicsDevice.SetRenderTarget(null);
            }

            int oldtex = -1;
            foreach (LightBox lb in current.LightBoxes)
            {
                box_light_effect.Parameters["Minimums"].SetValue(lb.Bounds.Min);
                box_light_effect.Parameters["Maximums"].SetValue(lb.Bounds.Max);
                box_light_effect.Parameters["Scale"].SetValue(CellSize);
                for (int i = 0; i < total_meshes; i++)
                {
                    int tex = (i / MeshesPerFile);
                    int subtex = i - (tex * MeshesPerFile);
                    int line = (subtex / MeshesPerLine);
                    int cell = subtex - (line * MeshesPerLine);
            
                    Vector2 offset = new Vector2(cell * CellSize, line * CellSize);
                    if (oldtex != tex)
                    {
                        Game1.Instance.GraphicsDevice.SetRenderTarget(lightmeshes[tex]);
                        oldtex = tex;
                    }
                    Game1.Instance.GraphicsDevice.BlendState = BlendState.Opaque;
                    Game1.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                    Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                    Wavefront w = current.GetMesh(i);
                    Matrix m = current.GetWorld(i);
            
                    box_light_effect.Parameters["World"].SetValue(m);
                    box_light_effect.Parameters["Offset"].SetValue(offset);
                    box_light_effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(m)));
                    box_light_effect.Parameters["EyePosition"].SetValue(camera.Position);

                    w.Draw(box_light_effect);
                   
                }
                Game1.Instance.GraphicsDevice.SetRenderTarget(null);
            }
            for (int i=0; i< lightmeshes.Length; i++)
            {
                Stream stream = File.Create("file" + i.ToString() + ".png");
                RenderTarget2D tex = lightmeshes[i];
                tex.SaveAsPng(stream, tex.Width, tex.Height);
                stream.Dispose();

            }
            isLit = true;
        }

        void SetupLights(int i)
        {
            int tex = (i / MeshesPerFile);
            int subtex = i - (tex * MeshesPerFile);
            int line = (subtex / MeshesPerLine);
            int cell = subtex - (line * MeshesPerLine);

            float cx = 1.0f / MeshesPerLine;

            Vector2 offset = new Vector2(cell * cx, line * cx);
            lit_effect.Parameters["Offset"].SetValue(offset);
            lit_effect.Parameters["LightTexture"].SetValue(lightmeshes[tex]);
            lit_effect.Parameters["Scale"].SetValue(cx);
        }
        #endregion


        public static void CreateLightVolume()
        {
            Instance.CreateLightVolume1();
        }

        public static void SetScene(Scene s)
        {
            Instance.current = s;
            Instance.SceneValues.Clear();
            foreach (TranslateAnimator ta in Instance.current.tanims)
            {
                Vector2 range = new Vector2(ta.MinValue, ta.MaxValue);
                Instance.SceneValues.Add(ta.Key, ta.MinValue);
                Instance.SceneRanges.Add(ta.Key, range);
                Instance.SceneDirections.Add(ta.Key, (ta.MaxValue - ta.MinValue) / 5.0f);
            }
        }
    }
}
