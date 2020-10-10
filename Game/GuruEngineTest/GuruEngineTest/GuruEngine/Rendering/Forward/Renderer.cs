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
using GuruEngine.Helpers;
using GuruEngine.Rendering;
using GuruEngine.World;
using GuruEngine.Assets;
using GuruEngine.Maths;
using GuruEngine.Rendering.Particles;
using GuruEngine.World.Weather;
using GuruEngine.Rendering.EffectPasses;

namespace GuruEngine
{
    public class ForwardRenderer:RenderInterface
    {
        public static ForwardRenderer Instance;


        private RenderTargetCube rtc;
        private RenderTargetCube rtc2;
        public RenderTarget2D shadows;
        public RenderTarget2D backbuffer;
        public RenderTarget2D staging_texture;

        private Dictionary<String, Effect> loadedShaders = new Dictionary<string, Effect>();

        private float[] cascadeSplits;
        private Vector3[] frustumCorners;

        Matrix View;
        Matrix Projection;
        Matrix ViewInverse;

        Vector4 SunColour;
        Vector4 SunDirection;
        Vector4 AmbientColour;

        Vector3 CameraPosition;
        Vector3 CameraForward;

        public GraphicsDevice device;
        float time = 0;
        SpriteBatch spriteBatch;

        Matrix globalShadowMatrix;
        Vector4[] CascadeOffsets;
        Vector4[] CascadeScales;
        float[] CascadeSplits;

        public VertexBuffer OceanVB;
        public IndexBuffer OceanIB;

        public ForwardRenderer(GraphicsDevice Device)
        {
            Instance = this;
            device = Device;
            bufferedRenderCommandsA = new List<RenderCommandSet>();
            bufferedRenderCommandsB = new List<RenderCommandSet>();
            updatingRenderCommands = bufferedRenderCommandsA;
            currentLightManager = lightManagerA;
#if MULTI_THREADED
            renderCommandsReady = new ManualResetEvent(false);
            renderActive = new ManualResetEvent(true);
            renderCompleted = new ManualResetEvent(true);
#endif
        }

        public override void Initialise()
        {

#region Setup cascade shadow mapping
            if (Renderer.GetSettings().CascadeShadowMaps)
            {
                shadows = new RenderTarget2D(device, Renderer.GetSettings().ShadowMapSize, Renderer.GetSettings().ShadowMapSize, false, SurfaceFormat.Single, DepthFormat.Depth24, 1, RenderTargetUsage.DiscardContents, false, Renderer.GetSettings().ShadowMapCascades);

                cascadeSplits = new float[4];
                cascadeSplits[0] = Renderer.GetSettings().SplitDistance0;
                cascadeSplits[1] = Renderer.GetSettings().SplitDistance1;
                cascadeSplits[2] = Renderer.GetSettings().SplitDistance2;
                cascadeSplits[3] = Renderer.GetSettings().SplitDistance3;

                frustumCorners = new Vector3[8];

                CascadeOffsets = new Vector4[Renderer.GetSettings().ShadowMapCascades];
                CascadeScales = new Vector4[Renderer.GetSettings().ShadowMapCascades];
                CascadeSplits = new float[Renderer.GetSettings().ShadowMapCascades];
            }
#endregion

#region Register standard shaders

            AssetManager.AddShaderToQue(@"Shaders\Forward\Windsock");
            AssetManager.AddShaderToQue(@"Shaders\Forward\MeshPartShader");
            AssetManager.AddShaderToQue(@"Shaders\Forward\Glass");
            AssetManager.AddShaderToQue(@"Shaders\Forward\Ocean");
            AssetManager.AddShaderToQue(@"Shaders\Forward\Mirror");
            AssetManager.AddShaderToQue(@"Shaders\Forward\ShadowMap");
            AssetManager.AddShaderToQue(@"Shaders\2D\ParticleEffect");
            AssetManager.AddShaderToQue(@"Shaders\Forward\Textured");
            AssetManager.AddShaderToQue(@"Shaders\2D\RadialBlur");
#endregion

            rtc = new RenderTargetCube(device, 256, false, SurfaceFormat.Color, DepthFormat.None);
            rtc2 = new RenderTargetCube(device, 256, false, SurfaceFormat.Color, DepthFormat.None);

            backbuffer = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            staging_texture = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height);
            spriteBatch = new SpriteBatch(device);

            
        }

        public override void CleanUp()
        {

        }

        bool done = false;

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="state"></param>
        /// <param name="gt"></param>
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

            time += gt.ElapsedGameTime.Milliseconds / 1000.0f;


            device.DepthStencilState = DepthStencilState.Default;
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

#region Shadows
            if (Renderer.GetSettings().CascadeShadowMaps)
            {

                Vector3 lightDirection = new Vector3(state.SunDirection.X, state.SunDirection.Y, state.SunDirection.Z);
                Effect shadowMapEffect = loadedShaders["ShadowMap"];
                globalShadowMatrix = MakeGlobalShadowMatrix(state);
                for (var cascadeIdx = 0; cascadeIdx < Renderer.GetSettings().ShadowMapCascades; ++cascadeIdx)
                {
                    // Set the shadow map as the render target
                    device.SetRenderTarget(shadows, cascadeIdx);
                    device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                    // Get the 8 points of the view frustum in world space
                    ResetViewFrustumCorners();


                    float prevSplitDist = cascadeIdx == 0 ? 0.0f : cascadeSplits[cascadeIdx - 1];
                    float splitDist = cascadeSplits[cascadeIdx];

                    var invViewProj = Matrix.Invert(state.View * state.Projection);
                    for (var i = 0; i < 8; ++i)
                    {
                        Vector4 t = Vector4.Transform(frustumCorners[i], invViewProj);
                        frustumCorners[i] = new Vector3(t.X, t.Y, t.Z);
                    }

                    // Get the corners of the current cascade slice of the view frustum
                    for (var i = 0; i < 4; ++i)
                    {
                        Vector3 cornerRay = frustumCorners[i + 4] - frustumCorners[i];
                        Vector3 nearCornerRay = cornerRay * prevSplitDist;
                        Vector3 farCornerRay = cornerRay * splitDist;
                        frustumCorners[i + 4] = frustumCorners[i] + farCornerRay;
                        frustumCorners[i] = frustumCorners[i] + nearCornerRay;
                    }

                    // Calculate the centroid of the view frustum slice
                    var frustumCenter = Vector3.Zero;
                    for (var i = 0; i < 8; ++i)
                        frustumCenter = frustumCenter + frustumCorners[i];
                    frustumCenter /= 8.0f;

                    // Pick the up vector to use for the light camera
                    var upDir = state.camera.Right;

                    Vector3 minExtents;
                    Vector3 maxExtents;

                    if (Renderer.GetSettings().StabilizeCascades)
                    {
                        // This needs to be constant for it to be stable
                        upDir = Vector3.Up;

                        // Calculate the radius of a bounding sphere surrounding the frustum corners
                        var sphereRadius = 0.0f;
                        for (var i = 0; i < 8; ++i)
                        {
                            var dist = (frustumCorners[i] - frustumCenter).Length();
                            sphereRadius = Math.Max(sphereRadius, dist);
                        }

                        sphereRadius = (float)Math.Ceiling(sphereRadius * 16.0f) / 16.0f;

                        maxExtents = new Vector3(sphereRadius);
                        minExtents = -maxExtents;
                    }
                    else
                    {
                        // Create a temporary view matrix for the light
                        Vector3 lightCameraPos = frustumCenter;
                        Vector3 lookAt = frustumCenter - lightDirection;
                        Matrix lightView = Matrix.CreateLookAt(lightCameraPos, lookAt, upDir);

                        // Calculate an AABB around the frustum corners
                        Vector3 mins = new Vector3(float.MaxValue);
                        Vector3 maxes = new Vector3(float.MinValue);
                        for (var i = 0; i < 8; ++i)
                        {
                            Vector4 t = Vector4.Transform(frustumCorners[i], lightView);
                            Vector3 corner = new Vector3(t.X, t.Y, t.Z);
                            mins = Vector3.Min(mins, corner);
                            maxes = Vector3.Max(maxes, corner);
                        }

                        minExtents = mins;
                        maxExtents = maxes;

                        // Adjust the min/max to accommodate the filtering size
                        float scale = (Renderer.GetSettings().ShadowMapSize + Renderer.GetSettings().FixedFilterKernelSize) / (float)Renderer.GetSettings().ShadowMapSize;
                        minExtents.X *= scale;
                        minExtents.Y *= scale;
                        maxExtents.X *= scale;
                        maxExtents.Y *= scale;
                    }

                    Vector3 cascadeExtents = maxExtents - minExtents;

                    // Get position of the shadow camera
                    Vector3 shadowCameraPos = frustumCenter + lightDirection * -minExtents.Z;

                    // Come up with a new orthographic camera for the shadow caster
                    Matrix shadowCamera = Matrix.CreateOrthographicOffCenter(minExtents.X, maxExtents.X, minExtents.Y, maxExtents.Y, 0, cascadeExtents.Z);
                    Matrix look = Matrix.CreateLookAt(shadowCameraPos, frustumCenter, upDir);

                    if (Renderer.GetSettings().StabilizeCascades)
                    {
                        // Create the rounding matrix, by projecting the world-space origin and determining the fractional offset in texel space
                        Matrix shadowMatrixTemp = look * shadowCamera;
                        Vector4 shadowOrigin = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                        shadowOrigin = Vector4.Transform(shadowOrigin, shadowMatrixTemp);
                        shadowOrigin = shadowOrigin * (Renderer.GetSettings().ShadowMapSize / 2.0f);

                        Vector4 roundedOrigin = MathsHelper.Round(shadowOrigin);
                        Vector4 roundOffset = roundedOrigin - shadowOrigin;
                        roundOffset = roundOffset * (2.0f / Renderer.GetSettings().ShadowMapSize);
                        roundOffset.Z = 0.0f;
                        roundOffset.W = 0.0f;

                        shadowCamera.M41 += roundOffset.X;
                        shadowCamera.M42 += roundOffset.Y;
                        shadowCamera.M43 += roundOffset.Z;
                        shadowCamera.M44 += roundOffset.W;

                    }
                    Matrix ViewProjection = look * shadowCamera;
                    foreach (RenderCommandSet rcs in renderingRenderCommands)
                    {
                        foreach (RenderCommand r in rcs.Commands)
                        {
                            if (r.CastsShadows)
                            {
                                device.RasterizerState = Renderer.GetRasteriser(RasteriserStates.ShadowMap);
                                device.BlendState = BlendState.Opaque;
                                device.DepthStencilState = DepthStencilState.Default;
                                Matrix worldViewProjection = r.World * ViewProjection;

                                shadowMapEffect.Parameters["WorldViewProjection"].SetValue(worldViewProjection);
                                foreach (EffectPass p in shadowMapEffect.CurrentTechnique.Passes)
                                {
                                    p.Apply();

                                    device.SetVertexBuffer(r.vbuffer);
                                    device.Indices = r.ibuffer;

                                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, r.StartVertex, 0, r.PrimitiveCount);
                                }

                            }
                        }
                    }

                    // Apply the scale/offset matrix, which transforms from [-1,1]
                    // post-projection space to [0,1] UV space
                    Matrix texScaleBias = Matrix.CreateScale(0.5f, -0.5f, 1.0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.0f);
                    Matrix shadowMatrix = state.View * state.Projection;
                    shadowMatrix = shadowMatrix * texScaleBias;

                    // Store the split distance in terms of view space depth
                    float clipDist = 60000 - 0.01f;

                    CascadeSplits[cascadeIdx] = 0.01f + splitDist * clipDist;

                    // Calculate the position of the lower corner of the cascade partition, in the UV space
                    // of the first cascade partition
                    Matrix invCascadeMat = Matrix.Invert(shadowMatrix);
                    Vector4 tf = Vector4.Transform(Vector3.Zero, invCascadeMat);
                    Vector3 cascadeCorner = new Vector3(tf.X, tf.Y, tf.Z);
                    cascadeCorner = Vector3.Transform(cascadeCorner, globalShadowMatrix);

                    // Do the same for the upper corner
                    tf = Vector4.Transform(Vector3.One, invCascadeMat);
                    Vector3 otherCorner = new Vector3(tf.X, tf.Y, tf.Z);
                    otherCorner = Vector3.Transform(otherCorner, globalShadowMatrix);

                    // Calculate the scale and offset
                    var cascadeScale = Vector3.One / (otherCorner - cascadeCorner);
                    CascadeOffsets[cascadeIdx] = new Vector4(-cascadeCorner, 0.0f);
                    CascadeScales[cascadeIdx] = new Vector4(cascadeScale, 1.0f);

                }
                device.SetRenderTarget(null);
            }

#endregion

            for (int pass = 0; pass < RenderPasses.TotalPasses; pass++)
            {
                if (pass == RenderPasses.Particles)
                {
                    lock (particleSystems)
                    {
                        ParticleSystem[] ps = particleSystems.Values.ToArray();

                        for (int i=0; i<ps.Length; i++)
                        {
                            ps[i].Update(gt);
                            ps[i].Draw(gt, View, Projection);
                        }
                    }
                }
                else
                {
                    foreach (RenderCommandSet renderingRenderCommand in renderingRenderCommands)
                    {
                        if (pass == renderingRenderCommand.RenderPass)
                        {
                            switch (pass)
                            {
#region Sky rendering
                                case RenderPasses.Sky:
                                    {
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
                                        if (!done)
                                        {
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
                                                device.Clear(Color.SkyBlue);
                                                device.DepthStencilState = DepthStencilState.None;
                                                RenderSkyCommand(renderingRenderCommand, state, viewMatrix);
                                                device.Flush();
                                            }

                                            device.SetRenderTarget(null);
                                            AssetManager.Instance.environment = rtc;

                                            
                                        }

                                        // Clean up after above
                                        Matrix World = Matrix.CreateTranslation(pos);
                                        if (Renderer.GetSkyType() == Skies.SimpleScattered)
                                        {
                                            Effect effect = loadedShaders[@"Shaders\Forward\ScatteredSky"];
                                            effect.Parameters["View"].SetValue(View);
                                            effect.Parameters["WorldViewProjection"].SetValue(World * View * Projection);
                                            effect.Parameters["ViewInverse"].SetValue(Matrix.Invert(View));
                                            effect.Parameters["World"].SetValue(World);
                                        }
                                        else
                                        {
                                            Effect effect = loadedShaders[@"Shaders\Forward\TracedSky"];
                                            effect.Parameters["View"].SetValue(View);
                                            effect.Parameters["Projection"].SetValue(Projection);
                                            effect.Parameters["World"].SetValue(World);
                                        }
                                        if (Renderer.Instance.effectpasses.Count > 0)
                                        {
                                            device.SetRenderTarget(backbuffer);
                                            device.DepthStencilState = DepthStencilState.None;
                                        }
                                        device.Clear(Color.SkyBlue);
                                        // Draw the sky into the scene
                                        renderingRenderCommand.World = World;
                                        renderingRenderCommand.View = View;
                                        RenderACommand(renderingRenderCommand, state);

                                    }
                                    break;
#endregion

#region Moon, Stars, and Planets
                                case RenderPasses.Ephemeris:
                                    {
                                        RenderACommand(renderingRenderCommand, state);
                                    }
                                    break;
#endregion

                                case RenderPasses.Terrain:
                                    {
                                        RenderACommand(renderingRenderCommand, state);
                                    }
                                    break;

                                case RenderPasses.Transparent:
                                    {
                                        RenderACommand(renderingRenderCommand, state);
                                    }
                                    break;

                                default:
                                    {
                                        RenderACommand(renderingRenderCommand, state);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            lock (particleSystems)
            {
                ParticleSystem[] ps = particleSystems.Values.ToArray();

                for (int i = 0; i < ps.Length; i++)
                {
                    ps[i].Update(gt);
                    ps[i].Draw(gt, View, Projection);
                }
            }
            /// Fullscreen effect passes
            if (Renderer.Instance.effectpasses.Count > 0)
            {
                device.SetRenderTarget(null);
                Rectangle src = new Rectangle(0, 0, backbuffer.Width, backbuffer.Height);
                foreach (RenderEffectPass pass in Renderer.Instance.effectpasses)
                {
                    switch(pass.Type)
                    {
                        case EffectPassType.BlackOut:
                            {
                                BlackoutEffectPass roep = (BlackoutEffectPass)pass;
                                Vector4 drawcolour = new Vector4(0, 0, 0, roep.Value);

                                device.DepthStencilState = DepthStencilState.None;
                                device.SetRenderTarget(staging_texture);
                                device.Clear(Color.White);

                                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                                spriteBatch.Draw(backbuffer, src, Color.White);
                                spriteBatch.FillRectangle(src, Color.FromNonPremultiplied(drawcolour));
                                spriteBatch.End();

                                device.SetRenderTarget(backbuffer);
                                device.Clear(Color.Black);
                                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                                spriteBatch.Draw(staging_texture, src, Color.White);
                                spriteBatch.End();
                                device.SetRenderTarget(null);
                            }
                            break;
                        case EffectPassType.RedOut:
                            {
                                RedOutEffectPass roep = (RedOutEffectPass)pass;
                                Vector4 drawcolour = new Vector4(1, 0, 0, roep.Value);

                                device.DepthStencilState = DepthStencilState.None;
                                device.SetRenderTarget(staging_texture);
                                device.Clear(Color.White);

                                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                                spriteBatch.Draw(backbuffer, src, Color.White);
                                spriteBatch.FillRectangle(src, Color.FromNonPremultiplied(drawcolour));
                                spriteBatch.End();

                                device.SetRenderTarget(backbuffer);
                                device.Clear(Color.Black);
                                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                                spriteBatch.Draw(staging_texture, src, Color.White);
                                spriteBatch.End();

                                device.SetRenderTarget(null);
                            }
                            break;
                        case Rendering.EffectPasses.EffectPassType.Hypoxia:
                            {
                                Effect effect = loadedShaders[@"Shaders\2D\RadialBlur"];
                                HypoxiaEffectPass hp = (HypoxiaEffectPass)pass;

                                effect.Parameters["BlurIntensity"].SetValue(hp.BlurLevel);
                                float f = 1.0f - hp.BlackoutLevel;
                                Vector4 drawcolour = new Vector4(f, f, f, f);

                                device.SetRenderTarget(staging_texture);
                                device.DepthStencilState = DepthStencilState.None;
                                spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.NonPremultiplied,SamplerState.LinearClamp,DepthStencilState.None,null,effect);
                                spriteBatch.Draw(backbuffer, src, Color.White);
                                spriteBatch.End();

                                device.SetRenderTarget(backbuffer);
                                spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend);
                                spriteBatch.Draw(staging_texture, src, Color.FromNonPremultiplied(drawcolour));
                                spriteBatch.End();

                                device.SetRenderTarget(null);
                            }
                            break;
                    }
                   
                }
                Renderer.Instance.effectpasses.Clear();
                spriteBatch.Begin();
                spriteBatch.Draw(backbuffer, src, Color.White);
                spriteBatch.End();

            }


            SignalRenderingComplete();
            Engine.EndDrawFrame(gt);
            SignalRendererFinished();


        }

        /// <summary>
        /// Render a command set
        /// </summary>
        /// <param name="renderingRenderCommand">Command set to render</param>
        /// <param name="state">World state</param>
        private void RenderACommand(RenderCommandSet renderingRenderCommand, WorldState state)
        {
            device.RasterizerState = Renderer.GetRasteriser(renderingRenderCommand.RS);
            device.DepthStencilState = renderingRenderCommand.DS;

            if (renderingRenderCommand.IsStaticMesh)
            {
                device.BlendState = renderingRenderCommand.blend;
                device.RasterizerState = Renderer.GetRasteriser(renderingRenderCommand.RS);
                renderingRenderCommand.fx.Parameters["World"].SetValue(renderingRenderCommand.World);
                renderingRenderCommand.fx.Parameters["View"].SetValue(renderingRenderCommand.View);
                foreach (ModelMesh mesh in renderingRenderCommand.mesh.Meshes)
                {
                    mesh.Draw();
                }
            }
            else
            {

                foreach (RenderCommand r in renderingRenderCommand.Commands)
                {
                    if (r.OwnerDraw)
                    {
                        r.Draw(device);
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
                        device.BlendState = r.blendstate;

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

        private void RenderSkyCommand(RenderCommandSet renderingRenderCommand, WorldState state, Matrix view)
        {
            Matrix World;
            device.BlendState = BlendState.Opaque;

            if (Renderer.GetSkyType() == Skies.Traced)
            {
                World = Matrix.CreateTranslation(CameraPosition);
                device.DepthStencilState = DepthStencilState.None;
            }
            else
            {
                Vector3 pos = CameraPosition + (Vector3.UnitY * -160.0f);
                World = Matrix.CreateTranslation(pos);
            }

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 1, 1, 10000);

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
        
        public override void AddDirectionalLight(Vector3 dir, Color col, bool isSun, bool isMoon)
        {
            currentLightManager.AddDirectionalLight(dir, col, isSun, isMoon);
        }

        public override void AddPointLight(Vector3 pos, Color color, float rad, float intensity)
        {
            currentLightManager.AddPointLight(pos, color, rad, intensity);
        }

#region Shader management
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

                        case ShaderVariables.Lit:
                            {
                                float h1 = MathUtils.HorizonDistance(r.World.Translation.Y);
                                if (h1 < 1)
                                    h1 = 1;
                                Vector2 i1, i2;
                                MathUtils.FindCircleCircleIntersections(0, 0, 6357000.0f, 0, r.World.Translation.Y + 6357000.0f, h1, out i1, out i2);
                                Vector3 direction = new Vector3(0, r.World.Translation.Y, 0) - new Vector3(i1.X, i1.Y - 6357000.0f, 0);
                                direction.Normalize();

                                float da = (float)Math.Asin(-direction.Y);
                                float lit = 0;
                                if (state.SunElevation > da)
                                    lit = 1;
                                fx.Parameters["LightMask"].SetValue(lit);
                            }
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

                        case ShaderVariables.SunColour:
                            fx.Parameters["SunColour"].SetValue(SunColour);
                            break;
                        case ShaderVariables.SunDirection:
                            fx.Parameters["SunDirection"].SetValue(SunDirection);
                            break;
                        case ShaderVariables.AmbientColour:
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
                            if (fx.Parameters["ShadowMap"] != null)
                                fx.Parameters["ShadowMap"].SetValue(shadows);

                            fx.Parameters["CascadeSplits"].SetValue(new Vector4(CascadeSplits[0], CascadeSplits[1], CascadeSplits[2], CascadeSplits[3]));
                            fx.Parameters["ShadowMatrix"].SetValue(globalShadowMatrix);
                            fx.Parameters["CascadeOffsets"].SetValue(CascadeOffsets);
                            fx.Parameters["CascadeScales"].SetValue(CascadeScales);
                            fx.Parameters["Bias"].SetValue(0.002f);

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

       
#endregion

#region Static methods
        

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
        

#endregion

#region Helpers
        

        /// <summary>
        /// Makes the "global" shadow matrix used as the reference point for the cascades.
        /// </summary>
        private Matrix MakeGlobalShadowMatrix(WorldState state)
        {
            // Get the 8 points of the view frustum in world space
            ResetViewFrustumCorners();

            Matrix ViewProjection = state.View * state.Projection;
            Matrix invViewProj = Matrix.Invert(ViewProjection);
            Vector3 frustumCenter = Vector3.Zero;
            for (int i = 0; i < 8; i++)
            {
                Vector4 t = Vector4.Transform(frustumCorners[i], invViewProj);
                frustumCorners[i] = new Vector3(t.X, t.Y, t.Z);
                frustumCenter += frustumCorners[i];
            }

            frustumCenter /= 8.0f;

            // Pick the up vector to use for the light camera
            Vector3 upDir = state.camera.Right;

            // This needs to be constant for it to be stable
            if (Renderer.GetSettings().StabilizeCascades)
                upDir = Vector3.Up;

            // Get position of the shadow camera
            Vector3 shadowCameraPos = frustumCenter + new Vector3(state.SunDirection.X, state.SunDirection.Y, state.SunDirection.Z) * -0.5f;

            // Come up with a new orthographic camera for the shadow caster
            Matrix shadowCamera = Matrix.CreateOrthographicOffCenter(-0.5f, 0.5f, -0.5f, 0.5f, 0.0f, 1.0f);
            Matrix look = Matrix.CreateLookAt(shadowCameraPos, frustumCenter, upDir);
            Matrix texScaleBias = Matrix.CreateScale(0.5f, -0.5f, 1.0f);
            texScaleBias.Translation = new Vector3(0.5f, 0.5f, 0.0f);
            return look * shadowCamera * texScaleBias;
        }

        private void ResetViewFrustumCorners()
        {
            frustumCorners[0] = new Vector3(-1.0f, 1.0f, 0.0f);
            frustumCorners[1] = new Vector3(1.0f, 1.0f, 0.0f);
            frustumCorners[2] = new Vector3(1.0f, -1.0f, 0.0f);
            frustumCorners[3] = new Vector3(-1.0f, -1.0f, 0.0f);
            frustumCorners[4] = new Vector3(-1.0f, 1.0f, 1.0f);
            frustumCorners[5] = new Vector3(1.0f, 1.0f, 1.0f);
            frustumCorners[6] = new Vector3(1.0f, -1.0f, 1.0f);
            frustumCorners[7] = new Vector3(-1.0f, -1.0f, 1.0f);
        }
#endregion
    }
}
