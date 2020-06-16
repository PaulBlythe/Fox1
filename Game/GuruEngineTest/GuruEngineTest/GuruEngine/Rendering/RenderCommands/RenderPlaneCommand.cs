using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using GuruEngine.World;

namespace GuruEngine.Rendering.RenderCommands
{
    public class RenderPlaneCommand : RenderCommand
    {
        BasicEffect basicEffect;
        VertexPositionColor[] dv;


        public RenderPlaneCommand(Matrix w, Vector3 position, float size, Color color)
        {
            OwnerDraw = true;

            basicEffect = new BasicEffect(Renderer.GetGraphicsDevice());
            basicEffect.TextureEnabled = false;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            blendstate = BlendState.Opaque;

            World = w;

            VertexPositionColor v1 = new VertexPositionColor();
            v1.Color = color;
            v1.Position = new Vector3(-size, 0, -size);

            VertexPositionColor v2 = new VertexPositionColor();
            v2.Color = color;
            v2.Position = new Vector3(size, 0, -size);

            VertexPositionColor v3 = new VertexPositionColor();
            v3.Color = color;
            v3.Position = new Vector3(size, 0, size);

            VertexPositionColor v4 = new VertexPositionColor();
            v4.Color = color;
            v4.Position = new Vector3(-size, 0, size);

            List<VertexPositionColor> verts = new List<VertexPositionColor>();
            verts.Add(v1);
            verts.Add(v2);
            verts.Add(v4);

            verts.Add(v2);
            verts.Add(v3);
            verts.Add(v4);
            dv = verts.ToArray();

        }

        public override void PreRender(GraphicsDevice dev)
        {
        }

        public override void Draw(GraphicsDevice dev)
        {
            basicEffect.GraphicsDevice.BlendState = BlendState.Opaque;
            basicEffect.World = World;
            basicEffect.View = WorldState.GetWorldState().View;
            basicEffect.Projection = WorldState.GetWorldState().Projection;

            foreach (EffectPass p in basicEffect.CurrentTechnique.Passes)
            {
                p.Apply();
                Renderer.GetGraphicsDevice().DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, dv, 0, 2);
            }
        }
    }
}
