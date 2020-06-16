using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GuruEngine.Assets
{
    public abstract class Material
    {
        public abstract void Apply(Effect fx);
    }
}
