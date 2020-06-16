using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine
{
    public  enum AssetRecordType
    {
        Texture,
        HIM,
        StaticMesh,
        MeshPart,
        MeshPartMaterialLibrary,
        Shader,
        SDFFont,
        MSDFFont,
        Particlesettings,
        SoundEffect,
        TYPES
    }

    public class AssetRecord
    {
        public int ReferenceCount = 0;
        public String Path = "";
        public object Asset;
        public AssetRecordType Type;
    }
}
