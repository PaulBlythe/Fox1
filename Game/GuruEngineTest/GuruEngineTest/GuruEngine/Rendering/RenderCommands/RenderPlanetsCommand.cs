using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using GuruEngine.Rendering;
using GuruEngine.World;
using GuruEngine.Maths;
using GuruEngine.World.Ephemeris;
using GuruEngine.Algebra;
using GuruEngine.World.Items;

namespace GuruEngine.Rendering.RenderCommands
{
    //struct PlanetData
    //{
    //    // Equatorial coordinates.
    //    public double RightAscension;
    //    public double Declination;
    //
    //    // Measures the brightness of the planet as seen on earth. Lower values mean more brightness.
    //    public double VisualMagnitude;
    //};

    public class RenderPlanetsCommand : RenderCommand
    {
        double earthRadius = 1;
        double earthEclipticLongitude;

        Texture2D texture;
        List<Vector3> Verts = new List<Vector3>();
        List<Color> Colours = new List<Color>();
        BasicEffect effect;
        SpriteBatch batch;
        Matrix world;
        const float Radius = 1800;

        public RenderPlanetsCommand(Texture2D tex)
        {
            texture = tex;
            OwnerDraw = true;

            effect = new BasicEffect(Renderer.GetGraphicsDevice());
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;
            effect.DiffuseColor = Color.White.ToVector3();
            effect.AmbientLightColor = Color.White.ToVector3();
            world = Matrix.CreateScale(1, -1, 1);
            batch = new SpriteBatch(Renderer.GetGraphicsDevice());
        }

        public override void PreRender(GraphicsDevice dev)
        {
            Verts.Clear();
            Colours.Clear();

            WorldState state = WorldState.GetWorldState();

            double epoch1990Days = Ephemeris.ToEpoch1990Days(state.GameTime, false);
            double epoch2000Centuries = Ephemeris.ToEpoch2000Centuries(state.GameTime, false);

            double _e = 0.409093 - 0.000227 * epoch2000Centuries;
            ComputeEarthPosition(epoch1990Days);

            for (int index = 0; index < planetElements.Length; index++)
            {
                double Np = ((2.0 * MathConstants.PI) / 365.242191) * (epoch1990Days / planetElements[index].Period);
                Np = DoubleClassExtension.InRange(Np);

                double Mp = Np + planetElements[index].EpochLongitude - planetElements[index].PerihelionLongitude;

                double heliocentricLongitude = Np + 2.0 * planetElements[index].Eccentricity * Math.Sin(Mp) + planetElements[index].EpochLongitude;
                heliocentricLongitude = DoubleClassExtension.InRange(heliocentricLongitude);

                double vp = heliocentricLongitude - planetElements[index].PerihelionLongitude;

                double radius = (planetElements[index].SemiMajorAxis * (1.0 - planetElements[index].Eccentricity * planetElements[index].Eccentricity)) / (1.0 + planetElements[index].Eccentricity * Math.Cos(vp));

                double heliocentricLatitude = Math.Asin(Math.Sin(heliocentricLongitude - planetElements[index].LongitudeAscendingNode) * Math.Sin(planetElements[index].Inclination));
                heliocentricLatitude = DoubleClassExtension.InRange(heliocentricLatitude);

                double y = Math.Sin(heliocentricLongitude - planetElements[index].LongitudeAscendingNode) * Math.Cos(planetElements[index].Inclination);
                double x = Math.Cos(heliocentricLongitude - planetElements[index].LongitudeAscendingNode);

                double projectedHeliocentricLongitude = Math.Atan2(y, x) + planetElements[index].LongitudeAscendingNode;

                double projectedRadius = radius * Math.Cos(heliocentricLatitude);

                double eclipticLongitude;

                if (index >= 2)
                {
                    eclipticLongitude = Math.Atan((earthRadius * Math.Sin(projectedHeliocentricLongitude - earthEclipticLongitude)) / (projectedRadius - earthRadius * Math.Cos(projectedHeliocentricLongitude - earthEclipticLongitude))) + projectedHeliocentricLongitude;
                }
                else
                {
                    eclipticLongitude = MathConstants.PI + earthEclipticLongitude + Math.Atan((projectedRadius * Math.Sin(earthEclipticLongitude - projectedHeliocentricLongitude)) / (earthRadius - projectedRadius * Math.Cos(earthEclipticLongitude - projectedHeliocentricLongitude)));
                }

                eclipticLongitude = DoubleClassExtension.InRange(eclipticLongitude);

                double eclipticLatitude = Math.Atan((projectedRadius * Math.Tan(heliocentricLatitude) * Math.Sin(eclipticLongitude - projectedHeliocentricLongitude)) / (earthRadius * Math.Sin(projectedHeliocentricLongitude - earthEclipticLongitude)));

                double ra = Math.Atan2((Math.Sin(eclipticLongitude) * Math.Cos(_e) - Math.Tan(eclipticLatitude) * Math.Sin(_e)), Math.Cos(eclipticLongitude));

                double dec = Math.Asin(Math.Sin(eclipticLatitude) * Math.Cos(_e) + Math.Cos(eclipticLatitude) * Math.Sin(_e) * Math.Sin(eclipticLongitude));

                double dist2 = earthRadius * earthRadius + radius * radius - 2 * earthRadius * radius * Math.Cos(heliocentricLongitude - earthEclipticLongitude);
                double dist = Math.Sqrt(dist2);

                double d = eclipticLongitude - heliocentricLongitude;
                double phase = 0.5 * (1.0 + Math.Cos(d));

                double visualMagnitude;

                if (index == 1)
                {
                    d *= MathHelper.ToDegrees((float)d);
                    visualMagnitude = -4.34 + 5.0 * Math.Log10(radius * dist) + 0.013 * d + 4.2E-7 * d * d * d;
                }
                else
                {
                    visualMagnitude = 5.0 * Math.Log10((radius * dist) / Math.Sqrt(phase)) + planetElements[index].VisualMagnitude;
                }

                Matrix m = Matrix.CreateRotationX((float)dec) * Matrix.CreateRotationY((float)ra);

                double c = 1.0 - (visualMagnitude / 8.0);
                float rd = MathHelper.SmoothStep(1800, 1200, (float)c);
                Verts.Add(Vector3.Transform(new Vector3(0, 0, rd), m));


                c = Math.Min(1, c);
                c = Math.Max(0.1333, c);
                Color sc = new Color(1, 1, 1, (float)c);
                Colours.Add(sc);

            }
        }


        private void ComputeEarthPosition(double _epoch1990Days)
        {
            double Np = ((2.0 * MathConstants.PI) / 365.242191) * (_epoch1990Days / 1.00004);
            Np = DoubleClassExtension.InRange(Np);

            double Mp = Np + MathHelper.ToRadians(99.403308f) - MathHelper.ToRadians(102.768413f);

            earthEclipticLongitude = Np + 2.0 * 0.016713 * Math.Sin(Mp) + MathHelper.ToRadians(99.403308f);

            earthEclipticLongitude = DoubleClassExtension.InRange(earthEclipticLongitude);

        }

        public override void Draw(GraphicsDevice dev)
        {
            world = Matrix.CreateScale(1, -1, 1);
            Matrix trans = Matrix.CreateRotationY(-MathHelper.ToRadians((float)WorldState.GetWorldState().LST)) * Matrix.CreateFromYawPitchRoll((float)WorldState.GetWorldState().CameraLongitude, (float)WorldState.GetWorldState().CameraLatitude, 0);

            effect.World = world;
            effect.View = Matrix.Identity;
            effect.Projection = WorldState.GetWorldState().Projection;

            Matrix mw = WorldState.GetWorldState().View * world;
            Vector2 Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, DepthStencilState.None, RasterizerState.CullNone, effect);
            for (int i = 0; i < Verts.Count; i++)
            {
                Vector3 worldspace = Vector3.Transform(Verts[i], trans);
                Vector3 viewSpaceTextPosition = Vector3.Transform(worldspace, mw);
                batch.Draw(texture, new Vector2(viewSpaceTextPosition.X, viewSpaceTextPosition.Y), null, Colours[i], 0.0f, Origin, 1.0f, SpriteEffects.None, viewSpaceTextPosition.Z);
            }
            batch.End();
        }



        private readonly OrbitalElements[] planetElements =
        {
          // Mercury
          new OrbitalElements(0.240852, MathHelper.ToRadians(60.750646f), MathHelper.ToRadians(77.299833f), 0.205633, 0.387099, MathHelper.ToRadians(7.004540f), MathHelper.ToRadians(48.212740f), 6.74, -0.42),

          // Venus
          new OrbitalElements(0.615211, MathHelper.ToRadians(88.455855f), MathHelper.ToRadians(131.430236f), 0.006778, 0.723332, MathHelper.ToRadians(3.394535f), MathHelper.ToRadians(76.589820f), 16.92, -4.40),

          // Mars
          new OrbitalElements(1.880932, MathHelper.ToRadians(240.739474f), MathHelper.ToRadians(335.874939f), 0.093396, 1.523688, MathHelper.ToRadians(1.849736f), MathHelper.ToRadians(49.480308f), 9.36, -1.52),

          // Jupiter
          new OrbitalElements(11.863075, MathHelper.ToRadians(90.638185f), MathHelper.ToRadians(14.170747f), 0.048482, 5.202561, MathHelper.ToRadians(1.303613f), MathHelper.ToRadians(100.353142f), 196.74, -9.40),

          // Saturn
          new OrbitalElements(29.471362, MathHelper.ToRadians(287.690033f), MathHelper.ToRadians(92.861407f),0.055581, 9.554747, MathHelper.ToRadians(2.488980f), MathHelper.ToRadians(113.576139f), 165.60, -8.88)
        };
    }
}
