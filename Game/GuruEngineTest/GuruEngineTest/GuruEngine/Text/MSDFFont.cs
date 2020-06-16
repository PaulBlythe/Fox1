using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using GuruEngine.Rendering;

namespace GuruEngine.Text
{
    public class MSDFFont
    {
        float Scale;
        Texture2D texture;
        public Effect effect;
        Dictionary<int, CharRecord> chars = new Dictionary<int, CharRecord>();
        GlyphBatch gbatch;

        public MSDFFont(String description)
        {
            
            using (TextReader reader = File.OpenText(description))
            {
                String line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    int i = int.Parse(parts[0]);
                    int x = int.Parse(parts[9]);
                    int y = int.Parse(parts[10]);
                    int w = int.Parse(parts[2]);
                    int h = int.Parse(parts[4]);
                    float xo = float.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture);
                    float yo = -float.Parse(parts[6], System.Globalization.CultureInfo.InvariantCulture);
                    float xa = float.Parse(parts[7], System.Globalization.CultureInfo.InvariantCulture);
                    chars.Add(i, new CharRecord(i, x, y, w, h, xo, yo, xa));
                }
            }

        }

        public void Setup(Texture2D tex, Effect eff, Color colour, float scale)
        {
            texture = tex;
            effect = eff;
            gbatch = new GlyphBatch(Renderer.GetGraphicsDevice(), tex, eff, colour);
            Scale = scale;

            eff.Parameters["pxRange"].SetValue(scale);
            eff.Parameters["textureSize"].SetValue(new Vector2(tex.Width, tex.Height));
            eff.Parameters["bgColor"].SetValue(Color.Transparent.ToVector4());
            eff.Parameters["fgColor"].SetValue(colour.ToVector4());


        }

        public GlyphBatch GetBatch()
        {
            return gbatch;
        }

        public void DrawString(String s, Vector2 location)
        {
            foreach (Char c in s)
            {
                CharRecord cr = chars[(int)c];
                Vector4 src = new Vector4((float)cr.X / (float)texture.Width, (float)cr.Y / (float)texture.Height, (float)cr.Width / (float)texture.Width, (float)cr.Height / texture.Height);
                Vector4 dst = new Vector4((location.X + (cr.xoffset * Scale)), (location.Y - (cr.yoffset * Scale)), ((cr.Width * Scale)), ((cr.Height * Scale)));

                gbatch.Draw(src, dst);
                location.X += (cr.xadvance) * Scale;
            }
        }

        public void DrawString(String s, Vector2 location, float oscale)
        {
            foreach (Char c in s)
            {
                CharRecord cr = chars[(int)c];
                Vector4 src = new Vector4((float)cr.X / (float)texture.Width, (float)cr.Y / (float)texture.Height, (float)cr.Width / (float)texture.Width, (float)cr.Height / texture.Height);
                Vector4 dst = new Vector4((location.X + (cr.xoffset * Scale * oscale)), (location.Y - (cr.yoffset * Scale * oscale)), ((cr.Width * Scale * oscale)), ((cr.Height * Scale * oscale)));

                gbatch.Draw(src, dst);
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


        public void Dispose()
        {
            chars.Clear();
        }
    }
}

