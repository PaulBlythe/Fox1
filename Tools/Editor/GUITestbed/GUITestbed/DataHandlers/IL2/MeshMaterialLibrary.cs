using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using GUITestbed.Tools;

namespace GUITestbed.DataHandlers.IL2
{
    public class MeshMaterialLibrary
    {
        public Material[] Materials;

        public MeshMaterialLibrary(String directory, BinaryReader bw)
        {
            int nmats = bw.ReadInt32();
            Materials = new Material[nmats];
            for (int i = 0; i < nmats; i++)
            {
                Materials[i] = new Material(bw);

                String texturepath = Path.Combine(directory, Materials[i].tname);
                if (Materials[i].tname.StartsWith("$"))
                {
                    texturepath = FilePaths.HIMTexturePath + Materials[i].tname.Substring(1);
                }
                texturepath += ".png";

                Materials[i].Texture = TextureLoader.Load(texturepath);
            }
        }

        public void Save(String filename)
        {
            FileStream writeStream = new FileStream(filename, FileMode.Create);
            BinaryWriter writeBinary = new BinaryWriter(writeStream);
            writeBinary.Write(Materials.Length);
            for (int i = 0; i < Materials.Length; i++)
            {
                Materials[i].SaveToFox1(writeBinary);
            }
            writeBinary.Close();
        }
    }
}
