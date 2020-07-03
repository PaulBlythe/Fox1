using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.FG
{
    public class LineNode
    {
        public enum NodeType
        {
            Plain,
            Bezier,
            Close,
            CloseBezier,
            End,
            EndBezier
        }

        public NodeType Type;
        public int LineType;
        public int LightingType = 0;
        public double Latitude;
        public double Longitude;
        public double ControlLatitude;
        public double ControlLongitude;

        public Vector2D pt = new Vector2D(0, 0);
        public Vector2D ctrl = new Vector2D(0, 0);


        public LineNode(String definition)
        {

            int tcode;
            definition = Parser.GetInt(definition, out tcode);
            switch (tcode)
            {
                case 111:
                    Type = NodeType.Plain;
                    definition = Parser.GetDouble(definition, out Latitude);
                    definition = Parser.GetDouble(definition, out Longitude);
                    definition = Parser.GetInt(definition, out LineType);
                    if (definition.Length > 1)
                    {
                        Parser.GetInt(definition, out LightingType);
                    }
                    break;

                case 112:
                    Type = NodeType.Bezier;
                    goto case 114;
                case 114:
                    if (tcode == 114) Type = NodeType.CloseBezier;
                    definition = Parser.GetDouble(definition, out Latitude);
                    definition = Parser.GetDouble(definition, out Longitude);
                    definition = Parser.GetDouble(definition, out ControlLatitude);
                    definition = Parser.GetDouble(definition, out ControlLongitude);
                    definition = Parser.GetInt(definition, out LineType);
                    if (definition.Length > 1)
                    {
                        Parser.GetInt(definition, out LightingType);
                    }
                    break;

                case 113:
                    Type = NodeType.Close;
                    definition = Parser.GetDouble(definition, out Latitude);
                    definition = Parser.GetDouble(definition, out Longitude);
                    definition = Parser.GetInt(definition, out LineType);
                    if (definition.Length > 1)
                    {
                        Parser.GetInt(definition, out LightingType);
                    }
                    break;

                case 115:
                    Type = NodeType.End;
                    definition = Parser.GetDouble(definition, out Latitude);
                    definition = Parser.GetDouble(definition, out Longitude);
                    break;

                case 116:
                    Type = NodeType.EndBezier;
                    definition = Parser.GetDouble(definition, out Latitude);
                    definition = Parser.GetDouble(definition, out Longitude);
                    definition = Parser.GetDouble(definition, out ControlLatitude);
                    definition = Parser.GetDouble(definition, out ControlLongitude);
                    break;

            }
            pt = new Vector2D(Longitude, Latitude);
            if (IsCurved())
                ctrl = new Vector2D(ControlLongitude, ControlLatitude);
        }

        public bool IsCurved()
        {
            switch (Type)
            {
                case NodeType.Bezier:
                case NodeType.CloseBezier:
                case NodeType.EndBezier:
                    return true;
            }
            return false;
        }

        public bool IsEnd()
        {
            switch (Type)
            {
                case NodeType.End:
                case NodeType.EndBezier:
                    return true;
            }
            return false;
        }
        public bool IsClose()
        {
            switch (Type)
            {
                case NodeType.Close:
                case NodeType.CloseBezier:
                    return true;
            }
            return false;
        }
    }
}
