using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using GuruEngine.Rendering;
using GuruEngine.Algebra;
using GuruEngine.World;

namespace GuruEngine.Rendering.RenderCommands
{
    public class RenderMoonCommand : RenderCommand
    {
        RenderTarget2D rt;
        Model Mesh;
        Effect effect;
        Texture2D texture;
        Matrix perspective;
        Matrix view;
        RasterizerState normal_rs;
        BasicEffect basicEffect;
        SpriteBatch spriteBatch;

        public RenderMoonCommand(Model mesh, Effect fx, Texture2D tex)
        {
            rt = new RenderTarget2D(Renderer.GetGraphicsDevice(), 256, 256);
            Mesh = mesh;
            effect = fx;
            texture = tex;

            perspective = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1, 1, 100000);
            view = Matrix.CreateLookAt(new Vector3(0, 0, -270), Vector3.Zero, Vector3.Up);

            normal_rs = new RasterizerState();
            normal_rs.CullMode = CullMode.CullClockwiseFace;
            normal_rs.DepthBias = 0;
            normal_rs.DepthClipEnable = true;
            normal_rs.FillMode = FillMode.Solid;
            normal_rs.ScissorTestEnable = false;
            normal_rs.MultiSampleAntiAlias = false;

            OwnerDraw = true;
            basicEffect = new BasicEffect(Renderer.GetGraphicsDevice());
            basicEffect.TextureEnabled = true;
            basicEffect.VertexColorEnabled = false;

            spriteBatch = new SpriteBatch(Renderer.GetGraphicsDevice());

#if DEBUG
            Renderer.Instance.RegisteredTextures.Add("Moon", rt);
#endif
        }

        public override void PreRender(GraphicsDevice dev)
        {
            dev.SetRenderTarget(rt);
            dev.Clear(Color.Transparent);

            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.Identity)));
            effect.Parameters["Projection"].SetValue(perspective);
            effect.Parameters["sun_height"].SetValue(WorldState.GetWorldState().SunDirection.Y);

            //double angle = WorldState.GetWorldState().MoonPhaseAngle;
            //Matrix mp = Matrix.CreateRotationY((float)angle);
            //Vector3 ld = Vector3.Transform(Vector3.Backward, mp);

            Vector3D dir = WorldState.GetWorldState().SunPosition - WorldState.GetWorldState().MoonPosition;
            Vector3 ld = dir.ToVector3F();

            effect.Parameters["LightDirection"].SetValue(ld);
            effect.Parameters["ModelTexture"].SetValue(texture);

            dev.RasterizerState = normal_rs;
            dev.DepthStencilState = DepthStencilState.Default;
            dev.BlendState = BlendState.Opaque;

            foreach (ModelMesh mesh in Mesh.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    mesh.Draw();
                }
            }

            dev.SetRenderTarget(null);

        }

        public override void Draw(GraphicsDevice dev)
        {
            BlendState bs = new BlendState();
            bs.ColorBlendFunction = BlendFunction.Add;
            bs.AlphaSourceBlend = Blend.SourceColor;
            bs.ColorSourceBlend = Blend.SourceColor;
            bs.AlphaDestinationBlend = Blend.One;
            bs.ColorDestinationBlend = Blend.One;

            Matrix invertY = Matrix.CreateScale(1, -1, 1);

            basicEffect.World = invertY;
            basicEffect.View = Matrix.Identity;
            basicEffect.Projection = WorldState.GetWorldState().Projection;


            spriteBatch.Begin(SpriteSortMode.Immediate, bs, null, DepthStencilState.None, RasterizerState.CullNone, basicEffect);

            Vector3 textPosition = WorldState.GetWorldState().MoonPosition.ToVector3F();
            textPosition.Normalize();
            textPosition *= 3000.0f;

            Vector3 viewSpaceTextPosition = Vector3.Transform(textPosition, WorldState.GetWorldState().View * invertY);


            Vector2 textOrigin = new Vector2(128, 128);

            float tilt = MathHelper.ToRadians((float)WorldState.GetWorldState().MoonTilt);

            spriteBatch.Draw(rt, new Vector2(viewSpaceTextPosition.X, viewSpaceTextPosition.Y), null, Color.White, tilt, textOrigin, 1, 0, viewSpaceTextPosition.Z);


            spriteBatch.End();
        }
    }
}
