using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GUITestbed.DataHandlers.Mapping.Types;

namespace GUITestbed.DataHandlers.Shape
{
    public class SHPPoint:SHPElement
    {
        public double X;
        public double Y;

        public SHPPoint(BinaryReader b)
        {
            X = b.ReadDouble();
            Y = b.ReadDouble();
            Type = 1;
        }
    }
}
