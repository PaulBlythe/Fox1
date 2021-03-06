﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Rendering.RenderCommands;
using GuruEngine.Text;
using GuruEngine.Assets;
using GuruEngine.Player.Records.WWII.British;
using GuruEngine.Player.Records.Common;
using GuruEngine.World.Aircraft;

namespace GuruEngine.Player.Records.Display.Pages.British.Form414
{
    public class Page1 :Page
    {
        int backdropID;
        int fontID;
        int fontTexID;
        int mfontTexID;
        int shaderID;
        int mshaderID;
        int mfontID;
        int mpageID;
        int shadowID;

        RenderSpritesCommand sprites;
        RenderTextCommand text;
        RenderTextCommand mtext;
        RenderSpritesCommand overlay;

        Texture2D backdrop;
        
        SDFFont font;
        MSDFFont mfont;

        Rectangle ba_src = new Rectangle(0, 0, 2048, 1024);

        bool Active = false;
        PlayerLog414 pform;

        public override void SetupRenderCommands()
        {
            set = new RenderCommandSet();
            set.blend = BlendState.AlphaBlend;
            set.DS = DepthStencilState.None;
            set.fx = null;
            set.IsStaticMesh = false;
            set.RenderPass = RenderPasses.Transparent;

            String ba = FilePaths.DataPath + @"Textures\Logs\British\WWII\front_shadow_2.png";
            AssetManager.AddTextureToQue(ba);
            shadowID = ba.GetHashCode();

            ba = FilePaths.DataPath + @"Textures\Generic\wood_texture.jpg";
            AssetManager.AddTextureToQue(ba);
            backdropID = ba.GetHashCode();

            ba = FilePaths.DataPath + @"Textures\Logs\British\WWII\midpage.png";
            AssetManager.AddTextureToQue(ba);
            mpageID = ba.GetHashCode();


            ba = FilePaths.DataPath + @"\Fonts\TimesNewRoman.txt";
            AssetManager.AddFontToQue(ba);
            fontID = ba.GetHashCode();

            ba = FilePaths.DataPath + @"Fonts\TimesNewRomanNormal.png";
            AssetManager.AddTextureToQue(ba);
            fontTexID = ba.GetHashCode();

            ba = @"Shaders\2D\SDFFont2";
            AssetManager.AddShaderToQue(ba);
            shaderID = ba.GetHashCode();

            ba = FilePaths.DataPath + @"Fonts\handwriting.png";
            AssetManager.AddTextureToQue(ba);
            mfontTexID = ba.GetHashCode();

            ba = @"Shaders\2D\MSDFShader";
            AssetManager.AddShaderToQue(ba);
            mshaderID = ba.GetHashCode();

            String filename = Path.Combine(FilePaths.DataPath, "Fonts");
            filename = Path.Combine(filename, "handwriting.csv");
            AssetManager.AddMFontToQue(filename);
            mfontID = filename.GetHashCode();

            pform = PlayerRecord.Instance.pilotsLog as PlayerLog414;
        }
        public override void Update()
        {
            if (!Active)
            {
                backdrop = AssetManager.Texture(backdropID);
                if (backdrop == null)
                    return;

                
                font = AssetManager.GetFont(fontID);
                if (font == null)
                    return;

                Texture2D fonttex = AssetManager.Texture(fontTexID);
                if (fonttex == null)
                    return;

                Texture2D midpage = AssetManager.Texture(mpageID);
                if (midpage == null)
                    return;

                Texture2D shadowtex = AssetManager.Texture(shadowID);
                if (shadowtex == null)
                    return;

                Effect effect = AssetManager.Shader(shaderID);
                if (effect == null)
                    return;

                Effect meffect = AssetManager.Shader(mshaderID);
                if (meffect == null)
                    return;

                Texture2D mfonttex = AssetManager.Texture(mfontTexID);
                if (mfonttex == null)
                    return;

                mfont = AssetManager.GetMFont(mfontID);
                if (mfont == null)
                    return;


                sprites = new RenderSpritesCommand();
                sprites.AddSprite(backdrop, ba_src, FullScreen(), Color.White);
                sprites.AddFilledRectangle(Scale(16, 14, 1890, 1060), Color.FromNonPremultiplied(0, 0, 0, 128));
                sprites.AddFilledRectangle(Scale(20, 10, 1890, 1060), Color.White);
                sprites.AddSprite(midpage, new Rectangle(0, 0, 64, 32), Scale(933, 10, 64, 1060), Color.White);

                sprites.AddLine(Scale(1040, 280, 1880, 280), Color.DarkGray, 4);
                sprites.AddLine(Scale(1042, 320, 1878, 320), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 360, 1878, 360), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 400, 1878, 400), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 440, 1878, 440), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 480, 1878, 480), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 520, 1878, 520), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 560, 1878, 560), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 590, 1878, 590), Color.DarkGray, 4);

                sprites.AddLine(Scale(1270, 280, 1270, 590), Color.DarkGray, 4);
                sprites.AddLine(Scale(1400, 280, 1400, 590), Color.DarkGray, 4);
                sprites.AddLine(Scale(1580, 280, 1580, 590), Color.DarkGray, 4);

                sprites.AddLine(Scale(1040, 665, 1880, 665), Color.DarkGray, 4);
                sprites.AddLine(Scale(1042, 705, 1878, 705), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 745, 1878, 745), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 785, 1878, 785), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 825, 1878, 825), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 865, 1878, 865), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 905, 1878, 905), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 945, 1878, 945), Color.DarkGray, 2);
                sprites.AddLine(Scale(1042, 985, 1878, 985), Color.DarkGray, 4);

                sprites.AddLine(Scale(1270, 665, 1270, 985), Color.DarkGray, 4);
                sprites.AddLine(Scale(1400, 665, 1400, 985), Color.DarkGray, 4);
                sprites.AddLine(Scale(1580, 665, 1580, 985), Color.DarkGray, 4);
                set.Commands.Add(sprites);


                font.Setup(fonttex, effect, Color.Black, Scale(1.8f));
                mfont.Setup(mfonttex, meffect, Color.Red, 0.5f);

                text = new RenderTextCommand(font.GetBatch());
                mtext = new RenderTextCommand(mfont.GetBatch());

                
                font.DrawString("T7351 wt. 11351 28,000 Bks. 2/40-Sir J.C.& S. Ltd. -16", Scale(80,40),0.3f);
                DrawCentred("INSTRUCTIONS", new Vector2(492.5f, 200), 1.4f);
                DrawCentred("[see K.R. & A.C.I., para 786]", new Vector2(492.5f, 210), 0.4f);
                DrawCentred("-------------------", new Vector2(492.5f, 240), 0.4f);
                DrawLeft("1.    This book is an official document and is the property of His Majesty's", new Vector2(100,300), 0.48f);
                DrawLeft("      Government.", new Vector2(100, 330), 0.48f);
                DrawLeft("2.    An accurate and detailed record is to be kept in the log of all flights", new Vector2(100, 400), 0.48f);
                DrawLeft("       undertaken by the individual to whom it relates.", new Vector2(100, 430), 0.48f);
                DrawLeft("3.    Monthly flying will be analysed by aircraft types and insterted in", new Vector2(100, 500), 0.48f);
                DrawLeft("       red ink.    The stamp will be inserted on the left hand page", new Vector2(100, 530), 0.48f);
                DrawLeft("       appropriately aligned to the ruling.", new Vector2(100, 560), 0.48f);
                DrawLeft("4.    The annual summary and assessment will be completed on Form", new Vector2(100, 630), 0.48f);
                DrawLeft("       414A and inserted in the appropriate page of the log. This form", new Vector2(100, 660), 0.48f);
                DrawLeft("       will also be used when a pilot is posted or attached to another unit", new Vector2(100, 690), 0.48f);
                DrawLeft("       for flying duties.", new Vector2(100, 720), 0.48f);

                DrawCentred("CERTIFICATES OF QUALIFICATION AS FIRST PILOT", new Vector2(492.5f + 945, 90), 0.6f);
                DrawCentred("________________________________________________", new Vector2(492.5f + 945, 80), 0.6f);
                DrawCentred("[K.R. & A.C.I., para 805, clause 5.]", new Vector2(492.5f + 945, 120), 0.4f);
                DrawLeft("Name                                         Rank", new Vector2(1140, 180), 0.6f);
                DrawCentred("___________________________________", new Vector2(492.5f + 945, 190), 0.6f);
                DrawLeft("(i)    Certified that the above named has qualified as a first pilot (day only)", new Vector2(1050, 260), 0.45f);

                DrawCentred("On (Type)", new Vector2(1040 + 115, 320), 0.4f);
                DrawCentred("Date", new Vector2(1270 + 65, 320), 0.4f);
                DrawCentred("Unit", new Vector2(1480, 320), 0.4f);
                DrawCentred("Signature and Rank", new Vector2(1700, 320), 0.4f);

                DrawLeft("(i)    Certified that the above named has qualified as a first pilot", new Vector2(1050, 650), 0.45f);
                DrawCentred("On (Type)", new Vector2(1040 + 115, 705), 0.4f);
                DrawCentred("Date", new Vector2(1270 + 65, 705), 0.4f);
                DrawCentred("Unit", new Vector2(1480, 705), 0.4f);
                DrawCentred("Signature and Rank", new Vector2(1700, 705), 0.4f);

                set.Commands.Add(text);

                mfont.DrawString(pform.PilotName, new Vector2(1300, 125));
                int ypos = 290;
                int y2 = 645;
                foreach (Certification c in pform.aircraftCertifications)
                {
                    if (c.DayOnly)
                    {
                        mfont.DrawString(c.Location, new Vector2(1450, ypos), 0.8f);
                        mfont.DrawString(c.CertifyingOfficer, new Vector2(1600, ypos), 0.8f);
                        mfont.DrawString(c.Date.ToShortDateString(), new Vector2(1280, ypos), 0.8f);
                        mfont.DrawString(AircraftNames.GetAircraftName(c.AircraftType), new Vector2(1060, ypos), 0.8f);
                        ypos += 36;
                    }
                    else
                    {
                        mfont.DrawString(c.Location, new Vector2(1450, y2), 0.8f);
                        mfont.DrawString(c.CertifyingOfficer, new Vector2(1600, y2), 0.8f);
                        mfont.DrawString(c.Date.ToShortDateString(), new Vector2(1280, y2), 0.8f);
                        mfont.DrawString(AircraftNames.GetAircraftName(c.AircraftType), new Vector2(1060, y2), 0.8f);
                        y2 += 36;
                    }
                }

                mfont.DrawString("Pilot officer", new Vector2(1685, 125));

                set.Commands.Add(mtext);

                overlay = new RenderSpritesCommand();
                overlay.AddSprite(shadowtex, new Rectangle(0, 0, 1024, 1024), FullScreen(), Color.White);

                set.Commands.Add(overlay);

                Active = true;
            }
            else
            {

                Renderer.AddRenderCommand(set);
            }

        }

        public override void Dispose()
        {
            sprites.Dispose();

            AssetManager.RemoveTextureReference(backdropID);
            AssetManager.RemoveTextureReference(fontTexID);
            AssetManager.RemoveTextureReference(mpageID);
            AssetManager.RemoveFontReference(fontID);
            AssetManager.RemoveMFontReference(mfontID);

            backdrop = null;
            font = null;
        }

        private void DrawCentred(String s, Vector2 pos, float scale)
        {
            Vector2 sz = font.MeasureString(s, scale);
            sz *= -0.5f;
            sz += Scale(pos);
            font.DrawString(s, sz,scale);
        }

        private void DrawLeft(String s, Vector2 pos, float scale)
        {
            Vector2 sz = Scale(pos);
            font.DrawString(s, sz, scale);
        }
    }
}