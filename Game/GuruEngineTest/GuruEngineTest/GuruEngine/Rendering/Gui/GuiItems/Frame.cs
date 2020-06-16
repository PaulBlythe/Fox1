using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.Text;

namespace GuruEngine.Rendering.Gui.GuiItems
{
    public class Frame : GuiItem
    {
        Vector4 topLeft;
        Vector4 topRight;
        Vector4 bottomLeft;
        Vector4 bottomRight;
        Vector4 topFill;
        Vector4 leftFill;
        Vector4 rightFill;
        Vector4 bottomFill;

        Vector4 fill;

        int sx, sy, ex, ey, cs;

        public List<GuiDrawRecord> drawRecords = new List<GuiDrawRecord>();

        /// <summary>
        /// Loads in the definition from the descriptor file
        /// </summary>
        public override void Load()
        {
            sx = GuiManager.Instance.FrameSource[0];
            sy = GuiManager.Instance.FrameSource[1];
            ex = GuiManager.Instance.FrameSource[2];
            ey = GuiManager.Instance.FrameSource[3];
            cs = GuiManager.Instance.FrameSource[4];
            float ts = GuiManager.Instance.GuiTextureSize;

            // Create the source regions, note this assumes the gui texture is 1024 by 1024
            topLeft = new Vector4(sx / ts, sy / ts, cs / ts, cs / ts);
            topRight = new Vector4((ex - cs) / ts, sy / ts, cs / ts, cs / ts);
            bottomLeft = new Vector4(sx / ts, (ey - cs) / ts, cs / ts, cs / ts);
            bottomRight = new Vector4((ex - cs) / ts, (ey - cs) / ts, cs / ts, cs / ts);
            fill = new Vector4((sx + cs) / ts, (sy + cs) / ts, ((ex - sx) - (2 * cs)) / ts, ((ey - sy) - (2 * cs)) / ts);
            topFill = new Vector4((sx + cs) / ts, sy / ts, ((ex - sx) - (2 * cs)) / ts, cs / ts);
            leftFill = new Vector4(sx / ts, (sy + cs) / ts, cs / ts, ((ey - sy) - (2 * cs)) / ts);
            rightFill = new Vector4((ex - cs) / ts, (sy + cs) / ts, cs / ts, ((ey - sy) - (2 * cs)) / ts);
            bottomFill = new Vector4((sx + cs) / ts, (ey - cs) / ts, ((ex - sx) - (2 * cs)) / ts, cs / ts);

            drawOrder = DrawOrder.First;
        }

        /// <summary>
        /// Create all the draw records 
        /// </summary>
        /// <param name="position"></param>
        public override void Position(Vector4 position)
        {
            Vector2 start = GuiManager.Instance.ScalePosition(position.X, position.Y);
            Vector2 size = GuiManager.Instance.ScalePosition(position.Z, position.W);

            // Fill all the main region
            GuiDrawRecord gdr = new GuiDrawRecord();
            gdr.source = fill;
            gdr.destination = new Vector4(start.X + cs - 1, start.Y + cs - 1, size.X - cs - cs + 1, size.Y - cs - cs + 1);
            drawRecords.Add(gdr);

            // add top left
            gdr = new GuiDrawRecord();
            gdr.source = topLeft;
            gdr.destination = new Vector4(start.X, start.Y, cs, cs);
            drawRecords.Add(gdr);

            // add top fill
            gdr = new GuiDrawRecord();
            gdr.source = topFill;
            gdr.destination = new Vector4(start.X + cs, start.Y, size.X - cs - cs + 1, cs);
            drawRecords.Add(gdr);

            // add top right
            gdr = new GuiDrawRecord();
            gdr.source = topRight;
            gdr.destination = new Vector4(start.X + size.X - cs, start.Y, cs, cs);
            drawRecords.Add(gdr);

            // add left fill
            gdr = new GuiDrawRecord();
            gdr.source = leftFill;
            gdr.destination = new Vector4(start.X, start.Y + cs, cs, size.Y - cs - cs);
            drawRecords.Add(gdr);

            // add right fill
            gdr = new GuiDrawRecord();
            gdr.source = rightFill;
            gdr.destination = new Vector4(start.X + size.X - cs, start.Y + cs, cs, size.Y - cs - cs);
            drawRecords.Add(gdr);

            // add bottom fill
            gdr = new GuiDrawRecord();
            gdr.source = bottomFill;
            gdr.destination = new Vector4(start.X + cs, start.Y + size.Y - cs, size.X - cs - cs, cs);
            drawRecords.Add(gdr);

            // add bottom left
            gdr = new GuiDrawRecord();
            gdr.source = bottomLeft;
            gdr.destination = new Vector4(start.X, start.Y + size.Y - cs, cs, cs);
            drawRecords.Add(gdr);

            // add bottom right
            gdr = new GuiDrawRecord();
            gdr.source = bottomRight;
            gdr.destination = new Vector4(start.X + size.X - cs, start.Y + size.Y - cs, cs, cs);
            drawRecords.Add(gdr);
        }

        /// <summary>
        /// Draw all records
        /// </summary>
        /// <param name="batch"></param>
        public override void Draw(GlyphBatch batch)
        {
            foreach (GuiDrawRecord gdr in drawRecords)
            {
                batch.Draw(gdr.source, gdr.destination);
            }
        }

        /// <summary>
        /// Update for a frame does nothing
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="mouseButtons"></param>
        /// <param name="dt"></param>
        /// <param name="wheelDelta"></param>
        public override void Update(Vector2 mousePosition, int mouseButtons, float dt, int wheelDelta)
        {

        }
    }
}
