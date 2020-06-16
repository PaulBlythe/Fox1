using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GUITestbed.DataHandlers.Mapping.Types;

namespace GUITestbed.DataHandlers.Shape
{
    public class SHPPointZ:SHPElement
    {
        public double X;
        public double Y;
        public double Z;
        public double M;

        public SHPPointZ(BinaryReader b)
        {
            X = b.ReadDouble();
            Y = b.ReadDouble();
            Z = b.ReadDouble();
            M = b.ReadDouble();
            Type = 11;
        }

    }
}
