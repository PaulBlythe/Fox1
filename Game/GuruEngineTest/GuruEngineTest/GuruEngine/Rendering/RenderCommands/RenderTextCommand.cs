using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Text;

namespace GuruEngine.Rendering.RenderCommands
{
    public class RenderTextCommand:RenderCommand
    {
        GlyphBatch Batch;

        public RenderTextCommand(GlyphBatch batch)
        {
            Batch = batch;
            OwnerDraw = true;
        }

        public override void Draw(GraphicsDevice dev)
        {
            Batch.Flush();
        }

        public override void PreRender(GraphicsDevice dev)
        {
           
        }

        
    }
}
