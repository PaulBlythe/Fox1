using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.SceneManagement
{
    public abstract class Scene
    {
        public int ID;

        public abstract void Init();
        public abstract void Update(float dt);
        public abstract void Load(ContentManager Content);
        public abstract void Draw(GameTime gt);
    }
}
