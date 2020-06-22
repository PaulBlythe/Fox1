using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Rendering;

namespace GuruEngineTest.Scenes.Generic.Helpers
{
    public abstract class MainMenuWidget
    {
        public Rectangle Region;
        public Vector2 Position;
        public bool Over;
        public bool OwnerDrawn;
        public int ID;
        public String Text;

        public abstract void Check(int x, int y, bool pressed);
        public abstract void SetRegion();
        public abstract void Draw(SpriteBatch batch, int x);

    }
}
