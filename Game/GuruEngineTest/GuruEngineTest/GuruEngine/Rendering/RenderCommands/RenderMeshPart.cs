using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Assets;
using GuruEngine.ECS.Components.Mesh;

namespace GuruEngine.Rendering.RenderCommands
{
    public class RenderMeshPart : RenderCommand
    {
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

        public int Setup(MultiMeshComponent mp, MeshMaterialLibrary mat, FaceGroup f)
        {
            if (mat.Materials[f.Material].Glass)
            {
                Shader = Renderer.GetShaderName("Glass"); 
                ShaderTechnique = "Glass";
                World = mp.Animation * mp.world;
                material = null;
                IsGlass = true;
                PType = PrimitiveType.TriangleList;
                MType = MeshType.IndexedPrimitives;
                BaseVertex = 0;
                StartVertex = f.StartVertex;
                StartIndex = f.StartFace * 3;
                PrimitiveCount = f.FaceCount;
                InstanceCount = 0;
                declaration = VertexPositionNormalTexture.VertexDeclaration;
                vbuffer = mp.mesh.vbuffer;
                ibuffer = mp.mesh.ibuffer;
                blendstate = BlendState.NonPremultiplied;


                SamplerStateID = Renderer.MapBoolsToSamplerState(false, false, false);
                Variables.Add(ShaderVariables.WorldViewProjection);
                Variables.Add(ShaderVariables.World);
                Variables.Add(ShaderVariables.EnvironmentMap);
                Variables.Add(ShaderVariables.WorldInverseTranspose);
                Variables.Add(ShaderVariables.ViewVector);
                Variables.Add(ShaderVariables.SunDirection);
                return 0;
            }
            
            else
            {
                Shader = Renderer.GetShaderName("MeshPartShader");
                ShaderTechnique = "Textured";
                World = mp.Animation * mp.world;
                material = mat.Materials[f.Material];
                PType = PrimitiveType.TriangleList;
                MType = MeshType.IndexedPrimitives;
                BaseVertex = 0;
                StartVertex = f.StartVertex;
                StartIndex = f.StartFace * 3;
                PrimitiveCount = f.FaceCount;
                CastsShadows = true;
                InstanceCount = 0;
                declaration = VertexPositionNormalTexture.VertexDeclaration;
                vbuffer = mp.mesh.vbuffer;
                ibuffer = mp.mesh.ibuffer;
                blendstate = BlendState.Opaque;
                if (mat.Materials[f.Material].tfBlend)
                    blendstate = BlendState.NonPremultiplied;
                if (mat.Materials[f.Material].tfBlendAdd)
                    blendstate = BlendState.Additive;

                SamplerStateID = Renderer.MapBoolsToSamplerState(((MeshPartMaterial)material).tfWrapX, ((MeshPartMaterial)material).tfWrapY, false);

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
            }
            bool isNight = mat.Materials[f.Material].tname.Contains("night");
            if (isNight)
                return 999;
            if (mat.Materials[f.Material].tfBlendAdd)
                return 998;
            int set = 2;
            if (mat.Materials[f.Material].Sort)
            {
                set = 1;
                if (Renderer.IsForward())
                    return set;
            }
            if (mat.Materials[f.Material].tfDoubleSided)
                set = 3;
            if (mat.Materials[f.Material].tfBlend)
                set = 4;
            return set;
        }


    }
}
