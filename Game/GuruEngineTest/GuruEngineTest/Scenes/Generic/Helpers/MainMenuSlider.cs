using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Rendering;
using GuruEngine.Audio;

namespace GuruEngineTest.Scenes.Generic.Helpers
{
    public class MainMenuSlider:MainMenuWidget
    {
        Rectangle r1;
        Rectangle r2;

        int grab_x = 0;

        bool grabbed = false;

        public MainMenuSlider(int id, String t, Vector2 pos)
        {
            ID = id;
            Text = t;
            Position = pos;
            OwnerDrawn = true;
            SetRegion();
        }

        public override void Check(int x, int y, bool pressed)
        {
            if (grabbed)
            {
                if (pressed)
                {
                    float v = (x - Region.X) / 380.0f;
                    switch (ID)
                    {
                        case -2:
                            AudioManager.SetMusicVolume(v);
                            break;
                        case -3:
                            AudioManager.SetSfxVolume(v);
                            break;
                    }
                }
                else
                {
                    grabbed = false;
                    if (ID == -3)
                        AudioManager.PlaySFXOnce("SOS");
                }
            }
            else
            {

                if ((r2.Contains(x, y)) && (pressed))
                {
                    grabbed = true;
                    grab_x = x;
                }
                else
                {
                    grabbed = false;
                }
            }
        }

        public override void Draw(SpriteBatch batch, int x)
        {
            r1 = Region;
            r1.X += x;
            batch.FillRectangle(r1, Color.DarkSlateGray);
            batch.DrawRectangle(r1, Color.White);

            r1.X += 2;
            r1.Width -= 4;
            r1.Y += 2;
            r1.Height -= 4;
            batch.DrawRectangle(r1, Color.Gray);

            float t = 1;

            switch (ID)
            {
                case -2:        // Music volume
                    {
                        t = AudioManager.GetMusicVolume();
                    }
                    break;
                case -3:
                    {
                        t = AudioManager.GetSFXVolume();
                    }
                    break;
            }

            r1.X = Region.X + x + (int)(t * 380);
            r1.Width = 20;
            batch.FillRectangle(r1, Color.White);

            r2 = r1;

            r1.X++;
            r1.Width -= 3;
            r1.Y++;
            r1.Height -= 3;
            batch.DrawRectangle(r1, Color.DarkGray);

        }

        public override void SetRegion()
        {
            Region = new Rectangle();
            Region.X = (int)Position.X - 3;
            Region.Y = (int)Position.Y - 3;
            Region.Height = 36;
            Region.Width = 400;
        }
    }
}
