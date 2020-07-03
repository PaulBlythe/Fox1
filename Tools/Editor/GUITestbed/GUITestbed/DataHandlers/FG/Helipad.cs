using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.DataHandlers.FG
{
    public class Helipad
    {
        public double Latitude;
        public double Longitude;
        public String designator;
        public double Heading;          // Degrees
        public double Length;           // Metres
        public double Width;            // Metres
        public int Surface;
        public int Markings;
        public int ShoulderType;
        public double Roughness;
        public int EdgeLighting;


        public Helipad(String definition)
        {
            definition = Parser.Skip(definition);
            definition = Parser.GetString(definition, out designator);
            definition = Parser.GetDouble(definition, out Latitude);
            definition = Parser.GetDouble(definition, out Longitude);
            definition = Parser.GetDouble(definition, out Heading);
            definition = Parser.GetDouble(definition, out Length);
            definition = Parser.GetDouble(definition, out Width);
            definition = Parser.GetInt(definition, out Surface);
            definition = Parser.GetInt(definition, out Markings);
            definition = Parser.GetInt(definition, out ShoulderType);
            definition = Parser.GetDouble(definition, out Roughness);
            definition = Parser.GetInt(definition, out EdgeLighting);
        }

        //public void Build(double clat, double clon, double elevation)
        //{
        //    Texture2D tex;
        //    switch (Surface)
        //    {
        //        case 1:
        //            tex = Game1.Instance.RunwayTextures["asphalt"];
        //            break;
        //        case 2:
        //            tex = Game1.Instance.RunwayTextures["concrete_h"];
        //            break;
        //        case 3:
        //            tex = Game1.Instance.RunwayTextures["grass"];
        //            break;
        //        case 4:
        //            tex = Game1.Instance.RunwayTextures["dirt"];
        //            break;
        //        case 5:
        //            tex = Game1.Instance.RunwayTextures["gravel"];
        //            break;
        //        case 12:
        //            tex = Game1.Instance.RunwayTextures["lake"];
        //            break;
        //        case 14:
        //            tex = Game1.Instance.RunwayTextures["snow"];
        //            break;
        //
        //        default:
        //            tex = Game1.Instance.White;
        //            break;
        //    }
        //    draws.Add(GeometryHelper.CreateHelipad(clat, clon, Latitude, Longitude, Heading, Width, Length, elevation, tex));
        //}
        //
        
    }
}
