using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.DataHandlers.FG
{
    public class Airport
    {
        /// <summary>
        ///   1     Land airport
        ///   16    Seaplane base
        ///   17    Heliport
        /// </summary>
        public int Type;

        /// <summary>
        /// Elevation of airport in feet above mean sea level
        /// </summary>
        public double Elevation;

        public bool HasControlTower;

        /// <summary>
        /// ICAO code for the airport
        /// </summary>
        public string ICAO;

        public string Name;

        public List<RadioFrequency> RadioStations = new List<RadioFrequency>();
        public List<LightingObject> Lighting = new List<LightingObject>();
        public List<Sign> Signs = new List<Sign>();
        public List<Windsock> Windsocks = new List<Windsock>();
        public List<Runway> Runways = new List<Runway>();
        public List<LinearFeature> LinearFeatures = new List<LinearFeature>();
        public List<Pavement> Pavements = new List<Pavement>();
        public List<Boundary> Boundaries = new List<Boundary>();
        public List<Helipad> Helipads = new List<Helipad>();



        public ViewPoint viewPoint = null;

        public bool built = false;

        /// <summary>
        /// Constructs an airport structure from a line of text
        /// </summary>
        /// <param name="definition"></param>
        public Airport(String definition)
        {

            definition = Parser.GetInt(definition, out Type);
            definition = Parser.GetDouble(definition, out Elevation);
            definition = Parser.GetBool(definition, out HasControlTower);
            definition = Parser.Skip(definition);
            definition = Parser.GetString(definition, out ICAO);
            definition = definition.Substring(1);
            Name = definition;

        }

        public void GetBounds(out double west, out double east, out double north, out double south)
        {
            west = 180;
            east = -180;
            south = -90;
            north = 90;


            {
                foreach (Boundary b in Boundaries)
                {
                    foreach (LineNode ln in b.Nodes)
                    {
                        if (ln.Latitude < north)
                            north = ln.Latitude;
                        if (ln.Latitude > south)
                            south = ln.Latitude;
                        if (ln.Longitude > east)
                            east = ln.Longitude;
                        if (ln.Longitude < west)
                            west = ln.Longitude;
                    }
                }
                // have to do the long one
                foreach (LinearFeature lf in LinearFeatures)
                {
                    foreach (LineNode ln in lf.Nodes)
                    {
                        if (ln.Latitude < north)
                            north = ln.Latitude;
                        if (ln.Latitude > south)
                            south = ln.Latitude;
                        if (ln.Longitude > east)
                            east = ln.Longitude;
                        if (ln.Longitude < west)
                            west = ln.Longitude;
                    }
                }
                foreach (Pavement pf in Pavements)
                {
                    foreach (LineNode ln in pf.Nodes)
                    {
                        if (ln.Latitude < north)
                            north = ln.Latitude;
                        if (ln.Latitude > south)
                            south = ln.Latitude;
                        if (ln.Longitude > east)
                            east = ln.Longitude;
                        if (ln.Longitude < west)
                            west = ln.Longitude;
                    }
                }
            }
        }

        //public void Build(MapZoomer zoom)
        //{
        //
        //    if (viewPoint == null)
        //    {
        //        double w, e, s, n;
        //        GetBounds(out w, out e, out n, out s);
        //        viewPoint = new ViewPoint();
        //        viewPoint.Latitude = (n + s) / 2;
        //        viewPoint.Longitude = (w + e) / 2;
        //        viewPoint.Height = 50;
        //    }
        //
        //    foreach (Runway r in Runways)
        //    {
        //        DrawableObject leftShoulder;
        //        DrawableObject rightShoulder;
        //        DrawableObject rway = GeometryHelper.BuildRunway(viewPoint.Latitude,
        //                                                         viewPoint.Longitude,
        //                                                         r.Ends[0].Latitude,
        //                                                         r.Ends[0].Longitude,
        //                                                         r.Ends[1].Latitude,
        //                                                         r.Ends[1].Longitude,
        //                                                         r.Type,
        //                                                         r.Width,
        //                                                         Elevation * Constants.FEET_TO_MTR,
        //                                                         r.ShoulderType,
        //                                                         out leftShoulder,
        //                                                         out rightShoulder);
        //
        //
        //        objects.Add(rway);
        //        if (leftShoulder != null)
        //        {
        //            objects.Add(leftShoulder);
        //        }
        //        if (rightShoulder != null)
        //        {
        //            objects.Add(rightShoulder);
        //        }
        //
        //        #region Runway markings
        //        switch (r.Ends[0].RunwayMarkings)
        //        {
        //            case 0:             // no markings
        //                break;
        //
        //            case 3:             // precision
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 3, 500)
        //                    );
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 2, 1500)
        //                    );
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 2, 2000)
        //                    );
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 1, 2500)
        //                    );
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 1, 3000)
        //                    );
        //                goto case 2;
        //
        //            case 2:             // none precision
        //            case 1:             // visual markings
        //                objects.Add(GeometryHelper.CreateRunwayThresholdMarkings(viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR));
        //
        //                objects.Add(GeometryHelper.CreateRunwayAimingPoint(viewPoint.Latitude,
        //                                                 viewPoint.Longitude,
        //                                                 r.Ends[0].Latitude,
        //                                                 r.Ends[0].Longitude,
        //                                                 r.Ends[1].Latitude,
        //                                                 r.Ends[1].Longitude,
        //                                                 r.Width,
        //                                                 0.01f + Elevation * Constants.FEET_TO_MTR));
        //
        //                objects.Add(GeometryHelper.CreateRunwayCentreLine(viewPoint.Latitude,
        //                                          viewPoint.Longitude,
        //                                          r.Ends[0].Latitude,
        //                                          r.Ends[0].Longitude,
        //                                          r.Ends[1].Latitude,
        //                                          r.Ends[1].Longitude,
        //                                          0.01f + Elevation * Constants.FEET_TO_MTR));
        //                break;
        //
        //        }
        //        switch (r.Ends[1].RunwayMarkings)
        //        {
        //            case 0:             // no markings
        //                break;
        //            case 3:             // precision
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 3, 500)
        //                    );
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 2, 1500)
        //                    );
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 2, 2000)
        //                    );
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 1, 2500)
        //                    );
        //                objects.Add(GeometryHelper.CreateRunwayTouchdownZone(
        //                                                                 viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR, 1, 3000)
        //                    );
        //                goto case 2;
        //            case 2:
        //            case 1:
        //                objects.Add(GeometryHelper.CreateRunwayThresholdMarkings(viewPoint.Latitude,
        //                                                                 viewPoint.Longitude,
        //                                                                 r.Ends[1].Latitude,
        //                                                                 r.Ends[1].Longitude,
        //                                                                 r.Ends[0].Latitude,
        //                                                                 r.Ends[0].Longitude,
        //                                                                 r.Width,
        //                                                                 0.01f + Elevation * Constants.FEET_TO_MTR));
        //
        //                objects.Add(GeometryHelper.CreateRunwayAimingPoint(viewPoint.Latitude,
        //                                 viewPoint.Longitude,
        //                                 r.Ends[1].Latitude,
        //                                 r.Ends[1].Longitude,
        //                                 r.Ends[0].Latitude,
        //                                 r.Ends[0].Longitude,
        //                                 r.Width,
        //                                 0.01f + Elevation * Constants.FEET_TO_MTR));
        //                break;
        //        }
        //        #endregion
        //
        //
        //    }
        //
        //    foreach (Helipad h in Helipads)
        //    {
        //        h.Build(viewPoint.Latitude, viewPoint.Longitude, (float)(Elevation * Constants.FEET_TO_MTR));
        //    }
        //
        //    foreach (Boundary b in Boundaries)
        //    {
        //        b.ConvertToChain(viewPoint.Latitude, viewPoint.Longitude, (float)(Elevation * Constants.FEET_TO_MTR));
        //    }
        //
        //    foreach (LinearFeature l in LinearFeatures)
        //    {
        //        l.ConvertToChain(viewPoint.Latitude, viewPoint.Longitude, (float)(Elevation * Constants.FEET_TO_MTR));
        //    }
        //    foreach (Pavement l in Pavements)
        //    {
        //        l.ConvertToChain(viewPoint.Latitude, viewPoint.Longitude, -0.01f + (float)(Elevation * Constants.FEET_TO_MTR));
        //    }
        //    foreach (LightingObject lo in Lighting)
        //    {
        //        lo.Build(this);
        //    }
        //
        //    #region Runway objects
        //    RunwayObjects = new STG(viewPoint.Latitude, viewPoint.Longitude, viewPoint.Height, 0.5);
        //    foreach (STGLine l in RunwayObjects.Lines)
        //    {
        //        Game1.Instance.objectDatabase.AddSTGObject(Path.Combine(Game1.MODEL_ROOT, l.Model), l.Model);
        //    }
        //    #endregion
        //
        //    built = true;
        //}

    }
}