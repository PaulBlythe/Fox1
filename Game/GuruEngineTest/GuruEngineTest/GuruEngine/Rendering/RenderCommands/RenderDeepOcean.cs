using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using GuruEngine.Rendering;
using GuruEngine.Assets;

namespace GuruEngine.Rendering.RenderCommands
{
    class RenderDeepOcean : RenderCommand
    {


        public RenderDeepOcean(int Width, int Height)
        {
            Shader = Renderer.GetShaderName("Ocean");
            ShaderTechnique = "Main";
            World = Matrix.Identity;
            PType = PrimitiveType.TriangleList;
            MType = MeshType.IndexedPrimitives;
            BaseVertex = 0;
            StartVertex = 0;
            StartIndex = 0;
            PrimitiveCount = (Width - 1) * (Height - 1) * 2;
            InstanceCount = 0;
            declaration = VertexPositionNormalTexture.VertexDeclaration;
            blendstate = BlendState.AlphaBlend;
            SamplerStateID = Renderer.MapBoolsToSamplerState(true, true, false);
            

            Variables.Add(ShaderVariables.World);
            Variables.Add(ShaderVariables.AmbientColour);
            Variables.Add(ShaderVariables.View);
            Variables.Add(ShaderVariables.Projection);
            Variables.Add(ShaderVariables.ViewVector);
            Variables.Add(ShaderVariables.SunDirection);
            Variables.Add(ShaderVariables.SunColour);
            Variables.Add(ShaderVariables.Texture01);
            Variables.Add(ShaderVariables.EnvironmentMap);
            Variables.Add(ShaderVariables.WorldViewProjection);
            Variables.Add(ShaderVariables.Time);
            Variables.Add(ShaderVariables.WorldInverseTranspose);
            //Variables.Add(ShaderVariables.Shadows);

            String normalpath = FilePaths.DataPath + @"Textures\wave1.png";

            AssetManager.AddTextureToQue(normalpath);
            textures = new int[] {normalpath.GetHashCode()};

        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="dev"></param>
        public override void Draw(GraphicsDevice dev)
        {

        }

        public override void PreRender(GraphicsDevice dev)
        {
            
        }
    }
}
