using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class StatusBar:Widget
    {
        Rectangle Region;
        FrameCounter counter;

        public String mode = "";

        public StatusBar()
        {
            Region = new Rectangle(0, GuiManager.Instance.Region.Bottom - 30, GuiManager.Instance.Region.Width, 30);
            counter = new FrameCounter();
        }

        public override void Update(float dt)
        {
            counter.Update(dt);
        }

        public override void Draw(SpriteBatch b)
        {

        }

        public override void Draw(GUIBatch b)
        {
            b.FillRectangle(Region, GuiManager.Instance.Theme.FillColour);
        }

        public override void Draw(GuiFont b)
        {
            String fps = string.Format("FPS: {0}", counter.AverageFramesPerSecond);
            b.DrawString(fps, new Vector2(3, Region.Y + 20), GuiManager.Instance.Theme.FontColour);

            b.DrawString(mode, new Vector2(128,Region.Y+20), GuiManager.Instance.Theme.FontColour);
        }
        public override void Message(string s)
        {

        }
    }
}
