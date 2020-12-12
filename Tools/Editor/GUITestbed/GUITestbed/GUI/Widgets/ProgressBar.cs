using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.GUI.Widgets
{
    public class ProgressBar : Widget
    {
        public int Value = 0;
        public int Max = 100;

        int span;
        Rectangle r;

        public ProgressBar(Rectangle region)
        {
            r = region;
            span = region.Height / 2;
        }

        public override void Update(float dt)
        {

        }
        public override void Draw(SpriteBatch b)
        {
            
        }

        public override void Draw(GUIBatch b)
        {
            float delta = (float)Value / (float)Max;
            float w = r.Width * delta;
           
            if (w < span)
            {
                w = span;
            }
            if (w > r.Width - span)
                w = r.Width - span;

            b.FillArc(new Vector2(r.X + span, r.Y + (r.Height / 2)), span, 32, MathHelper.ToRadians(90), MathHelper.ToRadians(180), Color.LightSkyBlue);

            int x1 = r.X + span;
            int x2 = (int)(r.X + w);


            if (w > span)
            {
                b.FillRectangle2(new Rectangle(x1, (int)r.Y + (r.Height / 2) - (r.Height / 4),(int) (x2-x1)-2, (r.Height / 2)), Color.LightSkyBlue);
            }
            b.FillArc(new Vector2(x2, r.Y + (r.Height / 2)), span, 32, MathHelper.ToRadians(270), MathHelper.ToRadians(180), Color.LightSkyBlue);

        }

        public override void Draw(GuiFont b)
        {
        }

        public override void Message(string s)
        {

        }
    }
}
