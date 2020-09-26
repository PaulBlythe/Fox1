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

using GuruEngine.Simulation.Components.Radar.Airbourne.Modern;
using GuruEngine.Simulation.Components.Radar.Airbourne;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.AircraftSystems.General;
using GuruEngine.Helpers;
using GuruEngine.Rendering;
using GuruEngine.Rendering.Primitives;
using GuruEngine.Assets;

//( Class F16Hud )
//( Type CockpitInstrument )
//( Connection Radar Radar1 )
//( Connection Transform WorldTransform )

namespace GuruEngine.ECS.Components.Cockpit.American.Modern.F16
{
    public class F16HudComponent: ECSGameComponent
    {
        #region Private variables
        AirbourneRadarInterface radar;
        ECSGameComponent radarComponent;
        WorldTransform transform;
        RenderTarget2D renderTarget;
        Texture2D symbols;
        SpriteFont font;
        SpriteBatch batch;

        Rectangle flightPathVector = new Rectangle(0, 0, 37, 20);

        enum HUDMode
        {
            Flight,
            Landing
        }
        HUDMode hudMode = HUDMode.Flight;

        float hdg, pitch, roll;
        bool HasPower = false;
        bool HadPower = false;
        float PowerUpTimer = 0;

        #endregion

        #region ECS Game Component Interface
        public override ECSGameComponent Clone()
        {
            return new F16HudComponent();
        }

        /// <summary>
        /// Connect up the required components
        /// </summary>
        /// <param name="components"></param>
        /// <param name="isList"></param>
        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                // no lists
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
                            transform = (WorldTransform)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;

                    case "Radar1":
                        {
                            string[] objects = parts[2].Split(':');
                            radar = (AirbourneRadarInterface)Parent.FindGameComponentByName(objects[0]);
                            radarComponent = Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::F16Hud:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        /// <summary>
        /// NUll for safety
        /// </summary>
        public override void DisConnect()
        {
            radar = null;
            radarComponent = null;
            transform = null;
        }

        /// <summary>
        /// No public contained objects
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override object GetContainedObject(string type)
        {
            return null;
        }

        /// <summary>
        /// Get the rendered HUD
        /// </summary>
        /// <returns></returns>
        public override Texture2D GetOffscreenTexture()
        {
            return renderTarget;
        }

        public override void HandleEvent(string evt)
        {

        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {

        }

        /// <summary>
        /// Textures
        ///  symbols ==  the symbols file
        /// Fonts
        ///  font    ==  the same as used for the MFD
        /// </summary>
        /// <param name="content"></param>
        public override void Load(ContentManager content)
        {
            GraphicsDevice device = Renderer.GetGraphicsDevice(); 
            symbols = content.Load<Texture2D>(@"Textures/Cockpit/Instruments/f16/hud");
            renderTarget = new RenderTarget2D(device, 512, 512, false, SurfaceFormat.Color, DepthFormat.None);
            font = content.Load<SpriteFont>(@"SpriteFonts/File");
            batch = new SpriteBatch(device);
        }

        /// <summary>
        /// Reconnect 
        /// </summary>
        /// <param name="other"></param>
        public override void ReConnect(GameObject other)
        {
            F16HudComponent othermfd = (F16HudComponent)other.FindGameComponentByName(Name);
            othermfd.radar = (AirbourneRadarInterface)other.FindGameComponentByName(radarComponent.Name);
            othermfd.transform = (WorldTransform)other.FindGameComponentByName(transform.Name);
            othermfd.radarComponent = (ECSGameComponent)other.FindGameComponentByName(radarComponent.Name);
        }

        public override void RenderOffscreenRenderTargets()
        {
            int centre_y = (512 - 128) / 2;
            Vector3 AngularOrientation = MathsHelper.QuaternionToYawPitchRoll(transform.GetOrientation());

            GraphicsDevice device = Renderer.GetGraphicsDevice();
            device.SetRenderTarget(renderTarget);
            device.Clear(Color.Transparent);

            if ((HasPower) && (HadPower))
            {

                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                LineRenderer.DrawArtificialHorizon(batch, hdg, pitch, roll, 256, 30, 5, 108.2f, 512, 512 - 128, Color.Green, 2);

                #region Draw the speed ladder
                float speed = MathsHelper.MpsToKnots(transform.Velocity);
                int ispeed = (int)speed;
                LineRenderer.DrawLeftTag(batch, 20, centre_y - 11, 70, 22, Color.Green, 2);

                String t = String.Format("{0}", ispeed);
                Vector2 s = font.MeasureString(t);
                s.Y = centre_y - (s.Y * 0.5f);
                s.X = 70 - s.X;
                batch.DrawString(font, t, s, Color.Green);

                LineRenderer.DrawLeftLadder(batch, font, 82, centre_y - 90, 20, 180, ispeed, 10, 50, 150, 10, Color.Green, 2);
                #endregion

                #region Draw the altitude ladder
                float alt = MathsHelper.MetresToFeet(transform.GetLocalPosition().Y);

                LineRenderer.DrawRightLadder(batch, font, 420, centre_y - 90, 20, 180, alt, 100, 500, 1500, 1000, Color.Green, 2);
                LineRenderer.DrawRightTag(batch, 440, centre_y - 11, 70, 22, Color.Green, 2);
                alt /= 1000.0f;
                t = String.Format("{0:0.000}", alt);
                s = font.MeasureString(t);
                s.Y = centre_y - (s.Y * 0.5f);
                s.X = 508 - s.X;
                batch.DrawString(font, t, s, Color.Green);

                #endregion

                #region Draw the heading ladder
                float ahdg = AngularOrientation.X;
                ahdg = MathsHelper.GetAnglesDegrees(ahdg);
                LineRenderer.DrawBottomLadder(batch, font, 256, centre_y + 120, 192, 20, ahdg, 5, 10, 32, 10, Color.Green, 2);

                Rectangle r = new Rectangle();
                r.X = 256 - 20;
                r.Y = centre_y + 135;
                r.Width = 40;
                r.Height = 20;
                LineRenderer.DrawBox(batch, r, Color.Green, 2);

                t = String.Format("{0:000}", (int)ahdg);
                s = font.MeasureString(t);
                s.Y = centre_y - (s.Y * 0.5f) + 145;
                s.X = 256 - (s.X * 0.5f);
                batch.DrawString(font, t, s, Color.Green);

                #endregion

                #region Roll indicator

                #endregion

                #region Pitch ladder
                float roll_value = AngularOrientation.Z;
                float pitch_value = MathHelper.ToDegrees(AngularOrientation.Y);
                float compression = 25.0f;
                float half_span = 160 * 0.5f;
                float y = 0;
                float scr_hole = 20;
                // TODO apply alpha and beta

                Vector2 lo = new Vector2();
                Vector2 li = new Vector2();
                Vector2 ro = new Vector2();
                Vector2 ri = new Vector2();
                Vector2 ri2 = new Vector2();
                Vector2 li2 = new Vector2();
                Vector2 l_tag = new Vector2();
                Vector2 r_tag = new Vector2();

                Vector2 Centre = new Vector2(256, centre_y);

                for (int i = -85; i < 90; i += 5)
                {
                    if (Math.Abs(i - pitch_value) < compression)
                    {
                        y = (float)(i - pitch_value) * compression + 0.5f;
                        lo.X = -half_span;
                        ro.X = half_span;
                        li.X = ri.X = 0;
                        lo.Y = ro.Y = li.Y = ri.Y = y;
                        lo.Y += 8;
                        ro.Y += 8;

                        li.X = -scr_hole;
                        ri.X = scr_hole;

                        li2 = li;
                        ri2 = ri;
                        li2.Y -= 3;
                        ri2.Y -= 3;

                        l_tag = lo;
                        r_tag = ro;
                        l_tag.X -= 4;
                        r_tag.X += 4;
                        l_tag.Y += 5;
                        r_tag.Y += 5;

                        lo = MathsHelper.Rotate2D(lo, -roll_value) + Centre;
                        li = MathsHelper.Rotate2D(li, -roll_value) + Centre;
                        ri = MathsHelper.Rotate2D(ri, -roll_value) + Centre;
                        ro = MathsHelper.Rotate2D(ro, -roll_value) + Centre;
                        li2 = MathsHelper.Rotate2D(li2, -roll_value) + Centre;
                        ri2 = MathsHelper.Rotate2D(ri2, -roll_value) + Centre;
                        l_tag = MathsHelper.Rotate2D(l_tag, -roll_value) + Centre;
                        r_tag = MathsHelper.Rotate2D(r_tag, -roll_value) + Centre;

                        LineRenderer.DrawLine(batch, lo, li, Color.Green);
                        LineRenderer.DrawLine(batch, ro, ri, Color.Green);
                        LineRenderer.DrawLine(batch, ri2, ri, Color.Green);
                        LineRenderer.DrawLine(batch, li2, li, Color.Green);

                        String pitch_tag = String.Format("{0}", i);
                        Vector2 size = font.MeasureString(pitch_tag) * 0.5f;


                        batch.DrawString(font, pitch_tag, l_tag, Color.Green, -roll_value, size, 1.0f, SpriteEffects.None, 1);
                        batch.DrawString(font, pitch_tag, r_tag, Color.Green, -roll_value, size, 1.0f, SpriteEffects.None, 1);

                    }

                }
                #endregion


                batch.End();
            }else if (!HadPower)
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                int a = (int)(256 * Math.Sin(PowerUpTimer * 2 * MathHelper.Pi));
                float w = 256 * PowerUpTimer * 2;
                if (w > 256)
                    w = 256;
                batch.Draw(AssetManager.GetWhite(), new Rectangle(256 - (int)w, 256 - (int)w, (int)w * 2, (int)w * 2), Color.FromNonPremultiplied(0, 255, 0, a));
                batch.End();
            }

            device.SetRenderTarget(null);
        }

        /// <summary>
        /// No parameters
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public override void SetParameter(string Name, string Value)
        {
            
        }

        public override void Update(float dt)
        {
            ECSGameComponent pm = Parent.FindSingleComponentByType<PowerManagementComponent>();
            if (pm != null)
            {
                PowerManagementComponent pmc = (PowerManagementComponent)pm;
                if (pmc.HasPower(5600.0f / 14.0f))
                {
                    HasPower = true;
                    if (!HadPower)
                    {
                        PowerUpTimer += dt;
                        if (PowerUpTimer >1)
                        {
                            HadPower = true;
                        }
                    }
                }else
                {
                    HasPower = false;
                    HadPower = false;
                    PowerUpTimer = 0;
                }
            }

            Quaternion orient = transform.GetOrientation();
            Vector3 angles = MathsHelper.QuaternionToYawPitchRoll(orient);

            hdg = MathHelper.ToDegrees(angles.X);
            pitch = MathHelper.ToDegrees(angles.Y);
            roll = MathHelper.ToDegrees(angles.Z);
        }
        #endregion
    }
}
