#define TEST

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

using GuruEngine.Rendering.Gui.Instruments;
using GuruEngine.Data.Code.Instruments.American;
using GuruEngine.Rendering;
using GuruEngine.GameLogic.Navigation;
using GuruEngine.Rendering.Primitives;
using GuruEngine.Assets;
using GuruEngine.InputDevices;
using GuruEngine.World;
using GuruEngine.ECS.Components;
using GuruEngine.ECS.Components.AircraftSystems.General;
using GuruEngine.ECS.Components.World;
using GuruEngine.Simulation.Components.Radar.Airbourne.Modern;
using GuruEngine.Simulation.Components.Radar.Airbourne;
using GuruEngine.Helpers;

//( Class ColourMFD )
//( Type CockpitInstrument )
//( Connection Radar Radar1 )
//( Connection Transform WorldTransform )

namespace GuruEngine.ECS.Components.Cockpit.American.Modern.F16
{
    public enum CMFDMappableEvents
    {
        RangeUp,
        RangeDown,
        Count
    }

    public class ColourMFD : ECSGameComponent
    {
        Texture2D overlay;
        Texture2D buttons;
        Effect mfd;
        public F16MFDData dataPacket = new F16MFDData();

        public AirbourneRadarInterface radar;
        public ECSGameComponent radarComponent;

        GameObject parent = null;
        public WorldTransform transform;

        GraphicsDevice Device;
        SpriteFont font;
        SpriteFont smallfont;
        SpriteBatch batch;
        RenderTarget2D renderTarget;
        InstrumentManager buttonManager;
        Rectangle buttonImageSrc = new Rectangle(42, 26, 40, 42);
        Rectangle brightnessUpSrc = new Rectangle(38, 190, 37, 57);
        Rectangle brightnessDownSrc = new Rectangle(76, 190, 37, 57);
        Rectangle contrastUpSrc = new Rectangle(38, 130, 36, 57);
        Rectangle contrastDownSrc = new Rectangle(76, 130, 36, 57);
        Rectangle symbolUpSrc = new Rectangle(38, 70, 36, 57);
        Rectangle symbolDownSrc = new Rectangle(76, 70, 36, 57);
        Rectangle gainUpSrc = new Rectangle(155, 71, 36, 57);
        Rectangle gainDownSrc = new Rectangle(195, 71, 36, 57);
        Rectangle display_region = new Rectangle(64, 64, 512 - 128, 512 - 128);
        bool HSDDepressed = true;
        bool HSDCoupledToFCR = false;
        bool HSDDisplayFCR = true;
        bool HSDDisplayPRE = true;
        bool HSDDisplayAIFF = true;
        bool HSDDisplayLINE1 = true;
        bool HSDDisplayLINE2 = true;
        bool HSDDisplayLINE3 = true;
        bool HSDDisplayRINGS = true;
        bool HSDDisplayADLINK = true;
        bool HSDDisplayGDLINK = true;
        bool HSDDisplayNAV1 = true;
        bool HSDDisplayNAV2 = true;
        bool HSDDisplayNAV3 = true;
        bool HasPower = false;
        bool HadPower = false;

        float PowerUpTimer = 0;

        Vector2 pickle = new Vector2(256, 256);

        public bool IsMaster = false; 

        Vector2[] OSB_locations = new Vector2[]
        {
            new Vector2(133, 70),
            new Vector2(196, 70),
            new Vector2(259, 70),
            new Vector2(317, 70),
            new Vector2(379, 70),

            new Vector2(445,146),
            new Vector2(445,201),
            new Vector2(445,256),
            new Vector2(445,310),
            new Vector2(445,365),

            new Vector2(379,440),
            new Vector2(317,440),
            new Vector2(259,440),
            new Vector2(196,440),
            new Vector2(133,440),

            new Vector2(62,365),
            new Vector2(62,310),
            new Vector2(62,256),
            new Vector2(62,201),
            new Vector2(62,146)


        };

        float brightness = 0;
        float contrast = 1;
        float symbolBrightness = 0;
        float gain = 0;
        int reset_count = 0;
        float reset_timer = 0;

        bool thumb_stick = false;

        Dictionary<String, string> wpn_map = new Dictionary<string, string>();

        enum MFDMode
        {
            Menu,
            Blank,
            HUD,
            RWR,
            RCCE,
            RESET,
            SMS,
            HSD,
            DTE,
            TEST,
            FLCS,
            FLIR,
            TFR,
            WPN,
            TGP,
            FCR,
            HSD_control
        };

        MFDMode mode;


        public ColourMFD()
        {
            Device = Renderer.GetGraphicsDevice();
            mode = MFDMode.Menu;
            renderTarget = new RenderTarget2D(Device, 512, 512, false, SurfaceFormat.Color, DepthFormat.Depth24);
            batch = new SpriteBatch(Device);
            buttonManager = new InstrumentManager();

            wpn_map.Add("AIM9L", "A-9L");
            wpn_map.Add("AIM9M", "A-9M");
            wpn_map.Add("AIM9N", "A-9N");
            wpn_map.Add("AIM9P", "A-9P");
            wpn_map.Add("AIM9X", "A-9X");
            wpn_map.Add("AIM120A", "A-120A");
            wpn_map.Add("AIM120B", "A-120B");
            wpn_map.Add("AIM120C", "A-120C");
            wpn_map.Add("DT300", "TK300");
            wpn_map.Add("DT370", "TK370");
            wpn_map.Add("DT600", "TK600");
            wpn_map.Add("M61A1", "GUN");
            UpdateStage = 4;
            HasPower = false;

#if TEST
            NavPoint np1 = new NavPoint(0, 0);
            np1.Range = 100;
            np1.Bearing = 180;
            dataPacket.Navpoints.Add(np1);

            np1 = new NavPoint(0, 0);
            np1.Range = 10;
            np1.Bearing = 0;
            dataPacket.Navpoints.Add(np1);

            np1 = new NavPoint(0, 0);
            np1.Range = 120;
            np1.Bearing = 35;
            dataPacket.Navpoints.Add(np1);

#endif
        }

        public override void Load(ContentManager content)
        {
            LoadContent(content);
        }

        public void LoadContent(ContentManager content)
        {
            overlay = content.Load<Texture2D>(@"Textures/Cockpit/Instruments/f16/overlay");
            buttons = content.Load<Texture2D>(@"Textures/Cockpit/Instruments/f16/buttons");
            font = content.Load<SpriteFont>(@"SpriteFonts/System");
            smallfont = content.Load<SpriteFont>(@"SpriteFonts/SmallSystem");
            mfd = content.Load<Effect>(@"Shaders/2D/mfd");

            buttonManager.RegisterButton(this, new Rectangle(113, 9, 40, 42), "CMFD1DOWN", "CMFD1UP", true);
            buttonManager.RegisterButton(this, new Rectangle(176, 9, 40, 42), "CMFD2DOWN", "CMFD2UP", true);
            buttonManager.RegisterButton(this, new Rectangle(234, 9, 40, 42), "CMFD3DOWN", "CMFD3UP", true);
            buttonManager.RegisterButton(this, new Rectangle(297, 9, 40, 42), "CMFD4DOWN", "CMFD4UP", true);
            buttonManager.RegisterButton(this, new Rectangle(359, 9, 40, 42), "CMFD5DOWN", "CMFD5UP", true);

            buttonManager.RegisterButton(this, new Rectangle(463, 125, 40, 42), "CMFD6DOWN", "CMFD6UP", true);
            buttonManager.RegisterButton(this, new Rectangle(463, 180, 40, 42), "CMFD7DOWN", "CMFD7UP", true);
            buttonManager.RegisterButton(this, new Rectangle(463, 234, 40, 42), "CMFD8DOWN", "CMFD8UP", true);
            buttonManager.RegisterButton(this, new Rectangle(463, 289, 40, 42), "CMFD9DOWN", "CMFD9UP", true);
            buttonManager.RegisterButton(this, new Rectangle(463, 344, 40, 42), "CMFD10DOWN", "CMFD10UP", true);

            buttonManager.RegisterButton(this, new Rectangle(113, 462, 40, 42), "CMFD15DOWN", "CMFD15UP", true);
            buttonManager.RegisterButton(this, new Rectangle(176, 462, 40, 42), "CMFD14DOWN", "CMFD14UP", true);
            buttonManager.RegisterButton(this, new Rectangle(234, 462, 40, 42), "CMFD13DOWN", "CMFD13UP", true);
            buttonManager.RegisterButton(this, new Rectangle(297, 462, 40, 42), "CMFD12DOWN", "CMFD12UP", true);
            buttonManager.RegisterButton(this, new Rectangle(359, 462, 40, 42), "CMFD11DOWN", "CMFD11UP", true);

            buttonManager.RegisterButton(this, new Rectangle(9, 125, 40, 42), "CMFD20DOWN", "CMFD20UP", true);
            buttonManager.RegisterButton(this, new Rectangle(9, 180, 40, 42), "CMFD19DOWN", "CMFD19UP", true);
            buttonManager.RegisterButton(this, new Rectangle(9, 234, 40, 42), "CMFD18DOWN", "CMFD18UP", true);
            buttonManager.RegisterButton(this, new Rectangle(9, 289, 40, 42), "CMFD17DOWN", "CMFD17UP", true);
            buttonManager.RegisterButton(this, new Rectangle(9, 344, 40, 42), "CMFD16DOWN", "CMFD16UP", true);

            buttonManager.RegisterButton(this, new Rectangle(10, 399, 37, 28), "BRIGHTNESSUP", "", false);
            buttonManager.RegisterButton(this, new Rectangle(10, 399 + 29, 37, 28), "BRIGHTNESSDOWN", "", false);

            buttonManager.RegisterButton(this, new Rectangle(466, 400, 37, 28), "CONTRASTUP", "", false);
            buttonManager.RegisterButton(this, new Rectangle(466, 429, 37, 28), "CONTRASTDOWN", "", false);

            buttonManager.RegisterButton(this, new Rectangle(466, 55, 37, 28), "SYMBOLUP", "", false);
            buttonManager.RegisterButton(this, new Rectangle(466, 84, 37, 28), "SYMBOLDOWN", "", false);

            buttonManager.RegisterButton(this, new Rectangle(10, 55, 37, 28), "GAINUP", "", false);
            buttonManager.RegisterButton(this, new Rectangle(10, 84, 37, 28), "GAINDOWN", "", false);
        }

        public override Texture2D GetOffscreenTexture()
        {
            return renderTarget;
        }

        public override object GetContainedObject(string type)
        {
            switch (type)
            {
                case "InstrumentManager":
                    return (object)buttonManager;

                default:
                    return null;
            }
        }

        public override void SetParameter(string Name, string Value)
        {
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {

        }

        public override void HandleEvent(string evt)
        {
            switch (mode)
            {
                #region HSD Control page
                case MFDMode.HSD_control:
                    {
                        switch (evt)
                        {
                            case "CMFD1UP":
                                {
                                    HSDDisplayFCR = !HSDDisplayFCR;
                                }
                                break;
                            case "CMFD2UP":
                                {
                                    HSDDisplayPRE = !HSDDisplayPRE;
                                }
                                break;
                            case "CMFD3UP":
                                {
                                    HSDDisplayAIFF = !HSDDisplayAIFF;
                                }
                                break;
                            case "CMFD5UP":
                                {
                                    mode = MFDMode.HSD;
                                }
                                break;
                            case "CMFD6UP":
                                {
                                    HSDDisplayLINE1 = !HSDDisplayLINE1;
                                }
                                break;
                            case "CMFD7UP":
                                {
                                    HSDDisplayLINE2 = !HSDDisplayLINE2;
                                }
                                break;
                            case "CMFD8UP":
                                {
                                    HSDDisplayLINE3 = !HSDDisplayLINE3;
                                }
                                break;
                            case "CMFD10UP":
                                {
                                    HSDDisplayRINGS = !HSDDisplayRINGS;
                                }
                                break;
                            case "CMFD12UP":
                                {
                                    mode = MFDMode.HSD;
                                }
                                break;
                            case "CMFD13UP":
                                {
                                    mode = MFDMode.SMS;
                                    dataPacket.sensorInput = F16MFDData.SensorInput.StoresManagementSystem;
                                }
                                break;
                            case "CMFD16UP":
                                {
                                    HSDDisplayADLINK = !HSDDisplayADLINK;
                                }
                                break;
                            case "CMFD17UP":
                                {
                                    HSDDisplayGDLINK = !HSDDisplayGDLINK;
                                }
                                break;
                            case "CMFD18UP":
                                {
                                    HSDDisplayNAV3 = !HSDDisplayNAV3;
                                }
                                break;
                            case "CMFD19UP":
                                {
                                    HSDDisplayNAV2 = !HSDDisplayNAV2;
                                }
                                break;
                            case "CMFD20UP":
                                {
                                    HSDDisplayNAV1 = !HSDDisplayNAV1;
                                }
                                break;
                            case "CMFD15UP":
                                {
                                    IsMaster = !IsMaster;
                                }
                                break;
                        }
                    }
                    break;
                #endregion

                #region HSD page
                case MFDMode.HSD:
                    {
                        switch (evt)
                        {
                            case "CMFD15UP":
                                {
                                    IsMaster = !IsMaster;
                                }
                                break;
                            case "CMFD1UP":
                                {
                                    HSDDepressed = !HSDDepressed;
                                }
                                break;
                            case "CMFD2UP":
                                {
                                    HSDCoupledToFCR = !HSDCoupledToFCR;
                                    
                                }
                                break;
                            case "CMFD5UP":
                                {
                                    mode = MFDMode.HSD_control;
                                }
                                break;
                            case "CMFD20UP":
                                {
                                    switch(dataPacket.HSDRange)
                                    {
                                        case 60:
                                            dataPacket.HSDRange = 120;
                                            break;
                                        case 30:
                                            dataPacket.HSDRange = 60;
                                            break;
                                        case 15:
                                            dataPacket.HSDRange = 30;
                                            break;

                                    }
                                }
                                break;

                            case "CMFD19UP":
                                {
                                    switch (dataPacket.HSDRange)
                                    {
                                        case 120:
                                            dataPacket.HSDRange = 60;
                                            break;
                                        case 60:
                                            dataPacket.HSDRange = 30;
                                            break;
                                        case 30:
                                            dataPacket.HSDRange = 15;
                                            break;

                                    }
                                }
                                break;

                            case "CMFD13UP":
                                reset_count++;
                                reset_timer = 0;
                                if (reset_count == 2)
                                    mode = MFDMode.Menu;
                                break;

                        }
                    }
                    break;
                #endregion

                #region Stores management
                case MFDMode.SMS:
                    {
                        switch(evt)
                        {
                            case "CMFD13UP":
                                reset_count++;
                                reset_timer = 0;
                                if (reset_count == 2)
                                    mode = MFDMode.Menu;
                                break;

                        }
                    }
                    break;
                #endregion

                #region Fire control radar
                case MFDMode.FCR:
                    {
                        switch (evt)
                        {
                            case "CMFD18UP":
                                {
                                    radar.HandleSystemInput("AzimuthUp");
                               }
                                break;
                            case "CMFD15UP":
                                {
                                    IsMaster = !IsMaster;
                                }
                                break;
                            case "CMFD20UP":
                                {
                                    radar.HandleSystemInput("RangeUp");
                                    
                                }
                                break;

                            case "CMFD19UP":
                                {
                                    radar.HandleSystemInput("RangeDown");
                                }
                                break;

                            case "CMFD17UP":
                                {
                                    radar.HandleSystemInput("BarsUp");
                                }
                                break;

                        }
                    }
                    break;
                #endregion

                #region Main menu
                case MFDMode.Menu:
                    {
                        switch (evt)
                        {
                            case "CMFD1UP":
                                mode = MFDMode.Blank;
                                break;
                            case "CMFD2UP":
                                mode = MFDMode.HUD;
                                dataPacket.sensorInput = F16MFDData.SensorInput.HUD;
                                break;
                            case "CMFD5UP":
                                mode = MFDMode.RESET;
                                break;
                            case "CMFD6UP":
                                mode = MFDMode.SMS;
                                dataPacket.sensorInput = F16MFDData.SensorInput.StoresManagementSystem;
                                break;
                            case "CMFD7UP":
                                mode = MFDMode.HSD;
                                dataPacket.sensorInput = F16MFDData.SensorInput.HorizontalSituationDisplay;
                                break;
                            case "CMFD20UP":
                                mode = MFDMode.FCR;
                                dataPacket.sensorInput = F16MFDData.SensorInput.FireControlRadar;
                                break;

                            default:
                                break;
                        }
                    }
                    break;
                #endregion


                case MFDMode.RESET:
                case MFDMode.Blank:
                    {
                        switch (evt)
                        {
                            case "CMFD12UP":
                            case "CMFD13UP":
                            case "CMFD14UP":
                                reset_count++;
                                reset_timer = 0;
                                if (reset_count == 2)
                                    mode = MFDMode.Menu;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }
            switch (evt)
            {
                case "BRIGHTNESSUP":
                    brightness += 0.01f;
                    break;

                case "BRIGHTNESSDOWN":
                    brightness -= 0.01f;
                    break;

                case "CONTRASTUP":
                    contrast += 0.01f;
                    break;

                case "CONTRASTDOWN":
                    contrast -= 0.01f;
                    break;

                case "SYMBOLUP":
                    symbolBrightness += 0.01f;
                    break;

                case "SYMBOLDOWN":
                    symbolBrightness -= 0.01f;
                    break;

                case "GAINUP":
                    gain += 0.01f;
                    break;

                case "GAINDOWN":
                    gain -= 0.01f;
                    break;
            }
        }

        public override void Update(float dt)
        {
            reset_timer += dt;
            if (reset_timer>1)
            {
                reset_timer = -1000;
                reset_count = 0;
            }

            ECSGameComponent pm = Parent.FindSingleComponentByType<PowerManagementComponent>();
            if (pm != null)
            {
                PowerManagementComponent pmc = (PowerManagementComponent)pm;
                HasPower = pmc.HasPower(5.0f/14.0f);
                if ((HasPower)&&(!HadPower))
                {
                    PowerUpTimer += dt;
                    if (PowerUpTimer > 1)
                    {
                        HadPower = true;
                    }
                } 
            }

            if (!HasPower)
                return;

            if (!HadPower)
                return;

            if (IsMaster)
            {
                if (InputDeviceManager.HasMappedInput("MFDMasterInput"))
                {
                    Vector2 ip = InputDeviceManager.GetPlayerAxes("MFDMasterInput");
                    thumb_stick = InputDeviceManager.GetPlayerBouncedButton("MFDMasterInput", "THUMBSTICK");
                    UpdatePickle(ip);

                }
                if (InputDeviceManager.HasMappedInput("MFDMasterSelectUp"))     
                {
                    switch (InputDeviceManager.GetPlayerInputType("MFDMasterSelectUp"))
                    {
                        case InputDescriptorType.HiHat:
                            {
                                HiHat hat = InputDeviceManager.GetPlayerHiHat("MFDMasterSelectUp");
                                if (hat.BouncedUp)
                                {
                                    SendEvent(CMFDMappableEvents.RangeUp);
                                }
                            }
                            break;
                    }
                }
                if (InputDeviceManager.HasMappedInput("MFDMasterSelectDown"))
                {
                    switch (InputDeviceManager.GetPlayerInputType("MFDMasterSelectDown"))
                    {
                        case InputDescriptorType.HiHat:
                            {
                                HiHat hat = InputDeviceManager.GetPlayerHiHat("MFDMasterSelectDown");
                                if (hat.BouncedDown)
                                {
                                    SendEvent(CMFDMappableEvents.RangeDown);
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                if (InputDeviceManager.HasMappedInput("MFDSlaveInput"))
                {
                    Vector2 ip = InputDeviceManager.GetPlayerAxes("MFDSlaveInput");
                    thumb_stick = InputDeviceManager.GetPlayerBouncedButton("MFDSlaveInput", "THUMBSTICK");
                    UpdatePickle(ip);

                }

                if (InputDeviceManager.HasMappedInput("MFDSlaveSelectUp"))
                {
                    switch (InputDeviceManager.GetPlayerInputType("MFDSlaveSelectUp"))
                    {
                        case InputDescriptorType.HiHat:
                            {
                                HiHat hat = InputDeviceManager.GetPlayerHiHat("MFDSlaveSelectUp");
                                if (hat.BouncedUp)
                                {
                                    SendEvent(CMFDMappableEvents.RangeUp);
                                }
                            }
                            break;
                    }
                }
                if (InputDeviceManager.HasMappedInput("MFDSlaveSelectDown"))
                {
                    switch (InputDeviceManager.GetPlayerInputType("MFDSlaveSelectDown"))
                    {
                        case InputDescriptorType.HiHat:
                            {
                                HiHat hat = InputDeviceManager.GetPlayerHiHat("MFDSlaveSelectDown");
                                if (hat.BouncedDown)
                                {
                                    SendEvent(CMFDMappableEvents.RangeDown);
                                }
                            }
                            break;
                    }
                }
            }


        }

        public override void RenderOffscreenRenderTargets()
        {
            Device.SetRenderTarget(renderTarget);
            Device.Clear(Color.Black);

            mfd.Parameters["brightness"].SetValue(brightness);
            mfd.Parameters["contrast"].SetValue(contrast);
            mfd.Parameters["invert"].SetValue(false);

            if ((HasPower)&&(HadPower))
            {
                switch (dataPacket.sensorInput)
                {
                    case F16MFDData.SensorInput.HUD:
                        {
                            F16HudComponent hud = (F16HudComponent)Parent.FindGameComponentByName("F16Hud_1");
                            Texture2D hud_tex = hud.GetOffscreenTexture();
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            batch.Draw(hud_tex, display_region, Color.White);
                            batch.End();
                        }
                        break;
                    case F16MFDData.SensorInput.StoresManagementSystem:
                        {
                        }
                        break;

                    #region FCR display
                    case F16MFDData.SensorInput.FireControlRadar:
                        {
                            #region Draw the flight wings
                            Vector2 delta = new Vector2(0, 8);
                            Vector2 top = new Vector2(256 - 24, 400);

                            Quaternion orientation = transform.GetOrientation();
                            Vector3 angles = MathsHelper.QuaternionToYawPitchRoll(orientation);
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            LineRenderer.DrawArtificialHorizon(batch, MathHelper.ToDegrees(angles.X), MathHelper.ToDegrees(angles.Y), MathHelper.ToDegrees(angles.Z), 100, 20, 10, 108.5f, 512, 512, Color.Green, 2);

                            #endregion

                            float x = 256 + ((radar.GetEmitterAngle() + radar.GetOffset()) * 220.0f) / 120.0f;
                            top = new Vector2(x, 406);
                            LineRenderer.DrawLine(batch, top, top + delta, Color.Cyan, 2);
                            top.X -= 4;
                            top.Y += 8;
                            delta.X = 8;
                            delta.Y = 0;
                            LineRenderer.DrawLine(batch, top, top + delta, Color.Cyan, 2);

                            #region Draw pitch bars
                            LineRenderer.DrawLine(batch, new Vector2(400, 156), new Vector2(416, 156), Color.Cyan, 2);
                            LineRenderer.DrawLine(batch, new Vector2(400, 256), new Vector2(416, 256), Color.Cyan, 2);
                            LineRenderer.DrawLine(batch, new Vector2(400, 356), new Vector2(416, 356), Color.Cyan, 2);
                            #endregion

                            #region Draw azimuth limits
                            if (radar.GetSubMode() == "SAM")
                            {
                                x = 256 + ((radar.GetAzimuth() + radar.GetOffset()) * 220.0f) / 120.0f;
                                top.X = x;
                                top.Y = 100;
                                delta.X = 0;
                                delta.Y = 298;
                                LineRenderer.DrawLine(batch, top, top + delta, Color.Cyan, 2);
                                x = 256 - ((radar.GetAzimuth() + radar.GetOffset()) * 220.0f) / 120.0f;
                                top.X = x;
                                LineRenderer.DrawLine(batch, top, top + delta, Color.Cyan, 2);
                            }
                            #endregion

                            #region Draw the azimuth and bars settings
                            delta.X = 8;
                            delta.Y = 0;
                            top.X = 100;
                            for (int y = 112; y < 444; y += 48)
                            {
                                top.Y = y;
                                LineRenderer.DrawLine(batch, top, top + delta, Color.Cyan, 2);
                            }
                            top.X = 92;
                            top.Y = 256 + (radar.GetTilt() * 288.0f) / 120.0f;
                            LineRenderer.DrawLine(batch, top, top + delta, Color.GreenYellow, 2);

                            batch.DrawString(font, "A", new Vector2(70, 240), Color.Green);
                            int az = (int)(radar.GetAzimuth() / 10);
                            batch.DrawString(font, String.Format("{0}", az), new Vector2(70, 260), Color.Green);

                            batch.DrawString(font, "B", new Vector2(70, 295), Color.Green);
                            batch.DrawString(font, String.Format("{0}", radar.GetBars()), new Vector2(70, 315), Color.Green);
                            #endregion

                            #region Draw any targets
                            Dictionary<int, AirbourneRadarTarget> targets = radar.GetTrackedTargets();
                            foreach (int i in targets.Keys)
                            {
                                AirbourneRadarTarget art = targets[i];
                                int j = 3;
                                int display_size = 440;
                                while (j >= 0)
                                {
                                    if (art.LocalPosition[j] != null)
                                    {
                                        Vector3 relative_position = art.LocalPosition[j] - transform.GetLocalPosition();
                                        float range = relative_position.Length() * 0.000621371f;   // range in miles
                                        range = display_size * (range / radar.GetRange());                  // in pixels
                                        float angle = MathHelper.ToDegrees(art.AngleOffTheNose);
                                        angle /= radar.GetAzimuth();
                                        angle *= 256;
                                        angle += 256;

                                        Rectangle r = new Rectangle((int)(angle - 4), display_size - (int)range - 4, 8, 8);
                                        int a = 255 - (j * 64);
                                        if (i == radar.GetBuggedTarget())
                                        {
                                            // get direction of flight
                                            Matrix m = Matrix.CreateFromQuaternion(art.Orientation);
                                            Vector3 direction = m.Forward;
                                            direction.Y = 0;
                                            direction.Normalize();

                                            // get relative direction
                                            m = Matrix.CreateFromQuaternion(transform.GetOrientation());
                                            direction = Vector3.Transform(direction, m);

                                            float relative_heading = (float)Math.Acos(Vector3.Dot(m.Forward, direction));

                                            LineRenderer.DrawTarget(batch, r, Color.FromNonPremultiplied(255, 255, 0, a), relative_heading, art.Velocity);

                                            float rhd = MathHelper.ToDegrees(art.AngleOffTheNose);
                                            if (rhd < 0)
                                            {
                                                rhd = -rhd;
                                                int rd = (int)(rhd);
                                                batch.DrawString(font, String.Format("{0:00}L", rd), new Vector2(70, 80), Color.Green);
                                            }
                                            else
                                            {
                                                int rd = (int)(rhd);
                                                batch.DrawString(font, String.Format("{0:00}R", rd), new Vector2(70, 80), Color.Green);
                                            }
                                            float th = MathsHelper.GetHeading(art.Orientation);
                                            th = MathHelper.ToDegrees(th);
                                            if (th < 0)
                                            {
                                                th = 360 - th;
                                            }
                                            int target_heading = (int)th;
                                            batch.DrawString(font, String.Format("{0:000}", target_heading), new Vector2(120, 80), Color.Green);

                                            int speed = (int)art.Velocity;
                                            batch.DrawString(font, speed.ToString(), new Vector2(332, 80), Color.Green);
                                        }
                                        else
                                        {
                                            batch.Draw(AssetManager.GetWhite(), r, Color.FromNonPremultiplied(255, 255, 0, a));
                                        }

                                        if (thumb_stick)
                                        {
                                            if (r.Contains(pickle))
                                            {
                                                radar.BugTarget(i);
                                                thumb_stick = false;
                                            }
                                        }
                                    }
                                    j--;
                                }

                            }
                            #endregion

                            batch.End();
                        }
                        break;
                    #endregion

                    #region HSD display
                    case F16MFDData.SensorInput.HorizontalSituationDisplay:
                        {
                            int radar_range = dataPacket.HSDRange;
                            if (HSDCoupledToFCR)
                                radar_range = radar.GetRange();

                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);

                            #region Bullseye handling
                            CircleRender.DrawCircle(14, new Vector2(90, 410), 24, 1, Color.Blue, batch);

                            String br = String.Format("{0}", dataPacket.BullseyeRange);
                            Vector2 pos = smallfont.MeasureString(br);
                            pos *= -0.5f;
                            pos += new Vector2(90, 410);
                            batch.DrawString(smallfont, br, pos, Color.Blue);

                            br = String.Format("{0:000}", dataPacket.BullseyeBearing);
                            pos = smallfont.MeasureString(br);
                            pos *= -0.5f;
                            pos += new Vector2(90, 432);
                            batch.DrawString(smallfont, br, pos, Color.Blue);

                            Vector2 cntr = new Vector2(90, 410);
                            Vector2 point1 = Rotate(cntr, new Vector2(0, 20), dataPacket.BullseyeBearing);
                            Vector2 point2 = Rotate(cntr, new Vector2(4, 14), dataPacket.BullseyeBearing);
                            Vector2 point3 = Rotate(cntr, new Vector2(-4, 14), dataPacket.BullseyeBearing);

                            LineRenderer.DrawLine(batch, point1, point2, Color.Blue, 2);
                            LineRenderer.DrawLine(batch, point1, point3, Color.Blue, 2);

                            float rangeinpixels = (float)dataPacket.BullseyeRange / (float)radar_range;
                            rangeinpixels *= 256;
                            Vector2 bullseye = Rotate(Vector2.Zero, new Vector2(0, rangeinpixels), dataPacket.BullseyeBearing);
                            #endregion

                            Vector2 centre;
                            if (HSDDepressed)
                            {
                                centre = new Vector2(256, 256 + 84);
                                if (HSDDisplayRINGS)
                                {
                                    float range3 = 768.0f / 3.0f;
                                    CircleRender.DrawCircle(range3, centre, 64, 1, Color.White, batch);
                                }
                            }
                            else
                            {
                                centre = new Vector2(256, 256);
                            }
                            if (HSDDisplayRINGS)
                            {
                                float range1 = 256.0f / 3.0f;
                                CircleRender.DrawCircle(range1, centre, 64, 1, Color.White, batch);
                                float range2 = 512.0f / 3.0f;
                                CircleRender.DrawCircle(range2, centre, 64, 1, Color.White, batch);
                            }

                            #region Navpoint render
                            if (HSDDisplayNAV1)
                            {
                                Vector2 start = Vector2.Zero;

                                for (int i = 0; i < dataPacket.Navpoints.Count; i++)
                                {
                                    float dr = dataPacket.Navpoints[i].Range / (float)radar_range;
                                    dr *= 256;
                                    Vector2 pos1 = Rotate(centre, new Vector2(0, dr), dataPacket.Navpoints[i].Bearing);
                                    CircleRender.DrawCircle(12, pos1, 16, 2, Color.White, batch);
                                    if (i > 0)
                                    {
                                        Vector2 dp = start - pos1;
                                        float length = dp.Length();
                                        float dl = 12.0f / length;
                                        Vector2 ls = new Vector2(MathHelper.Lerp(start.X, pos1.X, dl), (MathHelper.Lerp(start.Y, pos1.Y, dl)));
                                        Vector2 le = new Vector2(MathHelper.Lerp(start.X, pos1.X, 1.0f - dl), (MathHelper.Lerp(start.Y, pos1.Y, 1.0f - dl)));

                                        LineRenderer.DrawLine(batch, ls, le, Color.White, 2);
                                    }
                                    start = pos1;
                                }
                            }
                            #endregion

                            #region Icon render

                            #region Own plane
                            LineRenderer.DrawLine(batch, centre, centre + new Vector2(0, 24), Color.Blue, 1);
                            LineRenderer.DrawLine(batch, centre + new Vector2(-10, 6), centre + new Vector2(10, 6), Color.Blue, 1);
                            LineRenderer.DrawLine(batch, centre + new Vector2(-4, 21), centre + new Vector2(4, 21), Color.Blue, 1);
                            #endregion

                            #region Bullseye
                            CircleRender.DrawCircle(2, centre + bullseye, 4, 1, Color.Blue, batch);
                            CircleRender.DrawCircle(5, centre + bullseye, 8, 1, Color.Blue, batch);
                            CircleRender.DrawCircle(8, centre + bullseye, 16, 1, Color.Blue, batch);

                            #endregion

                            #endregion

                            batch.End();
                        }
                        break;
                        #endregion

                }


                switch (mode)
                {
                    #region Blank
                    case MFDMode.Blank:
                        {
                            Vector2 pos = font.MeasureString("BLANK");
                            pos *= -0.5f;
                            pos += new Vector2(256, 256);

                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            batch.DrawString(font, "BLANK", pos, Color.Green);

                            batch.End();
                        }
                        break;
                    #endregion

                    #region HUD mode
                    case MFDMode.HUD:
                        {
                            mfd.Parameters["invert"].SetValue(true);
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            DrawOSBInverted(2, "HUD", batch);
                            batch.End();
                        }
                        break;
                    #endregion

                    #region HSD control page
                    case MFDMode.HSD_control:
                        {
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            if (!HSDDisplayFCR) DrawOSB(1, "FCR", batch);
                            if (!HSDDisplayPRE) DrawOSB(2, "PRE", batch);
                            if (!HSDDisplayAIFF) DrawOSB(3, "AIFF", batch);
                            if (!HSDDisplayLINE1) DrawOSB(6, "LINE 1", batch);
                            if (!HSDDisplayLINE2) DrawOSB(7, "LINE 2", batch);
                            if (!HSDDisplayLINE3) DrawOSB(8, "LINE 3", batch);
                            if (!HSDDisplayRINGS) DrawOSB(10, "RINGS", batch);
                            if (!HSDDisplayADLINK) DrawOSB(16, "ADLNK", batch);
                            if (!HSDDisplayGDLINK) DrawOSB(17, "GDLNK", batch);
                            if (!HSDDisplayNAV3) DrawOSB(18, "NAV 3", batch);
                            if (!HSDDisplayNAV2) DrawOSB(19, "NAV 2", batch);
                            if (!HSDDisplayNAV1) DrawOSB(20, "NAV 1", batch);


                            DrawOSB(11, "DCLT", batch);
                            DrawOSB(13, "SMS", batch);
                            DrawOSB(14, "ELES", batch);
                            DrawOSB(15, "SWAP", batch);
                            batch.End();

                            mfd.Parameters["invert"].SetValue(true);
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            DrawOSBInverted(12, "HSD", batch);
                            DrawOSBInverted(5, "CNTL", batch);

                            if (HSDDisplayFCR) DrawOSBInverted(1, "FCR", batch);
                            if (HSDDisplayPRE) DrawOSBInverted(2, "PRE", batch);
                            if (HSDDisplayAIFF) DrawOSBInverted(3, "AIFF", batch);
                            if (HSDDisplayLINE1) DrawOSBInverted(6, "LINE 1", batch);
                            if (HSDDisplayLINE2) DrawOSBInverted(7, "LINE 2", batch);
                            if (HSDDisplayLINE3) DrawOSBInverted(8, "LINE 3", batch);
                            if (HSDDisplayRINGS) DrawOSBInverted(10, "RINGS", batch);
                            if (HSDDisplayADLINK) DrawOSBInverted(16, "ADLNK", batch);
                            if (HSDDisplayGDLINK) DrawOSBInverted(17, "GDLNK", batch);
                            if (HSDDisplayNAV3) DrawOSBInverted(18, "NAV 3", batch);
                            if (HSDDisplayNAV2) DrawOSBInverted(19, "NAV 2", batch);
                            if (HSDDisplayNAV1) DrawOSBInverted(20, "NAV 1", batch);

                            batch.End();
                        }
                        break;
                    #endregion

                    #region Fire control radar
                    case MFDMode.FCR:
                        {
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);

                            DrawOSB(1, radar.GetMode(), batch);
                            DrawOSB(2, radar.GetSubMode(), batch);


                            DrawOSB(4, "OVRD", batch);
                            DrawOSB(5, "CTRL", batch);
                            DrawOSB(11, "DCLT", batch);
                            DrawOSB(14, "TEST", batch);
                            DrawOSB(15, "SWAP", batch);

                            if (radar.GetRange() < 120)
                            {
                                LineRenderer.DrawLine(batch, new Vector2(80, 134), new Vector2(64, 150), Color.Green, 2);
                                LineRenderer.DrawLine(batch, new Vector2(80, 134), new Vector2(96, 150), Color.Green, 2);
                                LineRenderer.DrawLine(batch, new Vector2(64, 150), new Vector2(96, 150), Color.Green, 2);
                            }

                            String range = String.Format("{0}", radar.GetRange());
                            batch.DrawString(font, range, new Vector2(72, 163), Color.Green);

                            if (radar.GetRange() > 15)
                            {
                                LineRenderer.DrawLine(batch, new Vector2(80, 199 + 5), new Vector2(64, 188), Color.Green, 2);
                                LineRenderer.DrawLine(batch, new Vector2(80, 199 + 5), new Vector2(96, 188), Color.Green, 2);
                                LineRenderer.DrawLine(batch, new Vector2(64, 183 + 5), new Vector2(96, 188), Color.Green, 2);
                            }

                            batch.End();

                            mfd.Parameters["invert"].SetValue(true);
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            DrawOSBInverted(12, "FCR", batch);
                            batch.End();
                        }
                        break;
                    #endregion

                    #region Stores management system
                    case MFDMode.SMS:
                        {
                            DrawSMS(batch);

                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);

                            DrawOSB(1, "STBY", batch);
                            DrawOSB(5, "CLR", batch);
                            DrawOSB(11, "S-J", batch);
                            DrawOSB(12, "DTE", batch);
                            DrawOSB(15, "SWAP", batch);

                            batch.End();

                            mfd.Parameters["invert"].SetValue(true);
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            DrawOSBInverted(14, "SMS", batch);
                            batch.End();

                        }
                        break;
                    #endregion

                    #region Main menu
                    case MFDMode.Menu:
                        {
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);

                            DrawOSB(1, "BLANK", batch);
                            DrawOSB(2, "HUD", batch);
                            DrawOSB(3, "RWR", batch);
                            DrawOSB(4, "RCCE", batch);
                            DrawOSB(5, "RESET\nMENU", batch);


                            DrawOSB(8, "DTE", batch);
                            DrawOSB(9, "TEST", batch);
                            DrawOSB(10, "FLCS", batch);
                            DrawOSB(11, "DCLT", batch);

                            DrawOSB(15, "SWAP", batch);
                            DrawOSB(16, "FLIR", batch);
                            DrawOSB(17, "TFR", batch);
                            DrawOSB(18, "WPN", batch);
                            DrawOSB(19, "TGP", batch);

                            switch (dataPacket.sensorInput)
                            {
                                case F16MFDData.SensorInput.FireControlRadar:
                                    {
                                        DrawOSB(12, "DTE", batch);
                                        DrawOSB(6, "SMS", batch);
                                        DrawOSB(7, "HSD", batch);
                                    }
                                    break;
                                case F16MFDData.SensorInput.DataTransferEquipment:
                                    {
                                        DrawOSB(14, "FCR", batch);
                                        DrawOSB(20, "FCR", batch);
                                        DrawOSB(6, "SMS", batch);
                                        DrawOSB(7, "HSD", batch);
                                    }
                                    break;
                                case F16MFDData.SensorInput.StoresManagementSystem:
                                    {
                                        DrawOSB(14, "FCR", batch);
                                        DrawOSB(12, "DTE", batch);
                                        DrawOSB(7, "HSD", batch);
                                    }
                                    break;
                                case F16MFDData.SensorInput.HorizontalSituationDisplay:
                                    {
                                        DrawOSB(14, "FCR", batch);
                                        DrawOSB(20, "FCR", batch);
                                        DrawOSB(6, "SMS", batch);
                                        DrawOSB(12, "DTE", batch);
                                    }
                                    break;

                            }

                            batch.End();

                            mfd.Parameters["invert"].SetValue(true);
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            switch (dataPacket.sensorInput)
                            {
                                case F16MFDData.SensorInput.FireControlRadar:
                                    {
                                        DrawOSBInverted(14, "FCR", batch);
                                        DrawOSBInverted(20, "FCR", batch);
                                    }
                                    break;
                                case F16MFDData.SensorInput.DataTransferEquipment:
                                    {
                                        DrawOSBInverted(12, "DTE", batch);
                                    }
                                    break;
                                case F16MFDData.SensorInput.StoresManagementSystem:
                                    {
                                        DrawOSBInverted(6, "SMS", batch);
                                    }
                                    break;
                                case F16MFDData.SensorInput.HorizontalSituationDisplay:
                                    {
                                        DrawOSBInverted(7, "HSD", batch);
                                    }
                                    break;
                            }

                            batch.End();

                        }
                        break;
                    #endregion

                    #region Reset menu
                    case MFDMode.RESET:
                        {
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);

                            DrawOSB(1, "BLANK", batch);
                            DrawOSB(6, "SBC DAY\nRESET", batch);
                            DrawOSB(7, "SBC NIGHT\nRESET", batch);
                            DrawOSB(8, "SBC DFLT\nRESET", batch);
                            DrawOSB(9, "SBC DAY\nSET", batch);
                            DrawOSB(10, "SBC NIGHT\nSET", batch);
                            DrawOSB(11, "DCLT", batch);
                            DrawOSB(12, "DTE", batch);
                            DrawOSB(15, "SWAP", batch);
                            DrawOSB(18, "NVIS\nOVRD", batch);
                            DrawOSB(19, "PROG DCLT\nRESET", batch);
                            DrawOSB(20, "MSMD\nRESET", batch);

                            switch (dataPacket.sensorInput)
                            {
                                case F16MFDData.SensorInput.FireControlRadar:
                                    break;

                                default:
                                    DrawOSB(14, "FCR", batch);
                                    break;
                            }

                            batch.End();

                            mfd.Parameters["invert"].SetValue(true);
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);

                            DrawOSBInverted(5, "RESET\nMENU", batch);

                            switch (dataPacket.sensorInput)
                            {
                                case F16MFDData.SensorInput.FireControlRadar:
                                    DrawOSBInverted(14, "FCR", batch);
                                    break;

                                default:

                                    break;
                            }
                            batch.End();
                        }
                        break;
                    #endregion

                    #region Horizontal situation display
                    case MFDMode.HSD:
                        {
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            DrawOSB(1, "DEP", batch);

                            DrawOSB(3, "NORM", batch);
                            if (dataPacket.Messages.Count == 0)
                                DrawOSB(4, "MSG", batch);
                            DrawOSB(5, "CNTL", batch);
                            DrawOSB(7, "FRZ", batch);
                            DrawOSB(11, "S-J", batch);
                            DrawOSB(12, "SMS", batch);
                            DrawOSB(15, "SWAP", batch);

                            if (!HSDCoupledToFCR)
                            {
                                if (dataPacket.HSDRange < 120)
                                {
                                    LineRenderer.DrawLine(batch, new Vector2(80, 134), new Vector2(64, 150), Color.Green, 2);
                                    LineRenderer.DrawLine(batch, new Vector2(80, 134), new Vector2(96, 150), Color.Green, 2);
                                    LineRenderer.DrawLine(batch, new Vector2(64, 150), new Vector2(96, 150), Color.Green, 2);
                                }

                                String range = String.Format("{0}", dataPacket.HSDRange);
                                batch.DrawString(font, range, new Vector2(72, 159), Color.Green);

                                if (dataPacket.HSDRange > 15)
                                {
                                    LineRenderer.DrawLine(batch, new Vector2(80, 199), new Vector2(64, 183), Color.Green, 2);
                                    LineRenderer.DrawLine(batch, new Vector2(80, 199), new Vector2(96, 183), Color.Green, 2);
                                    LineRenderer.DrawLine(batch, new Vector2(64, 183), new Vector2(96, 183), Color.Green, 2);
                                }

                            }
                            else
                            {
                                DrawOSB(2, "DCPL", batch);
                            }
                            batch.End();

                            mfd.Parameters["invert"].SetValue(true);
                            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
                            DrawOSBInverted(14, "HSD", batch);
                            if (dataPacket.Messages.Count > 0)
                                DrawOSBInverted(4, "MSG", batch);
                            if (!HSDCoupledToFCR)
                                DrawOSBInverted(2, "DCPL", batch);
                            batch.End();
                        }
                        break;
                    #endregion


                    default:
                        break;
                }

                #region Pickle display
                mfd.Parameters["invert"].SetValue(false);
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);

                Vector2 ptop = pickle - new Vector2(8, 12);
                Vector2 pbottom = pickle - new Vector2(8, -12);
                LineRenderer.DrawLine(batch, ptop, pbottom, Color.White, 2);

                ptop = pickle + new Vector2(8, 12);
                pbottom = pickle + new Vector2(8, -12);
                LineRenderer.DrawLine(batch, ptop, pbottom, Color.White, 2);

                batch.End();
                #endregion

            }else
            {
                batch.Begin();
                int a = (int) (256 * Math.Sin(PowerUpTimer * 2 * MathHelper.Pi));
                float w = 256 * PowerUpTimer * 2;
                if (w > 256)
                    w = 256;
                batch.Draw(AssetManager.GetWhite(), new Rectangle(256 - (int)w, 256 - (int)w, (int)w * 2, (int)w * 2), Color.FromNonPremultiplied(0, 255, 0, a));
                batch.End();
            }
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            batch.Draw(overlay, new Rectangle(0, 0, 512, 512), Color.White);

            // Handle the standard buttons
            for (int i = 0; i < 20; i++)
            {
                if (buttonManager.buttons[i].state != 0)
                {
                    batch.Draw(buttons, buttonManager.buttons[i].Location, buttonImageSrc, Color.White);
                }
            }

            #region Brightness rocker
            if (buttonManager.buttons[20].state != 0)
            {
                batch.Draw(buttons, new Rectangle(10, 399, 37, 57), brightnessUpSrc, Color.White);
            }
            if (buttonManager.buttons[21].state != 0)
            {
                batch.Draw(buttons, new Rectangle(10, 399, 37, 57), brightnessDownSrc, Color.White);
            }
            #endregion

            #region Contrast rocker
            if (buttonManager.buttons[22].state != 0)
            {
                batch.Draw(buttons, new Rectangle(466, 400, 36, 57), contrastUpSrc, Color.White);
            }
            if (buttonManager.buttons[23].state != 0)
            {
                batch.Draw(buttons, new Rectangle(466, 400, 36, 57), contrastDownSrc, Color.White);
            }
            #endregion

            #region Symbol brightness rocker
            if (buttonManager.buttons[24].state != 0)
            {
                batch.Draw(buttons, new Rectangle(466, 55, 36, 57), symbolUpSrc, Color.White);
            }
            if (buttonManager.buttons[25].state != 0)
            {
                batch.Draw(buttons, new Rectangle(466, 55, 36, 57), symbolDownSrc, Color.White);
            }
            #endregion

            #region Gain rocker
            if (buttonManager.buttons[26].state != 0)
            {
                batch.Draw(buttons, new Rectangle(10, 55, 36, 57), gainUpSrc, Color.White);
            }
            if (buttonManager.buttons[27].state != 0)
            {
                batch.Draw(buttons, new Rectangle(10, 55, 36, 57), gainDownSrc, Color.White);
            }
            #endregion

            batch.End();

            

            Device.SetRenderTarget(null);
        }

        public override void Connect(String comps, bool isList)
        {
            if (Name.EndsWith("1"))
                IsMaster = true;
            else
                IsMaster = false;

            string[] parts = comps.Split('#');
            if (isList)
            {
                // no lists
            }else
            {
                switch (parts[1])
                {
                    case "Root":
                        {
                            string[] objects = parts[2].Split(':');
                            parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    case "WorldTransform":
                        {
                            string[] objects = parts[2].Split(':');
                            transform = (WorldTransform)parent.FindGameComponentByName(objects[0]);
                        }
                        break;

                    case "Radar1":
                        {
                            string[] objects = parts[2].Split(':');
                            radar = (AirbourneRadarInterface)parent.FindGameComponentByName(objects[0]);
                            radarComponent = parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::OnAPG68v5:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            radar = null;
            transform = null;
        }

        public override ECSGameComponent Clone()
        {
            return new ColourMFD();
        }

        public override void ReConnect(GameObject other)
        {
            ColourMFD othermfd = (ColourMFD)other.FindGameComponentByName(Name);
            othermfd.radar = (AirbourneRadarInterface)other.FindGameComponentByName(radarComponent.Name);
            othermfd.transform = (WorldTransform)other.FindGameComponentByName(transform.Name);
            othermfd.IsMaster = IsMaster;
        }

        #region Display helpers
        private String GetWeaponString(int slot, out int count)
        {
            String wpn = "----------";
            count = 0;
            AircraftState player = WorldState.GetPlayerAircraftState();

            if (player.Stations.Count >= slot)
            {
                if (player.Stations[slot] != null)
                {
                    String wpnid = player.Stations[slot].ID;
                    if (wpn_map.ContainsKey(wpnid))
                    {
                        wpn = String.Format("{0}  {1}", player.Stations[slot].Rounds, wpn_map[wpnid]);
                        count = player.Stations[slot].Rounds;
                    }
                    else
                    {
                        if (wpnid != "NONE")
                            wpn = wpnid;
                    }
                }
            }
            return wpn;
        }

        private void Draw3Stations(String s, int count, int x, int y)
        {
            Vector2 pos;
            switch (count)
            {
                case 1:
                    {
                        pos = font.MeasureString(s);
                        pos *= -0.5f;
                        Vector2 pos2 = new Vector2(x, y);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);

                        pos = font.MeasureString("----------");
                        pos *= -0.5f;
                        pos2 = new Vector2(x, y + 20);
                        batch.DrawString(font, "----------", pos2 + pos, Color.Green);
                        pos2 = new Vector2(x, y + 40);
                        batch.DrawString(font, "----------", pos2 + pos, Color.Green);

                    }
                    break;

                case 2:
                    {
                        pos = font.MeasureString(s);
                        pos *= -0.5f;
                        Vector2 pos2 = new Vector2(x, y);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);

                        pos2 = new Vector2(x, y + 20);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);

                        pos = font.MeasureString("----------");
                        pos *= -0.5f;
                        pos2 = new Vector2(x, y + 40);
                        batch.DrawString(font, "----------", pos2 + pos, Color.Green);

                    }
                    break;

                case 3:
                    {
                        pos = font.MeasureString(s);
                        pos *= -0.5f;
                        Vector2 pos2 = new Vector2(x, y);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);

                        pos2 = new Vector2(x, y + 20);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);

                        pos2 = new Vector2(x, y + 40);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);

                    }
                    break;

                default:
                    {
                        pos = font.MeasureString(s);
                        pos *= -0.5f;
                        Vector2 pos2 = new Vector2(x, y);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);
                        pos2 = new Vector2(x, y + 20);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);
                        pos2 = new Vector2(x, y + 40);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);
                    }
                    break;

            }
        }

        private void Draw2Stations(String s, int count, int x, int y)
        {
            Vector2 pos;
            switch (count)
            {
                case 1:
                    {
                        pos = font.MeasureString(s);
                        pos *= -0.5f;
                        Vector2 pos2 = new Vector2(x, y);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);

                        pos = font.MeasureString("----------");
                        pos *= -0.5f;
                        pos2 = new Vector2(x, y + 20);
                        batch.DrawString(font, "----------", pos2 + pos, Color.Green);

                    }
                    break;

                case 2:
                    {
                        pos = font.MeasureString(s);
                        pos *= -0.5f;
                        Vector2 pos2 = new Vector2(x, y);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);

                        pos2 = new Vector2(x, y + 20);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);

                    }
                    break;

                default:
                    {
                        pos = font.MeasureString(s);
                        pos *= -0.5f;
                        Vector2 pos2 = new Vector2(x, y);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);
                        pos2 = new Vector2(x, y + 20);
                        batch.DrawString(font, s, pos2 + pos, Color.Green);
                    }
                    break;

            }
        }

        private void DrawOSBInverted(int station, String text, SpriteBatch batch)
        {
            if (text.Contains("\n"))
            {
                string[] parts = text.Split('\n');
                Vector2 size1 = font.MeasureString(parts[0]);
                Vector2 size2 = font.MeasureString(parts[1]);
                Rectangle bounds = new Rectangle();
                bounds.Width = 4 + (int)Math.Max(size1.X, size2.X);
                bounds.Height = (int)(size1.Y + size2.Y + 2.5f);

                switch (station)
                {
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        {
                            bounds.X = (int) OSB_locations[station - 1].X;
                            if (station<16)
                            {
                                bounds.X -= bounds.Width;
                            }
                            bounds.Y = (int)(0.5f + OSB_locations[station - 1].Y - (bounds.Height * 0.5f));
                            batch.Draw(AssetManager.GetBlack(), bounds, Color.Green);

                            Vector2 offset = new Vector2();
                            offset.X = 0;
                            if (size2.X > size1.X)
                            {
                                offset.Y = size1.Y * -0.5f;
                                DrawOSBSingleInverted(station, parts[0], batch, offset, false);
                                offset.Y *= -1.0f;
                                DrawOSBSingleInverted(station, parts[1], batch, offset, false);

                            }
                            else
                            {
                                // todo handle bounding box 
                                offset.Y = size1.Y * -0.5f;
                                DrawOSBSingleInverted(station, parts[0], batch, offset, false);
                                offset.Y *= -1.0f;
                                DrawOSBSingleInverted(station, parts[1], batch, offset, false);
                            }

                        }
                        break;
                    default:
                        {
                            bounds.Y = (int)OSB_locations[station - 1].Y - 9;
                            if (station > 5)
                            {
                                bounds.Y -= (bounds.Height + 9);
                            }
                            bounds.X = (int)(0.5f + OSB_locations[station - 1].X - (bounds.Width * 0.5f));
                            batch.Draw(AssetManager.GetWhite(), bounds, Color.Green);

                            DrawOSBSingleInverted(station, parts[0], batch, Vector2.Zero, false);
                            DrawOSBSingleInverted(station, parts[1], batch, new Vector2(0, 20), false);
                        }
                        break;
                }

            }
            else
            {
                DrawOSBSingleInverted(station, text, batch, Vector2.Zero, true);
            }
        }

        private void DrawOSB(int station, String text, SpriteBatch batch)
        {
            if (text.Contains("\n"))
            {
                string[] parts = text.Split('\n');
                Vector2 size1 = font.MeasureString(parts[0]);
                Vector2 size2 = font.MeasureString(parts[1]);
                Rectangle bounds = new Rectangle();
                bounds.Width = 4 + (int)Math.Max(size1.X, size2.X);
                bounds.Height = (int)(size1.Y + size2.Y + 2.5f);

                switch (station)
                {
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        {
                            bounds.X = (int)OSB_locations[station - 1].X;
                            if (station < 16)
                            {
                                bounds.X -= bounds.Width;
                            }
                            bounds.Y = (int)(0.5f + OSB_locations[station - 1].Y - (bounds.Height * 0.5f));
                            batch.Draw(AssetManager.GetBlack(), bounds, Color.Black);

                            Vector2 offset = new Vector2();
                            offset.X = 0;
                            if (size2.X > size1.X)
                            {
                                offset.Y = size1.Y * -0.5f;
                                DrawOSBSingle(station, parts[0], batch, offset, false);
                                offset.Y *= -1.0f;
                                DrawOSBSingle(station, parts[1], batch, offset, false);

                            }else
                            {
                                offset.Y = size1.Y * -0.5f;
                                DrawOSBSingle(station, parts[0], batch, offset, false);
                                offset.Y *= -1.0f;
                                DrawOSBSingle(station, parts[1], batch, offset, false);
                            }
                            
                        }
                        break;
                    default:
                        {
                            bounds.Y = (int)OSB_locations[station - 1].Y - 9;
                            if (station > 5)
                            {
                                bounds.Y -= (bounds.Height + 9);
                            }
                            bounds.X = (int)(0.5f + OSB_locations[station - 1].X - (bounds.Width * 0.5f));
                            batch.Draw(AssetManager.GetBlack(), bounds, Color.Black);

                            DrawOSBSingle(station, parts[0], batch, Vector2.Zero, false);
                            DrawOSBSingle(station, parts[1], batch, new Vector2(0, 20), false);
                        }
                        break;
                }
               
            }
            else
            {
                DrawOSBSingle(station, text, batch, Vector2.Zero, true);
            }
        }

        private void DrawOSBSingleInverted(int station, String text, SpriteBatch batch, Vector2 offset, bool drawBox)
        {
            Vector2 pos = font.MeasureString(text);
            Rectangle bounds = new Rectangle();
            bounds.Width = (int)(pos.X + 4.5f);
            bounds.Height = (int)(pos.Y + 4.5f);

            switch (station - 1)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    {                       
                        pos *= -0.5f;
                        pos += OSB_locations[station - 1];
                        bounds.X = (int)(pos.X - 2.5f);
                        bounds.Y = (int)(pos.Y - 2.5f);
                        if (drawBox)
                            batch.Draw(AssetManager.GetBlack(), bounds, Color.Green);
                        
                        batch.DrawString(font, text, pos + offset, Color.Green);
                    }
                    break;

                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    {
                        pos.Y *= -0.5f;
                        pos.X *= -1.0f;
                        pos += OSB_locations[station - 1];
                        bounds.X = (int)(pos.X - 2.5f);
                        bounds.Y = (int)(pos.Y - 2.5f);
                        if (drawBox)
                            batch.Draw(AssetManager.GetWhite(), bounds, Color.Green);
                        batch.DrawString(font, text, pos + offset, Color.Green);
                    }
                    break;

                default:
                    {
                        pos.Y *= -0.5f;
                        pos.X = 0.0f;
                        pos += OSB_locations[station - 1];
                        bounds.X = (int)(pos.X - 2.5f);
                        bounds.Y = (int)(pos.Y - 2.5f);
                        if (drawBox)
                            batch.Draw(AssetManager.GetWhite(), bounds, Color.Green);
                        batch.DrawString(font, text, pos + offset, Color.Green);
                    }
                    break;

            }

        }

        private void DrawOSBSingle(int station, String text, SpriteBatch batch, Vector2 offset, bool drawBox)
        { 
            Vector2 pos = font.MeasureString(text);
            Rectangle bounds = new Rectangle();
            bounds.Width = (int)(pos.X + 4.5f);
            bounds.Height = (int)(pos.Y + 4.5f);

            switch (station - 1)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    {
                        pos *= -0.5f;
                        pos += OSB_locations[station - 1];
                        bounds.X = (int)(pos.X - 2.5f);
                        bounds.Y = (int)(pos.Y - 2.5f);
                        if (drawBox)
                            batch.Draw(AssetManager.GetBlack(), bounds, Color.Black);
                        batch.DrawString(font, text, pos + offset, Color.Green);
                    }
                    break;

                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    {
                        pos.Y *= -0.5f;
                        pos.X *= -1.0f;
                        pos += OSB_locations[station - 1];
                        bounds.X = (int)(pos.X - 2.5f);
                        bounds.Y = (int)(pos.Y - 2.5f);
                        if (drawBox)
                            batch.Draw(AssetManager.GetBlack(), bounds, Color.Black);
                        batch.DrawString(font, text, pos + offset, Color.Green);
                    }
                    break;

                default:
                    {
                        pos.Y *= -0.5f;
                        pos.X = 0.0f;
                        pos += OSB_locations[station - 1];
                        bounds.X = (int)(pos.X - 2.5f);
                        bounds.Y = (int)(pos.Y - 2.5f);
                        if (drawBox)
                            batch.Draw(AssetManager.GetBlack(), bounds, Color.Black);
                        batch.DrawString(font, text, pos + offset, Color.Green);
                    }
                    break;

            }

        }

        private void DrawSMS(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mfd);
            Vector2 pos;
            pos = new Vector2(65, 145);
            batch.DrawString(font, "PGU20", pos, Color.Green);

            int count;
            String s = GetWeaponString(13, out count);
            String gun = String.Format("{0}GUN", count / 10);
            pos = new Vector2(65, 125);
            batch.DrawString(font, gun, pos, Color.Green);

            s = GetWeaponString(6, out count);
            pos = font.MeasureString(s);
            pos *= -0.5f;
            pos += new Vector2(256, 100);
            batch.DrawString(font, s, pos, Color.Green);

            s = GetWeaponString(5, out count);
            pos = font.MeasureString(s);
            pos *= -0.5f;
            pos += new Vector2(256, 120);
            batch.DrawString(font, s, pos, Color.Green);

            s = GetWeaponString(7, out count);
            pos = font.MeasureString(s);
            pos *= -0.5f;
            pos += new Vector2(256, 140);
            batch.DrawString(font, s, pos, Color.Green);

            s = GetWeaponString(7, out count);
            Draw3Stations(s, count, 150, 180);
            s = GetWeaponString(8, out count);
            Draw3Stations(s, count, 362, 180);

            s = GetWeaponString(2, out count);
            pos = font.MeasureString(s);
            pos *= -0.5f;
            pos += new Vector2(110, 240);
            batch.DrawString(font, s, pos, Color.Green);

            s = GetWeaponString(3, out count);
            Draw2Stations(s, count, 110, 260);

            s = GetWeaponString(10, out count);
            pos = font.MeasureString(s);
            pos *= -0.5f;
            pos += new Vector2(402, 240);
            batch.DrawString(font, s, pos, Color.Green);

            s = GetWeaponString(9, out count);
            Draw2Stations(s, count, 402, 260);

            s = GetWeaponString(1, out count);
            Draw2Stations(s, count, 92, 300);

            s = GetWeaponString(11, out count);
            Draw2Stations(s, count, 512 - 92, 300);

            pos = font.MeasureString("1   LNCH");
            pos *= -0.5f;
            pos.X = 0;
            pos += new Vector2(62, 354);
            batch.DrawString(font, "1   LNCH", pos, Color.Green);

            pos = font.MeasureString("1   LNCH");
            pos *= -0.5f;
            pos.X = 0;
            pos += new Vector2(512 - 111, 354);
            batch.DrawString(font, "1   LNCH", pos, Color.Green);

            s = GetWeaponString(0, out count);
            pos = font.MeasureString(s);
            pos *= -0.5f;
            pos.X = 0;
            pos += new Vector2(62, 376);
            batch.DrawString(font, s, pos, Color.Green);

            s = GetWeaponString(12, out count);
            pos = font.MeasureString(s);
            pos *= -0.5f;
            pos.X = 0;
            pos += new Vector2(512 - 111, 376);
            batch.DrawString(font, s, pos, Color.Green);
            batch.End();
        }

        private Vector2 Rotate(Vector2 centre, Vector2 input, float bearing)
        {
            float sinb = (float)Math.Sin(MathHelper.ToRadians(bearing));
            float cosb = (float)Math.Cos(MathHelper.ToRadians(bearing));

            Vector2 point2 = new Vector2();
            point2.X = centre.X + (input.Y * sinb) - (input.X * cosb);
            point2.Y = centre.Y - (input.Y * cosb) - (input.X * sinb);

            return point2;
        }

        private void UpdatePickle(Vector2 inp)
        {
            pickle = (inp + new Vector2(1, 1)) * 256;

            //if (inp.LengthSquared() > 0.1)
            {
               // pickle += 2 * inp;
                if (pickle.X < 64)
                    pickle.X = 64;
                if (pickle.X > 512 - 64)
                    pickle.X = 512 - 64;
                if (pickle.Y < 64)
                    pickle.Y = 64;
                if (pickle.Y > 512 - 64)
                    pickle.Y = 512 - 64;
            }
        }

        private float ArcTanAngle(float X, float Y)
        {
            if (X == 0)
            {
                if (Y == 1)
                    return (float)MathHelper.PiOver2;
                else
                    return (float)-MathHelper.PiOver2;
            }
            else if (X > 0)
                return (float)Math.Atan(Y / X);
            else if (X < 0)
            {
                if (Y > 0)
                    return (float)Math.Atan(Y / X) + MathHelper.Pi;
                else
                    return (float)Math.Atan(Y / X) - MathHelper.Pi;
            }
            else
                return 0;
        }

        //returns Euler angles that point from one point to another
        private Vector3 AngleTo(Vector3 from, Vector3 location)
        {
            Vector3 angle = new Vector3();
            Vector3 v3 = Vector3.Normalize(location - from);
            angle.X = (float)Math.Asin(v3.Y);
            angle.Y = ArcTanAngle(-v3.Z, -v3.X);
            return angle;
        }
        private Vector3 QuaternionToEuler(Quaternion rotation)
        {
            Vector3 rotationaxes = new Vector3();

            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);
            rotationaxes = AngleTo(new Vector3(), forward);
            if (rotationaxes.X == MathHelper.PiOver2)
            {
                rotationaxes.Y = ArcTanAngle(up.Z, up.X);
                rotationaxes.Z = 0;
            }
            else if (rotationaxes.X == -MathHelper.PiOver2)
            {
                rotationaxes.Y = ArcTanAngle(-up.Z, -up.X);
                rotationaxes.Z = 0;
            }
            else
            {
                up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationaxes.Y));
                up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationaxes.X));
                rotationaxes.Z = ArcTanAngle(up.Y, -up.X);
            }
            return rotationaxes;
        }
        #endregion

        #region Input mapper
        void SendEvent(CMFDMappableEvents evt)
        {
            switch (evt)
            {
                case CMFDMappableEvents.RangeUp:
                    {
                        switch (mode)
                        {
                            case MFDMode.HSD:
                            case MFDMode.FCR:
                                {
                                    HandleEvent("CMFD20UP");
                                }
                                break;
                        }
                    }
                    break;
                case CMFDMappableEvents.RangeDown:
                    {
                        switch (mode)
                        {
                            case MFDMode.HSD:
                            case MFDMode.FCR:
                                {
                                    HandleEvent("CMFD19UP");
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        #endregion

    }
}
