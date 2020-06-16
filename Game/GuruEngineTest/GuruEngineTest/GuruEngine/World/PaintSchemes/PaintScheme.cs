using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.World.PaintSchemes
{
    public abstract class PaintScheme
    {
        public Dictionary<String, String> TextureOverrides = new Dictionary<string, string>();
    }
}
