using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using GuruEngine.DebugHelpers;
using GuruEngine.World;
using GuruEngine.Helpers;
using GuruEngine.Assets;
using GuruEngine.Rendering.Particles;

#if DEBUG
using GuruEngineTest;
#endif

namespace GuruEngine.Rendering
{
    public class Renderer
    {
        public static Renderer Instance;
        public RenderSettings renderSettings = new RenderSettings();
        public Dictionary<RasteriserStates, RasterizerState> rasterStates = new Dictionary<RasteriserStates, RasterizerState>();
        public Dictionary<int, SamplerState> samplerStates = new Dictionary<int, SamplerState>();

        #region Useful stuff
        public Texture2D RandomVectors;
        public RenderInterface current;
        public Random rand = new Random();
        public Vector4[] UniformSpaceVectors;
        #endregion

        GraphicsDevice device;
        BufferManager bufferManager;
        LightManager lightManager;

        bool UsingForwardRenderer;


#if DEBUG
        RasterizerState normal_rs;
        
        GlobalDebugMenu gmenu;
        public Dictionary<String, Texture2D> RegisteredTextures = new Dictionary<string, Texture2D>();
        DebugLineDraw dld = new DebugLineDraw();
        BasicEffect debug_effect;
        SpriteBatch spriteBatch;
        WorldState worldstate = null;
        Effect depthDisplay;
#endif

        public Renderer(GraphicsDevice dev, bool Forward)
        {
            Instance = this;
            UsingForwardRenderer = Forward;
            bufferManager = new BufferManager();
            lightManager = new LightManager();

            device = dev;
            if (Forward)
            {
                current = new ForwardRenderer(dev);
            }
            else
            {
                current = new Deferred.DeferredRender(dev);
            }

            // Start with 14 uniformly distributed vectors.  We choose the 8 corners of the cube
            // and the 6 center points along each cube face.  We always alternate the points on 
            // opposites sides of the cubes.  This way we still get the vectors spread out even
            // if we choose to use less than 14 samples.
            UniformSpaceVectors = new Vector4[14];

            // 8 cube corners
            UniformSpaceVectors[0] = new Vector4(+1.0f, +1.0f, +1.0f, 0.0f);
            UniformSpaceVectors[1] = new Vector4(-1.0f, -1.0f, -1.0f, 0.0f);

            UniformSpaceVectors[2] = new Vector4(-1.0f, +1.0f, +1.0f, 0.0f);
            UniformSpaceVectors[3] = new Vector4(+1.0f, -1.0f, -1.0f, 0.0f);

            UniformSpaceVectors[4] = new Vector4(+1.0f, +1.0f, -1.0f, 0.0f);
            UniformSpaceVectors[5] = new Vector4(-1.0f, -1.0f, +1.0f, 0.0f);

            UniformSpaceVectors[6] = new Vector4(-1.0f, +1.0f, -1.0f, 0.0f);
            UniformSpaceVectors[7] = new Vector4(+1.0f, -1.0f, +1.0f, 0.0f);

            // 6 centers of cube faces
            UniformSpaceVectors[8] = new Vector4(-1.0f, 0.0f, 0.0f, 0.0f);
            UniformSpaceVectors[9] = new Vector4(+1.0f, 0.0f, 0.0f, 0.0f);

            UniformSpaceVectors[10] = new Vector4(0.0f, -1.0f, 0.0f, 0.0f);
            UniformSpaceVectors[11] = new Vector4(0.0f, +1.0f, 0.0f, 0.0f);

            UniformSpaceVectors[12] = new Vector4(0.0f, 0.0f, -1.0f, 0.0f);
            UniformSpaceVectors[13] = new Vector4(0.0f, 0.0f, +1.0f, 0.0f);

            for (int i = 0; i < 14; ++i)
            {
                // Create random lengths in [0.25, 1.0].
                float s = (float)((rand.NextDouble() * 0.75) + 0.25);

                Vector4 v = s * UniformSpaceVectors[i];
                v.Normalize();
                UniformSpaceVectors[i] = v;
            }
        }

        

        public void Initialise()
        {
            #region Setup rasteriser state library

            RasterizerState normal_rs;
            normal_rs = new RasterizerState();
            normal_rs.CullMode = CullMode.CullClockwiseFace;
            normal_rs.DepthBias = 0;
            normal_rs.DepthClipEnable = true;
            normal_rs.FillMode = FillMode.Solid;
            normal_rs.ScissorTestEnable = false;
            normal_rs.MultiSampleAntiAlias = true;
            rasterStates.Add(RasteriserStates.Normal, normal_rs);

            RasterizerState nod_rs;
            nod_rs = new RasterizerState();
            nod_rs.CullMode = CullMode.CullCounterClockwiseFace;
            nod_rs.DepthBias = 0;
            nod_rs.DepthClipEnable = false;
            nod_rs.FillMode = FillMode.Solid;
            nod_rs.ScissorTestEnable = false;
            nod_rs.MultiSampleAntiAlias = true;
            rasterStates.Add(RasteriserStates.NoDepth, nod_rs);

            RasterizerState wireframe_rs;
            wireframe_rs = new RasterizerState();
            wireframe_rs.CullMode = CullMode.CullCounterClockwiseFace;
            wireframe_rs.DepthBias = 0;
            wireframe_rs.DepthClipEnable = true;
            wireframe_rs.FillMode = FillMode.WireFrame;
            wireframe_rs.ScissorTestEnable = false;
            wireframe_rs.MultiSampleAntiAlias = true;
            rasterStates.Add(RasteriserStates.Wireframe, wireframe_rs);

            RasterizerState normal_nocull;
            normal_nocull = new RasterizerState();
            normal_nocull.CullMode = CullMode.None;
            normal_nocull.DepthBias = 0;
            normal_nocull.DepthClipEnable = true;
            normal_nocull.FillMode = FillMode.Solid;
            normal_nocull.ScissorTestEnable = false;
            normal_nocull.MultiSampleAntiAlias = true;
            rasterStates.Add(RasteriserStates.NormalNoCull, normal_nocull);

            RasterizerState nodepth_nocull;
            nodepth_nocull = new RasterizerState();
            nodepth_nocull.CullMode = CullMode.None;
            nodepth_nocull.DepthBias = 0;
            nodepth_nocull.DepthClipEnable = false;
            nodepth_nocull.FillMode = FillMode.Solid;
            nodepth_nocull.ScissorTestEnable = false;
            nodepth_nocull.MultiSampleAntiAlias = true;
            rasterStates.Add(RasteriserStates.NoDepthNoCull, nodepth_nocull);

            nodepth_nocull = new RasterizerState();
            nodepth_nocull.CullMode = CullMode.None;
            nodepth_nocull.DepthBias = 0;
            nodepth_nocull.DepthClipEnable = false;
            nodepth_nocull.FillMode = FillMode.Solid;
            nodepth_nocull.ScissorTestEnable = false;
            nodepth_nocull.MultiSampleAntiAlias = false;
            rasterStates.Add(RasteriserStates.ShadowMap, nodepth_nocull);

            nodepth_nocull = new RasterizerState();
            nodepth_nocull.CullMode = CullMode.CullCounterClockwiseFace;
            nodepth_nocull.DepthBias = 0;
            nodepth_nocull.DepthClipEnable = true;
            nodepth_nocull.FillMode = FillMode.Solid;
            nodepth_nocull.ScissorTestEnable = false;
            nodepth_nocull.MultiSampleAntiAlias = false;
            rasterStates.Add(RasteriserStates.CullCounterclockwise, nodepth_nocull);

            nodepth_nocull = new RasterizerState();
            nodepth_nocull.CullMode = CullMode.CullClockwiseFace;
            nodepth_nocull.DepthBias = 0;
            nodepth_nocull.DepthClipEnable = true;
            nodepth_nocull.FillMode = FillMode.Solid;
            nodepth_nocull.ScissorTestEnable = false;
            nodepth_nocull.MultiSampleAntiAlias = false;
            rasterStates.Add(RasteriserStates.CullClockwise, nodepth_nocull);
            #endregion

            #region Setup sampler state library
            for (int i = 0; i < 64; i++)
            {
                samplerStates.Add(i, MapIntToSamplerState(i));
            }
            #endregion

            #region Setup common structures
            VertexPositionTexture[] myVertices = new VertexPositionTexture[128 * 128];

            for (int x = 0; x < 128; x++)
                for (int y = 0; y < 128; y++)
                {
                    myVertices[x + y * 128].Position = new Vector3(y, 0, x);

                    myVertices[x + y * 128].TextureCoordinate.X = (float)x / 10.0f;
                    myVertices[x + y * 128].TextureCoordinate.Y = (float)y / 10.0f;

                }

            VertexBuffer OceanVB = new VertexBuffer(Renderer.GetGraphicsDevice(), VertexPositionTexture.VertexDeclaration, 128 * 128, BufferUsage.WriteOnly);
            OceanVB.SetData(myVertices);

            BufferManager.AddNamedVertexBuffer("OceanVB", OceanVB);

            //Index 
            short[] terrainIndices = new short[(128 - 1) * (128 - 1) * 6];
            for (short x = 0; x < 128 - 1; x++)
            {
                for (short y = 0; y < 128 - 1; y++)
                {
                    terrainIndices[(x + y * (128 - 1)) * 6] = (short)((x + 1) + (y + 1) * 128);
                    terrainIndices[(x + y * (128 - 1)) * 6 + 1] = (short)((x + 1) + y * 128);
                    terrainIndices[(x + y * (128 - 1)) * 6 + 2] = (short)(x + y * 128);

                    terrainIndices[(x + y * (128 - 1)) * 6 + 3] = (short)((x + 1) + (y + 1) * 128);
                    terrainIndices[(x + y * (128 - 1)) * 6 + 4] = (short)(x + y * 128);
                    terrainIndices[(x + y * (128 - 1)) * 6 + 5] = (short)(x + (y + 1) * 128);
                }
            }

            IndexBuffer OceanIB = new IndexBuffer(Renderer.GetGraphicsDevice(), typeof(short), (128 - 1) * (128 - 1) * 6, BufferUsage.WriteOnly);
            OceanIB.SetData(terrainIndices);
            BufferManager.AddNamedIndexBuffer("OceanIB", OceanIB);
            #endregion

            #region Generate standard textures

            Color[] initData = new Color[256 * 256];
            for (int i = 0; i < 256; ++i)
            {
                for (int j = 0; j < 256; ++j)
                {
                    Vector3 v = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
                    v.Normalize();

                    initData[i * 256 + j] = new Color(v.X, v.Y, v.Z, 1.0f);
                }
            }
            RandomVectors = new Texture2D(device, 256, 256);
            RandomVectors.SetData<Color>(initData);

            #endregion


            current.Initialise();
#if DEBUG
            debug_effect = new BasicEffect(device);
            gmenu = new GlobalDebugMenu();

            normal_rs = new RasterizerState();
            normal_rs.CullMode = CullMode.CullClockwiseFace;
            normal_rs.DepthBias = 0;
            normal_rs.DepthClipEnable = true;
            normal_rs.FillMode = FillMode.Solid;
            normal_rs.ScissorTestEnable = false;
            normal_rs.MultiSampleAntiAlias = true;

            spriteBatch = new SpriteBatch(device);

            depthDisplay = Game1.GameInstance.Content.Load<Effect>("Shaders/DisplayDepth");
#endif
        }

        public void CleanUp()
        {
            samplerStates.Clear();
            rasterStates.Clear();
            current.CleanUp();

        }

        public void StartFrame()
        {
            current.StartFrame();
        }

        public void EndFrame()
        {
            current.EndFrame();
        }

        public void Draw(WorldState state, GameTime gt)
        {
#if DEBUG
            worldstate = state;
#endif
            current.Draw(state,gt);

#if DEBUG
            device.RasterizerState = Renderer.GetRasteriser(RasteriserStates.Normal);
            device.DepthStencilState = DepthStencilState.Default;
            device.BlendState = BlendState.Opaque;

            if (worldstate != null)
            {
                debug_effect.World = Matrix.Identity;
                debug_effect.Projection = WorldState.GetWorldState().Projection;
                debug_effect.View = WorldState.GetWorldState().View;
                debug_effect.LightingEnabled = false;
                debug_effect.TextureEnabled = false;
                DebugLineDraw.DrawAll(debug_effect);
            }


            gmenu.Draw(spriteBatch);

            Rectangle debugregion = new Rectangle(0, device.Viewport.Height - 256, 256, 256);
            if (DebugRenderSettings.RenderMoonTexture)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(RegisteredTextures["Moon"], debugregion, Color.White);
                spriteBatch.End();
                debugregion.X += 256;
            }
            if (DebugRenderSettings.RenderDepthMap)
            {
                if (!UsingForwardRenderer)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.Opaque,null,null,null,depthDisplay);
                    spriteBatch.Draw(Deferred.DeferredRender.Instance.depthRT, debugregion, Color.White);
                    spriteBatch.End();
                    debugregion.X += 256;
                }
            }
            if (DebugRenderSettings.RenderShadowMap)
            {
                if (UsingForwardRenderer)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(((ForwardRenderer)current).shadows, debugregion, Color.White);
                    spriteBatch.End();
                    debugregion.X += 256;
                }
            }
            if (DebugRenderSettings.RenderSSAOTexture)
            {
                if (!UsingForwardRenderer)
                {
                    if (renderSettings.SSAOBlur)
                    {
                        spriteBatch.Begin();
                        spriteBatch.Draw(((Deferred.DeferredRender)current).temp, debugregion, Color.White);
                        spriteBatch.End();
                    }
                    else
                    {
                        spriteBatch.Begin();
                        spriteBatch.Draw(((Deferred.DeferredRender)current).ssaoRT, debugregion, Color.White);
                        spriteBatch.End();
                    }
                    debugregion.X += 256;
                }
            }
            if (DebugRenderSettings.RenderClock)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(AssetManager.GetDebugFont(), WorldState.GetWorldState().GameTime.ToShortDateString(), new Vector2(device.Viewport.Width - 200, 20), Color.Crimson);
                spriteBatch.DrawString(AssetManager.GetDebugFont(), WorldState.GetWorldState().GameTime.ToShortTimeString(), new Vector2(device.Viewport.Width - 200, 40), Color.Crimson);
                spriteBatch.End();
            }

#endif

        }

        public void AddCommand(RenderCommandSet rs)
        {
            current.AddCommand(rs);
        }

        #region Shader management
        public void AddShader(String name, Effect fx)
        {
            current.AddShader(name, fx);
        }

        public Effect ApplyShader(WorldState state, RenderCommand r)
        {
            return current.ApplyShader(state, r);
        }
        #endregion

        #region Sampler state management
        public static int MapToSamplerStateID(TextureAddressMode x, TextureAddressMode y, TextureAddressMode w)
        {
            int res = 0;

            switch (x)
            {
                case TextureAddressMode.Border:
                    break;
                case TextureAddressMode.Clamp:
                    res += 1;
                    break;
                case TextureAddressMode.Mirror:
                    res += 2;
                    break;
                case TextureAddressMode.Wrap:
                    res += 3;
                    break;
            }
            switch (y)
            {
                case TextureAddressMode.Border:
                    break;
                case TextureAddressMode.Clamp:
                    res += 4;
                    break;
                case TextureAddressMode.Mirror:
                    res += 8;
                    break;
                case TextureAddressMode.Wrap:
                    res += 12;
                    break;
            }
            switch (w)
            {
                case TextureAddressMode.Border:
                    break;
                case TextureAddressMode.Clamp:
                    res += 16;
                    break;
                case TextureAddressMode.Mirror:
                    res += 32;
                    break;
                case TextureAddressMode.Wrap:
                    res += 48;
                    break;
            }


            return res;
        }

        public static SamplerState MapIntToSamplerState(int i)
        {
            SamplerState s = new SamplerState();
            int x = (i & 3);
            int y = ((i >> 2) & 3);
            int w = ((i >> 4) & 3);
            switch (x)
            {
                case 0:
                    s.AddressU = TextureAddressMode.Border;
                    break;
                case 1:
                    s.AddressU = TextureAddressMode.Clamp;
                    break;
                case 2:
                    s.AddressU = TextureAddressMode.Mirror;
                    break;
                case 3:
                    s.AddressU = TextureAddressMode.Wrap;
                    break;
            }
            switch (y)
            {
                case 0:
                    s.AddressV = TextureAddressMode.Border;
                    break;
                case 1:
                    s.AddressV = TextureAddressMode.Clamp;
                    break;
                case 2:
                    s.AddressV = TextureAddressMode.Mirror;
                    break;
                case 3:
                    s.AddressV = TextureAddressMode.Wrap;
                    break;
            }
            switch (w)
            {
                case 0:
                    s.AddressW = TextureAddressMode.Border;
                    break;
                case 1:
                    s.AddressW = TextureAddressMode.Clamp;
                    break;
                case 2:
                    s.AddressW = TextureAddressMode.Mirror;
                    break;
                case 3:
                    s.AddressW = TextureAddressMode.Wrap;
                    break;
            }
            return s;
        }

        public static int MapBoolsToSamplerState(bool wrapx, bool wrapy, bool wrapw)
        {
            int x = 1;
            if (wrapx)
                x = 3;
            int y = 4;
            if (wrapy)
                y = 12;
            int w = 16;
            if (wrapw)
                w = 48;

            return x + y + w;
        }
        #endregion

        #region Static methods
        public static GraphicsDevice GetGraphicsDevice()
        {
            return Renderer.Instance.device;
        }

        public static Renderer GetCurrentRenderer()
        {
            return Renderer.Instance;
        }

        public static void AddRenderCommand(RenderCommandSet rs)
        {
            Renderer.Instance.AddCommand(rs);
        }

        public static bool IsHDREnabled()
        {
            return Renderer.Instance.renderSettings.HDREnabled;
        }

        public static Skies GetSkyType()
        {
            return Renderer.Instance.renderSettings.SkyType;
        }

        public static float GetAspectRatio()
        {
            return Instance.device.Viewport.AspectRatio;
        }

        public static String GetShaderName(String ID)
        {
            if (Instance.UsingForwardRenderer)
            {
                switch (ID)
                {
                    case "MeshPartShader":
                        return @"Shaders\Forward\MeshPartShader";
                    case "Ocean":
                        return @"Shaders\Forward\Ocean";
                    case "Glass":
                        return @"Shaders\Forward\Glass";
                    case "Windsock":
                        return @"Shaders\Forward\Windsock";
                    case "Textured":
                        return @"Shaders\Forward\Textured";

                    default:
                        throw new Exception("missing shader");
                }
            }
            else
            {
                switch (ID)
                {
                    case "Windsock":
                        return @"Shaders\Deferred\Windsock";

                    case "MeshPartShader":
                        return @"Shaders\Deferred\MeshPart";
                    case "Ocean":
                        return @"Shaders\Deferred\Ocean";
                    case "Glass":
                        return @"Shaders\Forward\Glass";
                    case "Cirrus":
                        return @"Shaders\Deferred\CirrusClouds";
                    case "Textured":
                        return @"Shaders\Deferred\Textured";
                }
            }
            return "";
        }

        public static void AddPointLight(Vector3 pos, Color color, float rad, float intensity)
        {
            Renderer.Instance.current.AddPointLight(pos, color, rad, intensity);
        }

        public static void AddDirectionalLight(Vector3 dir, Color col, bool isSun, bool isMoon)
        {
            Renderer.Instance.current.AddDirectionalLight(dir, col, isSun, isMoon);
        }

        public static RenderSettings GetSettings()
        {
            return Instance.renderSettings;
        }

        public static Matrix CreateCubeFaceLookAtViewMatrix(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            var vector = Vector3.Normalize(cameraPosition - cameraTarget);
            var vector2 = -Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
            var vector3 = Vector3.Cross(-vector, vector2);
            Matrix result = Matrix.Identity;
            result.M11 = vector2.X;
            result.M12 = vector3.X;
            result.M13 = vector.X;
            result.M14 = 0f;
            result.M21 = vector2.Y;
            result.M22 = vector3.Y;
            result.M23 = vector.Y;
            result.M24 = 0f;
            result.M31 = vector2.Z;
            result.M32 = vector3.Z;
            result.M33 = vector.Z;
            result.M34 = 0f;
            result.M41 = -Vector3.Dot(vector2, cameraPosition);
            result.M42 = -Vector3.Dot(vector3, cameraPosition);
            result.M43 = -Vector3.Dot(vector, cameraPosition);
            result.M44 = 1f;
            return result;
        }

        public static RasterizerState GetRasteriser(RasteriserStates state)
        {
            return Instance.rasterStates[state];
        }

        public static SamplerState GetSamplerState(int state)
        {
            return Instance.samplerStates[state];
        }

        public static bool IsForward()
        {
            return Instance.UsingForwardRenderer;
        }
        #endregion




    }
}
