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

namespace GuruEngine.Helpers
{
    public class IOHelper
    {
        public static Vector3 ReadVector3(BinaryReader b)
        {
            float x = b.ReadSingle();
            float y = b.ReadSingle();
            float z = b.ReadSingle();
            return new Vector3(x, y, z);
        }
        public static Vector2 ReadVector2(BinaryReader b)
        {
            float x = b.ReadSingle();
            float y = b.ReadSingle();
            return new Vector2(x, y);
        }
        public static Color ColourFromString(String Value)
        {
            string[] parts = Value.Split(';');
            Color res = new Color();

            res.R = byte.Parse(parts[0]);
            res.G = byte.Parse(parts[0]);
            res.B = byte.Parse(parts[0]);
            res.A = byte.Parse(parts[0]);

            return res;
        }
    }
}
