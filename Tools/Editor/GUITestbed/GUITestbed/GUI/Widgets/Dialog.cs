using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class Dialog:Widget
    {
        public String Name;
        public List<Widget> Children = new List<Widget>();
        Rectangle Region;

        public Dialog(String title, Rectangle r)
        {
            Region = r;
            Name = title;
            Panel p = new Panel(r);
            Children.Add(p);

            Rectangle top = new Rectangle(r.X, r.Y, r.Width, 32);
            p = new Panel(top);
            Children.Add(p);

            Button b = new Button(new Rectangle(r.X + r.Width - 30, r.Y, 30, 32), "X");
            Children.Add(b);

            Label l = new Label(new Vector2(r.X + 2, r.Y + 24),title);
            Children.Add(l);
        }

        public override void Update(float dt)
        {
            foreach (Widget w in Children)
                w.Update(dt);
        }

        public override void Draw(GUIBatch b)
        {
            foreach (Widget w in Children)
                w.Draw(b);
        }

        public override void Draw(GuiFont b)
        {
            foreach (Widget w in Children)
                w.Draw(b);
        }

        public override void Message(string s)
        {

        }

        public override void Draw(SpriteBatch b)
        {
            foreach (Widget w in Children)
                w.Draw(b);
        }
    }
}
