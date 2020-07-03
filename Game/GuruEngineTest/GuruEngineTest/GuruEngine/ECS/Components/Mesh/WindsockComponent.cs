using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.Assets;
using GuruEngine.ECS.Components.World;
using GuruEngine.Rendering;
using GuruEngine.DebugHelpers;
using GuruEngine.Physics.Aircraft;
using GuruEngine.Rendering.RenderCommands;
using GuruEngine.Rendering.Objects;
using GuruEngine.World.Weather;

//( Class WindsockComponent )
//( Type AnimatedMesh )
//( Connection WorldTransform Transform )
//( Parameter Float Height )
//( Parameter Bool Lit )

namespace GuruEngine.ECS.Components.Mesh
{
    public class WindsockComponent : ECSGameComponent
    {
        float Height;
        bool Lit;
        WorldTransform transform;
        int TexID = -1;
        int TexID2 = -1;
        Vector3 tpos;

        RenderCommandSet GeometrySet = null;

        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            WindsockComponent other = new WindsockComponent();
            other.Height = Height;
            other.Lit = Lit;
            other.transform = transform;
            return (ECSGameComponent)other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                
            }
            else
            {
                switch (parts[1])
                {
                    case "Root":
                        {
                            string[] objects = parts[2].Split(':');
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    case "WorldTransform":
                        {
                            string[] objects = parts[2].Split(':');
                            if (objects[0] != "")
                                transform = (WorldTransform)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;


                    default:
                        throw new Exception("GameComponent::Windsock:: Unknown direct connection request to " + parts[1]);
                }
            }
        }

        public override void DisConnect()
        {
            transform = null;
        }

        public override object GetContainedObject(string type)
        {
            return null;
        }

        public override Texture2D GetOffscreenTexture()
        {
            return null;
        }

        public override void HandleEvent(string evt)
        {
        }

        public override void Load(ContentManager content)
        {
            GeometrySet = new RenderCommandSet();
            GeometrySet.RenderPass = (int)RenderPasses.Geometry;
            GeometrySet.blend = BlendState.Opaque;
            GeometrySet.DS = DepthStencilState.Default;
            GeometrySet.RS = RasteriserStates.NormalNoCull;

            String normalpath = FilePaths.DataPath + @"Textures\windsock.png";
            AssetManager.AddTextureToQue(normalpath);
            TexID = normalpath.GetHashCode();
            normalpath = FilePaths.DataPath + @"Textures\Wood01.png";
            AssetManager.AddTextureToQue(normalpath);
            TexID2 = normalpath.GetHashCode();

            tpos = new Vector3(0, Height, 0.6f);
            TriangleMesh t = Geometry.Geometry.GenerateTaperedCylinder(Vector3.Zero, 0.6f, 0.3f, 4, 16, 7);

            RenderSimpleMeshCommand r = new RenderSimpleMeshCommand("Windsock", "Textured", TexID);
            r.vbuffer = new VertexBuffer(Renderer.GetGraphicsDevice(), VertexPositionNormalTexture.VertexDeclaration, t.Vertices.Length, BufferUsage.WriteOnly);
            r.vbuffer.SetData<VertexPositionNormalTexture>(t.Vertices);
            r.BaseVertex = 0;
            r.StartIndex = 0;
            r.StartVertex = 0;
            r.PrimitiveCount = t.Indices.Length / 3;

            r.ibuffer = new IndexBuffer(Renderer.GetGraphicsDevice(), IndexElementSize.SixteenBits, t.Indices.Length, BufferUsage.WriteOnly);
            r.ibuffer.SetData<short>(t.Indices);
            r.Variables.Add(ShaderVariables.WindSpeed);
            r.Variables.Add(ShaderVariables.Time);

            GeometrySet.Commands.Add(r);

            t = Geometry.Geometry.GeneratePost(Height, 0.1f);

            r = new RenderSimpleMeshCommand("Textured", "Textured", TexID2);
            r.vbuffer = new VertexBuffer(Renderer.GetGraphicsDevice(), VertexPositionNormalTexture.VertexDeclaration, t.Vertices.Length, BufferUsage.WriteOnly);
            r.vbuffer.SetData<VertexPositionNormalTexture>(t.Vertices);
            r.BaseVertex = 0;
            r.StartIndex = 0;
            r.StartVertex = 0;
            r.PrimitiveCount = t.Indices.Length / 3;

            r.ibuffer = new IndexBuffer(Renderer.GetGraphicsDevice(), IndexElementSize.SixteenBits, t.Indices.Length, BufferUsage.WriteOnly);
            r.ibuffer.SetData<short>(t.Indices);


            GeometrySet.Commands.Add(r);

        }

        public override void ReConnect(GameObject other)
        {
            WindsockComponent otherC = (WindsockComponent)other.FindGameComponentByName(Name);

            otherC.Lit = Lit;
            otherC.Height = Height;        
            otherC.Parent = other;
            otherC.transform = (WorldTransform)other.FindGameComponentByName(transform.Name);
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Height")
            {
                Height = float.Parse(Value);
            }
            if (Name == "Lit")
            {
                Lit = bool.Parse(Value);
            }
        }

        public override void Update(float dt)
        {
            Matrix w = transform.GetMatrix();
            GeometrySet.World = w;
            CopyMatrix(ref GeometrySet.Commands[1].World, ref w);

            float ws = WeatherManager.GetWindSpeed();
            ws = 1 - (Math.Min(ws, 25) / 25.0f);
            ws *= MathHelper.PiOver2;
            Matrix t = Matrix.CreateRotationX(ws) * Matrix.CreateTranslation(tpos);
            CopyMatrix(ref GeometrySet.Commands[0].World, ref t);
            Renderer.AddRenderCommand(GeometrySet);
        }

        private void Draw()
        {
            
        }

        #endregion

    }
}
