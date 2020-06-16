using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.Rendering.Gui;
using GuruEngine.Text;

namespace GuruEngine.Rendering.Gui.GuiItems
{
    public class Label : GuiItem
    {
        public enum Alignment
        {
            Left,
            Centre,
            Right
        }

        String text;
        Vector2 pos;
        Alignment al;
        GuiManager.FontStyle style;
        Color colour;


        public Label(String Text, Alignment Align, GuiManager.FontStyle Style, Color Colour)
        {
            style = Style;
            text = Text;
            al = Align;
            colour = Colour;
            drawOrder = DrawOrder.Last;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public override void Load()
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="mouseButtons"></param>
        /// <param name="dt"></param>
        /// <param name="wheelDelta"></param>
        public override void Update(Vector2 mousePosition, int mouseButtons, float dt, int wheelDelta)
        {
        }

        public override void Draw(GlyphBatch batch)
        {
            SDFFont font = GuiManager.Instance.GetFont(style);
            font.StartFontRendering(colour);
            font.DrawString(text, pos);
            font.EndFontRendering();

        }

        public override void Position(Vector4 position)
        {
            Vector2 start = GuiManager.Instance.ScalePosition(position.X, position.Y);
            switch (al)
            {
                case Alignment.Left:
                    {
                        pos = start;
                    }
                    break;

                case Alignment.Right:
                    {
                        SDFFont font = GuiManager.Instance.GetFont(style);
                        Vector2 size = font.MeasureString(text);
                        start.X -= size.X;
                        pos = start;
                    }
                    break;

                case Alignment.Centre:
                    {
                        SDFFont font = GuiManager.Instance.GetFont(style);
                        Vector2 size = font.MeasureString(text);
                        start.X -= (size.X / 2);
                        pos = start;
                    }
                    break;
            }
        }
    }
}
