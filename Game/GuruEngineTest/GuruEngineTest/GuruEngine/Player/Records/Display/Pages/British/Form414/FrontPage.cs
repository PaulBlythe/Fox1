using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Rendering;
using GuruEngine.Rendering.RenderCommands;
using GuruEngine.Text;
using GuruEngine.Assets;
using GuruEngine.Player.Records.WWII.British;

namespace GuruEngine.Player.Records.Display.Pages.British.Form414
{
    public class FrontPage : Page
    {
        int backdropID;
        int frontID;
        int fontID;
        int fontTexID;
        int mfontTexID;
        int shaderID;
        int shadowID;
        int mshaderID;
        int mfontID;

        RenderSpritesCommand sprites;
        RenderTextCommand text;
        RenderTextCommand mtext;

        Rectangle ba_src = new Rectangle(0, 0, 2048, 1024);
        Rectangle fa_src = new Rectangle(0, 0, 1024, 1024);
        Rectangle fa_dest;

        Texture2D backdrop;
        Texture2D front;
        SDFFont font;
        MSDFFont mfont;
        PlayerLog414 pform;

        String text1 = "ROYAL AIR FORCE";
        Vector2 text1_pos;

        String text2 = "PILOT'S FLYING";
        Vector2 text2_pos;

        String text3 = "LOG BOOK";
        Vector2 text3_pos;

        String text4 = "FORM 414";
        Vector2 text4_pos;

        String text5 = "Name......................................................................";
        Vector2 text5_pos;

        String text6 = "_______";
        Vector2 text6_pos;
        Vector2 text7_pos;

        Vector2 text8_pos;


        bool Active = false;

        public override void SetupRenderCommands()
        {
            set = new RenderCommandSet();
            set.blend = BlendState.AlphaBlend;
            set.DS = DepthStencilState.None;
            set.fx = null;
            set.IsStaticMesh = false;
            set.RenderPass = RenderPasses.Transparent;

            String ba = FilePaths.DataPath + @"Textures\Generic\wood_texture.jpg";
            AssetManager.AddTextureToQue(ba);
            backdropID = ba.GetHashCode();

            ba = FilePaths.DataPath + @"Textures\Logs\British\WWII\pilot_front.png";
            AssetManager.AddTextureToQue(ba);
            frontID = ba.GetHashCode();

            ba = FilePaths.DataPath + @"Textures\Logs\British\WWII\front_shadow.png";
            AssetManager.AddTextureToQue(ba);
            shadowID = ba.GetHashCode();

            ba = FilePaths.DataPath + @"Fonts\TimesNewRoman.txt";
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

            fa_dest = Scale(960, 10, 950, 1060);

            pform = PlayerRecord.Instance.pilotsLog as PlayerLog414;

        }

        public override void Update()
        {
            if (!Active)
            {
                backdrop = AssetManager.Texture(backdropID);
                if (backdrop == null)
                    return;

                front = AssetManager.Texture(frontID);
                if (front == null)
                    return;

                font = AssetManager.GetFont(fontID);
                if (font == null)
                    return;

                Texture2D fonttex = AssetManager.Texture(fontTexID);
                if (fonttex == null)
                    return;

                Texture2D shadowtex = AssetManager.Texture(shadowID);
                if (fonttex == null)
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
                sprites.AddSprite(shadowtex, new Rectangle(0, 0, 1024, 1024), FullScreen(), Color.White);
                sprites.AddSprite(front, fa_src, fa_dest, Color.White);
                set.Commands.Add(sprites);

                font.Setup(fonttex, effect, Color.Black, Scale(1.8f));
                mfont.Setup(mfonttex, meffect, Color.Black, 0.5f);

                text = new RenderTextCommand(font.GetBatch());
                mtext = new RenderTextCommand(mfont.GetBatch());
                
                text1_pos = font.MeasureString(text1);
                text1_pos *= -0.5f;
                text1_pos += Scale(new Vector2(960 + 480, 219));

                text2_pos = font.MeasureString(text2);
                text2_pos *= -0.5f;
                text2_pos += Scale(new Vector2(960 + 480, 500));

                text3_pos = font.MeasureString(text3);
                text3_pos *= -0.5f;
                text3_pos += Scale(new Vector2(960 + 480, 580));

                text4_pos = font.MeasureString(text4, 0.4f);
                text4_pos *= -0.5f;
                text4_pos += Scale(new Vector2(960 + 800, 80));

                text5_pos = font.MeasureString(text5, 0.5f);
                text5_pos *= -0.5f;
                text5_pos += Scale(new Vector2(960 + 480, 880));

                text6_pos = font.MeasureString(text6);
                text6_pos *= -0.5f;
                text6_pos += Scale(new Vector2(960 + 480, 650));

                text7_pos = font.MeasureString(text6);
                text7_pos *= -0.5f;
                text7_pos += Scale(new Vector2(960 + 480, 289));


                font.DrawString(text1, text1_pos);
                font.DrawString(text2, text2_pos);
                font.DrawString(text3, text3_pos);
                font.DrawString(text4, text4_pos, 0.4f);
                font.DrawString(text5, text5_pos, 0.5f);
                font.DrawString(text6, text6_pos);
                font.DrawString(text6, text7_pos);

                set.Commands.Add(text);

                text8_pos = Scale(new Vector2(960 + 300, 810));
                mfont.DrawString(pform.PilotName, text8_pos);

                set.Commands.Add(mtext);

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

            AssetManager.RemoveTextureReference(frontID);
            AssetManager.RemoveTextureReference(backdropID);
            AssetManager.RemoveTextureReference(fontTexID);
            AssetManager.RemoveFontReference(fontID);
            AssetManager.RemoveMFontReference(mfontID);

            backdrop = null;
            front = null;
            font = null;
        }
    }
}
