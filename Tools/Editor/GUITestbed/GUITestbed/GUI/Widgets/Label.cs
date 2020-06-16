using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.GUI.Widgets
{
    public class Label : Widget
    {
        String Text;
        Vector2 Location;


        public Label(Vector2 pos, String s)
        {
            Text = s;
            Location = pos;
        }

        public override void Update(float dt)
        {
            
        }

        public override void Draw(GUIBatch b)
        {
            
        }

        public override void Draw(GuiFont b)
        {
            b.DrawString(Text, Location, GuiManager.Instance.Theme.FontColour);
        }

        public override void Message(string s)
        {

        }

        public override void Draw(SpriteBatch b)
        {
        }
    }
}
