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
using GuruEngine;

namespace GuruEngineTest.Scenes.Debug
{
    public class DebugSceneSelection : Scene
    {
        GraphicsDevice device;
        Effect backdrop;
        SpriteBatch batch;
        Rectangle fs;
        Rectangle lg;
        Texture2D Noise;
        Texture2D Logo;
        float time = 0;
        SpriteFont font;
        MouseState ms;
        MouseState oldms;
        bool press = false;

        String[] Sections = new string[]
        {
            "Game",
            "Debug",
            "Tools",
        };

        String[] GameEntries = new string[]
        {
            "Main menu",
            "RAF pilot record",
            "Loading screen",
        };

        String[] DebugEntries = new string[]
        {
            "Carrier test",
            "Aircraft physics",
            "Object tester",
        };

        String[] TestEntries = new string[]
        {
            "Particle editor",
        };

        List<Rectangle> Buttons = new List<Rectangle>();
        List<String> Texts = new List<string>();

        List<Rectangle> ActiveButtons = new List<Rectangle>();
        List<String> ActiveTexts = new List<string>();

        public DebugSceneSelection()
        {
            ID = "DebugSceneSelection".GetHashCode();
        }

        public override void Init()
        {
            device = Renderer.GetGraphicsDevice();
            fs = new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height);
            batch = new SpriteBatch(device);
            lg = new Rectangle(10, 100, 256, 256);
            Setup(0);
        }

        public override void Load(ContentManager Content)
        {
            backdrop = Content.Load<Effect>(@"Shaders\2D\Loading");
            backdrop.Parameters["aspectratio"].SetValue(device.Viewport.AspectRatio);
            backdrop.Parameters["iResolution"].SetValue(new Vector2(device.Viewport.Width, device.Viewport.Height));
            Noise = Content.Load<Texture2D>(@"Textures\noise");
            Logo = Content.Load<Texture2D>(@"Textures\SGG");
            font = Content.Load<SpriteFont>(@"Fonts\MainMenu");
        }

        public override void Update(float dt)
        {
            time += dt;
            backdrop.Parameters["iTime"].SetValue(time);
           
            ms = Mouse.GetState();

            if ((oldms.LeftButton == ButtonState.Pressed) && (ms.LeftButton == ButtonState.Released))
            {
                press = true;
            }
            oldms = ms;
        }

        public override void Draw(GameTime gt)
        {
            batch.Begin(0, BlendState.Opaque, null, null, null, backdrop);
            batch.Draw(Noise, fs, Color.White);
            batch.End();

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            batch.Draw(Logo, lg, Color.White);
            int i = 0;
            foreach(Rectangle r in Buttons)
            {
                if (r.Contains(ms.X, ms.Y))
                {
                    batch.FillRectangle(r, Color.LightSlateGray);
                    if (press)
                    {
                        SubsectionSetup(i);
                        press = false;
                    }
                }
                else
                    batch.FillRectangle(r, Color.DarkSlateGray);

                batch.DrawRectangle(r, Color.White);
                Vector2 sz = font.MeasureString(Texts[i]);
                sz *= -0.5f;
                sz.X = r.X + (r.Width / 2) + sz.X;
                sz.Y = r.Y + (r.Height / 2) + sz.Y + 5;
                batch.DrawString(font, Texts[i], sz, Color.White);

                i++;
            }

            i = 0;
            foreach (Rectangle r in ActiveButtons)
            {
                if (r.Contains(ms.X, ms.Y))
                {
                    batch.FillRectangle(r, Color.LightSlateGray);
                    if (press)
                    {
                        press = false;
                        Game1.GameInstance.nextscene = ActiveTexts[i];
                    }
                }
                else
                    batch.FillRectangle(r, Color.DarkSlateGray);

                batch.DrawRectangle(r, Color.White);
                Vector2 sz = font.MeasureString(ActiveTexts[i]);
                sz *= -0.5f;
                sz.X = r.X + (r.Width / 2) + sz.X;
                sz.Y = r.Y + (r.Height / 2) + sz.Y + 5;
                batch.DrawString(font, ActiveTexts[i], sz, Color.White);

                i++;
            }
            batch.End();
        }

        void Setup(int c)
        {
            Buttons.Clear();
            Texts.Clear();

            int count = 0;
            switch (c)
            {

                case 0:
                    {
                        count = 3;
                        Texts.Add(Sections[0]);
                        Texts.Add(Sections[1]);
                        Texts.Add(Sections[2]);
                    }
                    break;
            }

            int step = device.Viewport.Width - (lg.Width + lg.X) - (300 * count);
            step /= (count + 1);
            int x = lg.Width + lg.X + step;
            Rectangle r = new Rectangle(x, lg.Y + 100, 300, 40);
            Buttons.Add(r);
            int i = 1;
            while (i<count)
            {
                x += step;
                x += 300;
                r = new Rectangle(x, lg.Y + 100, 300, 40);
                Buttons.Add(r);
                i++;
            }
        }

        void SubsectionSetup(int c)
        {
            ActiveButtons.Clear();
            ActiveTexts.Clear();
            press = false;
            String[] tt = new string[0];
            switch (c)
            {
                case 0:
                    {
                        tt = GameEntries;
                    }
                    break;

                case 1:
                    {
                        tt = DebugEntries;
                    }
                    break;

                case 2:
                    {
                        tt = TestEntries;
                    }
                    break;
            }

            int step = device.Viewport.Width - (lg.Width + lg.X) - (250 * tt.Length);
            step /= (tt.Length + 1);
            int x = lg.Width + lg.X + step;
            int y = lg.Y + 200;
            for (int i=0;  i < tt.Length; i++)
            {
                Rectangle r = new Rectangle(x, y, 250, 35);
                x += step;
                x += 250;
                if (x > device.Viewport.Width - 250)
                {
                    x = lg.Width + lg.X + step;
                    y += 50;
                }
                ActiveButtons.Add(r);
                ActiveTexts.Add(tt[i]);
            }
        }
    }
}
