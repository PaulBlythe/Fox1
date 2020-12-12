using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.Rendering._3D
{

    public struct VertexPositionNormalColor
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Color;

        public static int SizeInBytes = (3 + 3 + 3) * sizeof(float);

        static readonly VertexDeclaration MyVertexDeclaration = new VertexDeclaration(new VertexElement[]
        {
            new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
            new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
            new VertexElement( sizeof(float) * 6, VertexElementFormat.Vector3, VertexElementUsage.Color, 0 ),
        });

        public VertexDeclaration VertexDeclaration
        {
            get { return MyVertexDeclaration; }
        }
    }
}
