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
    public class Button : GuiItem
    {
        Vector4 src_down;
        Vector4 src_normal;
        Vector4 src_up;
        Vector4 display_pos;

        Vector2 text_pos;

        int button_state = 0;
        String text;
        String evt;

        public Button(String Text, String EventID)
        {
            text = Text;
            evt = EventID;
            drawOrder = DrawOrder.Both;
        }

        public override void Load()
        {
            src_down   = new Vector4(GuiManager.Instance.ButtonSource[0], GuiManager.Instance.ButtonSource[1], GuiManager.Instance.ButtonSource[2], GuiManager.Instance.ButtonSource[3]);
            src_normal = new Vector4(GuiManager.Instance.ButtonSource[4], GuiManager.Instance.ButtonSource[5], GuiManager.Instance.ButtonSource[6], GuiManager.Instance.ButtonSource[7]);
            src_up     = new Vector4(GuiManager.Instance.ButtonSource[8], GuiManager.Instance.ButtonSource[9], GuiManager.Instance.ButtonSource[10], GuiManager.Instance.ButtonSource[11]);

            float scale = 1.0f / GuiManager.Instance.GuiTextureSize;
            src_down *= scale;
            src_normal *= scale;
            src_up *= scale;

        }

        public override void Position(Vector4 position)
        {
            Vector2 start = GuiManager.Instance.ScalePosition(position.X, position.Y);
            Vector2 size = GuiManager.Instance.ScalePosition(position.Z, position.W);

            display_pos = new Vector4(start.X, start.Y, size.X, size.Y);

            float cx = start.X + (size.X * 0.5f);
            float cy = start.Y + (size.Y * 0.5f);

            Vector2 ts = GuiManager.Instance.GetFont(GuiManager.FontStyle.Medium).MeasureString(text);
            text_pos = new Vector2(cx - (ts.X * 0.5f), cy + 8);
        }

        public override void Update(Vector2 mousePosition, int mouseButtons, float dt, int wheelDelta)
        {
            if ((mousePosition.X >= display_pos.X) &&
                (mousePosition.Y >= display_pos.Y) &&
                (mousePosition.X < (display_pos.X + display_pos.Z)) &&
                (mousePosition.Y < (display_pos.Y + display_pos.W)))
            {
                if ((mouseButtons & 1) != 0)
                {
                    button_state = 1;
                }else
                {
                    if (button_state == 1)
                    {
                        GuiManager.Instance.BroadcastMessage(evt);
                    }
                    button_state = 2;
                }
            }else
            {
                if (button_state == 1)
                {
                    GuiManager.Instance.BroadcastMessage(evt);
                }
                button_state = 0;
            }
             
        }

        public override void Draw(GlyphBatch batch)
        {
            if (batch != null)
            {
                switch (button_state)
                {
                    case 1:
                        batch.Draw(src_down, display_pos);
                        break;
                    case 2:
                        batch.Draw(src_up, display_pos);
                        break;
                    default:
                        batch.Draw(src_normal, display_pos);
                        break;
                }
            }
            else
            {
                SDFFont font = GuiManager.Instance.GetFont(GuiManager.FontStyle.Medium);
                font.StartFontRendering(Color.White);
                font.DrawString(text, text_pos);
                font.EndFontRendering();
            }
        }
    }
}
