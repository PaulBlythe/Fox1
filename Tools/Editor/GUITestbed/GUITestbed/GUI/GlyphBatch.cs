using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.GUI
{
    public class GlyphBatch
    {
        GraphicsDevice Device;
        Effect current_effect;
        List<VertexPositionColorTexture> Vertices = new List<VertexPositionColorTexture>();
        Texture2D Texture;
        RasterizerState stat = new RasterizerState();

        public GlyphBatch(GraphicsDevice dev, Texture2D tex, Effect effect)
        {
            Device = dev;
            Texture = tex;
            current_effect = effect;
            stat.CullMode = CullMode.None;
            current_effect.Parameters["Texture"].SetValue(tex);
        }

        public void Begin()
        {
            Vertices.Clear();
        }

        public void End()
        {
            Device.BlendState = BlendState.NonPremultiplied;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = stat;
            VertexPositionColorTexture[] v = Vertices.ToArray();

            foreach (EffectPass pass in current_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, v, 0, v.Length / 3);
            }
        }

        public void Draw(Vector4 src, Vector4 dst, Color colour)
        {
            VertexPositionColorTexture v1 = new VertexPositionColorTexture();
            v1.Position = new Vector3(dst.X, dst.Y, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X, src.Y);
            v1.Color = colour;
            Vertices.Add(v1);

            v1 = new VertexPositionColorTexture();
            v1.Position = new Vector3(dst.X + dst.Z, dst.Y, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X + src.Z, src.Y);
            v1.Color = colour;
            Vertices.Add(v1);

            v1 = new VertexPositionColorTexture();
            v1.Position = new Vector3(dst.X, dst.Y + dst.W, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X, src.Y + src.W);
            v1.Color = colour;
            Vertices.Add(v1);

            v1 = new VertexPositionColorTexture();
            v1.Position = new Vector3(dst.X + dst.Z, dst.Y, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X + src.Z, src.Y);
            v1.Color = colour;
            Vertices.Add(v1);

            v1 = new VertexPositionColorTexture();
            v1.Position = new Vector3(dst.X + dst.Z, dst.Y + dst.W, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X + src.Z, src.Y + src.W);
            v1.Color = colour;
            Vertices.Add(v1);

            v1 = new VertexPositionColorTexture();
            v1.Position = new Vector3(dst.X, dst.Y + dst.W, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X, src.Y + src.W);
            v1.Color = colour;
            Vertices.Add(v1);

        }
    }
}
