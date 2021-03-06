﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.FG
{
    public class Runway
    {
        public double Width;    // in metres
        public int Type;
        public int ShoulderType;
        public double Smoothness;
        public bool RunwayCentreLights;
        public int RunwayEdgeLighting;
        public bool AutogenerateDistanceRemaining;

        public RunwayEnd[] Ends = new RunwayEnd[2];

        public Runway(string definition)
        {
            definition = Parser.Skip(definition);
            definition = Parser.GetDouble(definition, out Width);
            definition = Parser.GetInt(definition, out Type);
            definition = Parser.GetInt(definition, out ShoulderType);
            definition = Parser.GetDouble(definition, out Smoothness);
            definition = Parser.GetBool(definition, out RunwayCentreLights);
            definition = Parser.GetInt(definition, out RunwayEdgeLighting);
            definition = Parser.GetBool(definition, out AutogenerateDistanceRemaining);

            Ends[0] = new RunwayEnd();
            definition = Ends[0].Set(definition);

            Ends[1] = new RunwayEnd();
            definition = Ends[1].Set(definition);

        }

        public string GetRunwayType()
        {
            switch (Type)
            {
                case 1:
                    return "Asphalt";
                case 2:
                    return "Concrete";
                case 3:
                    return "Grass";
                case 4:
                    return "Dirt";
                case 5:
                    return "Gravel";
                case 12:
                    return "Dry lakebed";
                case 13:
                    return "Water";
                case 14:
                    return "Snow";

            }
            return "Transparent";
        }
    }
}
