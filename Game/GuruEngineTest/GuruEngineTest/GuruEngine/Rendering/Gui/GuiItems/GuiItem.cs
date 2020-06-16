using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.Text;

namespace GuruEngine.Rendering.Gui.GuiItems
{
    public abstract class GuiItem
    {
        public abstract void Load();
        public abstract void Update(Vector2 mousePosition, int mouseButtons, float dt, int wheelDelta);
        public abstract void Draw(GlyphBatch batch);
        public abstract void Position(Vector4 position);

        public enum DrawOrder
        {
            First,
            Last,
            Both
        }
        public DrawOrder drawOrder;

        public enum GuiWidgetIdentifiers
        {
            Frame,
            Label,
            Button,
            SmallButton,
            Total
        }
    }
}
