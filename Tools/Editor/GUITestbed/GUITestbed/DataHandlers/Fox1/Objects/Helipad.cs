using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GUITestbed.Rendering._3D;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class Helipad
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;
        public String Surface;
        public double Heading;
        public double Length;
        public double Width;
        public String Type;
        public bool Closed;
        public bool Transparent;


        public Helipad(XmlNode node)
        {
            Latitude = Double.Parse(node.Attributes["lat"]?.InnerText);
            Longitude = Double.Parse(node.Attributes["lon"]?.InnerText);
            Altitude = Cartography.ReadFloat(node.Attributes["alt"]?.InnerText);
            Heading = Double.Parse(node.Attributes["heading"]?.InnerText);
            Length = Cartography.ReadFloat(node.Attributes["length"]?.InnerText);
            Width = Cartography.ReadFloat(node.Attributes["width"]?.InnerText);

            Surface = node.Attributes["surface"]?.InnerText;
            Type = node.Attributes["type"]?.InnerText;

            Closed = ((node.Attributes["closed"]?.InnerText) == "YES");
            Transparent = ((node.Attributes["transparent"]?.InnerText) == "YES");
        }
        

        public RunwayObject Build(Vector2D airport_centre)
        {
            float alt = (float)(Altitude * Constants.FEET_TO_MTR);

            RunwayObject r = new RunwayObject();

            Vector2D centre = Cartography.ConvertToLocalised(airport_centre.Y, airport_centre.X, Latitude, Longitude);
            float l = (float)(Length * Constants.FEET_TO_MTR) * 0.5f;
            float w = (float)(Width * Constants.FEET_TO_MTR) * 0.5f;

            Vector2D direction = new Vector2D(0, 1);
            direction = Vector2D.Rotate(direction, new Vector2D(0, 0), Heading * Constants.DEG_TO_RAD);

            Vector2D start;
            Vector2D end;

            start = centre + (direction * l);
            end = centre - (direction * l);

            Vector2D right = new Vector2D(direction.Y, -direction.X);
            right *= w;

            Vector2D tl = start - right;
            Vector2D tr = start + right;
            Vector2D bl = end - right;
            Vector2D br = end + right;

            r.Verts = new VertexPositionNormalTexture[4];

            r.Verts[0] = new VertexPositionNormalTexture();
            r.Verts[0].Position = new Vector3((float)tl.X, alt, (float)tl.Y);
            r.Verts[0].Normal = Vector3.Up;
            r.Verts[0].TextureCoordinate = new Vector2(0, 0);

            r.Verts[1] = new VertexPositionNormalTexture();
            r.Verts[1].Position = new Vector3((float)tr.X, alt, (float)tr.Y);
            r.Verts[1].Normal = Vector3.Up;
            r.Verts[1].TextureCoordinate = new Vector2(1, 0);

            r.Verts[2] = new VertexPositionNormalTexture();
            r.Verts[2].Position = new Vector3((float)bl.X, alt, (float)bl.Y);
            r.Verts[2].Normal = Vector3.Up;
            r.Verts[2].TextureCoordinate = new Vector2(0, 1);

            r.Verts[3] = new VertexPositionNormalTexture();
            r.Verts[3].Position = new Vector3((float)br.X, alt, (float)br.Y);
            r.Verts[3].Normal = Vector3.Up;
            r.Verts[3].TextureCoordinate = new Vector2(1, 1);

            r.Indices = new short[6];
            r.Indices[0] = 0;
            r.Indices[1] = 1;
            r.Indices[2] = 2;

            r.Indices[3] = 1;
            r.Indices[4] = 3;
            r.Indices[5] = 2;

            String texname = "";

            switch (Surface)
            {
                case "CONCRETE":
                    texname += "c";
                    break;

                case "ASPHALT":
                    texname += "a";
                    break;

                default:
                    throw new Exception("Helipad missing surface type");
            }

            switch (Type)
            {
                case "CIRCLE":
                    texname += "c";
                    break;

                default:
                    throw new Exception("Helipad missing shape type");
            }
            texname += "_helipad";
            r.texture = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/" + texname);
            r.fx = ShaderManager.GetEffect("Shaders/DiffuseFog");

            return r;
        }
    }
}
