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

namespace GuruEngineTest.Scenes
{
    public class MainMenu : Scene
    {
        GraphicsDevice device;
        Effect backdrop;
        Texture2D Noise;
        Texture2D Logo;
        SpriteBatch batch;
        SpriteFont font;
        Rectangle fs;
        Rectangle lg;
        float time = 0;
        MouseState OldMouseState;
        int TransitState = 0;
        int NextState = 0;
        float TransitTime = 0;

        List<MainMenuItem> Items = new List<MainMenuItem>();

        Vector2 lift = new Vector2(-2, -2);

        public override void Init()
        {
            device = Renderer.GetGraphicsDevice();
            batch = new SpriteBatch(device);
            fs = new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height);

            SetTopLevel();

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

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            switch (TransitState)
            {
                #region Menu on
                case 0:
                    {
                        foreach (MainMenuItem mi in Items)
                        {
                            if (mi.IsSlider)
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
                                        DoAction(mi.ID);
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
                        float t = TransitTime;

                        foreach (MainMenuItem mi in Items)
                        {
                            if (mi.IsSlider)
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
                    break;
                #endregion

                #region Menu coming on
                case 3:
                    {
                        float t = TransitTime;

                        foreach (MainMenuItem mi in Items)
                        {
                            if (mi.IsSlider)
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
            switch (TransitState)
            {
                case 1:
                    {
                        TransitTime += dt * 2;
                        if (TransitTime > 1.6)
                        {
                            TransitTime = 1;
                            TransitState++;
                        }
                    }
                    break;

                case 2:
                    {
                        switch (NextState)
                        {
                            case 0:     // Back to main menu
                                SetTopLevel();
                                break;
                            case 1:
                                SetNewGame();
                                break;
                            case 3:     // Enter top level options menu
                                SetTopOptions();
                                break;
                            case 12:
                                SetAudioOptions();
                                break;
                            case 21:
                                SetSinglePlayer();
                                break;
                        }
                        TransitState++;

                    }
                    break;

                case 3:
                    {
                        TransitTime += dt * 2;
                        if (TransitTime > 1.6)
                        {
                            TransitTime = 1;
                            TransitState = 0;
                        }
                    }
                    break;

            }
        }

        void DoAction(int i)
        {
            switch (i)
            {
                case 4:
                    {
                        Game1.GameInstance.Exit();
                    }
                    break;

                case 15:
                    {
                        TransitState++;
                        NextState = 0;
                    }
                    break;

                case 16:
                    {
                        TransitState++;
                        NextState = 3;
                    }
                    break;

                default:
                    {
                        TransitState++;
                        NextState = i;
                    }
                    break;
            }
            TransitTime = 0;
        }

        #region Menu level setup methods
        void SetTopLevel()
        {
            Items.Clear();

            MainMenuItem item1 = new MainMenuItem();
            item1.ID = 1;
            item1.Text = StringLocalizer.GetString(StringIDS.NewGame);
            item1.Position = new Vector2(50, 400);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 2;
            item1.Text = StringLocalizer.GetString(StringIDS.LoadGame);
            item1.Position = new Vector2(50, 440);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 3;
            item1.Text = StringLocalizer.GetString(StringIDS.Options);
            item1.Position = new Vector2(50, 480);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 4;
            item1.Text = StringLocalizer.GetString(StringIDS.Exit);
            item1.Position = new Vector2(50, 520);
            item1.SetRegion();
            Items.Add(item1);
        }

        void SetTopOptions()
        {
            Items.Clear();

            MainMenuItem item1 = new MainMenuItem();
            item1.ID = 11;
            item1.Text = StringLocalizer.GetString(StringIDS.Display);
            item1.Position = new Vector2(50, 400);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 12;
            item1.Text = StringLocalizer.GetString(StringIDS.Audio);
            item1.Position = new Vector2(50, 440);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 13;
            item1.Text = StringLocalizer.GetString(StringIDS.Language);
            item1.Position = new Vector2(50, 480);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 14;
            item1.Text = StringLocalizer.GetString(StringIDS.Network);
            item1.Position = new Vector2(50, 520);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 15;
            item1.Text = StringLocalizer.GetString(StringIDS.Back);
            item1.Position = new Vector2(50, 560);
            item1.SetRegion();
            Items.Add(item1);
        }

        void SetAudioOptions()
        {
            Items.Clear();

            MainMenuItem item1 = new MainMenuItem();
            item1.ID = -1;
            item1.Text = StringLocalizer.GetString(StringIDS.MusicVolume);
            item1.Position = new Vector2(50, 400);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = -1;
            item1.Text = StringLocalizer.GetString(StringIDS.SoundEffectVolume);
            item1.Position = new Vector2(50, 440);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 16;
            item1.Text = StringLocalizer.GetString(StringIDS.Back);
            item1.Position = new Vector2(50, 560);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = -2;
            item1.SetAsSlider();
            item1.Position = new Vector2(350, 400);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = -3;
            item1.SetAsSlider();
            item1.Position = new Vector2(350, 440);
            item1.SetRegion();
            Items.Add(item1);
        }

        void SetNewGame()
        {
            Items.Clear();

            MainMenuItem item1 = new MainMenuItem();
            item1.ID = 21;
            item1.Text = StringLocalizer.GetString(StringIDS.SinglePlayer);
            item1.Position = new Vector2(50, 400);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 22;
            item1.Text = StringLocalizer.GetString(StringIDS.MultiPlayer);
            item1.Position = new Vector2(50, 440);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 0;
            item1.Text = StringLocalizer.GetString(StringIDS.Back);
            item1.Position = new Vector2(50, 560);
            item1.SetRegion();
            Items.Add(item1);
        }

        void SetSinglePlayer()
        {
            Items.Clear();

            MainMenuItem item1 = new MainMenuItem();
            item1.ID = 31;
            item1.Text = StringLocalizer.GetString(StringIDS.FreeFlight);
            item1.Position = new Vector2(50, 400);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 32;
            item1.Text = StringLocalizer.GetString(StringIDS.SingleMission);
            item1.Position = new Vector2(50, 440);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 33;
            item1.Text = StringLocalizer.GetString(StringIDS.Career);
            item1.Position = new Vector2(50, 480);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 34;
            item1.Text = StringLocalizer.GetString(StringIDS.Campaign);
            item1.Position = new Vector2(50, 520);
            item1.SetRegion();
            Items.Add(item1);

            item1 = new MainMenuItem();
            item1.ID = 1;
            item1.Text = StringLocalizer.GetString(StringIDS.Back);
            item1.Position = new Vector2(50, 560);
            item1.SetRegion();
            Items.Add(item1);
        }
        #endregion
    }
}
