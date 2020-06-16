using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GUITestbed.DataHandlers.Fox1.Objects;
using GUITestbed.Rendering;

namespace GUITestbed.Rendering._3D
{
    public class RunwayObject
    {
        public VertexPositionNormalTexture[] Verts;
        public short[] Indices;
        public Effect fx;
        public Texture2D texture;

        public RasterizerState rs;

        public RunwayObject()
        {
            rs = new RasterizerState();
            rs.CullMode = CullMode.None;
        }

        public void UpdateLighting(Vector4 Ambient, Vector4 Sun, Vector4 sunpos)
        {
            fx.Parameters["AmbientColour"].SetValue(new Vector3(Ambient.X, Ambient.Y, Ambient.Z));
            fx.Parameters["Texture"].SetValue(texture);
            fx.Parameters["SunDirection"].SetValue(new Vector3(sunpos.X, sunpos.Y, sunpos.Z));
            fx.Parameters["FogColour"].SetValue(new Vector3(Sun.X, Sun.Y, Sun.Z));
            fx.Parameters["FogStart"].SetValue(18000.0f);
            fx.Parameters["FogEnd"].SetValue(42000.0f);
        }

        public void Draw(Camera camera)
        {

            Game1.Instance.GraphicsDevice.RasterizerState = rs;
            Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            fx.Parameters["Projection"].SetValue(camera.Projection);
            fx.Parameters["View"].SetValue(camera.View);
            fx.Parameters["World"].SetValue(Matrix.Identity);
            fx.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.Identity)));
            fx.Parameters["ViewVector"].SetValue(camera.Transform.Translation);

            foreach (EffectPass pass in fx.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Verts, 0, Verts.Length, Indices, 0, Indices.Length/3, VertexPositionNormalTexture.VertexDeclaration);
            }

            //DebugLineDraw.DrawLine(Verts[0].Position, Verts[0].Position + (Vector3.Up * 100), Color.Red);
            //DebugLineDraw.DrawLine(Verts[1].Position, Verts[1].Position + (Vector3.Up * 100), Color.Red);
            //DebugLineDraw.DrawLine(Verts[2].Position, Verts[2].Position + (Vector3.Up * 100), Color.Red);
            //DebugLineDraw.DrawLine(Verts[3].Position, Verts[3].Position + (Vector3.Up * 100), Color.Red);

        }
    }
}
