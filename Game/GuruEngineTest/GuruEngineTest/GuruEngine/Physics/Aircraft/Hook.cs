using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.Physics.Aircraft
{
    public class Hook
    {
        public String Name;
        public Matrix Transform;

        public Hook()
        {
            Name = "";
            Transform = Matrix.Identity;
        }

        public void Load(String [] parts)
        {
            Name = parts[0];
            Transform.M11 = float.Parse(parts[1]);
            Transform.M12 = float.Parse(parts[2]);
            Transform.M13 = float.Parse(parts[3]);
            Transform.M21 = float.Parse(parts[4]);
            Transform.M22 = float.Parse(parts[5]);
            Transform.M23 = float.Parse(parts[6]);
            Transform.M31 = float.Parse(parts[7]);
            Transform.M32 = float.Parse(parts[8]);
            Transform.M33 = float.Parse(parts[9]);
            Transform.M41 = float.Parse(parts[10]);
            Transform.M42 = float.Parse(parts[11]);
            Transform.M43 = float.Parse(parts[12]);
            //Transform.M44 = -1;

            Transform = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(-90), MathHelper.ToRadians(90)) * Transform;
            //Transform *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(90), MathHelper.ToRadians(-90), 0) ;
        }
    }
}
