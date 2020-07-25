using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace GuruEngine.SceneManagement
{
    public class SceneManager
    {
        public static SceneManager Instance;
        Scene current = null;
        public ContentManager content;
        public bool Paused = false;

        public SceneManager(ContentManager Content)
        {
            content = Content;
            Instance = this;
        }

        /// <summary>
        /// Jump straight to a scene
        /// </summary>
        /// <param name="sceneID"></param>
        public void SetScene(Scene scene)
        {
            if ((current == null)||(current.ID != scene.ID))
            {
                current = scene;
                current.Init();
                current.Load(content);
            }
        }

        public void Update(float dt)
        {
            if (current != null)
            {
                if (Paused)
                    dt = 0;
                current.Update(dt);
            }
        }

        public void Draw(GameTime gt)
        {
            if (current != null)
            {
                current.Draw(gt);
            }
        }
    }
}
