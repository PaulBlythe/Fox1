using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GuruEngine.Text
{
    public class GlyphBatch
    {
        GraphicsDevice Device;
        Matrix projection;
        Effect current_effect;
        List<VertexPositionTexture> verts = new List<VertexPositionTexture>();
        Texture2D Texture;
        public Color DrawColour;
        RasterizerState stat = new RasterizerState();
        SamplerState ss;

        public GlyphBatch(GraphicsDevice device, Texture2D texture, Effect effect, Color color)
        {
            Device = device;

            projection = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, 0.1f, 1);
            current_effect = effect;
            Texture = texture;
            DrawColour = color;
            stat.CullMode = CullMode.None;
            ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            ss.Filter = TextureFilter.Linear;
        }
        public void Flush()
        {
            if (verts.Count == 0)
                return;

            current_effect.Parameters["fgColor"].SetValue(DrawColour.ToVector4());
            current_effect.Parameters["Texture"].SetValue(Texture);
            current_effect.Parameters["Projection"].SetValue(projection);
            Device.SamplerStates[0] = ss;
            Device.RasterizerState = stat;

            VertexPositionTexture[] lverts = verts.ToArray();
            Device.BlendState = BlendState.NonPremultiplied;
            Device.DepthStencilState = DepthStencilState.None;

            foreach (EffectPass pass in current_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, lverts, 0, verts.Count / 3);
            }
            //verts.Clear();
        }

        public void StartSprite(Texture2D Texture, Effect effect, Color color)
        {
            Flush();
            verts.Clear();
            current_effect = effect;
            DrawColour = color;
        }

        public void End()
        {
            Flush();

        }

        public void Draw(Vector4 src, Vector4 dst)
        {
            VertexPositionTexture v1 = new VertexPositionTexture();
            v1.Position = new Vector3(dst.X, dst.Y, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X, src.Y);
            verts.Add(v1);

            v1 = new VertexPositionTexture();
            v1.Position = new Vector3(dst.X + dst.Z, dst.Y, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X + src.Z, src.Y);
            verts.Add(v1);

            v1 = new VertexPositionTexture();
            v1.Position = new Vector3(dst.X, dst.Y + dst.W, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X, src.Y + src.W);
            verts.Add(v1);

            v1 = new VertexPositionTexture();
            v1.Position = new Vector3(dst.X + dst.Z, dst.Y, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X + src.Z, src.Y);
            verts.Add(v1);

            v1 = new VertexPositionTexture();
            v1.Position = new Vector3(dst.X + dst.Z, dst.Y + dst.W, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X + src.Z, src.Y + src.W);
            verts.Add(v1);

            v1 = new VertexPositionTexture();
            v1.Position = new Vector3(dst.X, dst.Y + dst.W, -0.5f);
            v1.TextureCoordinate = new Vector2(src.X, src.Y + src.W);
            verts.Add(v1);

        }

    }
}

