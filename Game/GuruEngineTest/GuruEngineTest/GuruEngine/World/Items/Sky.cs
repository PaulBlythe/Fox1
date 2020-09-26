using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.World;
using GuruEngine.Rendering;
using GuruEngine.Assets;

namespace GuruEngine.World.Items
{
    public class Sky : WorldItem
    {
        float Theta;
        public float Phi = 0.0f;

        int GUID;
        int shaderGUID;
        int dayGUID;
        int nightGUID;
        int sunsetGUID;

        Effect fx;
        RenderCommandSet rendercommand;


        bool mapped = false;

        private Vector4 lightColorAmbient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
        private Vector4 fogColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        private float fDensity = 0.0028f;
        private float sunLightness = 0.2f;
        private float sunRadiusAttenuation = 256.0f;
        private float largeSunLightness = 0.2f;
        private float largeSunRadiusAttenuation = 1.0f;
        private float dayToSunsetSharpness = 1.5f;
        private float hazeTopAltitude = 100.0f;

        public Sky()
        {
            UpdatePass = 1;

            String mesh = @"StaticMeshes\World\skydome";
            AssetManager.AddStaticMeshToQue(mesh);
            GUID = mesh.GetHashCode();

            String shader = @"Shaders\Forward\ScatteredSky";
            AssetManager.AddShaderToQue(shader);
            shaderGUID = shader.GetHashCode();

            String day = @"Textures\Sky\SkyDay";
            AssetManager.AddTextureToQue(day);
            dayGUID = day.GetHashCode();

            String night = @"Textures\Sky\SkyNight";
            AssetManager.AddTextureToQue(night);
            nightGUID = night.GetHashCode();

            String sunset = @"Textures\Sky\Sunset";
            AssetManager.AddTextureToQue(sunset);
            sunsetGUID = sunset.GetHashCode();

            
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

                Texture2D day = AssetManager.Texture(dayGUID);
                if (day == null)
                    return;

                Texture2D night = AssetManager.Texture(nightGUID);
                if (night == null)
                    return;

                Texture2D sunset = AssetManager.Texture(sunsetGUID);
                if (sunset == null)
                    return;

                fx.Parameters["SkyTextureNight"].SetValue(night);
                fx.Parameters["SkyTextureSunset"].SetValue(sunset);
                fx.Parameters["SkyTextureDay"].SetValue(day);
                fx.Parameters["FogColor"].SetValue(fogColor);
                fx.Parameters["fDensity"].SetValue(fDensity);
                fx.Parameters["SunLightness"].SetValue(sunLightness);
                fx.Parameters["sunRadiusAttenuation"].SetValue(sunRadiusAttenuation);
                fx.Parameters["largeSunLightness"].SetValue(largeSunLightness);
                fx.Parameters["largeSunRadiusAttenuation"].SetValue(largeSunRadiusAttenuation);
                fx.Parameters["dayToSunsetSharpness"].SetValue(dayToSunsetSharpness);
                fx.Parameters["hazeTopAltitude"].SetValue(hazeTopAltitude);
                fx.Parameters["isSkydome"].SetValue(true);

                foreach (ModelMesh meshp in mesh.Meshes)
                {
                    foreach (ModelMeshPart mp in meshp.MeshParts)
                    {
                        mp.Effect = fx;
                    }
                }
                rendercommand = new RenderCommandSet();
                rendercommand.IsStaticMesh = true;
                rendercommand.DS = DepthStencilState.None;
                rendercommand.mesh = mesh;
                rendercommand.RS = RasteriserStates.NoDepth;
                rendercommand.RenderPass = RenderPasses.Sky;
                mapped = true;
            }
            DateTime when = WorldState.GetWorldState().GameTime;

            int minutes = (when.Hour * 60) + when.Minute;
            Theta = (float)(minutes * Math.PI / 12.0 / 60.0f);

            //state.SunDirection.X = (float)(Math.Sin(Theta) * Math.Cos(Phi));
            //state.SunDirection.Y = (float)(Math.Cos(Theta));
            //state.SunDirection.Z = (float)(Math.Sin(Theta) * Math.Sin(Phi));
            //state.SunDirection.W = 1.0f;
           
            Vector3 sd = -state.SunDirectionRefracted.ToVector3F();
            state.SunDirection.X = sd.X;
            state.SunDirection.Y = sd.Y;
            state.SunDirection.Z = sd.Z;
            state.SunDirection.W = 1.0f;

            state.SunDirection.Normalize();

            

            float sun_angle = MathHelper.ToDegrees(Theta) % 360;
            if (sun_angle < 180.0f)
                sun_angle = 180 - sun_angle;
            else
                sun_angle = Math.Abs(180.0f - sun_angle);

            double red_scat_f, red_scat_corr_f, green_scat_f, blue_scat_f;
            double[] sun_color = new double[3];
            double path_distance;

            sun_angle = (float)(sun_angle * Math.PI / 180.0);
            float visibility = 22000;
            float lat = 0;

            float sun_exp2_punch_through = (float)(2.0 / Math.Log(visibility));
            float aerosol_factor = (float)(80.5 / Math.Log(visibility / 99.9));
            float rel_humidity = 0.5f;
            float density_avg = 0.7f;

            const double r_earth_pole = 6356752.314;
            const double r_tropo_pole = 6356752.314 + 8000;
            const double epsilon_earth2 = 6.694380066E-3;
            const double epsilon_tropo2 = 9.170014946E-3;

            double r_tropo = r_tropo_pole / Math.Sqrt(1 - (epsilon_tropo2 * Math.Pow(Math.Cos(lat), 2)));
            double r_earth = r_earth_pole / Math.Sqrt(1 - (epsilon_earth2 * Math.Pow(Math.Cos(lat), 2)));

            double position_radius = r_earth;

            double gamma = Math.PI - sun_angle;
            double sin_beta = (position_radius * Math.Sin(gamma)) / r_tropo;
            if (sin_beta > 1.0) sin_beta = 1.0;
            double alpha = Math.PI - gamma - Math.Asin(sin_beta);

            // OK, now let's calculate the distance the light travels
            path_distance = Math.Sqrt(Math.Pow(position_radius, 2) + Math.Pow(r_tropo, 2) - (2 * position_radius * r_tropo * Math.Cos(alpha)));

            red_scat_f = (aerosol_factor * path_distance * density_avg) / 5E+07;
            red_scat_corr_f = sun_exp2_punch_through / (1 - red_scat_f);
            sun_color[0] = 1 - red_scat_f;

            green_scat_f = (aerosol_factor * path_distance * density_avg) / 8.8938E+06;
            sun_color[1] = 1 - green_scat_f;

            blue_scat_f = (aerosol_factor * path_distance * density_avg) / 3.607E+06;
            sun_color[2] = 1 - blue_scat_f;

            double saturation = 1 - (rel_humidity / 200);
            sun_color[1] += ((1 - saturation) * (1 - sun_color[1]));
            sun_color[2] += ((1 - saturation) * (1 - sun_color[2]));

            if (sun_color[0] > 1.0) sun_color[0] = 1.0;
            if (sun_color[0] < 0.0) sun_color[0] = 0.0;
            if (sun_color[1] > 1.0) sun_color[1] = 1.0;
            if (sun_color[1] < 0.0) sun_color[1] = 0.0;
            if (sun_color[2] > 1.0) sun_color[2] = 1.0;
            if (sun_color[2] < 0.0) sun_color[2] = 0.0;

            state.SunColour = new Vector4((float)sun_color[0], (float)sun_color[1], (float)sun_color[2], 1);
            state.AmbientColour = state.SunColour * 0.25f;

            Renderer.AddDirectionalLight(new Vector3(state.SunDirection.X,state.SunDirection.Y,state.SunDirection.Z), Color.FromNonPremultiplied(state.SunColour), true, false); 

            Vector3 pos = state.CameraPosition + (Vector3.UnitY * -120.0f);
            Matrix World = Matrix.CreateTranslation(pos);
            Matrix WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(World));

            fx.Parameters["WorldInverseTranspose"].SetValue(WorldInverseTranspose);
            fx.Parameters["WorldViewProjection"].SetValue(World * state.View * state.Projection);
            fx.Parameters["ViewInverse"].SetValue(state.ViewInverse);
            fx.Parameters["World"].SetValue(World);
            fx.Parameters["LightDirection"].SetValue(state.SunDirection);
            fx.Parameters["LightColor"].SetValue(state.SunColour);
            fx.Parameters["LightColorAmbient"].SetValue(lightColorAmbient);
            fx.Parameters["MoonPosition"].SetValue(WorldState.GetWorldState().MoonPosition.ToVector3F());

            Renderer.AddRenderCommand(rendercommand);
        }
       
    }
}
