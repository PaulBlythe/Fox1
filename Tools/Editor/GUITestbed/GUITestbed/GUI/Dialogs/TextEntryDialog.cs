using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GUITestbed.GUI.Widgets;


namespace GUITestbed.GUI.Dialogs
{
    public class TextEntryDialog : Dialog
    {
        CallBack resultHandler;
        TextBox t;

        public TextEntryDialog(String title, CallBack caller)
            : base(title, new Microsoft.Xna.Framework.Rectangle((1920 - 500) / 2, (1000 - 100) / 2, 500, 130))
        {
            Vector2 centre = new Vector2(1920 / 2, 500);

            t = new TextBox(new Rectangle((int)centre.X - 210, (int)centre.Y - 10, 420, 30), "");
            base.Children.Add(t);

            Button b = new Button(new Rectangle((int)centre.X - 240, (int)centre.Y + 35, 100, 30), "Cancel");
            base.Children.Add(b);
            b = new Button(new Rectangle((int)centre.X + 140, (int)centre.Y + 35, 100, 30), "OK");
            base.Children.Add(b);

            resultHandler = caller;
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
            if (s == "Button:OK")
            {
                resultHandler(t.Text);
                
            }else if(s == "Button:Cancel")
            {
                resultHandler(null);
            }
            base.Message(s);
        }
    }
}
