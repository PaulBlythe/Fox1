using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using GuruEngine.Rendering;
using GuruEngine.Assets;
using GuruEngine.World.Calculators;

namespace GuruEngine.World.Items
{
    public class TracedSky : WorldItem
    {
        #region Configuration parameters
        public float SunIntensity = 5;
        public float ScatteringSkyLightScale = 0.000005f;
        public float PlanetRadius = 6360e3f;
        public float AtmosphereHeight = 160e3f;
        public float ScaleHeight = 15e3f;
        public int NumberOfSamples = 5;
        public Vector3 BetaRayleigh = new Vector3(6.95e-6f, 11.8e-6f, 24.4e-6f);
        public Vector3 BetaMie = new Vector3(2e-5f) / 40;
        public float GMie = 0.99f;      // Use 0.99 to create a sun disk procedurally.
        public float Transmittance = 1;
        public bool UseBaseColour = true;
        public Vector3 BaseHorizonColor = new Vector3(0.043f, 0.090f, 0.149f) * 0.01f;
        public Vector3 BaseZenithColor = new Vector3(0.024f, 0.051f, 0.102f) * 0.01f;
        public float BaseColorShift = 0.5f;
        #endregion


        Effect fx;
        RenderCommandSet rendercommand;
        int shaderGUID;
        int GUID;
        bool mapped = false;

        EffectParameter parameterView;
        EffectParameter parameterProjection;
        EffectParameter parameterSunDirection;
        EffectParameter parameterRadii;
        EffectParameter parameterNumberOfSamples;
        EffectParameter parameterBetaRayleigh;
        EffectParameter parameterBetaMie;
        EffectParameter parameterGMie;
        EffectParameter parameterSunIntensity;
        EffectParameter parameterTransmittance;
        EffectParameter parameterBaseHorizonColor;
        EffectParameter parameterBaseZenithColor;
        EffectParameter parameterWorld;

        Vector3 SunColour;

        public TracedSky()
        {
            UpdatePass = 1;

            String shader = @"Shaders\Forward\TracedSky";
            AssetManager.AddShaderToQue(shader);
            shaderGUID = shader.GetHashCode();

            String mesh = @"StaticMeshes\box";
            AssetManager.AddStaticMeshToQue(mesh);
            GUID = mesh.GetHashCode();

        }

        public override void Update(WorldState state)
        {
            if (!mapped)
            {
                Model mesh = AssetManager.StaticMesh(GUID);
                if (mesh == null)
                    return;

                fx = AssetManager.Shader(shaderGUID);
                if (fx == null)
                    return;

                parameterView = fx.Parameters["View"];
                parameterWorld = fx.Parameters["World"];
                parameterProjection = fx.Parameters["Projection"];
                parameterSunDirection = fx.Parameters["SunDirection"];
                parameterRadii = fx.Parameters["Radii"];
                parameterNumberOfSamples = fx.Parameters["NumberOfSamples"];
                parameterBetaRayleigh = fx.Parameters["BetaRayleigh"];
                parameterBetaMie = fx.Parameters["BetaMie"];
                parameterGMie = fx.Parameters["GMie"];
                parameterSunIntensity = fx.Parameters["SunIntensity"];
                parameterTransmittance = fx.Parameters["Transmittance"];
                parameterBaseHorizonColor = fx.Parameters["BaseHorizonColor"];
                parameterBaseZenithColor = fx.Parameters["BaseZenithColor"];

                rendercommand = new RenderCommandSet();
                rendercommand.IsStaticMesh = true;
                rendercommand.fx = fx;
                rendercommand.DS = DepthStencilState.None;
                rendercommand.mesh = mesh;
                rendercommand.RS = RasteriserStates.NoDepthNoCull;
                rendercommand.RenderPass = RenderPasses.Sky;
                rendercommand.blend = BlendState.Opaque;

                foreach (ModelMesh meshp in mesh.Meshes)
                {
                    foreach (ModelMeshPart mp in meshp.MeshParts)
                    {
                        mp.Effect = fx;
                    }
                }
                mapped = true;
            }


            SunColour = Ephemeris.Ephemeris.ExtraterrestrialSunlight * ScatteringSkyLightScale;
            float ObserverAltitude = state.CameraPosition.Y;
            state.SunDirection.Normalize();
            state.SunColour = new Vector4(SunColour, 1);
            state.AmbientColour = new Vector4(SunColour, 1) * 0.25f;

            Renderer.AddDirectionalLight(new Vector3(state.SunDirection.X, state.SunDirection.Y, state.SunDirection.Z), Color.FromNonPremultiplied(state.SunColour),true, false);

            parameterProjection.SetValue(state.Projection);
            parameterSunDirection.SetValue(state.SunDirectionRefracted.ToVector4());
            parameterSunIntensity.SetValue((Vector3)(SunIntensity * SunColour));
            parameterRadii.SetValue(new Vector4(AtmosphereHeight + PlanetRadius, PlanetRadius, ObserverAltitude + PlanetRadius, ScaleHeight));
            parameterNumberOfSamples.SetValue(NumberOfSamples);
            parameterBetaRayleigh.SetValue((Vector3)BetaRayleigh);
            parameterBetaMie.SetValue((Vector3)BetaMie);
            parameterGMie.SetValue(GMie);
            parameterTransmittance.SetValue(Transmittance);
            

            if (UseBaseColour)
            {
                parameterBaseHorizonColor.SetValue((Vector4)new Vector4(BaseHorizonColor, BaseColorShift));
                parameterBaseZenithColor.SetValue(BaseZenithColor);
                if (Renderer.IsHDREnabled())
                {
                    fx.CurrentTechnique = fx.Techniques["LinearWithBaseColour"];
                }
                else
                {
                    fx.CurrentTechnique = fx.Techniques["GammaWithBaseColour"];
                }
            }
            else
            {
                if (Renderer.IsHDREnabled())
                {
                    fx.CurrentTechnique = fx.Techniques["Linear"];
                }
                else
                {
                    fx.CurrentTechnique = fx.Techniques["Gamma"];
                }
            }
            Renderer.AddRenderCommand(rendercommand);
        }
    }
}
