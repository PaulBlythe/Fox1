using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class SmallButton : Widget
    {
        String Text;
        public Rectangle Region;
        MouseState oldms;

        public SmallButton(Rectangle r, String text)
        {
            Text = text;
            Region = r;
        }

        public override void Update(float dt)
        {
            MouseState ms = Mouse.GetState();
            if (Region.Contains(ms.X, ms.Y))
            {
                if ((ms.LeftButton == ButtonState.Released) && (oldms.LeftButton == ButtonState.Pressed))
                {
                    String r = "SmallButton:" + Text;
                    if (Parent == null)
                        GuiManager.Instance.HandleEvent(r);
                    else
                        Parent.Message(r);
                }
            }
            oldms = ms;
        }

        public override void Draw(GUIBatch b)
        {
            MouseState ms = Mouse.GetState();
            {
                if (Region.Contains(ms.X, ms.Y))
                    b.FillRectangle(Region, GuiManager.Instance.Theme.HiliteColour);
                else
                    b.FillRectangle(Region, GuiManager.Instance.Theme.FillColour);
            }
        }

        public override void Draw(SpriteBatch b)
        {
        }

        public override void Draw(GuiFont b)
        {
            String t = Text;

            Vector2 w = b.MeasureString(Text,0.75f);

            if (w.X >= Region.Width)
            {
                t = Text.Substring(0, 12);
                t += "~1 ]";
                w = b.MeasureString(t, 0.75f);
            }

            w.X *= -0.5f;
            w.X += (Region.X + (Region.Width / 2));
            w.Y = (Region.Y + (Region.Height - (w.Y / 2)));
            b.DrawString(t, w, GuiManager.Instance.Theme.FontColour,0.75f);
        }

        public void Draw(GUIBatch b , Vector2 offset)
        {
            Rectangle t = Region;
            t.X += (int)offset.X;
            t.Y += (int)offset.Y;
            MouseState ms = Mouse.GetState();
            {
                if (t.Contains(ms.X, ms.Y))
                    b.FillRectangle(t, GuiManager.Instance.Theme.HiliteColour);
                else
                    b.FillRectangle(t, GuiManager.Instance.Theme.FillColour);
            }
        }

        public void Draw(GuiFont b, Vector2 offset)
        {
            String tt = Text;
            Rectangle t = Region;
            t.X += (int)offset.X;
            t.Y += (int)offset.Y;

            Vector2 w = b.MeasureString(Text, 0.75f);
            if (w.X >= Region.Width)
            {
                tt = Text.Substring(0, 12);
                tt += "~1 ]";
                w = b.MeasureString(tt, 0.75f);
            }

            w.X *= -0.5f;
            w.X += (t.X + (Region.Width / 2));
            w.Y = (t.Y + (Region.Height - (w.Y / 2)));
            b.DrawString(tt, w, GuiManager.Instance.Theme.FontColour, 0.75f);
        }

        public override void Message(string s)
        {

        }

        public void SetPosition(int x, int y)
        {
            Region.X = x;
            Region.Y = y;
        }
    }
}
