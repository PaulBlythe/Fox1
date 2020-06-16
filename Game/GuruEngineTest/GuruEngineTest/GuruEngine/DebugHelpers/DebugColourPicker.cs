using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Assets;

namespace GuruEngine.DebugHelpers
{
    public class DebugColourPicker
    {
        public int return_code = 0;

        public Color result;

        Rectangle CancelButton = new Rectangle(510, 665, 100, 30);
        Rectangle OKButton = new Rectangle(1390, 665, 100, 30);
        MouseState oldmousestate;
        Rectangle slider1;
        Rectangle slider2;
        Rectangle slider3;
        Rectangle slider4;
        Rectangle slider5;
        Rectangle slider6;
        Rectangle slider7;

        Rectangle pickle1;
        Rectangle pickle2;
        Rectangle pickle3;
        Rectangle pickle4;
        Rectangle pickle5;
        Rectangle pickle6;
        Rectangle pickle7;
        Rectangle cs = new Rectangle(502, 102, 256, 256);

        Rectangle[] StandardColours = new Rectangle[30];
        Color[] SCols = new Color[30];

        int grabbed = -1;

        Texture2D ColorSpace;

        Color[] pixels = new Color[256 * 256];

        public DebugColourPicker(Color start)
        {
            result = start;
            ColorSpace = new Texture2D(Renderer.GetGraphicsDevice(), 256, 256);

            GenerateColourSpace();

            slider1 = new Rectangle(900, 110, 256 + 16, 22);
            slider2 = new Rectangle(900, 140, 256 + 16, 22);
            slider3 = new Rectangle(900, 170, 256 + 16, 22);
            slider4 = new Rectangle(900, 200, 256 + 16, 22);
            slider5 = new Rectangle(900, 300, 256 + 16, 22);
            slider6 = new Rectangle(900, 330, 256 + 16, 22);
            slider7 = new Rectangle(900, 360, 256 + 16, 22);

            pickle1 = new Rectangle(900, 109, 16, 24);
            pickle2 = new Rectangle(900, 139, 16, 24);
            pickle3 = new Rectangle(900, 169, 16, 24);
            pickle4 = new Rectangle(900, 199, 16, 24);
            pickle5 = new Rectangle(900, 299, 16, 24);
            pickle6 = new Rectangle(900, 329, 16, 24);
            pickle7 = new Rectangle(900, 359, 16, 24);

            int pos = 0;
            for (int y = 0; y < 2; y++)
            {
                for (int i = 0; i < 15; i++)
                {
                    StandardColours[pos] = new Rectangle(550 + (i*60), 500 + (y * 60), 48, 48);
                    pos++;
                }
            }

            SCols[0] = Color.FromNonPremultiplied(0, 0, 0, 255);
            SCols[1] = Color.FromNonPremultiplied(255, 0, 0, 255);
            SCols[2] = Color.FromNonPremultiplied(255, 255, 0, 255);
            SCols[3] = Color.FromNonPremultiplied(0, 255, 0, 255);
            SCols[4] = Color.FromNonPremultiplied(0, 255, 255, 255);
            SCols[5] = Color.FromNonPremultiplied(0, 0, 255, 255);
            SCols[6] = Color.FromNonPremultiplied(255, 0, 255, 255);
            SCols[7] = Color.FromNonPremultiplied(255, 255, 255, 255);
            SCols[8] = Color.FromNonPremultiplied(128, 0, 0, 255);
            SCols[9] = Color.FromNonPremultiplied(128, 128, 0, 255);
            SCols[10] = Color.FromNonPremultiplied(0, 128, 0, 255);
            SCols[11] = Color.FromNonPremultiplied(0, 128, 128, 255);
            SCols[12] = Color.FromNonPremultiplied(0, 0, 128, 255);
            SCols[13] = Color.FromNonPremultiplied(128, 0, 128, 255);
            SCols[14] = Color.FromNonPremultiplied(128, 128, 128, 255);

            SCols[15 + 0] = Color.FromNonPremultiplied(0, 0, 0, 128);
            SCols[15 + 1] = Color.FromNonPremultiplied(255, 0, 0, 128);
            SCols[15 + 2] = Color.FromNonPremultiplied(255, 255, 0, 128);
            SCols[15 + 3] = Color.FromNonPremultiplied(0, 255, 0, 128);
            SCols[15 + 4] = Color.FromNonPremultiplied(0, 255, 255, 128);
            SCols[15 + 5] = Color.FromNonPremultiplied(0, 0, 255, 128);
            SCols[15 + 6] = Color.FromNonPremultiplied(255, 0, 255, 128);
            SCols[15 + 7] = Color.FromNonPremultiplied(255, 255, 255, 128);
            SCols[15 + 8] = Color.FromNonPremultiplied(128, 0, 0, 128);
            SCols[15 + 9] = Color.FromNonPremultiplied(128, 128, 0, 128);
            SCols[15 + 10] = Color.FromNonPremultiplied(0, 128, 0, 128);
            SCols[15 + 11] = Color.FromNonPremultiplied(0, 128, 128, 128);
            SCols[15 + 12] = Color.FromNonPremultiplied(0, 0, 128, 128);
            SCols[15 + 13] = Color.FromNonPremultiplied(128, 0, 128, 128);
            SCols[15 + 14] = Color.FromNonPremultiplied(128, 128, 128, 128);


        }

        public void Draw(SpriteBatch batch)
        {
            MouseState ms = Mouse.GetState();
            bool click = ((ms.LeftButton == ButtonState.Released) && (oldmousestate.LeftButton == ButtonState.Pressed));
            Color temp = result;

            batch.FillRectangle(new Rectangle(500, 100, 1000, 600), Color.DarkGray);
            batch.DrawRectangle(new Rectangle(500, 100, 1000, 600), Color.White);

            batch.FillRectangle(new Rectangle(501, 101, 258, 258), Color.DarkSlateGray);
            batch.Draw(ColorSpace,cs , Color.White);

            if (click)
            {
                if (cs.Contains(ms.X,ms.Y))
                {
                    int dx = ms.X - cs.X;
                    int dy = ms.Y - cs.Y;
                    result = pixels[dx + (dy * 256)];
                }
            }

            batch.FillRectangle(new Rectangle(1220, 165, 130, 130), Color.DarkSlateGray);
            batch.FillRectangle(new Rectangle(1221, 165, 128, 128), result);

            batch.DrawString(AssetManager.GetDebugFont(), "R", new Vector2(802, 110), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "G", new Vector2(802, 140), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "B", new Vector2(802, 170), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "A", new Vector2(802, 200), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "H", new Vector2(802, 300), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "S", new Vector2(802, 330), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "L", new Vector2(802, 360), Color.White);

            batch.FillRectangle(new Rectangle(840, 110, 50, 24), Color.DarkSlateGray);
            batch.FillRectangle(new Rectangle(840, 140, 50, 24), Color.DarkSlateGray);
            batch.FillRectangle(new Rectangle(840, 170, 50, 24), Color.DarkSlateGray);
            batch.FillRectangle(new Rectangle(840, 200, 50, 24), Color.DarkSlateGray);
            batch.FillRectangle(new Rectangle(840, 300, 50, 24), Color.DarkSlateGray);
            batch.FillRectangle(new Rectangle(840, 330, 50, 24), Color.DarkSlateGray);
            batch.FillRectangle(new Rectangle(840, 360, 50, 24), Color.DarkSlateGray);

            batch.DrawString(AssetManager.GetDebugFont(), result.R.ToString(), new Vector2(842, 109), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), result.G.ToString(), new Vector2(842, 139), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), result.B.ToString(), new Vector2(842, 169), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), result.A.ToString(), new Vector2(842, 199), Color.White);

            batch.FillRectangle(slider1, Color.DarkSlateGray);
            batch.DrawRectangle(slider1, Color.White);

            batch.FillRectangle(slider2, Color.DarkSlateGray);
            batch.DrawRectangle(slider2, Color.White);

            batch.FillRectangle(slider3, Color.DarkSlateGray);
            batch.DrawRectangle(slider3, Color.White);

            batch.FillRectangle(slider4, Color.DarkSlateGray);
            batch.DrawRectangle(slider4, Color.White);

            batch.FillRectangle(slider5, Color.DarkSlateGray);
            batch.DrawRectangle(slider5, Color.White);

            batch.FillRectangle(slider6, Color.DarkSlateGray);
            batch.DrawRectangle(slider6, Color.White);

            batch.FillRectangle(slider7, Color.DarkSlateGray);
            batch.DrawRectangle(slider7, Color.White);

            for (int i = 0; i < 30; i++)
            {
                batch.FillRectangle(StandardColours[i], SCols[i]);
                batch.DrawRectangle(StandardColours[i], Color.White);
                if (click)
                {
                    if (StandardColours[i].Contains(ms.X,ms.Y))
                    {
                        result = SCols[i];
                    }
                }
            }

            pickle1.X = 900 + result.R;
            pickle2.X = 900 + result.G;
            pickle3.X = 900 + result.B;
            pickle4.X = 900 + result.A;

            double[] hsl = RgBtoHsl(result);
            pickle5.X = 900 + (int)(255 * (hsl[0] / 360.0));
            pickle6.X = 900 + (int)(255 * hsl[1]);
            pickle7.X = 900 + (int)(255 * hsl[2]);

            int hi = (int)(255 * (hsl[0] / 360.0));
            int si = (int)(255 * hsl[1]);
            int li = (int)(255 * hsl[2]);

            batch.DrawString(AssetManager.GetDebugFont(), hi.ToString(), new Vector2(842, 299), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), si.ToString(), new Vector2(842, 329), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), li.ToString(), new Vector2(842, 359), Color.White);


            batch.FillRectangle(pickle1, Color.Gray);
            batch.DrawRectangle(pickle1, Color.White);
            batch.FillRectangle(pickle2, Color.Gray);
            batch.DrawRectangle(pickle2, Color.White);
            batch.FillRectangle(pickle3, Color.Gray);
            batch.DrawRectangle(pickle3, Color.White);
            batch.FillRectangle(pickle4, Color.Gray);
            batch.DrawRectangle(pickle4, Color.White);
            batch.FillRectangle(pickle5, Color.Gray);
            batch.DrawRectangle(pickle5, Color.White);
            batch.FillRectangle(pickle6, Color.Gray);
            batch.DrawRectangle(pickle6, Color.White);
            batch.FillRectangle(pickle7, Color.Gray);
            batch.DrawRectangle(pickle7, Color.White);


            result.R = (byte)CheckPickle(ms, pickle1, 0, result.R);
            result.G = (byte)CheckPickle(ms, pickle2, 1, result.G);
            result.B = (byte)CheckPickle(ms, pickle3, 2, result.B);
            result.A = (byte)CheckPickle(ms, pickle4, 3, result.A);

            int thi = CheckPickle(ms, pickle5, 4, hi);
            int tsi = CheckPickle(ms, pickle6, 5, si);
            int tli = CheckPickle(ms, pickle7, 6, li);

            if ((thi != hi) || (tsi != si) ||(tli != li))
            {
                double dh = (360.0 * thi) / 255.0;
                double ds = (tsi / 255.0);
                double dl = (tli / 255.0);

                result = HsLtoRgb(dh, ds, dl, result.A);
            }

            if (temp != result)
            {
                GenerateColourSpace();
            }

            if (CancelButton.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(CancelButton, Color.DarkGray);
                if (click)
                    return_code = 1;
            }
            else
            {
                batch.FillRectangle(CancelButton, Color.DarkSlateGray);
            }
            batch.DrawRectangle(CancelButton, Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Cancel", new Vector2(520, 666), Color.White);

            if (OKButton.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(OKButton, Color.DarkGray);
                if (click)
                    return_code = 2;
            }
            else
            {
                batch.FillRectangle(OKButton, Color.DarkSlateGray);
            }
            batch.DrawRectangle(OKButton, Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "OK", new Vector2(1420, 666), Color.White);



            oldmousestate = ms;
        }

        int CheckPickle(MouseState ms, Rectangle r, int index, int value)
        {
            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (grabbed == index)
                {
                    value = ms.X - 900;
                    value = Math.Max(value, 0);
                    value = Math.Min(value, 255);
                }
                else
                {
                    if (r.Contains(ms.X, ms.Y))
                    {
                        grabbed = index;
                    }
                }
            }
            else
            {
                if (grabbed == index)
                    grabbed = -1;
            }

            return value;
        }

        void GenerateColourSpace()
        {
            double h = RgBtoHsl(result)[0];

           
            int pos = 0;
            for (int l = 0; l < 256; l++)
            {
                double ld = ((double)l / 255.0);
                for (int s = 0; s < 256; s++)
                {
                    double sd = ((double)s / 255.0);
                    pixels[pos++] = HsLtoRgb(h, sd, ld, 255);
                }
            }

            ColorSpace.SetData<Color>(pixels);
        }

        /// <summary>
        /// Converts HSL to RGB, with a specified output Alpha.
        /// Arguments are limited to the defined range:
        /// does not raise exceptions.
        /// </summary>
        /// <param name="h">Hue, must be in [0, 360].</param>
        /// <param name="s">Saturation, must be in [0, 1].</param>
        /// <param name="l">Luminance, must be in [0, 1].</param>
        /// <param name="a">Output Alpha, must be in [0, 255].</param>
        Color HsLtoRgb(double h, double s, double l, int a = 255)
        {
            h = Math.Max(0, Math.Min(360D, h));
            s = Math.Max(0, Math.Min(1D, s));
            l = Math.Max(0, Math.Min(1D, l));
            a = Math.Max(0, Math.Min(255, a));

            // achromatic argb (gray scale)
            if (Math.Abs(s) < 0.000000000000001)
            {
                return Color.FromNonPremultiplied(

                        Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
                        Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
                        Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
                        a);
            }

            double q = l < .5D
                    ? l * (1D + s)
                    : (l + s) - (l * s);
            double p = (2D * l) - q;

            double hk = h / 360D;
            double[] T = new double[3];
            T[0] = hk + (1D / 3D); // Tr
            T[1] = hk; // Tb
            T[2] = hk - (1D / 3D); // Tg

            for (int i = 0; i < 3; i++)
            {
                if (T[i] < 0D)
                    T[i] += 1D;
                if (T[i] > 1D)
                    T[i] -= 1D;

                if ((T[i] * 6D) < 1D)
                    T[i] = p + ((q - p) * 6D * T[i]);
                else if ((T[i] * 2D) < 1)
                    T[i] = q;
                else if ((T[i] * 3D) < 2)
                    T[i] = p + ((q - p) * ((2D / 3D) - T[i]) * 6D);
                else
                    T[i] = p;
            }

            return Color.FromNonPremultiplied(
                    Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{T[0] * 255D:0.00}")))),
                    Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{T[1] * 255D:0.00}")))),
                    Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{T[2] * 255D:0.00}")))),
                    a);
        }

        /// <summary>
        /// Converts RGB to HSL. Alpha is ignored.
        /// Output is: { H: [0, 360], S: [0, 1], L: [0, 1] }.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        public static double[] RgBtoHsl(Color color)
        {
            double h = 0D;
            double s = 0D;
            double l;

            // normalize red, green, blue values
            double r = color.R / 255D;
            double g = color.G / 255D;
            double b = color.B / 255D;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            // hue
            if (Math.Abs(max - min) < 0.000000000000001)
                h = 0D; // undefined
            else if ((Math.Abs(max - r) < 0.000000000000001)
                    && (g >= b))
                h = (60D * (g - b)) / (max - min);
            else if ((Math.Abs(max - r) < 0.000000000000001)
                    && (g < b))
                h = ((60D * (g - b)) / (max - min)) + 360D;
            else if (Math.Abs(max - g) < 0.000000000000001)
                h = ((60D * (b - r)) / (max - min)) + 120D;
            else if (Math.Abs(max - b) < 0.000000000000001)
                h = ((60D * (r - g)) / (max - min)) + 240D;

            // luminance
            l = (max + min) / 2D;

            // saturation
            if ((Math.Abs(l) < 0.000000000000001)
                    || (Math.Abs(max - min) < 0.000000000000001))
                s = 0D;
            else if ((0D < l)
                    && (l <= .5D))
                s = (max - min) / (max + min);
            else if (l > .5D)
                s = (max - min) / (2D - (max + min)); //(max-min > 0)?

            return new[]
            {
                Math.Max(0D, Math.Min(360D, double.Parse($"{h:0.##}"))),
                Math.Max(0D, Math.Min(1D, double.Parse($"{s:0.##}"))),
                Math.Max(0D, Math.Min(1D, double.Parse($"{l:0.##}")))
            };
        }
    }
}
