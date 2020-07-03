using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GuruEngine.Rendering.VertexDeclarations
{
    public struct AnimatedVertex
    {
        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public Vector3 Normal;
        public float AnimationValue;

        public static int SizeInBytes =  (3 + 2 + 3 + 1) * 4;
        public static VertexDeclaration OceanVertexDeclaration = new VertexDeclaration
         (
                new VertexElement(sizeof(float) * 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 5, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(sizeof(float) * 8, VertexElementFormat.Single,  VertexElementUsage.TextureCoordinate, 1)
         );
    }
}
