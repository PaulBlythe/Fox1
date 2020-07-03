using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.World.Weather;

namespace GuruEngine.Rendering.RenderCommands
{
    public class RenderCirrusCommand : RenderCommand
    {
        Effect effect;
        
        public RenderCirrusCommand()
        {
            Shader = Renderer.GetShaderName("Cirrus");
            ShaderTechnique = "BasicColorDrawing";
            World = Matrix.Identity;


        }

        public override void PreRender(GraphicsDevice dev)
        {
        }

        public override void Draw(GraphicsDevice dev)
        {
            effect.Parameters["cirrus_height"].SetValue(WeatherManager.GetCirrusHeight());
            effect.Parameters["cirrus_layer_thickness"].SetValue(WeatherManager.GetCirrusAmount());

            dev.RasterizerState = Renderer.GetRasteriser(RasteriserStates.CullCounterclockwise);

        }
    }
}
