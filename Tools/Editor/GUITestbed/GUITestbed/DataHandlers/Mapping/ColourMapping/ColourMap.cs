using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.DataHandlers.Mapping.ColourMapping
{
    public abstract class ColourMap
    {
        public abstract ColourRecord GetColour(String type);
        public abstract int GetSize(String type);
    }
}
