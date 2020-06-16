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

namespace GuruEngine.Rendering.Gui.Instruments
{
    public abstract class CockpitInstrument
    {
        public GraphicsDevice Device;

        public abstract void Update(float dt, Vector2 MousePosition, bool LeftButtonDown);
        public abstract void Render();
        public abstract Texture2D GetTexture();
        public abstract void HandleEvent(String evt);
        public abstract void LoadContent(ContentManager content);
        public abstract void Release();
    }
}
