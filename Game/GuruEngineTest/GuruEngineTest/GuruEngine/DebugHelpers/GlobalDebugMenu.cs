using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Assets;
using GuruEngine.World;
using GuruEngine.DebugHelpers;
using GuruEngine.Rendering;
using GuruEngine.World.Weather;

namespace GuruEngine.DebugHelpers
{
    public abstract class GDMenuItem
    {
        public List<Rectangle> buttons = new List<Rectangle>();
        public List<String> text = new List<string>();
        public List<Vector2> textpositions = new List<Vector2>();
        public List<Rectangle> checkboxes = new List<Rectangle>();

        public abstract GDMenuItem HandleEvent(int i);
        public abstract void DrawExtra(SpriteBatch batch);
    }

    #region Menus
    public class TopLevelMenu : GDMenuItem
    {
        public TopLevelMenu()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 65, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 115, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 165, 286, 32);
            buttons.Add(b);

            text.Add("Renderer");
            text.Add("Physics");
            text.Add("World");
            text.Add("Sound");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 70));
            textpositions.Add(new Vector2(18, 120));
            textpositions.Add(new Vector2(18, 170));
        }

        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    return new RendererTopMenu();
                case 1:
                    return new PhysicsTopLevelMenu();
                case 2:
                    return new WorldTopLevelMenu();
                case 3:
                    break;
            }
            return this;
        }

        public override void DrawExtra(SpriteBatch batch)
        {
        }
    }

    public class DebugTexturedisplay : GDMenuItem
    {
        public DebugTexturedisplay()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (1 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (2 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (3 * 50), 286, 32);
            buttons.Add(b);


            b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            text.Add("Display moon texture");
            text.Add("Display shadow texture");
            text.Add("Display depth texture");

            text.Add("Display light texture");

            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (1 * 50)));
            textpositions.Add(new Vector2(18, 20 + (2 * 50)));
            textpositions.Add(new Vector2(18, 20 + (3 * 50)));


            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }

        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    DebugRenderSettings.RenderMoonTexture = !DebugRenderSettings.RenderMoonTexture;
                    break;
                case 1:
                    DebugRenderSettings.RenderShadowMap = !DebugRenderSettings.RenderShadowMap;
                    break;
                case 2:
                    DebugRenderSettings.RenderDepthMap = !DebugRenderSettings.RenderDepthMap;
                    break;              
                case 3:
                    DebugRenderSettings.RenderLightTexture = !DebugRenderSettings.RenderLightTexture;
                    break;

                case 4:
                    return new RendererTopMenu();
            }
            return this;
        }
        public override void DrawExtra(SpriteBatch batch)
        {
        }
    }

    public class DebugSSAO : GDMenuItem
    {
        public DebugSSAO()
        {
            Rectangle b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            b = new Rectangle(150, 25 + (3 * 50), 16, 16);
            buttons.Add(b);
            b = new Rectangle(290, 25 + (3 * 50), 16, 16);
            buttons.Add(b);

            b = new Rectangle(150, 25 + (2 * 50), 16, 16);
            buttons.Add(b);
            b = new Rectangle(290, 25 + (2 * 50), 16, 16);
            buttons.Add(b);

            b = new Rectangle(150, 25 + (1 * 50), 16, 16);
            buttons.Add(b);
            b = new Rectangle(290, 25 + (1 * 50), 16, 16);
            buttons.Add(b);

            b = new Rectangle(15, 15 + (4 * 50), 286, 32);
            buttons.Add(b);

            b = new Rectangle(15, 15 + (5 * 50), 286, 32);
            buttons.Add(b);

            b = new Rectangle(260, 15 + (0 * 50), 32, 32);
            checkboxes.Add(b);

            text.Add("SSAO toggle");
            text.Add("Area");
            text.Add("Falloff");
            text.Add("Radius");
            text.Add("Display SSAO texture");
            text.Add("Blur");

            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (1 * 50)));
            textpositions.Add(new Vector2(18, 20 + (2 * 50)));
            textpositions.Add(new Vector2(18, 20 + (3 * 50)));
            textpositions.Add(new Vector2(18, 20 + (4 * 50)));
            textpositions.Add(new Vector2(18, 20 + (5 * 50)));

            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }

        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    return new RendererTopMenu();
                case 1:
                    Renderer.Instance.renderSettings.SSAOSampleRadius -= 0.00001f;
                    break;
                case 2:
                    Renderer.Instance.renderSettings.SSAOSampleRadius += 0.0001f;
                    break;
                case 3:
                    Renderer.Instance.renderSettings.SSAODistanceScale -= 0.000001f;
                    break;
                case 4:
                    Renderer.Instance.renderSettings.SSAODistanceScale += 0.000001f;
                    break;
                case 5:
                    Renderer.Instance.renderSettings.SSAOArea -= 0.00001f;
                    break;
                case 6:
                    Renderer.Instance.renderSettings.SSAOArea += 0.00001f;
                    break;
                case 7:
                    DebugRenderSettings.RenderSSAOTexture = !DebugRenderSettings.RenderSSAOTexture;
                    break;
                case 8:
                    Renderer.Instance.renderSettings.SSAOBlur = !Renderer.Instance.renderSettings.SSAOBlur;
                    break;

                case 20:
                    if (Renderer.Instance.renderSettings.SSAOType == SSAOTypes.None)
                        Renderer.Instance.renderSettings.SSAOType = SSAOTypes.Simple;
                    else
                        Renderer.Instance.renderSettings.SSAOType = SSAOTypes.None;
                    break;
            }
            return this;
        }
        public override void DrawExtra(SpriteBatch batch)
        {
            if (Renderer.Instance.renderSettings.SSAOType != SSAOTypes.None)
            {
                Rectangle r = new Rectangle(checkboxes[0].X + 4, checkboxes[0].Y + 4, checkboxes[0].Width - 8, checkboxes[0].Height - 8);
                batch.FillRectangle(r, Color.Yellow);
            }

            String s = String.Format("{0:0.00000}", Renderer.Instance.renderSettings.SSAOSampleRadius);
            batch.DrawString(AssetManager.GetDebugFont(), s, new Vector2(180, 170), Color.White);
            s = String.Format("{0:0.000000}", Renderer.Instance.renderSettings.SSAODistanceScale);
            batch.DrawString(AssetManager.GetDebugFont(), s, new Vector2(180, 120), Color.White);
            s = String.Format("{0:0.000000}", Renderer.Instance.renderSettings.SSAOArea);
            batch.DrawString(AssetManager.GetDebugFont(), s, new Vector2(180, 70), Color.White);
        }
    }

    public class RenderSettings : GDMenuItem
    {
        public RenderSettings()
        {
            Rectangle b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            b = new Rectangle(150, 25 + (0 * 50), 16, 16);
            buttons.Add(b);
            b = new Rectangle(290, 25 + (0 * 50), 16, 16);
            buttons.Add(b);

            text.Add("Gamma");
            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }

        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    return new RendererTopMenu();
                case 1:
                    Renderer.Instance.renderSettings.gamma -= 0.1f;
                    break;
                case 2:
                    Renderer.Instance.renderSettings.gamma += 0.1f;
                    break;

            }
            return this;
        }
        public override void DrawExtra(SpriteBatch batch)
        {

            String s = String.Format("{0:0.00}", Renderer.Instance.renderSettings.gamma);
            batch.DrawString(AssetManager.GetDebugFont(), s, new Vector2(180, 20), Color.White);

        }
    }

    public class RendererTopMenu : GDMenuItem
    {
        public RendererTopMenu()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (1 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (2 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            text.Add("Textures");
            text.Add("SSAO");
            text.Add("Settings");
            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (1 * 50)));
            textpositions.Add(new Vector2(18, 20 + (2 * 50)));

            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }
        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    return new DebugTexturedisplay();
                case 1:
                    return new DebugSSAO();
                case 2:
                    return new RenderSettings();
                case 3:
                    return new TopLevelMenu();
            }
            return this;
        }
        public override void DrawExtra(SpriteBatch batch)
        {
        }
    }

    public class WeatherMenu : GDMenuItem
    {
        public WeatherMenu()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (1 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (2 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (3 * 50), 286, 32);
            buttons.Add(b);

            b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            text.Add("Wind speed +");
            text.Add("Wind speed -");
            text.Add("Wind direction +");
            text.Add("Wind direction -");

            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (1 * 50)));
            textpositions.Add(new Vector2(18, 20 + (2 * 50)));
            textpositions.Add(new Vector2(18, 20 + (3 * 50)));

            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }
        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    WeatherManager.SetWindSpeed(WeatherManager.GetWindSpeed() + 1);
                    break;
                case 1:
                    WeatherManager.SetWindSpeed(WeatherManager.GetWindSpeed() - 1);
                    break;
                case 2:
                    WeatherManager.SetWindDirection(WeatherManager.GetWindDirectionDegrees() + 5);
                    break;
                case 3:
                    WeatherManager.SetWindDirection(WeatherManager.GetWindDirectionDegrees() - 5);
                    break;
                case 4:
                    return new WorldTopLevelMenu();
            }
            return this;
        }
        public override void DrawExtra(SpriteBatch batch)
        {
            String t = String.Format("Wind speed {0} ms", WeatherManager.GetWindSpeed());
            batch.DrawString(AssetManager.GetDebugFont(), t, new Vector2(960, 30), Color.Black);
            t = String.Format("Wind direction {0} degrees", WeatherManager.GetWindDirectionDegrees());
            batch.DrawString(AssetManager.GetDebugFont(), t, new Vector2(960, 50), Color.Black);
        }
    }

    public class PhysicsTopLevelMenu : GDMenuItem
    {
        public PhysicsTopLevelMenu()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (1 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (2 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (3 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (4 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            text.Add("Display collision meshes");
            text.Add("Display hooks");
            text.Add("Display turrets");
            text.Add("Display aiming");
            text.Add("Display bullets");
            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (1 * 50)));
            textpositions.Add(new Vector2(18, 20 + (2 * 50)));
            textpositions.Add(new Vector2(18, 20 + (3 * 50)));
            textpositions.Add(new Vector2(18, 20 + (4 * 50)));
            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }

        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    DebugRenderSettings.RenderCollisionMeshes = !DebugRenderSettings.RenderCollisionMeshes;
                    break;
                case 1:
                    DebugRenderSettings.RenderHooks = !DebugRenderSettings.RenderHooks;
                    break;
                case 2:
                    DebugRenderSettings.RenderTurrets = !DebugRenderSettings.RenderTurrets;
                    break;
                case 3:
                    DebugRenderSettings.RenderAimPoints = !DebugRenderSettings.RenderAimPoints;
                    break;
                case 4:
                    DebugRenderSettings.RenderBullets = !DebugRenderSettings.RenderBullets;
                    break;
                case 5:
                    return new TopLevelMenu();

            }
            return this;
        }

        public override void DrawExtra(SpriteBatch batch)
        {
        }
    }

    public class WorldTopLevelMenu : GDMenuItem
    {
        public WorldTopLevelMenu()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (1 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (2 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (3 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (4 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            text.Add("Advance time 1 hour");
            text.Add("Show clock");
            text.Add("Time * 10");
            text.Add("Time / 10");
            text.Add("Weather");
            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (1 * 50)));
            textpositions.Add(new Vector2(18, 20 + (2 * 50)));
            textpositions.Add(new Vector2(18, 20 + (3 * 50)));
            textpositions.Add(new Vector2(18, 20 + (4 * 50)));
            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }

        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    WorldState.GetWorldState().GameTime = WorldState.GetWorldState().GameTime.AddHours(1);
                    break;
                case 1:
                    DebugRenderSettings.RenderClock = !DebugRenderSettings.RenderClock;
                    break;
                case 2:
                    WorldState.ChangeTimeStepMultiplier(10);
                    break;
                case 3:
                    WorldState.ChangeTimeStepMultiplier(1.0f / 10.0f);
                    break;
                case 4:
                    return new WeatherMenu();
                case 5:
                    return new TopLevelMenu();
            }
            return this;
        }
        public override void DrawExtra(SpriteBatch batch)
        {

        }
    }

    #endregion

    public class GlobalDebugMenu
    {
        GDMenuItem current;

        public GlobalDebugMenu()
        {

        }

#if DEBUG
        bool _active = false;

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                current = null;
            }
        }

        Rectangle Screen = new Rectangle(10, 10, 300, 640);
        KeyboardState olds;
        MouseState oms;

        public void Draw(SpriteBatch batch)
        {
            KeyboardState ks = Keyboard.GetState();
            if ((ks.IsKeyDown(Keys.F11)) && (olds.IsKeyUp(Keys.F11)))
            {
                Active = !Active;
                current = new TopLevelMenu();
                if (Active)
                {
                    SceneManagement.SceneManager.Instance.Paused = true;
                }
                else
                {
                    SceneManagement.SceneManager.Instance.Paused = false;
                }
            }

            if (Active)
            {
                MouseState ms = Mouse.GetState();

                batch.Begin();
                batch.Draw(AssetManager.GetWhite(), Screen, Color.FromNonPremultiplied(0, 0, 0, 192));

                foreach (Rectangle r in current.buttons)
                {
                    if (r.Contains(ms.X, ms.Y))
                        batch.Draw(AssetManager.GetWhite(), r, Color.Black);
                    else
                        batch.Draw(AssetManager.GetWhite(), r, Color.DarkGray);
                }
                for (int i = 0; i < current.text.Count; i++)
                {
                    batch.DrawString(AssetManager.GetDebugFont(), current.text[i], current.textpositions[i], Color.White);
                }
                foreach (Rectangle r in current.checkboxes)
                {
                    batch.DrawRectangle(r, Color.White);
                    Rectangle r2 = new Rectangle(r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2);
                    batch.DrawRectangle(r2, Color.Blue);
                }

                current.DrawExtra(batch);
                batch.End();

                if (ms.LeftButton == ButtonState.Released)
                {
                    if (oms != null)
                    {
                        if (oms.LeftButton == ButtonState.Pressed)
                        {
                            for (int i = 0; i < current.buttons.Count; i++)
                            {
                                if (current.buttons[i].Contains(ms.X, ms.Y))
                                {
                                    current = current.HandleEvent(i);
                                    break;
                                }
                            }

                            for (int i = 0; i < current.checkboxes.Count; i++)
                            {
                                if (current.checkboxes[i].Contains(ms.X, ms.Y))
                                {
                                    current = current.HandleEvent(i + 20);
                                    break;
                                }
                            }

                        }
                    }
                }
                oms = ms;
            }
            olds = ks;
        }
#endif
    }
}
