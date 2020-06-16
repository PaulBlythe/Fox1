using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.GUI
{
    public abstract class Widget
    {
        public Widget Parent = null;
        public abstract void Update(float dt);
        public abstract void Draw(GUIBatch b);
        public abstract void Draw(GuiFont b);
        public abstract void Draw(SpriteBatch b);
        public abstract void Message(String s);
    }
}
