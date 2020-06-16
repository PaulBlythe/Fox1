using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GUITestbed.DataHandlers.Mapping.Types;
using GUITestbed.DataHandlers.Mapping.Projections;
using GUITestbed.DataHandlers.Mapping.ColourMapping;
using GUITestbed.DataHandlers;
using GUITestbed.DataHandlers.Shape;
using GUITestbed;
using GUITestbed.Rendering;
using GUITestbed.GUI;


namespace GUITestbed.DataHandlers.Shape
{
    public class ShapeFile
    {
        Int32 FileCode;                             // big endian
        UInt32[] Unused = new UInt32[5];            // big endian
        int FileLength;                             // in 16 bit words including header . big endian

        Int32 Version;
        Int32 ShapeType;

        Region TotalArea;

        double MinZ;
        double MaxZ;
        double MinM;
        double MaxM;


        List<SHPElement> Elements = new List<SHPElement>();
        List<String> Types = new List<string>();

        public ShapeFile(String filename)
        {

            using (BinaryReader b = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                FileCode = ReadBigEndian(b);

                Unused[0] = (UInt32)ReadBigEndian(b);
                Unused[1] = (UInt32)ReadBigEndian(b);
                Unused[2] = (UInt32)ReadBigEndian(b);
                Unused[3] = (UInt32)ReadBigEndian(b);
                Unused[4] = (UInt32)ReadBigEndian(b);

                FileLength = 2 * (int)ReadBigEndian(b);

                Version = b.ReadInt32();
                ShapeType = b.ReadInt32();
                TotalArea = new Region(b);

                MinZ = b.ReadDouble();
                MaxZ = b.ReadDouble();
                MinM = b.ReadDouble();
                MaxM = b.ReadDouble();

                while (b.BaseStream.Position < FileLength)
                {
                    int rn = ReadBigEndian(b);
                    int rs = ReadBigEndian(b);
                    int ShapeType = b.ReadInt32();
                    switch (ShapeType)
                    {
                        case 0:     // Null
                            break;
                        case 1:     //Point     
                            Elements.Add(new SHPPoint(b));
                            break;
                        case 3:     //PolyLine
                            Elements.Add(new SHPPolyLine(b));
                            break;
                        case 5:     //Polygon
                            Elements.Add(new SHPPolygon(b));
                            break;
                        case 8:     //MultiPoint
                            Elements.Add(new SHPMultiPoint(b));
                            break;
                        case 11:     //PointZ
                            Elements.Add(new SHPPointZ(b));
                            break;
                        default:
                            throw new Exception("Unknown shape file element " + ShapeType.ToString());
                    }
                }
            }
        }

        public void DrawText(SpriteBatch batch, SpriteFont font, Region display_region, Vector2 scale, Projection projection, Database db, Color color, int DisplayHeight)
        {
            int i = 0;
            
            DoublePoint origin = new DoublePoint(display_region.MinX, display_region.MinY);

            batch.Begin();
            while (i < Elements.Count)
            {
                if (Elements[i].Type == 1)
                {
                    SHPPoint p = (SHPPoint)Elements[i];
                    if (display_region.Contains(p.X, p.Y))
                    {
                        Vector2 ip1 = projection.Project(origin, p.X, p.Y, scale);
                        ip1.X += 4;
                        ip1.Y = DisplayHeight - ip1.Y;
                        String name = db.GetRecord(i, "NAME");
                        try
                        {
                            batch.DrawString(font, name, ip1, color);
                        }
                        catch (Exception e) { }
                    }
                }
                i++;
            }
            batch.End();
        }

        public void Draw(SpriteBatch batch, Region display_region, Vector2 scale, Projection projection, String classtype, Database db, ColourMap map, int DisplayHeight)
        {
            int i = 0;

            DoublePoint origin = new DoublePoint(display_region.MinX, display_region.MinY);
            batch.Begin();


            while (i < Elements.Count)
            {
                bool doit = false;
                if (Elements[i].region == null)
                    doit = true;
                else if (display_region.Intersects(Elements[i].region))
                {
                    doit = true;
                }
                if (doit)
                {
                    if ((db.GetRecord(i, "Featurecla") == classtype) || (classtype == "ALL"))
                    {
                        String t = db.GetRecord(i, "Type");
                        if (t == null)
                            t = classtype;

                        ColourRecord r = map.GetColour((db.GetRecord(i, "Featurecla")));
                        switch (Elements[i].Type)
                        {
                            case 1:
                                {
                                    SHPPoint p = (SHPPoint)Elements[i];

                                    if (display_region.Contains(p.X,p.Y))
                                    {
                                        Vector2 ip1 = projection.Project(origin, p.X, p.Y, scale);
                                        int size = map.GetSize("city");
                                        if (db.GetRecord(i, "Featurecla") == "Admin-1 capital")
                                            size *= 2;

                                        Rectangle rd = new Rectangle(((int)ip1.X) -(size/2) , (DisplayHeight - (int)ip1.Y)-(size/2), size, size);
                                        batch.FillRectangle(rd, map.GetColour("city").Colour);

                                        
                                    }
                                }
                                break;
                            case 5:
                                {

                                    List<Edge> edges = new List<Edge>();

                                    SHPPolygon sp = (SHPPolygon)Elements[i];
                                    for (int ii = 0; ii < sp.NumParts; ii++)
                                    {
                                        List<Vector2> Poly = new List<Vector2>();
                                        int s1 = sp.Parts[ii];
                                        int s2;
                                        if ((ii + 1) == sp.NumParts)
                                        {
                                            s2 = sp.NumPoints - 1;
                                        }
                                        else
                                        {
                                            s2 = sp.Parts[ii + 1] - 1;
                                        }
                                        while (s1 < s2)
                                        {
                                            SHPPoint p1 = sp.Points[s1];
                                            Vector2 ip1 = projection.Project(origin, p1.X, p1.Y, scale);
                                            ip1.Y = DisplayHeight - ip1.Y;
                                            Poly.Add(ip1);
                                            s1++;
                                        }
                                        int e = 0;
                                        while (e < Poly.Count - 1)
                                        {
                                            Edge edge = new Edge(Poly[e], Poly[e + 1]);
                                            edges.Add(edge);
                                            e++;
                                        }
                                        Edge gg = new Edge(Poly[0], Poly[Poly.Count - 1]);
                                        edges.Add(gg);

                                    }
                                    ScanLineConverter.ScanConvertPolygon(batch, r.Colour, edges);



                                }
                                break;
                            case 3:
                                {
                                    SHPPolyLine sp = (SHPPolyLine)Elements[i];
                                    for (int ii = 0; ii < sp.NumParts; ii++)
                                    {
                                        int s1 = sp.Parts[ii];
                                        int s2;
                                        if ((ii + 1) == sp.NumParts)
                                        {
                                            s2 = sp.NumPoints - 1;
                                        }
                                        else
                                        {
                                            s2 = sp.Parts[ii + 1] - 1;
                                        }

                                        while (s1 < s2)
                                        {
                                            SHPPoint p1 = sp.Points[s1];
                                            SHPPoint p2 = sp.Points[s1 + 1];

                                            Vector2 ip1 = projection.Project(origin, p1.X, p1.Y, scale);
                                            Vector2 ip2 = projection.Project(origin, p2.X, p2.Y, scale);

                                            batch.DrawLine(ip1.X, DisplayHeight - ip1.Y, ip2.X, DisplayHeight - ip2.Y, r.Colour, r.Width);
                                            s1++;

                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                i++;
            }

            batch.End();
        }

        private Int32 ReadBigEndian(BinaryReader b)
        {
            Int32 res = 0;
            res += b.ReadByte();
            res = res << 8;
            res += b.ReadByte();
            res = res << 8;
            res += b.ReadByte();
            res = res << 8;
            res += b.ReadByte();

            return res;
        }

    }
}
