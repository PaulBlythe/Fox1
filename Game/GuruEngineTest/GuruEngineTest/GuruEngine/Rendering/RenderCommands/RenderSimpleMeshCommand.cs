using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace GuruEngine.Rendering.RenderCommands
{
    public class RenderSimpleMeshCommand : RenderCommand
    {
        public VertexPositionNormalTexture[] Verts;
        public short[] Indices;

        public RenderSimpleMeshCommand(String effect, String technique, int Texid)
        {
            OwnerDraw = false;
            Shader = Renderer.GetShaderName(effect);
            ShaderTechnique = technique;
            SamplerStateID = Renderer.MapBoolsToSamplerState(true, true, false);
            PType = PrimitiveType.TriangleList;
            MType = MeshType.IndexedPrimitives;
            textures = new int[1];
            textures[0] = Texid;

            Variables.Add(ShaderVariables.World);
            Variables.Add(ShaderVariables.SunDirection);
            Variables.Add(ShaderVariables.View);
            Variables.Add(ShaderVariables.Projection);
            Variables.Add(ShaderVariables.ViewVector);
            Variables.Add(ShaderVariables.WorldInverseTranspose);
            Variables.Add(ShaderVariables.SunColour);
            Variables.Add(ShaderVariables.AmbientColour);
            Variables.Add(ShaderVariables.Lit);
            Variables.Add(ShaderVariables.MoonLit);
            Variables.Add(ShaderVariables.Texture01);

        }

        public override void PreRender(GraphicsDevice dev)
        {
           
        }

        public override void Draw(GraphicsDevice dev)
        {
        }
    }
}
