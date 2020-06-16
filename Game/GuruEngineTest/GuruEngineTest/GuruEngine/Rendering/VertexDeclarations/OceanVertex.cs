using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GuruEngine.Rendering.VertexDeclarations
{
    public struct OceanVertex
    {
        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public Vector3 Tangent;
        public Vector3 Binormal;
        public Vector3 Normal;

        public static int SizeInBytes = (3 + 3 + 4 + 4) * 4;
        public static VertexDeclaration OceanVertexDeclaration = new VertexDeclaration
         (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 5, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)

         );
    }
}
