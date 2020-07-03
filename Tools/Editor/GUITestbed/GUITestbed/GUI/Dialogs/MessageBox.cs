using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GUITestbed.GUI.Widgets;

namespace GUITestbed.GUI.Dialogs
{
    public class MessageBox : Dialog
    {
        public MessageBox(String title, String msg)
            : base(title, new Microsoft.Xna.Framework.Rectangle((1920 - 500) / 2, (1000 - 100) / 2, 500, 100))
        {
            Vector2 centre = new Vector2(1920 / 2, 500);
            Vector2 w = GuiManager.Instance.font.MeasureString(msg);
            w.X *= -0.5f;
            w.X += centre.X;
            w.Y =  centre.Y + 30 - (w.Y / 2);
            Label l = new Label(w, msg);
            base.Children.Add(l);

        }
        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void Draw(GUIBatch b)
        {
            base.Draw(b);
        }

        public override void Draw(GuiFont b)
        {
            base.Draw(b);
        }

        public override void Message(string s)
        {
            GuiManager.Instance.DelayedRemoveTop();

            base.Message(s);
        }
    }
}
