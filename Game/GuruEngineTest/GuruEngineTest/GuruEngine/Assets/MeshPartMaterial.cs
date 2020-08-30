using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Rendering;

namespace GuruEngine.Assets
{
    public class MeshPartMaterial:Material
    {
        public String Name;
        public bool tfDoubleSided;
        public bool Sort;
        public bool Glass;
        public float Ambient;
        public float Diffuse;
        public float Specular;
        public float SpecularPow;
        public float Shine;
        public float[] Colour = new float[4];
        public bool tfWrapX;
        public bool tfWrapY;
        public bool tfMinLinear;
        public bool tfMagLinear;
        public bool tfBlend;
        public bool tfBlendAdd;
        public bool tfNoTexture;
        public float tfDepthOffset;
        public bool tfNoWriteZ;
        public int TextureID;
        public float AlphaTestVal;
        public bool tfTranspBorder;
        public bool tfTestA;
        public bool tfTestZ;
        public bool BumpMapped;
        public int NormalTex;
        public String tname = "";
        public int TextureGUID;
        public RasterizerState deferred_rs = null;


        public MeshPartMaterial(BinaryReader bw)
        {
            Name = bw.ReadString();

            tfDoubleSided = bw.ReadBoolean();
            Sort = bw.ReadBoolean();
            Glass = bw.ReadBoolean();
            Ambient = bw.ReadSingle();                  //  Ambient Instensity
            Diffuse = bw.ReadSingle();                  //  Diffuse Intensity
            Specular = bw.ReadSingle();                 //  Not used
            SpecularPow = bw.ReadSingle();              //  Specular Power
            Shine = bw.ReadSingle();                    //  Specular Intensity
            tfWrapX = bw.ReadBoolean();
            tfWrapY = bw.ReadBoolean();
            tfMinLinear = bw.ReadBoolean();
            tfMagLinear = bw.ReadBoolean();
            tfBlend = bw.ReadBoolean();
            tfBlendAdd = bw.ReadBoolean();
            tfNoTexture = bw.ReadBoolean();
            tfDepthOffset = bw.ReadSingle();
            tfNoWriteZ = bw.ReadBoolean();
            AlphaTestVal = bw.ReadSingle();             //  Alpha cut off value
            tfTranspBorder = bw.ReadBoolean();
            tfTestA = bw.ReadBoolean();                 //  Test alpha bool           
            tfTestZ = bw.ReadBoolean();
            tname = bw.ReadString();                    //  Texture name
            Colour[0] = bw.ReadSingle();                //  Diffuse colour
            Colour[1] = bw.ReadSingle();                //  Diffuse colour
            Colour[2] = bw.ReadSingle();                //  Diffuse colour
            Colour[3] = bw.ReadSingle();                //  Diffuse colour

            deferred_rs = new RasterizerState();
            deferred_rs.FillMode = FillMode.Solid;
            deferred_rs.CullMode = CullMode.CullClockwiseFace;
            if (tfDoubleSided)
                deferred_rs.CullMode = CullMode.None;
            deferred_rs.DepthBias = -(tfDepthOffset * 0.001f);
            deferred_rs.DepthClipEnable = true;

        }

        public override void Apply(Effect fx)
        {
            
            if (Glass)
            {

            }
            else
            {
               
                fx.Parameters["MaterialColour"].SetValue(new Vector4(Colour[0], Colour[1], Colour[2], Colour[3]));
                fx.Parameters["DiffuseIntensity"].SetValue(Diffuse);
                fx.Parameters["SpecularIntensity"].SetValue(Specular);
                fx.Parameters["Shininess"].SetValue(SpecularPow);
                if (Renderer.IsForward())
                {
                    fx.Parameters["AmbientIntensity"].SetValue(Ambient);
                    fx.Parameters["SpecularColor"].SetValue(Color.White.ToVector4());
                    fx.Parameters["Damage"].SetValue(0.0f);
                    fx.Parameters["Blend"].SetValue(tfBlend);
                }
                
                fx.Parameters["AlphaCut"].SetValue(AlphaTestVal);
                fx.Parameters["TestAlpha"].SetValue(tfTestA);               
                fx.Parameters["ModelTexture"].SetValue(AssetManager.Texture(TextureGUID));
              
               
            }
           
        }

        
    }
}
