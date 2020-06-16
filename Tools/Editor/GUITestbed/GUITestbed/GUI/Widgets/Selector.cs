using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.GUI.Widgets
{
    public class Selector : Widget
    {
        Vector2 Position;
        Rectangle TextRegion;
        Vector2 ValuePos;
        String[] Choices;
        public int Selection;
        List<Widget> Children = new List<Widget>();


        public Selector(Vector2 position, String[] choices, int defaultvalue)
        {
            Choices = choices;
            Position = position;
            Selection = defaultvalue;

            Button b = new Button(new Rectangle((int)position.X, (int)position.Y, 28, 28), "-");
            b.Parent = this;
            Children.Add(b);

            b = new Button(new Rectangle(200 + (int)position.X, (int)position.Y, 28, 28), "+");
            b.Parent = this;
            Children.Add(b);

            TextRegion = new Rectangle(30 + (int)position.X, (int)position.Y, 170, 28);

            ValuePos = new Vector2(position.X + 34, position.Y + 22);

        }

        public override void Update(float dt)
        {
            foreach (Widget w in Children)
                w.Update(dt);
        }

        public override void Draw(GUIBatch b)
        {
            b.FillRectangle(TextRegion, GuiManager.Instance.Theme.BackColour);
            foreach (Widget w in Children)
                w.Draw(b);
        }

        public override void Draw(SpriteBatch b)
        {
        }

        public override void Draw(GuiFont b)
        {
            b.DrawString(Choices[Selection], ValuePos, GuiManager.Instance.Theme.AlternateFontColour);
            foreach (Widget w in Children)
                w.Draw(b);
        }
        public override void Message(string s)
        {
            switch (s)
            {
                case "Button:-":
                    if (Selection > 0)
                        Selection--;
                    break;
                case "Button:+":
                    if (Selection < Choices.Length - 1)
                        Selection++;
                    break;
            }
        }

        public String SelectedText()
        {
            return Choices[Selection];
        }
    }
}
