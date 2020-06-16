using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using GuruEngine.ECS;

namespace GuruEngine.Assets
{
    public class MeshMaterialLibrary
    {
        public MeshPartMaterial[] Materials;

        public MeshMaterialLibrary(String directory, BinaryReader bw)
        {
            int nmats = bw.ReadInt32();
            Materials = new MeshPartMaterial[nmats];
            for (int i=0; i< nmats; i++)
            {
                Materials[i] = new MeshPartMaterial(bw);

                String texturepath = Path.Combine(directory, Materials[i].tname);
                if (Materials[i].tname.StartsWith("$"))
                {
                    texturepath = FilePaths.HIMTexturePath + Materials[i].tname.Substring(1);
                }
                texturepath += ".png";
                AssetManager.AddTextureToQue(texturepath);
                Materials[i].TextureGUID = texturepath.GetHashCode();
            }


        }
        public MeshMaterialLibrary(String directory, BinaryReader bw, GameObject parent)
        {
            int nmats = bw.ReadInt32();
            Materials = new MeshPartMaterial[nmats];
            for (int i = 0; i < nmats; i++)
            {
                Materials[i] = new MeshPartMaterial(bw);
                String texturepath = Path.Combine(directory, Materials[i].tname);
                if (Materials[i].tname.StartsWith("$"))
                {
                    texturepath = FilePaths.HIMTexturePath + @"\" + Materials[i].tname.Substring(1);
                }
                if (parent.TextureOverrides.ContainsKey(Materials[i].Name))
                {
                    texturepath = parent.TextureOverrides[Materials[i].Name];
                }

                
                texturepath += ".png";
                AssetManager.AddTextureToQue(texturepath);
                Materials[i].TextureGUID = texturepath.GetHashCode();
            }


        }
    }
}
