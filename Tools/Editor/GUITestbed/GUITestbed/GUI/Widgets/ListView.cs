using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI.Widgets
{
    public class ListView : Widget
    {
        Rectangle Region;
        List<Widget> Children = new List<Widget>();
        List<Widget> ItemList = new List<Widget>();
        int skip = 0;
        MouseState oldmouseState;

        public ListView(Rectangle r)
        {
            Region = r;

            InvertedPanel ip = new InvertedPanel(r);
            Children.Add(ip);

            
        }

        public override void Update(float dt)
        {
            foreach (Widget w in Children)
                w.Update(dt);

            int i = 0;
            while(i<ItemList.Count)
            {
                ItemList[i].Update(dt);
                i++;
            }


            lock (ItemList)
            {
                int y = Region.Y + 10;
                int x = Region.X + 5;
                int limit = Region.X + Region.Width - 230;
                foreach (Widget w in ItemList)
                {
                    SmallButton sb = (SmallButton)w;
                    sb.SetPosition(x, y);
                    x += 230;
                    if (x > limit)
                    {
                        x = Region.X + 5;
                        y += 25;
                    }
                    
                }
            }
            MouseState ms = Mouse.GetState();
            if (Region.Contains(ms.X, ms.Y))
            {
                if (ms.ScrollWheelValue != oldmouseState.ScrollWheelValue)
                {
                    int dx = ms.ScrollWheelValue - oldmouseState.ScrollWheelValue;
                    if (Math.Abs(dx) > 10)
                    {
                        skip -= Math.Sign(dx);
                        skip = Math.Max(skip, 0);
                    }
                }
            }
            oldmouseState = ms;

        }

        public override void Draw(GUIBatch b)
        {
            foreach (Widget w in Children)
                w.Draw(b);

            int i = skip * 3;
            int limit = Region.Y + Region.Height - 20;
            int y = Region.Y + 5;

            Vector2 v = new Vector2(0, -25 * skip);

            while ((i<ItemList.Count)&&(y<limit))
            {
                SmallButton sb = (SmallButton)ItemList[i];

                Vector2 dl = new Vector2(sb.Region.X, sb.Region.Y);
                dl += v;

                if ((dl.Y < limit)&&(dl.Y > Region.Y))
                    sb.Draw(b,v);
                y = (int)dl.Y;
                i++;
            }

                
        }

        public override void Draw(GuiFont b)
        {
            foreach (Widget w in Children)
                w.Draw(b);

            Vector2 v = new Vector2(0, (skip * -25));
            int i = skip * 3;
            int limit = Region.Y + Region.Height - 20;
            int y = Region.Y + 5;
            while ((i < ItemList.Count) && (y < limit))
            {
                SmallButton sb = (SmallButton)ItemList[i];
                Vector2 dl = new Vector2(sb.Region.X, sb.Region.Y);
                dl += v;

                if ((dl.Y < limit) && (dl.Y > Region.Y))
                    sb.Draw(b,v);
                y = (int)dl.Y;
                i++;
            }
        }

        public override void Message(string s)
        {
            if (Parent != null)
                Parent.Message(s);
            else
            {
                OnClickEvent(s);
            }
        }

        public void Clear()
        {
            lock (ItemList)
            {
                ItemList.Clear();
            }
        }

        public void AddItem(String name)
        {
            lock (ItemList)
            {
                SmallButton sm = new SmallButton(new Rectangle(Region.X + 20, Region.Y, 225, 20), name);
                sm.Parent = this;
                ItemList.Add(sm);
            }
        }

        public override void Draw(SpriteBatch b)
        {
        }
    }
}
