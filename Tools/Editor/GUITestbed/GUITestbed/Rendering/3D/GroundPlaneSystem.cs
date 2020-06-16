using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.Rendering._3D
{
    public class GroundPlaneSystem
    {
        Texture2D texture;
        Effect fx;
        RasterizerState rs;
        VertexPositionNormalTexture[] Verts;
        short[] Indices;

        public GroundPlaneSystem(float alt, float textuescale, float size)
        {
            alt -= 1;
            Verts = new VertexPositionNormalTexture[4];

            VertexPositionNormalTexture v = new VertexPositionNormalTexture();
            v.Position = new Vector3(-size, alt, -size);
            v.Normal = Vector3.Up;
            v.TextureCoordinate = new Vector2(-textuescale, -textuescale);
            Verts[0] = v;

            v = new VertexPositionNormalTexture();
            v.Position = new Vector3(size, alt, -size);
            v.Normal = Vector3.Up;
            v.TextureCoordinate = new Vector2(textuescale, -textuescale);
            Verts[1] = v;

            v = new VertexPositionNormalTexture();
            v.Position = new Vector3(-size, alt, size);
            v.Normal = Vector3.Up;
            v.TextureCoordinate = new Vector2(-textuescale, textuescale);
            Verts[2] = v;

            v = new VertexPositionNormalTexture();
            v.Position = new Vector3(size, alt, size);
            v.Normal = Vector3.Up;
            v.TextureCoordinate = new Vector2(textuescale, textuescale);
            Verts[3] = v;

            Indices = new short[6];
            Indices[0] = 0;
            Indices[1] = 1;
            Indices[2] = 2;

            Indices[3] = 1;
            Indices[4] = 3;
            Indices[5] = 2;

            rs = new RasterizerState();
            rs.DepthBias = 1;
            rs.CullMode = CullMode.None;
        }

        public void Load()
        {
            texture = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/grass1");
            fx = Game1.Instance.Content.Load<Effect>("Shaders/DiffuseFog");


        }


        public void UpdateLighting(Vector4 Ambient, Vector4 Sun, Vector4 sunpos)
        {
            fx.Parameters["FogStart"].SetValue(18000.0f);
            fx.Parameters["FogEnd"].SetValue(42000.0f);
            fx.Parameters["AmbientColour"].SetValue(new Vector3(Ambient.X, Ambient.Y, Ambient.Z));
            fx.Parameters["Texture"].SetValue(texture);
            fx.Parameters["SunDirection"].SetValue(new Vector3(sunpos.X, sunpos.Y, sunpos.Z));
            fx.Parameters["FogColour"].SetValue(new Vector3(Sun.X, Sun.Y, Sun.Z));
        }

        public void Draw(Camera camera)
        {
            Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.Instance.GraphicsDevice.RasterizerState = rs;
            fx.Parameters["Projection"].SetValue(camera.Projection);
            fx.Parameters["View"].SetValue(camera.View);
            fx.Parameters["World"].SetValue(Matrix.Identity);
            fx.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.Identity)));
            fx.Parameters["ViewVector"].SetValue(camera.Transform.Translation);

            foreach (EffectPass pass in fx.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Verts, 0, 4, Indices, 0, 2, VertexPositionNormalTexture.VertexDeclaration);
            }
        }
    }
}
