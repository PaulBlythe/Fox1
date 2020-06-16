using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class CheckBox:Widget
    {
        Rectangle Region;
        Rectangle Inner;
        public bool Selected;
        MouseState olsd;
        Vector2 spos;

        public CheckBox(Vector2 location, bool selected)
        {
            Region = new Rectangle((int)location.X, (int)location.Y, 32, 32);
            Inner = new Rectangle(Region.X + 2, Region.Y + 2, 28, 28);
            Selected = selected;
            spos = new Vector2(Region.X + 10, Region.Y + 24);
        }
        public override void Update(float dt)
        {
            MouseState ms = Mouse.GetState();
            if (Region.Contains(ms.X,ms.Y))
            {
                if (ms.LeftButton == ButtonState.Released)
                {
                    if (olsd.LeftButton == ButtonState.Pressed)
                    {
                        Selected = !Selected;
                    }
                }
            }

            olsd = ms;
        }

        public override void Draw(GUIBatch b)
        {
            if (!Selected)
            {
                b.FillRectangle(Region, GuiManager.Instance.Theme.BackColour);
                b.FillRectangle(Inner, GuiManager.Instance.Theme.FillColour);
            }
            else
            {
                b.FillRectangle(Region, GuiManager.Instance.Theme.FillColour);
                b.FillRectangle(Inner, GuiManager.Instance.Theme.BackColour);
            }

        }

        public override void Draw(GuiFont b)
        {
            if (Selected)
            {
                b.DrawString("X", spos, GuiManager.Instance.Theme.AlternateFontColour);
            }
        }

        public override void Message(string s)
        {
            
        }

        public override void Draw(SpriteBatch b)
        {
        }

    }
}
