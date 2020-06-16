using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GUITestbed.Rendering._3D
{
    public class ShaderManager
    {
        static ShaderManager Instance;

        Dictionary<String, Effect> Shaders = new Dictionary<string, Effect>();

        public ShaderManager()
        {
            Instance = this;
        }

        public static void Preload(String s)
        {
            Instance.Shaders.Add(s, Game1.Instance.Content.Load<Effect>(s));            
        }

        public static Effect GetEffect(String s)
        {
            if (!Instance.Shaders.ContainsKey(s))
               Instance.Shaders.Add(s, Game1.Instance.Content.Load<Effect>(s));

            return Instance.Shaders[s].Clone();
        }
    }
}
