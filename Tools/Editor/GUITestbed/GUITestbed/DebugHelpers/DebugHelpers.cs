using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.DebugHelpers
{
    public static class DebugHelpers
    {
        public static void SaveDepthTexture(String name, Texture2D tex, float range)
        {
            Texture2D temp = new Texture2D(Game1.Instance.GraphicsDevice, tex.Width, tex.Height, false, SurfaceFormat.Color);
            float[] pixels = new float[tex.Width * tex.Height];
            tex.GetData<float>(pixels);

            float maxp, minp;
            CheckRange(pixels, out minp, out maxp);

            Color[] tempcol = new Color[tex.Width * tex.Height];

            for (int i = 0; i < temp.Width * temp.Height; i++)
            {
                float z = pixels[i];
                z /= range;
                z *= 255;
                byte cc = (byte)z;
                tempcol[i] = new Color(cc, cc, cc, (byte)255);
            }
            temp.SetData<Color>(tempcol);

            Stream stream = File.Create(name);
            temp.SaveAsPng(stream, temp.Width, temp.Height);
            stream.Dispose();
        }

        public static float linearize_depth(float d, float zNear, float zFar)
        {
            return zNear + zFar / (zFar + d * (zNear - zFar)); 

        }

        public static void SaveTexture(String name, Texture2D tex)
        {
            Stream stream = File.Create(name);
            tex.SaveAsPng(stream, tex.Width, tex.Height);
            stream.Dispose();
        }

        public static void CheckRange(float [] data, out float min, out float max)
        {
            float minval = float.MaxValue;
            float maxvalue = float.MinValue;
            for (int i=0; i<data.Length; i++)
            {
                if (data[i] < minval)
                    minval = data[i];
                if (data[i] > maxvalue)
                    maxvalue = data[i];
            }
            System.Console.WriteLine(String.Format("Range {0} to {1}  ({2})", minval, maxvalue, maxvalue - minval));

            min = minval;
            max = maxvalue;
        }
    }
}
