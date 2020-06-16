using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class InvertedPanel : Widget
    {
        Rectangle region;

        public InvertedPanel(Rectangle r)
        {
            region = r;
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(GUIBatch b)
        {
            b.FillRectangle(region, GuiManager.Instance.Theme.AlternateColour);
        }

        public override void Draw(GuiFont b)
        {

        }
        public override void Message(string s)
        {

        }

        public override void Draw(SpriteBatch b)
        {
        }
    }
}
