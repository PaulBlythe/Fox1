using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GuruEngine.Assets
{
    public class FaceGroup
    {
        public int StartVertex;
        public int VertexCount;
        public int StartFace;
        public int FaceCount;
        public int Material;

        public FaceGroup(BinaryReader b)
        {
            StartVertex = b.ReadInt32();
            VertexCount = b.ReadInt32();
            StartFace = b.ReadInt32();
            FaceCount = b.ReadInt32();
            Material = b.ReadInt32();
        }
    }
}
