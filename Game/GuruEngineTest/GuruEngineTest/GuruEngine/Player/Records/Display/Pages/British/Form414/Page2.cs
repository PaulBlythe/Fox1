using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Rendering.RenderCommands;
using GuruEngine.Text;
using GuruEngine.Assets;

namespace GuruEngine.Player.Records.Display.Pages.British.Form414
{
    public class Page2 : Page
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

                sprites.AddLine(Scale(25, 19, 950, 19), Color.Black, 3);
                sprites.AddLine(Scale(960 + 25, 19, 955 + 950, 19), Color.Black, 3);

                sprites.AddLine(Scale(100, 19, 100, 1000), Color.Black, 1);
                sprites.AddLine(Scale(70, 80, 70, 1000), Color.Black, 1);
                sprites.AddLine(Scale(25, 80, 100, 80), Color.Black, 1);
                sprites.AddLine(Scale(25, 100, 950, 100), Color.Black, 1);

                sprites.AddLine(Scale(100, 60, 300, 60), Color.Black, 1);
                sprites.AddLine(Scale(240, 60, 240, 1000), Color.Black, 1);
                sprites.AddLine(Scale(300, 19, 300, 1000), Color.Black, 1);

                sprites.AddLine(Scale(25, 1000, 950, 1000), Color.Black, 3);

                sprites.AddLine(Scale(450, 19, 450, 1000), Color.Black, 1);
                sprites.AddLine(Scale(650, 19, 650, 1000), Color.Black, 1);

                sprites.AddLine(Scale(960 + 25, 1000, 1900, 1000), Color.Black, 3);
                sprites.AddLine(Scale(960 + 25, 990, 1900, 990), Color.Black, 1);
                sprites.AddLine(Scale(960 + 25, 19, 985, 1002), Color.Black, 1);
                sprites.AddLine(Scale(960 + 22, 19, 982, 1002), Color.Black, 1);
                sprites.AddLine(Scale(960 + 25, 100, 1900, 100), Color.Black, 1);


                sprites.AddLine(Scale(985 + 70, 60, 985 + 70 , 1000), Color.Black, 1);
                sprites.AddLine(Scale(985 + 138, 40, 985 + 138, 1000), Color.Black, 1);
                sprites.AddLine(Scale(985 + 140, 40, 985 + 140, 1000), Color.Black, 1);
                sprites.AddLine(Scale(985, 40, 985 + 700, 40), Color.Black, 1);
                sprites.AddLine(Scale(985, 60, 985 + 138, 60), Color.Black, 1);
                sprites.AddLine(Scale(985, 85, 985 + 138, 85), Color.Black, 1);


                sprites.AddLine(Scale(985 + 278, 19, 985 + 278, 1000), Color.Black, 1);
                sprites.AddLine(Scale(985 + 280, 19, 985 + 280, 1000), Color.Black, 1);
                sprites.AddLine(Scale(985 + 140, 60, 985 + 278, 60), Color.Black, 1);
                sprites.AddLine(Scale(985 + 140, 85, 985 + 278, 85), Color.Black, 1);
                sprites.AddLine(Scale(985 + 210, 60, 985 + 210, 1000), Color.Black, 1);


                sprites.AddLine(Scale(985 + 698, 19, 985 + 698, 1000), Color.Black, 1);
                sprites.AddLine(Scale(985 + 700, 19, 985 + 700, 1000), Color.Black, 1);

                sprites.AddLine(Scale(985 + 489, 40, 985 + 489, 1000), Color.Black, 1);
                sprites.AddLine(Scale(985 + 491, 40, 985 + 491, 1000), Color.Black, 1);

                sprites.AddLine(Scale(985 + 489, 60, 985 + 281, 60), Color.Black, 1);
                sprites.AddLine(Scale(985 + 491, 60, 985 + 698, 60), Color.Black, 1);

                sprites.AddLine(Scale(985 + 489, 85, 985 + 281, 85), Color.Black, 1);
                sprites.AddLine(Scale(985 + 491, 85, 985 + 698, 85), Color.Black, 1);

                sprites.AddLine(Scale(985 + 420, 60, 985 + 420, 1000), Color.Black, 1);
                sprites.AddLine(Scale(985 + 350, 60, 985 + 350, 1000), Color.Black, 1);


                sprites.AddLine(Scale(985 + 560, 60, 985 + 560, 1000), Color.Black, 1);
                sprites.AddLine(Scale(985 + 630, 60, 985 + 630, 1000), Color.Black, 1);

                sprites.AddLine(Scale(985 + 700, 85, 1900, 85), Color.Black, 1);
                sprites.AddLine(Scale(985 + 700, 60, 1900, 60), Color.Black, 1);
                sprites.AddLine(Scale(985 + 807, 60, 985 + 807, 1000), Color.Black, 1);

                set.Commands.Add(sprites);


                font.Setup(fonttex, effect, Color.Black, Scale(1.8f));
                mfont.Setup(mfonttex, meffect, Color.Red, 0.5f);

                DrawCentred("Year", new Vector2(45, 60), 0.4f);
                DrawCentred("Aircraft", new Vector2(200, 60), 0.4f);
                DrawCentred("MONTH", new Vector2(40, 100), 0.18f);
                DrawCentred("DATE", new Vector2(80, 100), 0.18f);
                DrawCentred("Type", new Vector2(160, 95), 0.28f);
                DrawCentred("No.", new Vector2(270, 95), 0.28f);

                DrawCentred("Pilot, or", new Vector2(375, 60), 0.35f);
                DrawCentred("1st Pilot", new Vector2(375, 90), 0.35f);

                DrawCentred("2nd Pilot, Pupil", new Vector2(550, 60), 0.35f);
                DrawCentred("or Passenger", new Vector2(550, 90), 0.35f);

                DrawCentred("DUTY", new Vector2(800, 60), 0.4f);
                DrawCentred("(Including Results and Remarks)", new Vector2(800, 90), 0.35f);

                DrawCentred("GRAND TOTAL", new Vector2(440, 1033), 0.28f);
                DrawLeft("[Cols.(1) to (10)]", new Vector2(510, 1023), 0.2f);

                DrawLeft(".................Hrs", new Vector2(390, 1050), 0.28f);

                DrawLeft(".................Mins.", new Vector2(480, 1050), 0.28f);
                DrawLeft("Totals Carried Forward", new Vector2(750, 1040), 0.28f);

                DrawCentred("Single Engine Aircraft", new Vector2(960+145, 45), 0.28f);
                DrawCentred("DAY", new Vector2(982 + 70, 63), 0.23f);

                DrawCentred("Dual", new Vector2(982 + 35, 86), 0.25f);
                DrawCentred("Pilot", new Vector2(982 + 105, 86), 0.25f);

                DrawCentred("(1)", new Vector2(982 + 35, 103), 0.2f);
                DrawCentred("(2)", new Vector2(982 + 105, 103), 0.2f);

                DrawCentred("NIGHT", new Vector2(982 + 210, 63), 0.23f);
                DrawCentred("Dual", new Vector2(982 + 175, 86), 0.25f);
                DrawCentred("Pilot", new Vector2(982 + 245, 86), 0.25f);

                DrawCentred("(3)", new Vector2(982 + 175, 103), 0.2f);
                DrawCentred("(4)", new Vector2(982 + 245, 103), 0.2f);
                DrawCentred("(5)", new Vector2(982 + 315, 103), 0.2f);
                DrawCentred("(6)", new Vector2(982 + 385, 103), 0.2f);
                DrawCentred("(7)", new Vector2(982 + 455, 103), 0.2f);
                DrawCentred("(8)", new Vector2(982 + 525, 103), 0.2f);
                DrawCentred("(9)", new Vector2(982 + 595, 103), 0.2f);
                DrawCentred("(10)", new Vector2(982 + 665, 103), 0.2f);


                DrawCentred("(1)", new Vector2(982 + 35,  1005), 0.2f);
                DrawCentred("(2)", new Vector2(982 + 105, 1005), 0.2f);
                DrawCentred("(3)", new Vector2(982 + 175, 1005), 0.2f);
                DrawCentred("(4)", new Vector2(982 + 245, 1005), 0.2f);
                DrawCentred("(5)", new Vector2(982 + 315, 1005), 0.2f);
                DrawCentred("(6)", new Vector2(982 + 385, 1005), 0.2f);
                DrawCentred("(7)", new Vector2(982 + 455, 1005), 0.2f);
                DrawCentred("(8)", new Vector2(982 + 525, 1005), 0.2f);
                DrawCentred("(9)", new Vector2(982 + 595, 1005), 0.2f);
                DrawCentred("(10)", new Vector2(982 + 665, 1005), 0.2f);

                text = new RenderTextCommand(font.GetBatch());
                mtext = new RenderTextCommand(mfont.GetBatch());

                DrawCentred("Multi-Engine Aircraft", new Vector2(985 + 490, 45), 0.28f);
                DrawCentred("DAY", new Vector2(985 + 385, 63), 0.23f);
                DrawCentred("NIGHT", new Vector2(985 + 594, 63), 0.23f);

                DrawCentred("Dual", new Vector2(982 + 315, 86), 0.25f);
                DrawCentred("1st", new Vector2(982 + 385, 79), 0.2f);
                DrawCentred("Pilot", new Vector2(982 + 385, 88), 0.2f);
                DrawCentred("2nd", new Vector2(982 + 455, 79), 0.2f);
                DrawCentred("Pilot", new Vector2(982 + 455, 88), 0.2f);

                DrawCentred("Dual", new Vector2(982 + 525, 86), 0.25f);
                DrawCentred("1st", new Vector2(982 + 595, 79), 0.2f);
                DrawCentred("Pilot", new Vector2(982 + 595, 88), 0.2f);
                DrawCentred("2nd", new Vector2(982 + 665, 79), 0.2f);
                DrawCentred("Pilot", new Vector2(982 + 665, 88), 0.2f);

                DrawCentred("(11)", new Vector2(982 + 752, 1005), 0.2f);
                DrawCentred("(11)", new Vector2(982 + 752, 103), 0.2f);

                DrawCentred("(12)", new Vector2(982 + 859, 1005), 0.2f);
                DrawCentred("(12)", new Vector2(982 + 859, 103), 0.2f);

                DrawCentred("Dual", new Vector2(982 + 752, 86), 0.25f);
                DrawCentred("Pilot", new Vector2(982 + 859, 86), 0.25f);

                DrawCentred("INSTR/CLOUD", new Vector2(982 + 807, 41), 0.2f);
                DrawCentred("FLYING IIncl in", new Vector2(982 + 807, 52), 0.2f);
                DrawCentred("cols (1) to (10)", new Vector2(982 + 807, 64), 0.2f);
                set.Commands.Add(text);

                mfont.DrawString("No 2 EFTS STAVERTON", new Vector2(200, 100), 0.85f);
                mfont.DrawString("1939", new Vector2(40, 40), 0.75f);
                mfont.DrawString("May", new Vector2(35, 140), 0.55f);
                mfont.DrawString("1st", new Vector2(75, 140), 0.55f);
                mfont.DrawString("Tiger Moth", new Vector2(105, 140), 0.55f);
                mfont.DrawString("R5064", new Vector2(245, 140), 0.55f);
                mfont.DrawString("RO Perkins", new Vector2(305, 140), 0.55f);
                mfont.DrawString("Self", new Vector2(455, 140), 0.55f);
                mfont.DrawString("Air expierience", new Vector2(652, 140), 0.65f);
                mfont.DrawString("Familiarity cockpit layout", new Vector2(652, 165), 0.65f);
                mfont.DrawString("Effect of controls", new Vector2(652, 190), 0.65f);
                mfont.DrawString("Straight and level flight", new Vector2(652, 215), 0.65f);

                mfont.DrawString("1", new Vector2(1010, 140), 0.65f);


               
                mfont.DrawString("May", new Vector2(35, 250), 0.55f);
                mfont.DrawString("2nd", new Vector2(75, 250), 0.55f);
                mfont.DrawString("Tiger Moth", new Vector2(105, 250), 0.55f);
                mfont.DrawString("R5064", new Vector2(245, 250), 0.55f);
                mfont.DrawString("RO Perkins", new Vector2(305, 250), 0.55f);
                mfont.DrawString("Self", new Vector2(455, 250), 0.55f);
                mfont.DrawString("Climbing and gliding", new Vector2(652, 250), 0.65f);

                mfont.DrawString("1", new Vector2(1010, 250), 0.65f);

                mfont.DrawString("May", new Vector2(35, 285), 0.55f);
                mfont.DrawString("3rd", new Vector2(75, 285), 0.55f);
                mfont.DrawString("Tiger Moth", new Vector2(105, 285), 0.55f);
                mfont.DrawString("R5064", new Vector2(245, 285), 0.55f);
                mfont.DrawString("RO Perkins", new Vector2(305, 285), 0.55f);
                mfont.DrawString("Self", new Vector2(455, 285), 0.55f);
                mfont.DrawString("Medium turns", new Vector2(652, 285), 0.65f);

                mfont.DrawString("1", new Vector2(1010, 285), 0.65f);

                mfont.DrawString("May", new Vector2(35, 320), 0.55f);
                mfont.DrawString("4th", new Vector2(75, 320), 0.55f);
                mfont.DrawString("Tiger Moth", new Vector2(105, 320), 0.55f);
                mfont.DrawString("R5064", new Vector2(245, 320), 0.55f);
                mfont.DrawString("RO Perkins", new Vector2(305, 320), 0.55f);
                mfont.DrawString("Self", new Vector2(455, 320), 0.55f);
                mfont.DrawString("Medium turns", new Vector2(652, 320), 0.65f);

                mfont.DrawString("1.10", new Vector2(1010, 320), 0.65f);

                mfont.DrawString("May", new Vector2(35, 355), 0.55f);
                mfont.DrawString("5th", new Vector2(75, 355), 0.55f);
                mfont.DrawString("Tiger Moth", new Vector2(105, 355), 0.55f);
                mfont.DrawString("R5064", new Vector2(245, 355), 0.55f);
                mfont.DrawString("RO Perkins", new Vector2(305, 355), 0.55f);
                mfont.DrawString("Self", new Vector2(455, 355), 0.55f);
                mfont.DrawString("Climbing,gliding,stalling", new Vector2(652, 355), 0.65f);
                mfont.DrawString("Medium turns", new Vector2(652, 380), 0.65f);
                mfont.DrawString("Take off into wind", new Vector2(652, 405), 0.65f);
                mfont.DrawString("Gliding approach to landing", new Vector2(652, 430), 0.65f);

                mfont.DrawString("2.20", new Vector2(1010, 355), 0.65f);

                mfont.DrawString("May", new Vector2(35, 455), 0.55f);
                mfont.DrawString("8th", new Vector2(75, 455), 0.55f);
                mfont.DrawString("Tiger Moth", new Vector2(105, 455), 0.55f);
                mfont.DrawString("R5064", new Vector2(245, 455), 0.55f);
                mfont.DrawString("RO Perkins", new Vector2(305, 455), 0.55f);
                mfont.DrawString("Self", new Vector2(455, 455), 0.55f);
                mfont.DrawString("Take off into wind", new Vector2(652, 455), 0.65f);
                mfont.DrawString("Gliding approach to landing", new Vector2(652, 480), 0.65f);

                mfont.DrawString("1.20", new Vector2(1010, 455), 0.65f);

                mfont.DrawString("May", new Vector2(35, 510), 0.55f);
                mfont.DrawString("9th", new Vector2(75, 510), 0.55f);
                mfont.DrawString("Tiger Moth", new Vector2(105, 510), 0.55f);
                mfont.DrawString("R5064", new Vector2(245, 510), 0.55f);
                mfont.DrawString("RO Perkins", new Vector2(305, 510), 0.55f);
                mfont.DrawString("Self", new Vector2(455, 510), 0.55f);
                mfont.DrawString("Take off into wind", new Vector2(652, 510), 0.65f);
                mfont.DrawString("Gliding approach to landing", new Vector2(652, 535), 0.65f);

                mfont.DrawString("1.20", new Vector2(1010, 510), 0.65f);

                mfont.DrawString("May", new Vector2(35, 560), 0.55f);
                mfont.DrawString("10th", new Vector2(75, 560), 0.55f);
                mfont.DrawString("Tiger Moth", new Vector2(105, 560), 0.55f);
                mfont.DrawString("R5064", new Vector2(245, 560), 0.55f);
                mfont.DrawString("RO Perkins", new Vector2(305, 560), 0.55f);
                mfont.DrawString("Self", new Vector2(455, 560), 0.55f);
                mfont.DrawString("Take off into wind", new Vector2(652, 560), 0.65f);

                mfont.DrawString("1.30", new Vector2(1010, 560), 0.65f);

                mfont.DrawString("May", new Vector2(35, 590), 0.55f);
                mfont.DrawString("11th", new Vector2(75, 590), 0.55f);
                mfont.DrawString("Tiger Moth", new Vector2(105, 590), 0.55f);
                mfont.DrawString("R5064", new Vector2(245, 590), 0.55f);
                mfont.DrawString("RO Perkins", new Vector2(305, 590), 0.55f);
                mfont.DrawString("Self", new Vector2(455, 590), 0.55f);
                mfont.DrawString("Climbing,gliding,stalling", new Vector2(652, 590), 0.65f);

                mfont.DrawString("1.15", new Vector2(1010, 590), 0.65f);

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
            font.DrawString(s, sz, scale);
        }

        private void DrawLeft(String s, Vector2 pos, float scale)
        {
            Vector2 sz = Scale(pos);
            font.DrawString(s, sz, scale);
        }
    }
}