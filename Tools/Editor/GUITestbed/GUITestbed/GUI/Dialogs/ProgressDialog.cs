using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GUITestbed.GUI.Widgets;

namespace GUITestbed.GUI.Dialogs
{
    public class ProgressDialog:Dialog
    {
        public static ProgressDialog Instance;
        public int Value = 0;
        public int Max = 1000;
        ProgressBar pb;

        public ProgressDialog()
            : base("Progress", new Rectangle((1920 - 500) / 2, (1000 - 100) / 2, 500, 100))
        {
            Instance = this;
            int x = (1920 - 500) / 2;
            int y = (1000 - 100) / 2;

            Rectangle bar = new Rectangle(x + 40, y + 50, 420, 40);
            pb = new ProgressBar(bar);
            pb.Max = Max;
            base.Children.Add(pb);

        }

        public override void Update(float dt)
        {
            Value = (Value + 1) % Max;
            pb.Value = Value;

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
