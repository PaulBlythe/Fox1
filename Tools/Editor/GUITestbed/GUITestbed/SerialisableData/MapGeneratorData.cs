using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.SerialisableData
{
    public class MapGeneratorData : SerialisableObject
    {
        public float  StartLatitude;            // degrees 
        public float StartLongitude;            // degrees
        public float Size;                      // pixel size in metres
        public float Date;                      // year
        public int Width;                       // pixels
        public int Height;                      // pixels
        public bool Airfields;
        public bool Roads;
        public bool Rivers;
        public bool Cities;
        public bool Countries;

        public void Write(string path)
        {
            FileStream writeStream = new FileStream(path, FileMode.Create);
            BinaryWriter wb = new BinaryWriter(writeStream);
            {
                wb.Write(StartLatitude);
                wb.Write(StartLongitude);
                wb.Write(Size);
                wb.Write(Date);
                wb.Write(Width);
                wb.Write(Height);
                wb.Write(Width);
                wb.Write(Airfields);
                wb.Write(Roads);
                wb.Write(Rivers);
                wb.Write(Cities);
                wb.Write(Countries);
            }
            wb.Close();
            writeStream.Close();
        }

        public void Load(string path)
        {
            FileStream writeStream = new FileStream(path, FileMode.Open);
            BinaryReader wb = new BinaryReader(writeStream);
            {
                StartLatitude = wb.ReadSingle();
                StartLongitude = wb.ReadSingle();
                Size = wb.ReadSingle();
                Date = wb.ReadSingle();
                Width = wb.ReadInt32();
                Height = wb.ReadInt32();
                Width = wb.ReadInt32();
                Airfields = wb.ReadBoolean();
                Roads = wb.ReadBoolean();
                Rivers = wb.ReadBoolean();
                Cities = wb.ReadBoolean();
                Countries = wb.ReadBoolean();
            }
            wb.Close();
            writeStream.Close();
        }
    }
}
