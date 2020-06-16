using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.GUI
{
    public class CharRecord
    {
        public int ID;

        public int X;
        public int Y;
        public int Width;
        public int Height;

        public float xoffset;
        public float yoffset;
        public float xadvance;

        public CharRecord(int id, int x, int y, int w, int h, float xo, float yo, float xa)
        {
            ID = id;
            X = x;
            Y = y;
            Width = w;
            Height = h;
            xoffset = xo;
            yoffset = yo;
            xadvance = xa;
        }
    }

    public class GuiFont
    {
        float Scale;
        GlyphBatch gbatch;
        Dictionary<int, CharRecord> chars = new Dictionary<int, CharRecord>();
        Texture2D texture;

        public GuiFont(String description, Texture2D tex, Effect eff, GraphicsDevice dev, float scale)
        {
            texture = tex;
            char[] trims = new char[] { ' ', '\t' };
            char[] splits = new char[] { ' ', '=', '\t' };

            using (TextReader reader = File.OpenText(description))
            {
                String line = reader.ReadLine();
                line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                    int i = int.Parse(parts[2].TrimEnd(trims));
                    int x = int.Parse(parts[4].TrimEnd(trims));
                    int y = int.Parse(parts[6].TrimEnd(trims));
                    int w = int.Parse(parts[8].TrimEnd(trims));
                    int h = int.Parse(parts[10].TrimEnd(trims));
                    float xo = float.Parse(parts[12].TrimEnd(trims), System.Globalization.CultureInfo.InvariantCulture);
                    float yo = float.Parse(parts[14].TrimEnd(trims), System.Globalization.CultureInfo.InvariantCulture);
                    float xa = float.Parse(parts[16].TrimEnd(trims), System.Globalization.CultureInfo.InvariantCulture);
                    chars.Add(i, new CharRecord(i, x, y, w, h, xo, yo, xa));
                }
            }
            Scale = scale;
            gbatch = new GlyphBatch(dev, tex, eff);

        }

        public void Begin()
        {
            gbatch.Begin();
        }

        public void End()
        {
            gbatch.End();
        }

        public void DrawString(String s, Vector2 location, Color colour)
        {
            foreach (Char c in s)
            {
                CharRecord cr = chars[(int)c];
                Vector4 src = new Vector4((float)cr.X / (float)texture.Width, (float)cr.Y / (float)texture.Height, (float)cr.Width / (float)texture.Width, (float)cr.Height / texture.Height);
                Vector4 dst = new Vector4((location.X + (cr.xoffset * Scale)), (location.Y - (cr.yoffset * Scale)), ((cr.Width * Scale)), ((cr.Height * Scale)));

                gbatch.Draw(src, dst, colour);
                location.X += (cr.xadvance) * Scale;
            }
        }

        public void DrawString(String s, Vector2 location, Color colour, float oscale)
        {
            foreach (Char c in s)
            {
                CharRecord cr = chars[(int)c];
                Vector4 src = new Vector4((float)cr.X / (float)texture.Width, (float)cr.Y / (float)texture.Height, (float)cr.Width / (float)texture.Width, (float)cr.Height / texture.Height);
                Vector4 dst = new Vector4((location.X + (cr.xoffset * Scale * oscale)), (location.Y - (cr.yoffset * Scale * oscale)), ((cr.Width * Scale * oscale)), ((cr.Height * Scale * oscale)));

                gbatch.Draw(src, dst, colour);
                location.X += (cr.xadvance) * Scale * oscale;
            }
        }

        public Vector2 MeasureString(String s)
        {
            Vector2 result = new Vector2(0, 0);
            foreach (Char c in s)
            {
                CharRecord cr = chars[(int)c];
                float ch = cr.Height;
                if (ch > result.Y)
                    result.Y = ch;
                result.X += cr.xadvance;
            }

            return result * Scale;
        }

        public Vector2 MeasureString(String s, float oscale)
        {
            Vector2 result = new Vector2(0, 0);
            foreach (Char c in s)
            {
                CharRecord cr = chars[(int)c];
                float ch = cr.Height;
                if (ch > result.Y)
                    result.Y = ch;
                result.X += cr.xadvance;
            }

            return result * Scale * oscale;
        }

    }
}
