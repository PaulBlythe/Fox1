using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Xml;

using GUITestbed.GUI.Dialogs;
using GUITestbed.GUI;
using GUITestbed.SerialisableData;
using GUITestbed.Rendering;
using GUITestbed.Rendering._3D;
using GUITestbed.World;
using GUITestbed.DataHandlers.Mapping.Graphs;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class Airport
    {
        public String Country;
        public String State;
        public String City;
        public String Name;
        public Double Latitude;
        public Double Longitude;
        public float Altitude;
        public Double MagneticVariation;
        public float TrafficScalar;
        public float AirportTestRadius;
        public String Ident = "EGYC";
        public Location TowerLocation;
        public List<Runway> Runways = new List<Runway>();
        public List<Helipad> Helipads = new List<Helipad>();
        public List<RunwayPavement> Aprons = new List<RunwayPavement>();
        public RunwayTaxiwayPoint[] TaxiwayPoints;
        public List<RunwayTaxiwayPath> TaxiwayPaths = new List<RunwayTaxiwayPath>();
        public List<RunwayBoundaryFence> Fences = new List<RunwayBoundaryFence>();
        public ApronEdgeLight apronlights;

        TaxiwayGraphCreator tgc;

        List<RunwayObject> AirportObjects = new List<RunwayObject>();

        FenceObject fenceObject;

        public Airport(String f)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(f);

            XmlNode node = doc.DocumentElement.SelectSingleNode("Airport");
            if (node == null)
            {
                GuiManager.Instance.Add(new MessageBox("Load airport error", "No Airport node"));
                return;
            }

            Country = node.Attributes["country"]?.InnerText;
            State = node.Attributes["state"]?.InnerText;
            City = node.Attributes["city"]?.InnerText;
            Name = node.Attributes["name"]?.InnerText;
            Ident = node.Attributes["ident"]?.InnerText;

            Latitude = Double.Parse(node.Attributes["lat"]?.InnerText);
            Longitude = Double.Parse(node.Attributes["lon"]?.InnerText);
            MagneticVariation = Double.Parse(node.Attributes["magvar"]?.InnerText);

            Altitude = Cartography.ReadFloat(node.Attributes["alt"]?.InnerText);
            TrafficScalar = Cartography.ReadFloat(node.Attributes["trafficScalar"]?.InnerText);
            AirportTestRadius = Cartography.ReadFloat(node.Attributes["airportTestRadius"]?.InnerText);

            XmlNode tnode = node.SelectSingleNode("Tower");
            TowerLocation = new Location(tnode);

            XmlNodeList runways = node.SelectNodes("Runway");
            foreach (XmlNode n in runways)
            {
                Runways.Add(new Runway(n));
            }

            XmlNodeList helipads = node.SelectNodes("Helipad");
            foreach (XmlNode n in helipads)
            {
                Helipads.Add(new Helipad(n));
            }
            XmlNode xAprons = node.SelectSingleNode("Aprons");

            XmlNodeList aprons = xAprons.SelectNodes("Apron");
            foreach (XmlNode n in aprons)
            {
                Aprons.Add(new RunwayPavement(n));
            }

            XmlNodeList tps = node.SelectNodes("TaxiwayPoint");
            TaxiwayPoints = new RunwayTaxiwayPoint[tps.Count + 5];
            foreach (XmlNode n in tps)
            {
                RunwayTaxiwayPoint tp = new RunwayTaxiwayPoint(n);
                TaxiwayPoints[tp.Index] = tp;
            }

            XmlNodeList paths = node.SelectNodes("TaxiwayPath");
            foreach (XmlNode n in paths)
            {
                TaxiwayPaths.Add(new RunwayTaxiwayPath(n));
            }

            XmlNodeList fences = node.SelectNodes("BoundaryFence");
            foreach (XmlNode n in fences)
            {
                Fences.Add(new RunwayBoundaryFence(n));
            }
            tgc = new TaxiwayGraphCreator(TaxiwayPaths, TaxiwayPoints, Latitude, Longitude);

            tnode = node.SelectSingleNode("ApronEdgeLights");
            apronlights = new ApronEdgeLight(tnode);

            #region Build
            /// ******************************************************************************************* ///
            /// ***                                                                                     *** ///
            /// ***                                                                                     *** ///
            /// ******************************************************************************************* ///
            Vector2D centre = new Vector2D(Longitude, Latitude);
            foreach (Runway r in Runways)
            {
                r.Build(centre);
            }

            foreach (RunwayPavement r in Aprons)
            {
                AirportObjects.Add(r.Build(centre, (float)(0.1f + Altitude * Constants.FEET_TO_MTR)));
            }

            foreach (Helipad r in Helipads)
            {
                AirportObjects.Add(r.Build(centre));
            }

            fenceObject = new FenceObject();
            fenceObject.Load();

            foreach (RunwayBoundaryFence r in Fences)
            {
                List<Vector3> tempPoints = new List<Vector3>();

                for (int i=0; i<r.Points.Count; i++)
                {
                    Vector2D c = Cartography.ConvertToLocalised(Latitude, Longitude, r.Points[i].Latitude, r.Points[i].Longitude);
                    Vector3 cv = new Vector3((float)c.X, (float)(Altitude * Constants.FEET_TO_MTR), (float)c.Y);
                    tempPoints.Add(cv);
                    fenceObject.AddPost(cv);
                }

                for (int i=0; i< r.Points.Count-1; i++)
                {
                    fenceObject.AddFence(tempPoints[i], tempPoints[i + 1]);
                }
            }
            fenceObject.Finalise();

            #endregion
        }

        public void UpdateLighting(SkyDomeSystem sky)
        {
            Vector4 amb = sky.LightIntensity * sky.SunColor * 0.4f;
            Vector4 sc = sky.SunColor;
            Vector4 sd = sky.GetDirection();

            foreach (Runway r in Runways)
            {
                r.UpdateLighting(amb, sc, sd);
            }

            foreach (RunwayObject r in AirportObjects)
            {
                r.UpdateLighting(amb, sc, sd);
            }
            if (fenceObject!=null)
                fenceObject.UpdateLighting(amb, sc, sd);
        }

        public void Draw(Camera c)
        {
            foreach (Runway r in Runways)
            {
                r.Draw(c);
            }

            foreach (RunwayObject r in AirportObjects)
            {
                r.Draw(c);
            }
            if (fenceObject!= null)
                fenceObject.Draw(c);

           //foreach (TaxiwayChain tc in tgc.Chains)
           //{
           //     for (int i = 0; i < tc.Path.Count; i++)
           //     {
           //         RunwayTaxiwayPoint v1 = TaxiwayPoints[tc.Path[i].Start];
           //         RunwayTaxiwayPoint v2 = TaxiwayPoints[tc.Path[i].End];
           //
           //         Vector2D tv1 = Cartography.ConvertToLocalised(Latitude, Longitude, v1.Latitude, v1.Longitude);
           //         Vector2D tv2 = Cartography.ConvertToLocalised(Latitude, Longitude, v2.Latitude, v2.Longitude);
           //
           //         Vector3 vs = new Vector3((float)tv1.X, Altitude, (float)tv1.Y);
           //         Vector3 ve = new Vector3((float)tv2.X, Altitude, (float)tv2.Y);
           //
           //         DebugLineDraw.DrawLine(vs, ve, Color.Blue);
           //     }
           // }

        }

       

    }
}
