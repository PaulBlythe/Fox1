using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public struct ToolBarButton
    {
        public String Text;
        public Rectangle Region;

    }
    public class ToolBar : Widget
    {
        public List<ToolBarButton> buttons = new List<ToolBarButton>();
        MouseState oldstate;

        public void AddButton(String text, int Width)
        {
            ToolBarButton b = new ToolBarButton();
            b.Region = new Rectangle(GetWidth(), 0, Width, 30);
            b.Text = text;
            buttons.Add(b);
        }

        public void AddCloseButton()
        {
            int w = GuiManager.Instance.Region.Width;

            ToolBarButton b = new ToolBarButton();
            b.Region = new Rectangle(w-32, 0, 32, 30);
            b.Text = "X";
            buttons.Add(b);
        }

        public void Finalise()
        {
            int x = GetWidth();
            int w = GuiManager.Instance.Region.Width - x;
            w -= 32;

            ToolBarButton b = new ToolBarButton();
            b.Region = new Rectangle(x, 0, w, 30);
            b.Text = "";
            buttons.Add(b);
        }

        public override void Update(float dt)
        {
            MouseState ms = Mouse.GetState();
            if ((ms.LeftButton == ButtonState.Released) && (oldstate.LeftButton == ButtonState.Pressed))
            {
                foreach (ToolBarButton tb in buttons)
                {
                    if (tb.Region.Contains(ms.X, ms.Y))
                    {
                        GuiManager.Instance.HandleEvent("Toolbar:" + tb.Text);
                    }
                }
            }
            oldstate = ms;
        }

        public override void Draw(GUIBatch b)
        {
            MouseState ms = Mouse.GetState();

            foreach(ToolBarButton tb in buttons)
            {
                if ((tb.Region.Contains(ms.X,ms.Y)) && (tb.Text!=""))
                    b.FillRectangle(tb.Region, GuiManager.Instance.Theme.HiliteColour);
                else
                   b.FillRectangle(tb.Region, GuiManager.Instance.Theme.FillColour);
            }
        }

        public override void Draw(GuiFont b)
        {
            foreach (ToolBarButton tb in buttons)
            {
                Vector2 w = b.MeasureString(tb.Text);
                w.X *= -0.5f;
                w.X += (tb.Region.X + (tb.Region.Width / 2));
                w.Y = (tb.Region.Y + (tb.Region.Height - (w.Y/2)));
                b.DrawString(tb.Text, w, GuiManager.Instance.Theme.FontColour);

            }
        }

        public override void Draw(SpriteBatch b)
        {
            
        }

        private int GetWidth()
        {
            int res = 0;
            foreach(ToolBarButton tb in buttons)
            {
                if (tb.Text != "X")
                    res += tb.Region.Width;
            }
            return res;
        }

        public override void Message(string s)
        {

        }
    }
}
