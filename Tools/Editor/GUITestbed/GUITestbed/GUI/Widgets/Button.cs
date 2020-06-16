using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class Button : Widget
    {
        String Text;
        Rectangle Region;
        MouseState oldms;

        public Button(Rectangle r, String text)
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
                    String r = "Button:" + Text;
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

        public override void Draw(GuiFont b)
        {
            Vector2 w = b.MeasureString(Text);
            w.X *= -0.5f;
            w.X += (Region.X + (Region.Width / 2));
            w.Y = (Region.Y + (Region.Height - (w.Y / 2)));
            b.DrawString(Text, w, GuiManager.Instance.Theme.FontColour);
        }

        public override void Message(string s)
        {
            
        }

        public override void Draw(SpriteBatch b)
        {
        }
    }
}
