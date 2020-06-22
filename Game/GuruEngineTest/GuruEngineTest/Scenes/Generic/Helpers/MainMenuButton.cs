using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Rendering;

namespace GuruEngineTest.Scenes.Generic.Helpers
{
    public class MainMenuButton: MainMenuWidget
    {
        Rectangle r1;
        Rectangle r2;

        public MainMenuButton(int id, String t, Vector2 pos)
        {
            OwnerDrawn = false;
            ID = id;
            Text = t;
            Position = pos;
            SetRegion();
        }

        public override void SetRegion()
        {
            Region = new Rectangle();
            Region.X = (int)Position.X - 3;
            Region.Y = (int)Position.Y - 3;
            Region.Width = 250;
            Region.Height = 36;
        }

        public override void Check(int x, int y, bool pressed)
        {
            Over = (Region.Contains(x, y));
        }

        public override void Draw(SpriteBatch batch, int x)
        {
           
        }
    }
}
