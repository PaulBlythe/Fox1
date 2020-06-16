using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class GroupBox : Widget
    {
        Rectangle region;
        String Name;

        public GroupBox(String name, Rectangle r)
        {
            region = r;
            Name = name;
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch b)
        {
            Color col = GuiManager.Instance.Theme.GroupColour;

            b.DrawArc(new Vector2(region.X + 16, region.Y + 16), 16, 16, MathHelper.ToRadians(180), MathHelper.ToRadians(90), col, 1);
            b.DrawArc(new Vector2(region.X + 16, region.Y + region.Height - 16), 16, 16, MathHelper.ToRadians(90), MathHelper.ToRadians(90), col, 1);
            b.DrawArc(new Vector2(region.X + region.Width - 16, region.Y + 16), 16, 16, MathHelper.ToRadians(270), MathHelper.ToRadians(90), col, 1);
            b.DrawArc(new Vector2(region.X + region.Width - 16, region.Y + region.Height - 16), 16, 16, MathHelper.ToRadians(0), MathHelper.ToRadians(90), col, 1);

            b.DrawLine(new Vector2(region.X, region.Y + 16), new Vector2(region.X, region.Y + region.Height - 16), col, 1);
            b.DrawLine(new Vector2(region.X + 16, region.Y), new Vector2(region.X + 26, region.Y ), col, 1);
            b.DrawLine(new Vector2(region.X + 16, region.Y + region.Height), new Vector2(region.X + region.Width - 16, region.Y + region.Height), col, 1);
            b.DrawLine(new Vector2(region.X + region.Width, region.Y + region.Height - 16), new Vector2(region.X + region.Width, region.Y + 16), col, 1);
            b.DrawLine(new Vector2(region.X + 126, region.Y), new Vector2(region.X + region.Width - 16, region.Y), col, 1);
        }

        public override void Draw(GUIBatch b)
        {
        }

        public override void Draw(GuiFont b)
        {
            b.DrawString(Name, new Vector2(region.X + 28, region.Y + 10), GuiManager.Instance.Theme.GroupColour);
        }
        public override void Message(string s)
        {

        }
    }
}
