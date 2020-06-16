using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GUITestbed.GUI.Widgets;

namespace GUITestbed.GUI
{
    public abstract class GuiItem
    {
        public List<Widget> Widgets = new List<Widget>();

        public void Update(float dt)
        {
            lock (Widgets)
            {
                foreach (Widget w in Widgets)
                    w.Update(dt);
            }
        }
        public void Draw(SpriteBatch b)
        {
            lock (Widgets)
            {
                foreach (Widget w in Widgets)
                    w.Draw(b);
            }
        }
        public void Draw(GUIBatch b)
        {
            lock (Widgets)
            {
                foreach (Widget w in Widgets)
                    w.Draw(b);
            }
        }
        public void Draw(GuiFont b)
        {
            lock (Widgets)
            {
                foreach (Widget w in Widgets)
                    w.Draw(b);
            }
        }

        public ListView FindFirstListView()
        {
            foreach (Widget w in Widgets)
            {
                if (w is ListView)
                    return w as ListView;
            }
            return null;
        }

        public ListView FindListView(int i)
        {
            foreach (Widget w in Widgets)
            {
                if (w is ListView)
                {
                    i--;
                    if (i == 0)
                        return w as ListView;
                }
            }
            return null;
        }

        public abstract void HandleEvent(String s);
        
    }
}
