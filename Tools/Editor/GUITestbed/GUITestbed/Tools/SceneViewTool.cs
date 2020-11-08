using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GUITestbed.DataHandlers.Fox1;
using GUITestbed.DebugHelpers;
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
        Camera camera;
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

        Effect box_light_effect;
        Effect lit_effect;
        Effect spot_effect;
        Effect depth_only;

        float depth_bias = 0.004f;

        Rectangle displayArea = new Rectangle(256, 30, 1920 - 256 - 320, 950);

        public SceneViewTool()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), Game1.Instance.GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000);
            camera = new FreeCamera(new Vector3(0,3.85f,-5.8f), MathHelper.ToRadians(180), 0, 0.5f, 1000, Game1.Instance.GraphicsDevice);
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
            spot_effect = Game1.Instance.Content.Load<Effect>(@"Shaders\Spotlight");
            depth_only = Game1.Instance.Content.Load<Effect>(@"Shaders\DepthOnly");
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
                        if (camera is FreeCamera)
                        {
                            FreeCamera fcamera = (FreeCamera)camera;
                            float scalar = dt * 0.01f;
                            float x = ms.X - gpos.X;
                            float y = ms.Y - gpos.Y;
                            fcamera.Rotate(x * scalar, y * scalar);

                            int clicks = ms.ScrollWheelValue;
                            clicks -= oldclicks;
                            float dy = clicks * 0.5f;
                            fcamera.Move(new Vector3(0, dy, 0));
                        }
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
                        if (camera is FreeCamera)
                        {
                            FreeCamera fcamera = (FreeCamera)camera;
                            float scale = 0.5f;
                            if (ks.IsKeyDown(Keys.LeftShift))
                                scale *= 0.5f;
                            if (ks.IsKeyDown(Keys.LeftControl))
                                scale *= 0.5f;

                            int df = oldclicks - ms.ScrollWheelValue;
                            fcamera.Move(Vector3.Forward * (df * scale * 0.01f));
                        }
                    }

                }
            }
            if (ks.IsKeyDown(Keys.D))
                depth_bias -= 0.00001f;
            if (ks.IsKeyDown(Keys.C))
                depth_bias += 0.00001f;

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

        /// <summary>
        /// Draw
        /// </summary>
        public override void Draw()
        {
            sky.PreDraw(time);
            sky.Draw(camera.View, Projection, camera.Transform.Translation);
            if (ground != null)
                ground.Draw(camera);

            Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.Instance.GraphicsDevice.RasterizerState = rasterState;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), Game1.Instance.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100);

            if (current!=null)
            {
                if (isLit)
                {
                    for (int i = 0; i < current.Instances.Count; i++)
                    {
                        lit_effect.Parameters["LightTexture"].SetValue(current.Instances[i].LightMap);
                        ObjectInstance oi = current.Instances[i];
                        current.Meshes[oi.Object].Draw(lit_effect, Projection, camera.View, current.Instances[i].World);
                    }
                    for (int i = 0; i < current.tanims.Count; i++)
                    {
                        lit_effect.Parameters["LightTexture"].SetValue(current.tanims[i].LightMap);
                        TranslateAnimator ta = current.tanims[i];
                        float d = SceneValues[ta.Key] / (ta.MaxValue - ta.MinValue);
                        Vector3 pos = Vector3.Lerp(ta.LowPosition, ta.HighPosition, d);
                        Matrix w = Matrix.CreateFromYawPitchRoll(ta.Rotation.X, ta.Rotation.Y, ta.Rotation.Z) * Matrix.CreateTranslation(pos);

                        current.Meshes[ta.Object].Draw(lit_effect, Projection, camera.View, w);

                    }
                    //DebugLineDraw.DrawBoundingBox(current.LightBoxes[0].Bounds, Matrix.Identity, Color.Red);
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
            Game1.Instance.spriteBatch.Begin();
            Game1.Instance.spriteBatch.DrawString(Game1.Instance.debug_font, depth_bias.ToString(), new Vector2(950, 100), Color.White);
            Game1.Instance.spriteBatch.End();

        }

        public override void SaveResults(string path)
        {

        }

        #region Create light textures

        public const int LightTextureSize = 256;
        public const int LightMeshSize = 4096;
        public const int MeshesPerLine = (LightMeshSize / LightTextureSize);
        public const int MeshesPerFile = (MeshesPerLine * MeshesPerLine);
        public const float CellSize = (1.0f / (float)MeshesPerLine);

        /// <summary>
        /// Create light textures 
        /// </summary>
        public void CreateLightVolume1()
        {
            if (current == null)
                return;

            for (int i=0; i<current.Instances.Count; i++)
            {
                current.Instances[i].LightMap = new RenderTarget2D(Game1.Instance.GraphicsDevice, 256, 256, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                Game1.Instance.GraphicsDevice.SetRenderTarget(current.Instances[i].LightMap);
                Game1.Instance.GraphicsDevice.Clear(Color.Black);
                Game1.Instance.GraphicsDevice.SetRenderTarget(null);
            }
            for (int i = 0; i < current.tanims.Count; i++)
            {
                current.tanims[i].LightMap = new RenderTarget2D(Game1.Instance.GraphicsDevice, 256, 256, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                Game1.Instance.GraphicsDevice.SetRenderTarget(current.tanims[i].LightMap);
                Game1.Instance.GraphicsDevice.Clear(Color.Black);
                Game1.Instance.GraphicsDevice.SetRenderTarget(null);
            }

            #region Do each light box
            foreach (LightBox lb in current.LightBoxes)
            {
                box_light_effect.Parameters["Minimums"].SetValue(lb.Bounds.Min);
                box_light_effect.Parameters["Maximums"].SetValue(lb.Bounds.Max);
                box_light_effect.Parameters["Direction"].SetValue(lb.Direction);
                for (int i = 0; i < current.Instances.Count; i++)
                {
                    Game1.Instance.GraphicsDevice.SetRenderTarget(current.Instances[i].LightMap);
                    Game1.Instance.GraphicsDevice.BlendState = BlendState.Opaque;
                    Game1.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                    Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                    Wavefront w = current.GetMesh(i);
                    Matrix m = current.GetWorld(i);
            
                    box_light_effect.Parameters["World"].SetValue(m);
                    box_light_effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(m)));
                    box_light_effect.Parameters["EyePosition"].SetValue(camera.Position);

                    //w.Draw(box_light_effect);
                   
                }
                for (int i = 0; i < current.tanims.Count; i++)
                {
                    Game1.Instance.GraphicsDevice.SetRenderTarget(current.tanims[i].LightMap);
                    Game1.Instance.GraphicsDevice.BlendState = BlendState.Opaque;
                    Game1.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                    Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                    Wavefront w = current.GetMesh(i);
                    Matrix m = current.GetWorld(i);

                    box_light_effect.Parameters["World"].SetValue(m);
                    box_light_effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(m)));
                    box_light_effect.Parameters["EyePosition"].SetValue(camera.Position);

                    //w.Draw(box_light_effect);

                }

            }
            #endregion

            // do each spotlight
            foreach (SpotLight lb in current.SpotLights)
            {
                #region Depth buffer generation
                Vector3 lookAt = lb.Position - lb.Direction;
                Vector3 right = Vector3.Normalize(Vector3.Cross(lookAt, Vector3.Up));
                Vector3 localUp = Vector3.Normalize(Vector3.Cross(right, lookAt));
                if (float.IsNaN(right.X) || float.IsNaN(right.Y) || float.IsNaN(right.Z))
                {
                    localUp = Vector3.Forward;
                }

                RenderTarget2D depth = new RenderTarget2D(Game1.Instance.GraphicsDevice, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.Depth24,0,RenderTargetUsage.PreserveContents);
                Matrix lightView = Matrix.CreateLookAt(lb.Position, lookAt, localUp);
                Matrix lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 1.0f, 0.01f, 50);
                Matrix lightViewProjection = lightView * lightProjection;

                Game1.Instance.GraphicsDevice.BlendState = BlendState.Opaque;
                Game1.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                Game1.Instance.GraphicsDevice.SetRenderTarget(depth);
                Game1.Instance.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.White, 1, 0);
                depth_only.Parameters["ViewProjection"].SetValue(lightViewProjection);
               

                for (int i = 0; i < current.GetCount(); i++)
                {
                    Wavefront w = current.GetMesh(i);
                    Matrix m = current.GetWorld(i);                  
                    depth_only.Parameters["World"].SetValue(m);
                    w.Draw(depth_only);
                }

                Game1.Instance.GraphicsDevice.SetRenderTarget(null);
                #endregion

                Game1.Instance.GraphicsDevice.BlendState = BlendState.Additive;
                Game1.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.None;

                spot_effect.Parameters["LightViewProj"].SetValue(lightViewProjection);
                spot_effect.Parameters["ShadowTexture"]?.SetValue(depth);
                spot_effect.Parameters["LightPosition"].SetValue(lb.Position);
                spot_effect.Parameters["LightDirection"].SetValue(lb.Direction);
                spot_effect.Parameters["LightConeAngle"].SetValue((float)Math.Cos(lb.ConeAngle));
                spot_effect.Parameters["SpotCosOuterCone"].SetValue((float)Math.Cos(lb.ConeAngle + MathHelper.ToRadians(10)));
                //spot_effect.Parameters["DepthBias"].SetValue(depth_bias);
                for (int i = 0; i < current.GetCount(); i++)
                {
                    Game1.Instance.GraphicsDevice.SetRenderTarget(current.GetLightMap(i));                
                    Wavefront w = current.GetMesh(i);
                    Matrix m = current.GetWorld(i);

                    spot_effect.Parameters["World"].SetValue(m);
                    spot_effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(m)));
                   
                    w.Draw(spot_effect);
                }
                Game1.Instance.GraphicsDevice.SetRenderTarget(null);

            }
            Game1.Instance.GraphicsDevice.SetRenderTarget(null);
            isLit = true;
            //{
            //    SpotLight tlb = current.SpotLights[7];
            //   
            //
            //    Vector3 lookAt = tlb.Position - tlb.Direction;
            //    Vector3 right = Vector3.Normalize(Vector3.Cross(lookAt, Vector3.Up));
            //    Vector3 localUp = Vector3.Normalize(Vector3.Cross(right, lookAt));
            //    if (float.IsNaN(right.X) || float.IsNaN(right.Y) || float.IsNaN(right.Z))
            //    {
            //        localUp = Vector3.Forward;
            //    }
            //
            //    camera = new FixedCamera(tlb.Position, lookAt, localUp, 0.1f, 100, Game1.Instance.GraphicsDevice);
            //}
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
