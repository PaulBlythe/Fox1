using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class SideBar:Widget
    {
        public GuiItem Host;
        public String Name;
        public List<Widget> Children = new List<Widget>();
        Rectangle Region;


        public SideBar(String title, Rectangle r)
        {
            Region = r;
            Name = title;
            Panel p = new Panel(r);
            Children.Add(p);

            Rectangle top = new Rectangle(r.X, r.Y, r.Width, 32);
            p = new Panel(top);
            Children.Add(p);

            Label l = new Label(new Vector2(r.X + 6, r.Y + 24), title);
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

        public override void Draw(SpriteBatch b)
        {
        }

        public override void Draw(GuiFont b)
        {
            foreach (Widget w in Children)
                w.Draw(b);
        }

        public override void Message(string s)
        {
            Host.HandleEvent(Name + ":" + s);
        }
    }
}
