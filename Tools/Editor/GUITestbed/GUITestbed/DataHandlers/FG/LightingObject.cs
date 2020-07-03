using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using GUITestbed.World;

namespace GUITestbed.DataHandlers.FG
{
    public class LightingObject
    {
        public double Latitude;
        public double Longitude;
        public int Type;
        public double Heading;
        public double GlideSlopeAngle;
        public String Number;
        public String Description;

        public LightingObject(string definition)
        {
            definition = Parser.Skip(definition);
            definition = Parser.GetDouble(definition, out Latitude);
            definition = Parser.GetDouble(definition, out Longitude);
            definition = Parser.GetInt(definition, out Type);
            definition = Parser.GetDouble(definition, out Heading);
            definition = Parser.GetDouble(definition, out GlideSlopeAngle);
            definition = Parser.GetString(definition, out Number);
            if (definition.Length > 1)
            {
                definition = definition.Substring(1);
            }
            Description = definition;

        }

        public string GetLightType()
        {
            switch (Type)
            {
                case 1:
                    return "ALSF I";
                case 2:
                    return "ALSF II";
                case 3:
                    return "Calvert";
                case 4:
                    return "Calvert Cat II";
                case 5:
                    return "SSALR";
                case 6:
                    return "SSALF";
                case 7:
                    return "SALS";
                case 8:
                    return "MALSR";
                case 9:
                    return "MALSF";
                case 10:
                    return "MALS";
                case 11:
                    return "ODALS";
                case 12:
                    return "RAIL";
            }
            return "None";
        }

        private RunwayEnd GetRunwayEnd(Airport parent, out Vector2D direction, out double width)
        {
            RunwayEnd result = null;
            double distance = double.MaxValue;
            Runway host = null;
            int index = 0;

            foreach (Runway r in parent.Runways)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (!r.Ends[i].AttachedLightObject)
                    {
                        double dl = r.Ends[i].Latitude - Latitude;
                        double dr = r.Ends[i].Longitude - Longitude;
                        double dist = (dl * dl) + (dr * dr);
                        if (dist < distance)
                        {
                            dist = distance;
                            result = r.Ends[i];
                            host = r;
                            index = i;
                        }
                    }
                }
            }
            if (result == null)
            {
                throw new Exception("Cannot match light object to runway end");
            }
            result.AttachedLightObject = true;

            int index2 = index ^ 1;
            Vector2D e1 = Cartography.ConvertToLocalised(parent.viewPoint.Latitude, parent.viewPoint.Longitude, host.Ends[index].Latitude, host.Ends[index].Longitude);
            Vector2D e2 = Cartography.ConvertToLocalised(parent.viewPoint.Latitude, parent.viewPoint.Longitude, host.Ends[index2].Latitude, host.Ends[index2].Longitude);
            direction = e1 - e2;
            direction.Normalise();
            width = host.Width;

            return result;
        }

    }
}