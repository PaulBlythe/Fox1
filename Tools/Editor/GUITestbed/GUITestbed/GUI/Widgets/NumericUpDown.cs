using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.GUI.Widgets
{
    public class NumericUpDown : Widget
    {
        Vector2 Position;
        float MinValue;
        float MaxValue;
        public float Value;
        Rectangle TextRegion;
        Vector2 ValuePos;
        float step;

        List<Widget> Children = new List<Widget>();

        public NumericUpDown(Vector2 position, float min, float max, float value, float step)
        {
            MinValue = min;
            MaxValue = max;
            Value = value;
            Position = position;

            Button b = new Button(new Rectangle((int)position.X, (int)position.Y, 28, 28), "-");
            b.Parent = this;
            Children.Add(b);

            b = new Button(new Rectangle(200 + (int)position.X, (int)position.Y, 28, 28), "+");
            b.Parent = this;
            Children.Add(b);

            TextRegion = new Rectangle(30 + (int)position.X, (int)position.Y, 170, 28);

            ValuePos = new Vector2(position.X + 34, position.Y + 22);

            this.step = step;
        }

        public NumericUpDown(Vector2 position, float min, float max, float value)
        {
            MinValue = min;
            MaxValue = max;
            Value = value;
            Position = position;

            Button b = new Button(new Rectangle((int)position.X, (int)position.Y, 28, 28), "-");
            b.Parent = this;
            Children.Add(b);

            b = new Button(new Rectangle(200+(int)position.X, (int)position.Y, 28, 28), "+");
            b.Parent = this;
            Children.Add(b);

            TextRegion = new Rectangle(30 + (int)position.X, (int)position.Y, 170, 28);

            ValuePos = new Vector2(position.X + 34, position.Y + 22);

            step = (MaxValue - MinValue) / 100;
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

            b.DrawString(String.Format("{0:0.000}",Value), ValuePos, GuiManager.Instance.Theme.AlternateFontColour);
            foreach (Widget w in Children)
                w.Draw(b);
        }
        public override void Message(string s)
        {
            switch (s)
            {
                case "Button:-":
                    Value -= step;
                    break;
                case "Button:+":
                    Value += step;
                    break;
            }
        }
    }
}
