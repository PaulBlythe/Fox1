using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Assets;
using GuruEngine.ECS.Components.World;
using GuruEngine.Rendering;
using GuruEngine.DebugHelpers;
using GuruEngine.Physics.Aircraft;
using GuruEngine.Rendering.RenderCommands;
using GuruEngine.Rendering.Objects;
using GuruEngine.World.Weather;
using GuruEngine.Maths;

namespace GuruEngine.World.Developer
{
    public class GroundPlane : WorldItem
    {
        RenderCommandSet GeometrySet;
        int TexID;

        public GroundPlane()
        {
            GeometrySet = new RenderCommandSet();
            GeometrySet.RenderPass = (int)RenderPasses.Geometry;
            GeometrySet.blend = BlendState.Opaque;
            GeometrySet.DS = DepthStencilState.Default;
            GeometrySet.RS = RasteriserStates.NormalNoCull;

            String normalpath = FilePaths.DataPath + @"Textures\grass1.png";
            AssetManager.AddTextureToQue(normalpath);
            TexID = normalpath.GetHashCode();

            Vector3 tpos = new Vector3(0, -0.0001f, 0);
            TriangleMesh t = Geometry.Geometry.GeneratePlane(tpos, 10000, 50);

            RenderSimpleMeshCommand r = new RenderSimpleMeshCommand("Textured", "Textured", TexID);
            r.vbuffer = new VertexBuffer(Renderer.GetGraphicsDevice(), VertexPositionNormalTexture.VertexDeclaration, t.Vertices.Length, BufferUsage.WriteOnly);
            r.vbuffer.SetData<VertexPositionNormalTexture>(t.Vertices);
            r.BaseVertex = 0;
            r.StartIndex = 0;
            r.StartVertex = 0;
            r.blendstate = BlendState.Opaque;
            r.PrimitiveCount = t.Indices.Length / 3;

            r.ibuffer = new IndexBuffer(Renderer.GetGraphicsDevice(), IndexElementSize.SixteenBits, t.Indices.Length, BufferUsage.WriteOnly);
            r.ibuffer.SetData<short>(t.Indices);
            r.World = Matrix.Identity;
            GeometrySet.World = Matrix.Identity;
            GeometrySet.Commands.Add(r);

        }

        public override void Update(WorldState state)
        {
            Renderer.AddRenderCommand(GeometrySet);
        }
    }
}
