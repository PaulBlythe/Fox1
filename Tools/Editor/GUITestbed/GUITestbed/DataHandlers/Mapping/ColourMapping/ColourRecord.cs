using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.DataHandlers.Mapping.ColourMapping
{
    public class ColourRecord
    {
        public Color Colour;
        public int Width;

        public ColourRecord(Color c, int w)
        {
            Colour = c;
            Width = w;
        }
    }
}
