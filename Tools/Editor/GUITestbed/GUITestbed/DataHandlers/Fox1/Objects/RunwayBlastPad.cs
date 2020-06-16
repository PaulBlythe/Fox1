using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GUITestbed.Rendering._3D;
using GUITestbed.World;


namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class RunwayBlastPad
    {
        public String End;
        public float Length;            // in feet
        public float Width;             // in feet
        public String Surface;

        public RunwayBlastPad(XmlNode node)
        {
            End = node.Attributes["end"]?.InnerText;
            Surface = node.Attributes["surface"]?.InnerText;

            Length = Cartography.ReadFloat(node.Attributes["length"]?.InnerText);
            Width = Cartography.ReadFloat(node.Attributes["width"]?.InnerText);
        }

        

        public RunwayObject Build(Vector2D centre, Vector2D direction, double length, float altitude)
        {
            RunwayObject r = new RunwayObject();

            Vector2D start;
            Vector2D end;

            float uvx = (float)(Width * Constants.FEET_TO_MTR / 10.0);
            float uvy = (float)(Length * Constants.FEET_TO_MTR / 10.0);

            if (End == "PRIMARY")
            {
                start = centre + (direction * length * -0.5);
                end = start + (direction * Length * -Constants.FEET_TO_MTR);
            }
            else
            {
                start = centre + (direction * length * 0.5);
                end = start + (direction * Length * Constants.FEET_TO_MTR);
            }

            Vector2D right = new Vector2D(direction.Y, -direction.X);
            right *= Width * Constants.FEET_TO_MTR * 0.5f;

            Vector2D tl = start - right;
            Vector2D tr = start + right;
            Vector2D bl = end - right;
            Vector2D br = end + right;

            r.Verts = new VertexPositionNormalTexture[4];

            r.Verts[0] = new VertexPositionNormalTexture();
            r.Verts[0].Position = new Vector3((float)tl.X, altitude + 0.5f, (float)tl.Y);
            r.Verts[0].Normal = Vector3.Up;
            r.Verts[0].TextureCoordinate = new Vector2(0, 0);

            r.Verts[1] = new VertexPositionNormalTexture();
            r.Verts[1].Position = new Vector3((float)tr.X, altitude + 0.5f, (float)tr.Y);
            r.Verts[1].Normal = Vector3.Up;
            r.Verts[1].TextureCoordinate = new Vector2(uvx, 0);

            r.Verts[2] = new VertexPositionNormalTexture();
            r.Verts[2].Position = new Vector3((float)bl.X, altitude + 0.5f, (float)bl.Y);
            r.Verts[2].Normal = Vector3.Up;
            r.Verts[2].TextureCoordinate = new Vector2(0, uvy);

            r.Verts[3] = new VertexPositionNormalTexture();
            r.Verts[3].Position = new Vector3((float)br.X, altitude + 0.5f, (float)br.Y);
            r.Verts[3].Normal = Vector3.Up;
            r.Verts[3].TextureCoordinate = new Vector2(uvx, uvy);

            r.Indices = new short[6];
            r.Indices[0] = 0;
            r.Indices[1] = 1;
            r.Indices[2] = 2;

            r.Indices[3] = 1;
            r.Indices[4] = 3;
            r.Indices[5] = 2;

            switch (Surface)
            {
                case "CONCRETE":
                    r.texture = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/concrete");
                    break;

                case "ASPHALT":
                    r.texture = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/asphalt");
                    break;

                default:
                    throw new Exception("Blastpad missing surface type");
            }
            r.fx = ShaderManager.GetEffect("Shaders/DiffuseFog");

            return r;
        }
    }
}
