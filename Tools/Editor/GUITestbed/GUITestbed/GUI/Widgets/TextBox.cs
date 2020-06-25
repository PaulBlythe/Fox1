using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class TextBox : Widget
    {
        public String Text;
        public Rectangle Region;
        MouseState oldms;
        KeyboardState oldks;
        bool HasFocus = false;
        float time = 0;

        public TextBox(Rectangle r, String text)
        {
            Text = text;
            Region = r;
        }

        public override void Update(float dt)
        {
            time += dt;
            MouseState ms = Mouse.GetState();
            if (Region.Contains(ms.X, ms.Y))
            {
                if ((ms.LeftButton == ButtonState.Released) && (oldms.LeftButton == ButtonState.Pressed))
                {
                    HasFocus = true;
                }
            }
            oldms = ms;

            if (HasFocus)
            {
                KeyboardState ks = Keyboard.GetState();
                Keys[] pr = ks.GetPressedKeys();
                Keys[] opr = oldks.GetPressedKeys();

                List<Keys> filtered = new List<Keys>();
                foreach(Keys ss in pr)
                {
                    switch (ss)
                    {
                        case Keys.Enter:
                        case Keys.LeftShift:
                        case Keys.RightShift:
                            filtered.Add(ss);
                            break;

                        default:
                            {
                                if (!opr.Contains<Keys>(ss))
                                {
                                    filtered.Add(ss);
                                }
                            }
                            break;
                    }
                }

                Text += GuiManager.Instance.GetString(filtered.ToArray());
                if ((filtered.Contains<Keys>(Keys.Delete))|| (filtered.Contains<Keys>(Keys.Back)))
                {
                    Text = Text.Substring(0, Text.Length - 1);
                }
                if (filtered.Contains<Keys>(Keys.Enter))
                {
                    HasFocus = false;
                }

                oldks = ks;
            }

        }

        public override void Draw(GUIBatch b)
        {
            b.FillRectangle(Region, GuiManager.Instance.Theme.BackColour);
        }

        public override void Draw(SpriteBatch b)
        {
        }

        public override void Draw(GuiFont b)
        {
            Vector2 w = b.MeasureString(Text);
            float width = w.X;

            w.X *= -0.5f;
            w.X = (Region.X + 4);
            w.Y = (Region.Y + (Region.Height - (w.Y / 2)));
            b.DrawString(Text, w, GuiManager.Instance.Theme.AlternateFontColour);

            if (HasFocus)
            {
                if (time>0.5)
                {
                    w.X += width;
                    w.X += 5;
                    w.Y -= 1;
                    b.DrawString(":", w, GuiManager.Instance.Theme.AlternateFontColour);
                }
                if (time>=1)
                {
                    time = 0;
                }
            }
        }

        public override void Message(string s)
        {

        }

        
    }
}
