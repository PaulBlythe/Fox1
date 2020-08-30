using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GUITestbed.GUI.Widgets
{
    public class Slider : Widget
    {
        public float Value;
        Rectangle Region;
        Rectangle tag;
        public String ID;
        bool Grabbed = false;
        float minvalue = 0;
        float maxvalue = 1;

        public Slider(Rectangle r, float v, float min, float max, String id)
        {
            Region = r;
            Value = v;
            UpdateTag();
            ID = id;
            minvalue = min;
            maxvalue = max;
        }

        public Slider(Rectangle r, float v, String id)
        {
            Region = r;
            Value = v;
            UpdateTag();
            ID = id;
        }

        public override void Update(float dt)
        {
            float v = Value;

            MouseState ms = Mouse.GetState();
            if (tag.Contains(ms.X, ms.Y))
            {
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    Grabbed = true;
                }
            }
            if (Grabbed)
            {
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    float dw = Region.Width - 8;
                    Value = (float)(ms.X - Region.X) / dw;
                    Value *= (maxvalue - minvalue);
                    Value += minvalue;
                    Value = Math.Max(minvalue, Value);
                    Value = Math.Min(maxvalue, Value);
                   
                }
                else
                {
                    Grabbed = false;
                }
            }

            UpdateTag();

            if (Value!=v)
            {
                String r = ID;
                if (Parent == null)
                {
                    GuiManager.Instance.HandleEvent(r);
                    OnClickEvent(r);
                }
                else
                    Parent.Message(r);
            }
        }

        public override void Draw(GUIBatch b)
        {
            b.FillRectangle(Region, GuiManager.Instance.Theme.HiliteColour);
            b.FillRectangle(tag, GuiManager.Instance.Theme.FillColour);
        }

        public override void Draw(GuiFont b)
        {
           
        }

        public override void Draw(SpriteBatch b)
        {
            b.DrawRectangle(tag, Color.White);
        }

        public override void Message(string s)
        {
            
        }

        private void UpdateTag()
        {
            float delta = (Value - minvalue) / (maxvalue - minvalue);
          
            float c = (delta * Region.Width);

            float left = c - 8;
            if (left < 0)
                left = 0;

            float right = left + 16;
            if (right >= Region.X + Region.Width)
            {
                left = Region.Width - 16;
            }

            tag.X = (int)left + Region.X;
            tag.Y = Region.Y;
            tag.Width = 16;
            tag.Height = Region.Height + 1;
        }
    }
}
