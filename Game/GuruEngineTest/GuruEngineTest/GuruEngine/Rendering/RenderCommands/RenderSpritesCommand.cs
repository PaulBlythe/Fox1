using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GuruEngine.Rendering.RenderCommands
{
    class DrawRecord
    {
        public Rectangle src;
        public Rectangle dest;
        public Texture2D tex;
        public Color colour;
        public float Extra;
        public int Type;

        public DrawRecord(Texture2D t, Rectangle s, Rectangle d, Color c, int ty)
        {
            tex = t;
            src = s;
            dest = d;
            colour = c;
            Type = ty;
        }
    }


    public class RenderSpritesCommand:RenderCommand
    {
        SpriteBatch spriteBatch;
        List<DrawRecord> draws = new List<DrawRecord>();
      

        public RenderSpritesCommand()
        {
            spriteBatch = new SpriteBatch(Renderer.GetGraphicsDevice());
            OwnerDraw = true;
        }

        public override void PreRender(GraphicsDevice dev)
        {
        }

        public override void Draw(GraphicsDevice dev)
        {
            
            lock (draws)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.NonPremultiplied);
                foreach (DrawRecord dr in draws)
                {
                    switch (dr.Type)
                    {
                        case 0:
                            spriteBatch.Draw(dr.tex, dr.dest, dr.src, dr.colour);
                            break;
                        case 1:
                            spriteBatch.FillRectangle(dr.dest, dr.colour);
                            break;
                        case 2:
                            spriteBatch.DrawLine(dr.dest.X, dr.dest.Y, dr.dest.Width, dr.dest.Height, dr.colour, dr.Extra);
                            break;
                    }
                    
                }
                spriteBatch.End();
            }
        }

        public void AddSprite(Texture2D tex, Rectangle src, Rectangle dest, Color colour)
        {
            lock (draws)
            {
                draws.Add(new DrawRecord(tex, src, dest, colour,0));
            }
        }
        public void AddFilledRectangle(Rectangle dest, Color colour)
        {
            lock (draws)
            {
                draws.Add(new DrawRecord(null, new Rectangle(0,0,0,0), dest, colour, 1));
            }
        }
        public void AddLine(int x, int y, int x2, int y2, Color colour, float thickness)
        {
            lock (draws)
            {
                Rectangle d = new Rectangle(x, y, x2, y2);
                DrawRecord dr = new DrawRecord(null,d,d,colour,2);
                dr.Extra = thickness;

                draws.Add(dr);
            }
        }
        public void AddLine(Rectangle d, Color colour, float thickness)
        {
            lock (draws)
            {
                
                DrawRecord dr = new DrawRecord(null, d, d, colour, 2);
                dr.Extra = thickness;

                draws.Add(dr);
            }
        }

        public void Dispose()
        {
            draws.Clear();
            spriteBatch.Dispose();
        }
    }
}
