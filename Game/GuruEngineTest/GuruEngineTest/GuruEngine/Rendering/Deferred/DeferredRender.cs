using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.DebugHelpers;
using GuruEngine.Rendering.Lights;
using GuruEngine.Rendering.Primitives;
using GuruEngine.World;
using GuruEngine.Assets;
using GuruEngine.Maths;
using GuruEngine.Rendering.Particles;
using GuruEngine.World.Weather;

namespace GuruEngine.Rendering.Deferred
{
    public class DeferredRender : RenderInterface
    {
        public static DeferredRender Instance;

        RenderTargetCube rtc;

        public GraphicsDevice device;
        float time = 0;
        QuadRenderer QRender;

        Matrix View;
        Matrix Projection;
        Matrix ViewInverse;

        Vector4 SunColour;
        Vector4 SunDirection;
        Vector4 AmbientColour;

        Vector3 CameraPosition;
        Vector3 CameraForward;

        RasterizerState gbrs;
        RasterizerState nbrs;
        RasterizerState cullcw;
        RasterizerState cullccw;

        #region Deferred shaders
        Effect ClearBuffer;
        Effect MeshPartShader;
        Effect directionalLightEffect;
        Effect finalCombineEffect;
        Effect pointLightEffect;
        Effect ocean;
        Effect ssao;
        Effect restore;
        Effect textured;
        Effect windsock;
        Effect blur;
        Effect combinessao;
        #endregion

        public RenderTarget2D colorRT;     // Color and Specular Intensity
        public RenderTarget2D normalRT;    // Normals and Specular Power
        public RenderTarget2D depthRT;     // Depth
        public RenderTarget2D lightRT;     // Light accumulator
        public RenderTarget2D materialRT;  // Material properties (x = specular strength, y = specular power / 255)
        public RenderTarget2D skyRT;
        public RenderTarget2D final;
        public RenderTarget2D ssaoRT;
        public RenderTarget2D temp;

        private Dictionary<String, Effect> loadedShaders = new Dictionary<string, Effect>();

        RenderTargetBinding[] GBufferTargets;

        BlendState lightAdditive;

        Vector2 halfPixel;
        Model sphereModel;
        bool loaded = false;

        int GUID;

#if DEBUG
        KeyboardState oldks;
#endif


        public DeferredRender(GraphicsDevice Device)
        {
            Instance = this;
            device = Device;
            bufferedRenderCommandsA = new List<RenderCommandSet>();
            bufferedRenderCommandsB = new List<RenderCommandSet>();
            updatingRenderCommands = bufferedRenderCommandsA;
            renderCommandsReady = new ManualResetEvent(false);
            renderActive = new ManualResetEvent(true);
            renderCompleted = new ManualResetEvent(true);
            activeLightManager = lightManagerB;
            currentLightManager = lightManagerA;
        }

        public override void Initialise()
        {
            int backBufferWidth = device.Viewport.Width;
            int backBufferHeight = device.Viewport.Height;

            GBufferTargets = new RenderTargetBinding[4];

            colorRT = new RenderTarget2D(device, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            normalRT = new RenderTarget2D(device, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            materialRT = new RenderTarget2D(device, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            depthRT = new RenderTarget2D(device, backBufferWidth, backBufferHeight, false, SurfaceFormat.Single, DepthFormat.Depth24);
            lightRT = new RenderTarget2D(device, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
            skyRT = new RenderTarget2D(device, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
            final = new RenderTarget2D(device, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
            ssaoRT = new RenderTarget2D(device, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
            temp = new RenderTarget2D(device, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);

            GBufferTargets[0] = new RenderTargetBinding(colorRT);
            GBufferTargets[1] = new RenderTargetBinding(normalRT);
            GBufferTargets[2] = new RenderTargetBinding(depthRT);
            GBufferTargets[3] = new RenderTargetBinding(materialRT);

            QRender = new QuadRenderer(device);

            gbrs = new RasterizerState();
            gbrs.CullMode = CullMode.CullClockwiseFace;
            gbrs.DepthClipEnable = true;
            gbrs.FillMode = FillMode.Solid;
            gbrs.MultiSampleAntiAlias = false;

            nbrs = new RasterizerState();
            nbrs.CullMode = CullMode.None;
            nbrs.DepthClipEnable = false;
            nbrs.FillMode = FillMode.Solid;
            nbrs.MultiSampleAntiAlias = false;

            cullcw = new RasterizerState();
            cullccw = new RasterizerState();
            cullccw.CullMode = CullMode.CullCounterClockwiseFace;
            cullcw.CullMode = CullMode.CullClockwiseFace;

            lightAdditive = new BlendState();
            lightAdditive.AlphaBlendFunction = BlendFunction.Add;
            lightAdditive.ColorSourceBlend = Blend.One;
            lightAdditive.ColorDestinationBlend = Blend.One;
            lightAdditive.AlphaSourceBlend = Blend.One;
            lightAdditive.ColorSourceBlend = Blend.One;

            float hx = 0.5f / device.Viewport.Width;
            float hy = 0.5f / device.Viewport.Height;
            halfPixel = new Vector2(hx, hy);

            AssetManager.AddShaderToQue(@"Shaders\Deferred\ClearGBuffer");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\MeshPart");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\DirectionalLight");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\CombineFinal");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\PointLight");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\Ocean");
            AssetManager.AddShaderToQue(@"Shaders\2D\ParticleEffect");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\DSSAO");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\RestoreDepthBuffer");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\Textured");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\Windsock");
            AssetManager.AddShaderToQue(@"Shaders\SimpleBlur");
            AssetManager.AddShaderToQue(@"Shaders\Deferred\CombineSSAO");

            String mesh = @"StaticMeshes\sphere";
            AssetManager.AddStaticMeshToQue(mesh);
            GUID = mesh.GetHashCode();

            rtc = new RenderTargetCube(device, 256, false, SurfaceFormat.Color, DepthFormat.None);
        }

        public override void CleanUp()
        {
        }

        public override void Draw(WorldState state, GameTime gt)
        {
            WaitForUpdateToComplete();

            if (state != null)
            {
                lock (state)
                {
                    View = Copy(state.View);
                    Projection = Copy(state.Projection);
                    ViewInverse = Copy(state.ViewInverse);

                    SunColour = Copy(state.SunColour);
                    SunDirection = Copy(state.SunDirection);
                    AmbientColour = Copy(state.AmbientColour);

                    CameraPosition = Copy(state.CameraPosition);
                    CameraForward = Copy(state.CameraForward);
                }
            }

            SwapBuffers();
            TellUpdateToContinue();

            if (!loaded)
            {
                ClearBuffer = AssetManager.Shader(@"Shaders\Deferred\ClearGBuffer".GetHashCode());
                MeshPartShader = AssetManager.Shader(@"Shaders\Deferred\MeshPart".GetHashCode());
                directionalLightEffect = AssetManager.Shader(@"Shaders\Deferred\DirectionalLight".GetHashCode());
                finalCombineEffect = AssetManager.Shader(@"Shaders\Deferred\CombineFinal".GetHashCode());
                pointLightEffect = AssetManager.Shader(@"Shaders\Deferred\PointLight".GetHashCode());
                ocean = AssetManager.Shader(@"Shaders\Deferred\Ocean".GetHashCode());
                ssao = AssetManager.Shader(@"Shaders\Deferred\DSSAO".GetHashCode());
                restore = AssetManager.Shader(@"Shaders\Deferred\RestoreDepthBuffer".GetHashCode());
                textured = AssetManager.Shader(@"Shaders\Deferred\Textured".GetHashCode());
                windsock = AssetManager.Shader(@"Shaders\Deferred\Windsock".GetHashCode());
                blur = AssetManager.Shader(@"Shaders\SimpleBlur".GetHashCode());
                combinessao = AssetManager.Shader(@"Shaders\Deferred\CombineSSAO".GetHashCode());

                sphereModel = AssetManager.StaticMesh(GUID);
                if ((sphereModel == null) ||
                    (ClearBuffer == null) ||
                    (MeshPartShader == null) ||
                    (directionalLightEffect == null) ||
                    (finalCombineEffect == null) ||
                    (pointLightEffect == null) ||
                    (ssao == null) ||
                    (restore == null) ||
                    (textured == null) ||
                    (blur == null) ||
                    (combinessao == null) ||
                    (ocean == null))
                {
                    SignalRenderingComplete();
                    Engine.EndDrawFrame(gt);
                    SignalRendererFinished();
                    return;
                }
                foreach (ModelMesh mesh in sphereModel.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = pointLightEffect;
                    }
                }
            }
            loaded = true;
            time += gt.ElapsedGameTime.Milliseconds / 1000.0f;

            device.RasterizerState = Renderer.GetRasteriser(RasteriserStates.Normal);
            device.SamplerStates[0] = Renderer.GetSamplerState(Renderer.MapBoolsToSamplerState(true, true, true));
            device.BlendState = BlendState.Opaque;
            device.Clear(Color.Black);
            foreach (RenderCommandSet renderingRenderCommand in renderingRenderCommands)
            {
                foreach (RenderCommand r in renderingRenderCommand.Commands)
                {
                    r.PreRender(device);
                }
            }

            for (int pass = 0; pass < RenderPasses.TotalPasses; pass++)
            {
                if (pass == RenderPasses.Terrain)
                {
                    SetGBuffer();

                    device.RasterizerState = nbrs;
                    device.DepthStencilState = DepthStencilState.Default;
                    device.BlendState = BlendState.Opaque;

                    ClearGBuffer();
                }
                foreach (RenderCommandSet renderingRenderCommand in renderingRenderCommands)
                {
                    if (pass == renderingRenderCommand.RenderPass)
                    {
                        switch (pass)
                        {
#region Sky rendering
                            // there is only one sky object so this is safe
                            case RenderPasses.Sky:
                                {
                                    device.RasterizerState = gbrs;
                                    device.DepthStencilState = DepthStencilState.Default;
                                    device.BlendState = BlendState.Opaque;

                                    Vector3 pos;
                                    if (Renderer.GetSkyType() == Skies.SimpleScattered)
                                    {
                                        pos = CameraPosition + (Vector3.UnitY * -120.0f);

                                    }
                                    else
                                    {
                                        pos = CameraPosition;
                                        device.RasterizerState = Renderer.GetRasteriser(RasteriserStates.NoDepthNoCull);
                                        device.DepthStencilState = DepthStencilState.None;
                                    }

                                    CreateEnvironmentMap(renderingRenderCommand);

                                }
                                break;
#endregion

#region Moon, Stars, and Planets
                            case RenderPasses.Ephemeris:
                                {
                                    DrawToSkyBuffer(renderingRenderCommand, state);
                                }
                                break;
#endregion

                            case RenderPasses.Terrain:
                                {
                                    DrawToGBuffer(renderingRenderCommand, state);
                                }
                                break;

                            case RenderPasses.Transparent:
                                {
                                    //DrawToGBuffer(renderingRenderCommand, state);
                                }
                                break;

                            default:
                                {
                                    DrawToGBuffer(renderingRenderCommand, state);
                                }
                                break;
                        }
                    }
                }
            }
            ResolveGBuffer();

            DrawLights(View, Projection);

            switch (Renderer.Instance.renderSettings.SSAOType)
            {
                case SSAOTypes.Simple:
                    {
                        device.SetRenderTarget(ssaoRT);
                        device.DepthStencilState = DepthStencilState.None;
                        ssao.Parameters["depthMap"]?.SetValue(depthRT);
                        ssao.Parameters["normalMap"]?.SetValue(normalRT);
                        ssao.Parameters["randomMap"]?.SetValue(Renderer.Instance.RandomVectors);
                        ssao.Parameters["halfPixel"]?.SetValue(halfPixel);
                        ssao.Parameters["radius"].SetValue(Renderer.Instance.renderSettings.SSAOSampleRadius);
                        ssao.Parameters["falloff"].SetValue(Renderer.Instance.renderSettings.SSAODistanceScale);
                        ssao.Parameters["area"].SetValue(Renderer.Instance.renderSettings.SSAOArea);
                        ssao.Techniques[0].Passes[0].Apply();
                        QRender.Render(Vector2.One * -1, Vector2.One);

                        if (Renderer.Instance.renderSettings.SSAOBlur)
                        {
                            device.SetRenderTarget(temp);
                            blur.Parameters["colourMap"]?.SetValue(ssaoRT);
                            blur.Techniques[0].Passes[0].Apply();
                            QRender.Render(Vector2.One * -1, Vector2.One);
                            
                        }

                        device.SetRenderTarget(final);
                        if (Renderer.Instance.renderSettings.SSAOBlur)
                            combinessao.Parameters["ssao"].SetValue(temp);
                        else
                            combinessao.Parameters["ssao"].SetValue(ssaoRT);

                        combinessao.Parameters["colorMap"].SetValue(colorRT);
                        combinessao.Parameters["lightMap"].SetValue(lightRT);
                        combinessao.Parameters["halfPixel"].SetValue(halfPixel);
                        combinessao.Parameters["sky"].SetValue(skyRT);
                        combinessao.Parameters["depthmap"].SetValue(depthRT);
                        combinessao.Parameters["gamma"].SetValue(Renderer.Instance.renderSettings.gamma);

                        combinessao.Techniques[0].Passes[0].Apply();
                        QRender.Render(Vector2.One * -1, Vector2.One);
                        device.SetRenderTarget(null);
                    }
                    break;

                default:
                    device.SetRenderTarget(final);
                    
                    finalCombineEffect.Parameters["colorMap"].SetValue(colorRT);
                    finalCombineEffect.Parameters["lightMap"].SetValue(lightRT);
                    finalCombineEffect.Parameters["halfPixel"].SetValue(halfPixel);
                    finalCombineEffect.Parameters["sky"].SetValue(skyRT);
                    finalCombineEffect.Parameters["depthmap"].SetValue(depthRT);
                    finalCombineEffect.Parameters["gamma"].SetValue(Renderer.Instance.renderSettings.gamma);

                    finalCombineEffect.Techniques[0].Passes[0].Apply();
                    QRender.Render(Vector2.One * -1, Vector2.One);
                    device.SetRenderTarget(null);
                    break;
            }

            restore.Parameters["colorMap"].SetValue(final);
            restore.Parameters["depthmap"].SetValue(depthRT);
            restore.Techniques[0].Passes[0].Apply();
            QRender.Render(Vector2.One * -1, Vector2.One);

            //foreach (RenderCommandSet renderingRenderCommand in renderingRenderCommands)
            //{
            //    if (renderingRenderCommand.RenderPass == RenderPasses.Transparent)
            //    {
            //
            //    }
            //}

            SignalRenderingComplete();
            Engine.EndDrawFrame(gt);
            SignalRendererFinished();
        }

        private void DrawToGBuffer(RenderCommandSet rs, WorldState state)
        {

            device.RasterizerState = Renderer.GetRasteriser(rs.RS);
            device.DepthStencilState = rs.DS;
            if (rs.IsStaticMesh)
            {
                //device.BlendState = rs.blend;
                rs.fx.Parameters["World"].SetValue(rs.World);
                rs.fx.Parameters["View"].SetValue(rs.View);
                foreach (ModelMesh mesh in rs.mesh.Meshes)
                {
                    mesh.Draw();
                }
            }
            else
            {
                foreach (RenderCommand r in rs.Commands)
                {
                    if (r.OwnerDraw)
                    {
                        r.Draw(device);
                    }
                    else
                    {
                        if (r.IsGlass)
                        {

                        }
                        else
                        {
                            device.SetVertexBuffer(r.vbuffer);
                            device.Indices = r.ibuffer;

                            Effect fx = ApplyShader(state, r);
                            if (r.material != null)
                            {
                                if (r.material is MeshPartMaterial)
                                    device.RasterizerState = ((MeshPartMaterial)r.material).deferred_rs;
                                r.material.Apply(fx);
                            }
                            device.SamplerStates[0] = Renderer.GetSamplerState(r.SamplerStateID);
                            //device.BlendState = r.blendstate;

                            foreach (EffectPass p in fx.CurrentTechnique.Passes)
                            {
                                p.Apply();
                                switch (r.MType)
                                {
                                    case MeshType.IndexedPrimitives:
                                        device.DrawIndexedPrimitives(r.PType, r.StartVertex, r.StartIndex, r.PrimitiveCount);
                                        break;
                                    case MeshType.Primitives:
                                        device.DrawPrimitives(r.PType, r.StartVertex, r.PrimitiveCount);
                                        break;
                                    case MeshType.Instanced:
                                        device.DrawInstancedPrimitives(r.PType, r.BaseVertex, r.StartVertex, r.PrimitiveCount, r.InstanceCount);
                                        break;
                                    case MeshType.UserIndexedPrimitives:
                                        r.Draw(device);
                                        break;
                                    case MeshType.UserPrimitives:
                                        r.Draw(device);
                                        break;

                                }
                            }
                        }
                    }
                }
            }



        }

        /// <summary>
        /// Environment map creation
        /// </summary>
        /// <param name="renderingRenderCommand"></param>
        private void CreateEnvironmentMap(RenderCommandSet renderingRenderCommand)
        {
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 1, 1, 10000);
            Vector3 pos;
            if (Renderer.GetSkyType() == Skies.SimpleScattered)
            {
                pos = CameraPosition + (Vector3.UnitY * -120.0f);

            }
            else
            {
                pos = CameraPosition;
                device.RasterizerState = Renderer.GetRasteriser(RasteriserStates.NoDepthNoCull);
                device.DepthStencilState = DepthStencilState.None;
            }

            Matrix viewMatrix = Matrix.Identity;

            for (int i = 0; i < 6; i++)
            {
                CubeMapFace face = (CubeMapFace)i;
                switch (face)
                {
                    case CubeMapFace.NegativeX:
                        {
                            viewMatrix = Renderer.CreateCubeFaceLookAtViewMatrix(pos, pos + Vector3.Left, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.NegativeY:
                        {
                            viewMatrix = Renderer.CreateCubeFaceLookAtViewMatrix(pos, pos + Vector3.Down, Vector3.Backward);
                            break;
                        }
                    case CubeMapFace.NegativeZ:
                        {
                            viewMatrix = Renderer.CreateCubeFaceLookAtViewMatrix(pos, pos + Vector3.Forward, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveX:
                        {
                            viewMatrix = Renderer.CreateCubeFaceLookAtViewMatrix(pos, pos + Vector3.Right, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveY:
                        {
                            viewMatrix = Renderer.CreateCubeFaceLookAtViewMatrix(pos, pos + Vector3.Up, Vector3.Forward);
                            break;
                        }
                    case CubeMapFace.PositiveZ:
                        {
                            viewMatrix = Renderer.CreateCubeFaceLookAtViewMatrix(pos, pos + Vector3.Backward, Vector3.Up);
                            break;
                        }
                }
                device.SetRenderTarget(rtc, face);

                RenderSkyCommand(renderingRenderCommand, viewMatrix, Projection);
                device.Flush();
            }
            device.SetRenderTarget(skyRT);
            RenderSkyCommand(renderingRenderCommand, View, Projection);
            device.Flush();

            AssetManager.Instance.environment = rtc;

        }

#region Shader management
        /// <summary>
        /// Add a shader 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fx"></param>
        public override void AddShader(String name, Effect fx)
        {
            if (loadedShaders.ContainsKey(name))
            {
                LogHelper.Instance.Warning("Multiple loads of shader " + name);
                return;
            }
            loadedShaders.Add(name, fx);
        }

        public override Effect ApplyShader(WorldState state, RenderCommand r)
        {
            if (loadedShaders.ContainsKey(r.Shader))
            {
                Effect fx = loadedShaders[r.Shader];
                fx.CurrentTechnique = fx.Techniques[r.ShaderTechnique];

                foreach (ShaderVariables s in r.Variables)
                {
                    switch (s)
                    {
                        case ShaderVariables.WindSpeed:
                            if (fx.Parameters["WindSpeed"] != null)
                                fx.Parameters["WindSpeed"].SetValue(WeatherManager.GetWindSpeed());
                            break;
                        case ShaderVariables.WindDirection:
                            if (fx.Parameters["WindDirection"] != null)
                                fx.Parameters["WindDirection"].SetValue(WeatherManager.GetWindDirection());
                            break;
                        case ShaderVariables.WorldViewProjection:
                            fx.Parameters["WorldViewProjection"].SetValue(r.World * View * Projection);
                            break;
                        case ShaderVariables.World:
                            if (fx.Parameters["World"] != null)
                                fx.Parameters["World"].SetValue(r.World);
                            break;
                        case ShaderVariables.View:
                            fx.Parameters["View"].SetValue(View);
                            break;
                        case ShaderVariables.Projection:
                            fx.Parameters["Projection"].SetValue(Projection);
                            break;
                        case ShaderVariables.ViewInverse:
                            fx.Parameters["ViewInverse"].SetValue(ViewInverse);
                            break;
                        case ShaderVariables.Lit:
                            {
                                float h1 = MathUtils.HorizonDistance(r.World.Translation.Y);
                                Vector2 i1, i2;
                                MathUtils.FindCircleCircleIntersections(0, 0, 6357000.0f, 0, r.World.Translation.Y + 6357000.0f, h1, out i1, out i2);
                                Vector3 direction = new Vector3(0, r.World.Translation.Y, 0) - new Vector3(i1.X, i1.Y - 6357000.0f, 0);
                                direction.Normalize();

                                float da = (float)Math.Asin(-direction.Y);
                                float lit = 0;
                                if (state.SunElevation > da)
                                    lit = 1;
                                fx.Parameters["SunLit"].SetValue(lit);
                            }
                            break;

                        case ShaderVariables.MoonLit:
                            {
                                float h1 = MathUtils.HorizonDistance(r.World.Translation.Y);
                                Vector2 i1, i2;
                                MathUtils.FindCircleCircleIntersections(0, 0, 6357000.0f, 0, r.World.Translation.Y + 6357000.0f, h1, out i1, out i2);
                                Vector3 direction = new Vector3(0, r.World.Translation.Y, 0) - new Vector3(i1.X, i1.Y - 6357000.0f, 0);
                                direction.Normalize();

                                float da = (float)Math.Asin(-direction.Y);
                                float lit = 0;
                                float moonelevation = (float)Math.Asin(state.MoonPosition.Y);
                                if (state.MoonPosition.Y >= 0)
                                    lit = 1;
                                fx.Parameters["MoonLit"].SetValue(lit);
                            }
                            break;

                        case ShaderVariables.SunColour:
                            if (fx.Parameters["SunColour"] != null)
                                fx.Parameters["SunColour"].SetValue(SunColour);
                            break;
                        case ShaderVariables.SunDirection:
                            if (fx.Parameters["SunDirection"] != null)
                                fx.Parameters["SunDirection"].SetValue(SunDirection);
                            break;
                        case ShaderVariables.AmbientColour:
                            if (fx.Parameters["AmbientColour"] != null)
                                fx.Parameters["AmbientColour"].SetValue(AmbientColour);
                            break;
                        case ShaderVariables.WorldInverseTranspose:
                            Matrix m = Matrix.Transpose(Matrix.Invert(r.World));
                            fx.Parameters["WorldInverseTranspose"].SetValue(m);
                            break;
                        case ShaderVariables.EnvironmentMap:
                            if (fx.Parameters["environmentMap"] != null)
                                fx.Parameters["environmentMap"].SetValue(AssetManager.Instance.environment);
                            break;
                        case ShaderVariables.Time:
                            if (fx.Parameters["time"] != null)
                                fx.Parameters["time"].SetValue(time);
                            break;

                        case ShaderVariables.Texture01:
                            if (fx.Parameters["Texture1"] != null)
                                fx.Parameters["Texture1"].SetValue(AssetManager.Instance.GetTexture(r.textures[0]));
                            break;
                        case ShaderVariables.ViewVector:
                            if (fx.Parameters["ViewVector"] != null)
                                fx.Parameters["ViewVector"].SetValue(CameraPosition);
                            break;
                        case ShaderVariables.Shadows:
                            // if (fx.Parameters["ShadowMap"] != null)
                            //     fx.Parameters["ShadowMap"].SetValue(shadows);
                            //
                            // fx.Parameters["CascadeSplits"].SetValue(new Vector4(CascadeSplits[0], CascadeSplits[1], CascadeSplits[2], CascadeSplits[3]));
                            // fx.Parameters["ShadowMatrix"].SetValue(globalShadowMatrix);
                            // fx.Parameters["CascadeOffsets"].SetValue(CascadeOffsets);
                            // fx.Parameters["CascadeScales"].SetValue(CascadeScales);
                            // fx.Parameters["Bias"].SetValue(0.002f);

                            break;

                    }
                }
                fx.Techniques[r.ShaderTechnique].Passes[0].Apply();
                return fx;

            }
            else
            {
                LogHelper.Instance.Error("Missing shader " + r.Shader);
            }
            return null;
        }

#endregion

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

        void SetGBuffer()
        {
            device.SetRenderTargets(GBufferTargets);
        }

        void ResolveGBuffer()
        {
            device.SetRenderTargets(null);
        }

        void ClearGBuffer()
        {
            ClearBuffer.Techniques[0].Passes[0].Apply();
            QRender.Render(Vector2.One * -1, Vector2.One);

        }

        public override void AddDirectionalLight(Vector3 dir, Color col, bool isSun, bool isMoon)
        {
            currentLightManager.AddDirectionalLight(dir, col, isSun, isMoon);
        }

        public override void AddPointLight(Vector3 pos, Color color, float rad, float intensity)
        {
            currentLightManager.AddPointLight(pos, color, rad, intensity);
        }

        private void DrawDirectionalLight(Matrix View, Matrix Projection, Vector3 lightDirection, Color color, bool isSun, bool isMoon)
        {
            directionalLightEffect.Parameters["MaterialMap"].SetValue(materialRT);
            directionalLightEffect.Parameters["normalMap"].SetValue(normalRT);
            directionalLightEffect.Parameters["depthMap"].SetValue(depthRT);
            directionalLightEffect.Parameters["lightDirection"].SetValue(Vector3.Normalize(lightDirection));
            directionalLightEffect.Parameters["Color"].SetValue(color.ToVector3());
            directionalLightEffect.Parameters["cameraPosition"].SetValue(CameraPosition);
            directionalLightEffect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(View * Projection));
            directionalLightEffect.Parameters["halfPixel"].SetValue(halfPixel);
            directionalLightEffect.Parameters["isSun"].SetValue(isSun);
            directionalLightEffect.Parameters["isMoon"].SetValue(isMoon);
            directionalLightEffect.Techniques[0].Passes[0].Apply();
            QRender.Render(Vector2.One * -1, Vector2.One);

        }

        private void DrawLights(Matrix View, Matrix Projection)
        {
            device.SetRenderTarget(lightRT);

            //clear all components to 0
            device.Clear(Color.Transparent);
            device.BlendState = lightAdditive;
            device.RasterizerState = nbrs;

            foreach (GuruEngine.Rendering.Lights.DirectionalLight d in activeLightManager.directionLights)
            {
                DrawDirectionalLight(View, Projection, d.Direction, d.Colour, d.Sun, d.Moon);
            }

            //set the G-Buffer parameters
            if (pointLightEffect.Parameters["material"] != null)
                pointLightEffect.Parameters["material"].SetValue(materialRT);
            if (pointLightEffect.Parameters["normalMap"] != null)
                pointLightEffect.Parameters["normalMap"].SetValue(normalRT);
            if (pointLightEffect.Parameters["depthMap"] != null)
                pointLightEffect.Parameters["depthMap"].SetValue(depthRT);

            pointLightEffect.Parameters["View"].SetValue(View);
            pointLightEffect.Parameters["Projection"].SetValue(Projection);

            //parameters for specular computations
            pointLightEffect.Parameters["cameraPosition"].SetValue(CameraPosition);
            pointLightEffect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(View * Projection));

            foreach (GuruEngine.Rendering.Lights.PointLight pl in activeLightManager.pointLights)
            {
                DrawPointLight(pl.Position, pl.Colour, pl.Radius, pl.Intensity);
            }

            device.SetRenderTarget(null);
            device.RasterizerState = cullccw;
        }

        private void DrawPointLight(Vector3 lightPosition, Color color, float lightRadius, float lightIntensity)
        {
            //compute the light world matrix scale according to light radius, and translate it to light position
            Matrix sphereWorldMatrix = Matrix.CreateScale(lightRadius) * Matrix.CreateTranslation(lightPosition);
            pointLightEffect.Parameters["World"].SetValue(sphereWorldMatrix);
          
            //light position
            pointLightEffect.Parameters["lightPosition"].SetValue(lightPosition);

            //set the color, radius and Intensity
            pointLightEffect.Parameters["Color"].SetValue(color.ToVector3());
            pointLightEffect.Parameters["lightRadius"].SetValue(lightRadius);
            pointLightEffect.Parameters["lightIntensity"].SetValue(lightIntensity);

            //size of a halfpixel, for texture coordinates alignment
            pointLightEffect.Parameters["halfPixel"].SetValue(halfPixel);

            //calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(CameraPosition, lightPosition);

            //if we are inside the light volume, draw the sphere's inside face
            if (cameraToCenter < lightRadius)
                device.RasterizerState = cullcw;
            else
                device.RasterizerState = cullccw;

            pointLightEffect.CurrentTechnique = pointLightEffect.Techniques["BasicColorDrawing"];
            pointLightEffect.Techniques[0].Passes[0].Apply();

            foreach (ModelMesh mesh in sphereModel.Meshes)
            {
                mesh.Draw();
            }


        }

        private void RenderSkyCommand(RenderCommandSet renderingRenderCommand, Matrix view, Matrix Projection)
        {
            Matrix World;
            device.BlendState = BlendState.Opaque;

            if (Renderer.GetSkyType() == Skies.Traced)
            {
                Vector3 pos = CameraPosition + (Vector3.UnitY * -0.1f);
                World = Matrix.CreateTranslation(pos);
                device.DepthStencilState = DepthStencilState.None;
            }
            else
            {
                Vector3 pos = CameraPosition + (Vector3.UnitY * -160.0f);
                World = Matrix.CreateTranslation(pos);
            }

            foreach (ModelMesh mesh in renderingRenderCommand.mesh.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["World"].SetValue(World);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["WorldViewProjection"].SetValue(World * view * Projection);
                    effect.Parameters["ViewInverse"].SetValue(Matrix.Invert(view));
                    effect.Parameters["Projection"].SetValue(Projection);
                    mesh.Draw();
                }
            }

        }

        private void DrawToSkyBuffer(RenderCommandSet rs, WorldState state)
        {
            device.RasterizerState = Renderer.GetRasteriser(rs.RS);
            device.DepthStencilState = rs.DS;
            if (rs.IsStaticMesh)
            {
                //device.BlendState = rs.blend;
                rs.fx.Parameters["World"].SetValue(rs.World);
                rs.fx.Parameters["View"].SetValue(rs.View);
                foreach (ModelMesh mesh in rs.mesh.Meshes)
                {
                    mesh.Draw();
                }
            }
            else
            {
                foreach (RenderCommand r in rs.Commands)
                {
                    if (r.OwnerDraw)
                    {
                        r.Draw(device);
                    }
                    else
                    {
                        if (r.IsGlass)
                        {

                        }
                        else
                        {
                            device.SetVertexBuffer(r.vbuffer);
                            device.Indices = r.ibuffer;

                            Effect fx = ApplyShader(state, r);
                            if (r.material != null)
                            {
                                r.material.Apply(fx);
                            }
                            device.SamplerStates[0] = Renderer.GetSamplerState(r.SamplerStateID);
                            //device.BlendState = Bl;

                            foreach (EffectPass p in fx.CurrentTechnique.Passes)
                            {
                                p.Apply();
                                switch (r.MType)
                                {
                                    case MeshType.IndexedPrimitives:
                                        device.DrawIndexedPrimitives(r.PType, r.StartVertex, r.StartIndex, r.PrimitiveCount);
                                        break;
                                    case MeshType.Primitives:
                                        device.DrawPrimitives(r.PType, r.StartVertex, r.PrimitiveCount);
                                        break;
                                    case MeshType.Instanced:
                                        device.DrawInstancedPrimitives(r.PType, r.BaseVertex, r.StartVertex, r.PrimitiveCount, r.InstanceCount);
                                        break;
                                    case MeshType.UserIndexedPrimitives:
                                        r.Draw(device);
                                        break;
                                    case MeshType.UserPrimitives:
                                        r.Draw(device);
                                        break;



                                }
                            }
                        }
                    }
                }
            }
        }
    
    }
}
