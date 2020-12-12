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
        bool holdoff = false;

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
                holdoff = true;
                if (current !=null)
                {
                    content.Unload();
                }
                current = scene;
                current.Init();
                current.Load(content);
                holdoff = false;
            }
        }

        public void Update(float dt)
        {
            if ((current != null) && (!holdoff))
            {
                if (Paused)
                    dt = 0;
                current.Update(dt);
            }
        }

        public void Draw(GameTime gt)
        {
            if ((current != null) && (!holdoff))
            {
                current.Draw(gt);
            }
        }
    }
}
