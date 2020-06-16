using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GUITestbed.Rendering._3D;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class Runway
    {
        public double Latitude;
        public double Longitude;
        public float Altitude;
        public String Surface;
        public double Heading;
        public float Length;
        public float Width;
        public String Number;
        public String Designator;
        public float PatternAltitude;
        public bool PrimaryTakeoff;
        public bool PrimaryLanding;
        public String PrimaryPattern;
        public bool SecondaryTakeoff;
        public bool SecondaryLanding;
        public String SecondaryPattern;

        public RunwayLights Lights;
        public RunwayMarkings Markings;
        public List<RunwayBlastPad> BlastPads = new List<RunwayBlastPad>();
        public List<RunwayApproachLights> runwayApproachLights = new List<RunwayApproachLights>();
        public List<RunwayVasi> Vasis = new List<RunwayVasi>();
        public List<RunwayILS> ILS = new List<RunwayILS>();

        public List<RunwayObject> Objects = new List<RunwayObject>();

        public Runway(XmlNode node)
        {
            Surface = node.Attributes["surface"]?.InnerText;
            Number = node.Attributes["number"]?.InnerText;
            Designator = node.Attributes["designator"]?.InnerText;
            PrimaryPattern = node.Attributes["primaryPattern"]?.InnerText;
            SecondaryPattern = node.Attributes["secondaryPattern"]?.InnerText;

            Altitude = Cartography.ReadFloat(node.Attributes["alt"]?.InnerText);
            Length = Cartography.ReadFloat(node.Attributes["length"]?.InnerText);
            Width = Cartography.ReadFloat(node.Attributes["width"]?.InnerText);
            PatternAltitude = Cartography.ReadFloat(node.Attributes["patternAltitude"]?.InnerText);

            Latitude = double.Parse(node.Attributes["lat"]?.InnerText);
            Longitude = double.Parse(node.Attributes["lon"]?.InnerText);
            Heading = double.Parse(node.Attributes["heading"]?.InnerText);

            PrimaryTakeoff = ((node.Attributes["primaryTakeoff"]?.InnerText) == "YES");
            PrimaryLanding = ((node.Attributes["primaryLanding"]?.InnerText) == "YES");
            SecondaryTakeoff = ((node.Attributes["secondaryTakeoff"]?.InnerText) == "YES");
            SecondaryLanding = ((node.Attributes["secondaryLanding"]?.InnerText) == "YES");

            XmlNode rm = node.SelectSingleNode("Markings");
            Markings = new RunwayMarkings(rm);

            rm = node.SelectSingleNode("Lights");
            Lights = new RunwayLights(rm);

            XmlNodeList rms = node.SelectNodes("BlastPad");
            foreach (XmlNode n in rms)
            {
                BlastPads.Add(new RunwayBlastPad(n));
            }

            rms = node.SelectNodes("ApproachLights");
            foreach (XmlNode n in rms)
            {
                runwayApproachLights.Add(new RunwayApproachLights(n));
            }

            rms = node.SelectNodes("Vasi");
            foreach (XmlNode n in rms)
            {
                Vasis.Add(new RunwayVasi(n));
            }

            rms = node.SelectNodes("Ils");
            foreach (XmlNode n in rms)
            {
                ILS.Add(new RunwayILS(n));
            }

        }


        public void Build(Vector2D airport_centre)
        {
            Vector2D centre = Cartography.ConvertToLocalised(airport_centre.Y, airport_centre.X, Latitude, Longitude);
            Vector2D direction = new Vector2D(0, 1);
            direction = Vector2D.Rotate(direction, new Vector2D(0, 0), Constants.DEG_TO_RAD * Heading);

            double ll = Length * Constants.FEET_TO_MTR;
            double aa = Altitude * Constants.FEET_TO_MTR;

            foreach (RunwayBlastPad b in BlastPads)
            {
                Objects.Add(b.Build(centre, direction, ll, (float)aa));
            }

            Objects.Add(BuildRunway(centre, direction, ll, aa));

            foreach(RunwayApproachLights ral in runwayApproachLights)
            {
                if (ral.End == "PRIMARY")
                {
                    Vector2D right = new Vector2D(direction.Y, -direction.X);
                    Vector2D start = centre + direction * (float)(ll * 0.5);
                    start += right * (float)(Width * 0.7f * Constants.FEET_TO_MTR);
                    float step = (float)(Width * Constants.FEET_TO_MTR * 0.1f);
                    for (int i=0; i<15; i++)
                    {
                        Vector2D sp = start - (right * (i * step));
                        Vector3 pos = new Vector3((float)sp.X, (float)aa, (float)sp.Y);
                        LightManager.AddPointLight(pos, 0.05f, Color.Green);
                    }
                    switch (ral.System)
                    {
                        case "CALVERT":
                            {
                                // 9 lights along centre line
                                Vector2D p = centre + direction * (float)(ll * 0.5);
                                double sp = (300 * Constants.FEET_TO_MTR) / 8;
                                p += direction * sp;
                                for (int i=0; i<9; i++)
                                {
                                    Vector3 pos2 = new Vector3((float)p.X, (float)aa, (float)p.Y);
                                    LightManager.AddPointLight(pos2, 0.05f, Color.White);

                                    // first cross bar
                                    if (i == 4)
                                    {
                                        Vector2D ls = right * (float)(Width * 0.5 * Constants.FEET_TO_MTR);
                                        Vector2D ld = ls * 0.2;
                                        ls += p;
                                        for (int j=0; j<10; j++)
                                        {
                                            Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                            LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                            ls -= ld;
                                        }
                                    }
                                    p += direction * sp;
                                }
                                // second cross bar
                                {
                                    Vector2D ls = right * (float)(Width * 0.75 * Constants.FEET_TO_MTR);
                                    Vector2D ld = ls * 0.1;
                                    ls += p;
                                    for (int j = 0; j < 20; j++)
                                    {
                                        Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                        LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                        ls -= ld;
                                    }
                                }
                                p += direction * sp;
                                for (int i=0;i<4; i++)
                                {
                                    Vector2D p1 = p + right * 1.5;
                                    Vector3 pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p - right * 1.5;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);
                                    p += direction * sp;
                                }
                               
                                // third cross bar
                                {
                                    Vector2D ls = right * (float)(Width * 0.75 * Constants.FEET_TO_MTR);
                                    Vector2D ld = ls * 0.1;
                                    ls += p;
                                    for (int j = 0; j < 20; j++)
                                    {
                                        Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                        LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                        ls -= ld;
                                    }
                                }

                                p += direction * sp;
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2D p1 = p + right * 1.5;
                                    Vector3 pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p - right * 1.5;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);
                                    p += direction * sp;
                                }
                                // fourth cross bar
                                {
                                    Vector2D ls = right * (float)(Width * 0.8333 * Constants.FEET_TO_MTR);
                                    Vector2D ld = ls  / 12;
                                    ls += p;
                                    for (int j = 0; j < 24; j++)
                                    {
                                        Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                        LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                        ls -= ld;
                                    }
                                }
                                p += direction * sp;
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2D p1 = p + right * 3.5;
                                    Vector3 pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p - right * 3.5;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);
                                    p += direction * sp;
                                }
                                // last cross bar
                                {
                                    Vector2D ls = right * (float)(Width * Constants.FEET_TO_MTR);
                                    Vector2D ld = ls / 14;
                                    ls += p;
                                    for (int j = 0; j < 28; j++)
                                    {
                                        Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                        LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                        ls -= ld;
                                    }
                                }
                                p += direction * sp;
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2D p1 = p + right * 3.5;
                                    Vector3 pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p - right * 3.5;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);
                                    p += direction * sp;
                                }
                            }
                            break;
                        default:
                            throw new Exception("Unknown runway approach ligthing system " + ral.System);
                    }
                }
                if (ral.End == "SECONDARY")
                {
                    Vector2D right = new Vector2D(direction.Y, -direction.X);
                    Vector2D start = centre - direction * (float)(ll * 0.5);
                    start += right * (float)(Width * 0.7f * Constants.FEET_TO_MTR);
                    float step = (float)(Width * Constants.FEET_TO_MTR * 0.1f);
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2D sp = start - (right * (i * step));
                        Vector3 pos = new Vector3((float)sp.X, (float)aa, (float)sp.Y);
                        LightManager.AddPointLight(pos, 0.05f, Color.Green);
                    }

                    switch (ral.System)
                    {
                        case "CALVERT":
                            {
                                // 9 lights along centre line
                                Vector2D p = centre - direction * (float)(ll * 0.5);
                                double sp = (300 * Constants.FEET_TO_MTR) / 8;
                                p -= direction * sp;
                                for (int i = 0; i < 9; i++)
                                {
                                    Vector3 pos2 = new Vector3((float)p.X, (float)aa, (float)p.Y);
                                    LightManager.AddPointLight(pos2, 0.05f, Color.White);

                                    // first cross bar
                                    if (i == 4)
                                    {
                                        Vector2D ls = right * (float)(Width * 0.5 * Constants.FEET_TO_MTR);
                                        Vector2D ld = ls * 0.2;
                                        ls += p;
                                        for (int j = 0; j < 10; j++)
                                        {
                                            Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                            LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                            ls -= ld;
                                        }
                                    }
                                    p -= direction * sp;
                                }
                                // second cross bar
                                {
                                    Vector2D ls = right * (float)(Width * 0.75 * Constants.FEET_TO_MTR);
                                    Vector2D ld = ls * 0.1;
                                    ls += p;
                                    for (int j = 0; j < 20; j++)
                                    {
                                        Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                        LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                        ls -= ld;
                                    }
                                }
                                p -= direction * sp;
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2D p1 = p + right * 1.5;
                                    Vector3 pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p - right * 1.5;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);
                                    p -= direction * sp;
                                }
                                
                                // third cross bar
                                {
                                    Vector2D ls = right * (float)(Width * 0.75 * Constants.FEET_TO_MTR);
                                    Vector2D ld = ls * 0.1;
                                    ls += p;
                                    for (int j = 0; j < 20; j++)
                                    {
                                        Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                        LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                        ls -= ld;
                                    }
                                }

                                p -= direction * sp;
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2D p1 = p + right * 1.5;
                                    Vector3 pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p - right * 1.5;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);
                                    p -= direction * sp;
                                }
                                // fourth cross bar
                                {
                                    Vector2D ls = right * (float)(Width * 0.83333 * Constants.FEET_TO_MTR);
                                    Vector2D ld = ls /12;
                                    ls += p;
                                    for (int j = 0; j < 24; j++)
                                    {
                                        Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                        LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                        ls -= ld;
                                    }
                                }
                                p -= direction * sp;
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2D p1 = p + right * 3.5;
                                    Vector3 pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);
                                   

                                    p1 = p - right * 3.5;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);
                                    p -= direction * sp;
                                }

                                // last cross bar
                                {
                                    Vector2D ls = right * (float)(Width * Constants.FEET_TO_MTR);
                                    Vector2D ld = ls / 14;
                                    ls += p;
                                    for (int j = 0; j < 28; j++)
                                    {
                                        Vector3 pos3 = new Vector3((float)ls.X, (float)aa, (float)ls.Y);
                                        LightManager.AddPointLight(pos3, 0.05f, Color.White);
                                        ls -= ld;
                                    }
                                }

                                p -= direction * sp;
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2D p1 = p + right * 3.5;
                                    Vector3 pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);

                                    p1 = p;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);


                                    p1 = p - right * 3.5;
                                    pos2 = new Vector3((float)p1.X, (float)aa, (float)p1.Y);
                                    LightManager.AddPointLight(pos2, 0.04f, Color.White);
                                    p -= direction * sp;
                                }
                            }
                            break;
                        default:
                            throw new Exception("Unknown runway approach ligthing system " + ral.System);
                    }
                }
            }
        }

        public void UpdateLighting(Vector4 Ambient, Vector4 Sun, Vector4 sunpos)
        {
            foreach (RunwayObject ro in Objects)
            {
                ro.UpdateLighting(Ambient, Sun, sunpos);
            }
        }

        public void Draw(Camera c)
        {
            foreach (RunwayObject ro in Objects)
            {
                ro.Draw(c);
            }
        }

        RunwayObject BuildRunway(Vector2D centre, Vector2D direction, double length, double altitude)
        {
            RunwayObject result = new RunwayObject();
            List<VertexPositionNormalTexture> tempverts = new List<VertexPositionNormalTexture>();
            List<short> tempinds = new List<short>();

            switch (Surface)
            {
                case "ASPHALT":
                    result.texture = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/asphalt_tiles");
                    break;
                default:
                    throw new Exception("No tiles for runway surface " + Surface);
            }

            Vector2D right = new Vector2D(direction.Y, -direction.X);
            right *= 0.5f * Width * Constants.FEET_TO_MTR;
            Vector2D step = direction * 10.0f;

            short count = 0;

            float alt = (float)(altitude + 0.5f);
            float tx = 0.5f;
            float ty = 128.0f / 1024.0f;
            if (Markings.Edges)
                tx = 0.5f;

            Vector2D start = centre - direction * (float)(length * 0.5);
            int ld = 1;
            while (length > 0)
            {
                Vector2D end = start + step;
                Vector2D tl = start - right;
                Vector2D tr = start + right;
                Vector2D bl = end - right;
                Vector2D br = end + right;

                VertexPositionNormalTexture rVert = new VertexPositionNormalTexture();
                rVert.Position = new Vector3((float)tl.X, alt, (float)tl.Y);
                rVert.Normal = Vector3.Up;
                rVert.TextureCoordinate = new Vector2(tx, 0);
                tempverts.Add(rVert);

                rVert = new VertexPositionNormalTexture();
                rVert.Position = new Vector3((float)tr.X, alt, (float)tr.Y);
                rVert.Normal = Vector3.Up;
                rVert.TextureCoordinate = new Vector2(tx + 0.5f, 0);
                tempverts.Add(rVert);

                rVert = new VertexPositionNormalTexture();
                rVert.Position = new Vector3((float)bl.X, alt, (float)bl.Y);
                rVert.Normal = Vector3.Up;
                rVert.TextureCoordinate = new Vector2(tx, ty);
                tempverts.Add(rVert);

                rVert = new VertexPositionNormalTexture();
                rVert.Position = new Vector3((float)br.X, alt, (float)br.Y);
                rVert.Normal = Vector3.Up;
                rVert.TextureCoordinate = new Vector2(tx + 0.5f, ty);
                tempverts.Add(rVert);

                start = end;

                tempinds.Add(count);
                tempinds.Add((short)(count + 1));
                tempinds.Add((short)(count + 2));

                tempinds.Add((short)(count + 1));
                tempinds.Add((short)(count + 3));
                tempinds.Add((short)(count + 2));

                if ((Lights.Center == "HIGH") && (ld == 1))
                {
                    Color c = Color.White;
                    if (Lights.CenterRed)
                        c = Color.Red;
                    LightManager.AddPointLight(new Vector3((float)start.X, alt, (float)start.Y), 0.04f, c);
                }
                if (Lights.Edge == "HIGH")
                {
                    Color c = Color.White;

                    LightManager.AddPointLight(new Vector3((float)tl.X, alt, (float)tl.Y), 0.04f, c);
                    LightManager.AddPointLight(new Vector3((float)tr.X, alt, (float)tr.Y), 0.04f, c);
                }

                count += 4;

                length -= 10;
                ld ^= 1;
            }
            result.Indices = tempinds.ToArray();
            result.Verts = tempverts.ToArray();
            result.fx = ShaderManager.GetEffect("Shaders/DiffuseFog");

            return result;
        }
    }
}

