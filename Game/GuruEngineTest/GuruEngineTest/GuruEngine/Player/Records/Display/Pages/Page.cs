using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Rendering;

namespace GuruEngine.Player.Records.Display.Pages
{
    public abstract class Page
    {
        public RenderCommandSet set;

        public abstract void SetupRenderCommands();
        public abstract void Update();
        public abstract void Dispose();


        public Rectangle Scale(int x, int y, int w, int h)
        {
            int width = Renderer.GetGraphicsDevice().Viewport.Width;
            int height = Renderer.GetGraphicsDevice().Viewport.Height;

            if ((width == 1920) && (height == 1080))
            {
                return new Rectangle(x, y, w, h);
            }

            int x1 = (x * width) / 1920;
            int y1 = (y * height) / 1080;
            int w1 = (w * width) / 1920;
            int h1 = (h * height) / 1080;
            return new Rectangle(x1, y1, w1, h1);

        }
        public float Scale(float orig)
        {
            int width = Renderer.GetGraphicsDevice().Viewport.Width;
            return (orig * width / 1920.0f);
        }

        public Vector2 Scale(Vector2 orig)
        {
            int width = Renderer.GetGraphicsDevice().Viewport.Width;
            int height = Renderer.GetGraphicsDevice().Viewport.Height;

            if ((width == 1920) && (height == 1080))
            {
                return orig;
            }
            return new Vector2((orig.X * width) / 1920.0f, (orig.Y * height) / 1080.0f);
        }

        public Vector2 Scale(float x, float y)
        {
            int width = Renderer.GetGraphicsDevice().Viewport.Width;
            int height = Renderer.GetGraphicsDevice().Viewport.Height;

            if ((width == 1920) && (height == 1080))
            {
                return new Vector2(x, y);
            }
            return new Vector2((x * width) / 1920.0f, (y * height) / 1080.0f);
        }

        public Rectangle FullScreen()
        {
            int width = Renderer.GetGraphicsDevice().Viewport.Width;
            int height = Renderer.GetGraphicsDevice().Viewport.Height;
            return new Rectangle(0, 0, width, height);
        }


    }
}
