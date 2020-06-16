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

namespace GuruEngineTest.Scenes.Helpers
{
    public class MainMenuItem
    {
        public String Text;
        public int ID;
        public Rectangle Region;
        public Vector2 Position;
        public bool Over;
        public bool IsSlider;
        Rectangle r1;
        Rectangle r2;
        bool grabbed = false;
        int grab_x = 0;

        public MainMenuItem()
        {
            IsSlider = false;
        }

        public void SetAsSlider()
        {
            IsSlider = true;

        }

        public void SetRegion()
        {
            Region = new Rectangle();
            Region.X = (int)Position.X - 3;
            Region.Y = (int)Position.Y - 3;
            Region.Width = 250;
            Region.Height = 36;
            if (IsSlider)
                Region.Width = 400;
        }

        public void Check(int x, int y, bool pressed)
        {
            if (IsSlider)
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
            Over = (Region.Contains(x, y));

        }

        public void Draw(SpriteBatch batch, int x)
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
    }
}
