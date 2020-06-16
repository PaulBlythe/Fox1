using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Rendering.RenderCommands;
using GuruEngine.Assets;

namespace GuruEngine.World.Items
{
    public class Stars:WorldItem
    {
        int GUID;
        RenderCommandSet rendercommand;
        bool mapped = false;

        public Stars()
        {
            String day = FilePaths.DataPath + @"Textures\star.png";
            AssetManager.AddTextureToQue(day);
            GUID = day.GetHashCode();
        }

        public override void Update(WorldState state)
        {
            if (!mapped)
            {
                Texture2D day = AssetManager.Texture(GUID);
                if (day == null)
                    return;

                rendercommand = new RenderCommandSet();
                rendercommand.IsStaticMesh = false;
                rendercommand.DS = DepthStencilState.None;
                rendercommand.mesh = null;
                rendercommand.RS = RasteriserStates.NoDepthNoCull;
                rendercommand.RenderPass = RenderPasses.Ephemeris;
                rendercommand.fx = null;
                rendercommand.blend = BlendState.AlphaBlend;
                

                RenderStarsCommand rmc = new RenderStarsCommand(day);
                rendercommand.Commands.Add(rmc);

                

                mapped = true;
            }
            Renderer.AddRenderCommand(rendercommand);
        }
    }
}
