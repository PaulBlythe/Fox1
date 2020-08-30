using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.Tools
{
    public class TextureLoader
    {
        public static Texture2D Load(String file)
        {
            Texture2D tex;

            try
            {
                Stream read = new FileStream(file, FileMode.Open);
                tex = Texture2D.FromStream(Game1.Instance.GraphicsDevice, read);
                read.Close();
            }
            catch (Exception)
            {
                tex = new Texture2D(Game1.Instance.GraphicsDevice, 2, 2);
                
            }
            return tex;
        }
    }
}
