using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GuruEngine.SceneManagement;
using GuruEngine.Rendering;
using GuruEngine.Localization.Strings;
using GuruEngineTest.Scenes.Helpers;
using GuruEngine.Audio;
using GuruEngineTest.Scenes.Generic.MenuPages;
using GuruEngineTest.Scenes.Generic.Helpers;

namespace GuruEngineTest.Scenes
{
    public class MainMenu : Scene
    {
        public static MainMenu Instance;

        GraphicsDevice device;
        Effect backdrop;
        Texture2D Noise;
        Texture2D Logo;
        SpriteBatch batch;
        public SpriteFont font;
        Rectangle fs;
        Rectangle lg;
        float time = 0;
        MouseState OldMouseState;
        int TransitState = 0;
        float TransitTime = 0;

        MenuPage currentPage;
        MenuPage nextPage;

        Vector2 lift = new Vector2(-2, -2);

        public override void Init()
        {
            ID = "MainMenu".GetHashCode();
            Instance = this;
            device = Renderer.GetGraphicsDevice();
            batch = new SpriteBatch(device);
            fs = new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height);

            currentPage = new MainPage();

            lg = new Rectangle(10, 100, 256, 256);
            AudioManager.PlaySong(0);
            AudioManager.RegisterSoundEffect("SOS", @"SFX/sos");
            AudioManager.RegisterSoundEffect("CLICK", @"SFX/Click");
        }

        public override void Load(ContentManager Content)
        {
            Noise = Content.Load<Texture2D>(@"Textures\noise");
            backdrop = Content.Load<Effect>(@"Shaders\2D\Loading");
            font = Content.Load<SpriteFont>(@"Fonts\MainMenu");
            Logo = Content.Load<Texture2D>(@"Textures\SGG");

            backdrop.Parameters["aspectratio"].SetValue(device.Viewport.AspectRatio);
            backdrop.Parameters["iResolution"].SetValue(new Vector2(device.Viewport.Width, device.Viewport.Height));
        }

        public override void Draw(GameTime gt)
        {
            batch.Begin(0, BlendState.Opaque, null, null, null, backdrop);
            batch.Draw(Noise, fs, Color.White);
            batch.End();

            MouseState ms = Mouse.GetState();
            float dt = gt.ElapsedGameTime.Milliseconds / 1000.0f;

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            switch (TransitState)
            {
                #region Menu on
                case 0:
                    {
                        foreach (MainMenuWidget mi in currentPage.Items)
                        {
                            if (mi.OwnerDrawn)
                            {
                                mi.Check(ms.X, ms.Y, ms.LeftButton == ButtonState.Pressed);
                                mi.Draw(batch, 0);
                            }
                            else
                            {
                                if ((mi.Region.Contains(ms.X, ms.Y)) && (mi.ID >= 0))
                                {
                                    batch.DrawString(font, mi.Text, mi.Position + lift, Color.Firebrick);
                                    if ((ms.LeftButton == ButtonState.Released) && (OldMouseState.LeftButton == ButtonState.Pressed))
                                    {
                                        TransitState = currentPage.DoAction(mi.ID, out nextPage);
                                        if (nextPage != null)
                                            TransitTime = 0.0f;
                                        AudioManager.PlaySFXOnce("CLICK");
                                    }
                                }
                                else
                                {
                                    batch.DrawString(font, mi.Text, mi.Position, Color.Black);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Menu going off
                case 1:
                    {
                        TransitTime += dt * 2;
                        if (TransitTime > 1.6)
                        {
                            TransitTime = 1;
                            TransitState++;
                        }
                        else
                        {
                            float t = TransitTime;
                            foreach (MainMenuWidget mi in currentPage.Items)
                            {
                                if (mi.OwnerDrawn)
                                {
                                    int x = (int)MathHelper.SmoothStep(0, 2000, t);
                                    mi.Draw(batch, x);
                                }
                                else
                                {
                                    Vector2 delta = new Vector2(MathHelper.SmoothStep(0, -600, t), 0);
                                    batch.DrawString(font, mi.Text, mi.Position + delta, Color.Black);
                                }
                                t -= 0.1f;

                            }
                        }
                    }
                    break;
                #endregion

                case 2:
                    {
                        currentPage = nextPage;
                        TransitState++;
                        TransitTime = 0;

                    }
                    break;

                #region Menu coming on
                case 3:
                    {
                        float t = TransitTime;
                        TransitTime += dt * 2;
                        if (TransitTime > 1.6)
                        {
                            TransitState = 0;
                            TransitTime = 1;
                        }
                        foreach (MainMenuWidget mi in currentPage.Items)
                        {
                            if (mi.OwnerDrawn)
                            {
                                int x = (int)MathHelper.SmoothStep(2000, 0, t);
                                mi.Draw(batch, x);
                            }
                            else
                            {
                                Vector2 delta = new Vector2(MathHelper.SmoothStep(-600, 0, t), 0);
                                batch.DrawString(font, mi.Text, mi.Position + delta, Color.Black);
                            }
                            t -= 0.1f;
                        }
                    }
                    break;
                    #endregion

            }

            batch.Draw(Logo, lg, Color.White);
            batch.End();

            OldMouseState = ms;
        }

        public override void Update(float dt)
        {
            time += dt;
            backdrop.Parameters["iTime"].SetValue(time);
            
        }

    }
}
