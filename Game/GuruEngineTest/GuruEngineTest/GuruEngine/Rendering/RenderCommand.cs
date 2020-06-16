using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Assets;

namespace GuruEngine.Rendering
{
    public abstract class RenderCommand
    {
        public String Shader;
        public String ShaderTechnique;
        public List<ShaderVariables> Variables = new List<ShaderVariables>();
        public Matrix World;
        public PrimitiveType PType;
        public MeshType MType;
        public int BaseVertex;
        public int StartVertex;
        public int StartIndex;
        public int PrimitiveCount;
        public int InstanceCount;
        public VertexDeclaration declaration;
        public VertexBuffer vbuffer;
        public IndexBuffer ibuffer;
        public Material material = null;
        public int SamplerStateID;
        public BlendState blendstate = BlendState.AlphaBlend;
        public int[] textures;
        public bool OwnerDraw = false;
        public bool CastsShadows = false;
        public bool IsGlass = false;

        public abstract void Draw(GraphicsDevice dev);
        public abstract void PreRender(GraphicsDevice dev);
    }
}
