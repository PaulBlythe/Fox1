using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.GUI
{
    public struct  GUIVertex : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector4 Region;
        public Vector2 TextureCoordinates;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0,                     VertexElementFormat.Vector3, VertexElementUsage.Position,          0),
            new VertexElement(sizeof(float) * 3,     VertexElementFormat.Color,   VertexElementUsage.Color,             0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector4, VertexElementUsage.Normal,            0),
            new VertexElement(sizeof(float) * 7 + 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        public GUIVertex(Vector3 p, Color color, Vector4 r, Vector2 uv)
        {
            Position = p;
            Color = color;
            Region = r;
            TextureCoordinates = uv;
        }
    }
}
