using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Rendering.RenderCommands;

namespace GuruEngine.World.Developer
{
    public class WindTunnel :  WorldItem
    {
        RenderCommandSet rendercommand;

        public WindTunnel()
        {
            rendercommand = new RenderCommandSet();
            rendercommand.IsStaticMesh = false;
            rendercommand.DS = DepthStencilState.Default;
            rendercommand.mesh = null;
            rendercommand.RS = RasteriserStates.NormalNoCull;
            rendercommand.RenderPass = RenderPasses.Terrain;
            rendercommand.fx = null;
            rendercommand.blend = BlendState.Opaque;

            RenderPlaneCommand rmc = new RenderPlaneCommand(Matrix.Identity, Vector3.Zero, 10000, Color.DarkGreen);
            rendercommand.Commands.Add(rmc);
        }

        public override void Update(WorldState state)
        {
            Renderer.AddRenderCommand(rendercommand);
        }
    }
}
