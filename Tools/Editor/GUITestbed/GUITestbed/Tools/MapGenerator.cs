using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GUITestbed.DataHandlers.Shape;
using GUITestbed.DataHandlers;
using GUITestbed.SerialisableData;
using GUITestbed.DataHandlers.Mapping.Projections;
using GUITestbed.DataHandlers.Mapping.Types;
using GUITestbed.DataHandlers.Mapping.ColourMapping;
using GUITestbed.GUI;

namespace GUITestbed.Tools
{
    public class MapGenerator:Tool
    {
        CSV road_data;
        CSV coastline_data;
        CSV boundary_data;
        CSV marine_poly;
        CSV ne_10m_minor_islands;
        CSV ne_10m_rivers_lake_centerlines;
        CSV ne_10m_rivers_europe;
        CSV ne_10m_admin_0_boundary_lines_land;
        CSV ne_10m_populated_places;

        ShapeFile road_shape;
        ShapeFile coast_shape;
        ShapeFile boundary_shape;
        ShapeFile marine_shape;
        ShapeFile ne_10m_minor_islands_shape;
        ShapeFile ne_10m_rivers_lake_centerlines_shape;
        ShapeFile ne_10m_rivers_europe_shape;
        ShapeFile ne_10m_admin_0_boundary_lines_land_shape;
        ShapeFile ne_10m_populated_places_shape;

        MapGeneratorData data;

        RenderTarget2D generated_map;
        FlatProjection proj;
        Region reg;
        ColourMap map;
        Matrix Projection;
        SpriteFont font;

        public MapGenerator(MapGeneratorData dat)
        {
            font = Game1.Instance.Content.Load<SpriteFont>("Fonts/MapFont");

            Projection = Matrix.CreateOrthographicOffCenter(0f, dat.Width, 0, dat.Height,  0f, 1f);

            if (dat.Roads)
            {
                road_data = new CSV(Path.Combine(Game1.GISLocation, @"10m_cultural\ne_10m_roads.csv"));
                road_shape = new ShapeFile(Path.Combine(Game1.GISLocation, @"10m_cultural\ne_10m_roads.shp"));
            }
            if (dat.Rivers)
            {
                ne_10m_rivers_europe = new CSV(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_rivers_europe.csv"));
                ne_10m_rivers_europe_shape = new ShapeFile(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_rivers_europe.shp"));
                ne_10m_rivers_lake_centerlines = new CSV(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_rivers_lake_centerlines.csv"));
                ne_10m_rivers_lake_centerlines_shape = new ShapeFile(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_rivers_lake_centerlines.shp"));
            }
            if (dat.Countries)
            {
                ne_10m_admin_0_boundary_lines_land = new CSV(Path.Combine(Game1.GISLocation, @"10m_cultural\ne_10m_admin_0_boundary_lines_land.csv"));
                ne_10m_admin_0_boundary_lines_land_shape = new ShapeFile(Path.Combine(Game1.GISLocation, @"10m_cultural\ne_10m_admin_0_boundary_lines_land.shp"));
            }
            if (dat.Cities)
            {
                ne_10m_populated_places = new CSV(Path.Combine(Game1.GISLocation, @"10m_cultural\ne_10m_populated_places.csv"));
                ne_10m_populated_places_shape = new ShapeFile(Path.Combine(Game1.GISLocation, @"10m_cultural\ne_10m_populated_places.shp"));

            }

            coastline_data = new CSV(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_coastline.csv"));
            boundary_data = new CSV(Path.Combine(Game1.GISLocation, @"10m_cultural\ne_10m_admin_0_boundary_lines_land.csv"));
            marine_poly = new CSV(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_geography_marine_polys.csv"));
            ne_10m_minor_islands = new CSV(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_minor_islands.csv"));
            
            //ne_10m_geography_regions_polys = new CSV(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_geography_regions_polys.csv"));


            coast_shape = new ShapeFile(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_coastline.shp"));
            boundary_shape = new ShapeFile(Path.Combine(Game1.GISLocation, @"10m_cultural\ne_10m_admin_0_boundary_lines_land.shp"));
            marine_shape = new ShapeFile(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_geography_marine_polys.shp"));
            ne_10m_minor_islands_shape = new ShapeFile(Path.Combine(Game1.GISLocation, @"10m_physical\ne_10m_minor_islands.shp"));
            

            data = dat;

            proj = new FlatProjection(dat.Size);

            float wwidth = (dat.Width * dat.Size) / (1852 * 60);
            float wheight = (dat.Height * dat.Size) / (1852 * 60);
            float elat = dat.StartLatitude - wheight;
            float elon = dat.StartLongitude + wwidth;

            reg = new Region();
            reg.MinX = Math.Min(dat.StartLongitude, elon);
            reg.MaxX = Math.Max(dat.StartLongitude, elon);
            reg.MinY = Math.Min(dat.StartLatitude, elat);
            reg.MaxY = Math.Max(dat.StartLatitude, elat);

            map = new UKMap();

            generated_map = new RenderTarget2D(Game1.Instance.GraphicsDevice, data.Width, data.Height);
            Game1.Instance.GraphicsDevice.SetRenderTarget(generated_map);
            Game1.Instance.GraphicsDevice.Clear(Color.Khaki);

            marine_shape.Draw(Game1.Instance.spriteBatch, reg, new Vector2(1, 1), proj, "ALL", marine_poly, map, dat.Height);
            ne_10m_minor_islands_shape.Draw(Game1.Instance.spriteBatch, reg, new Vector2(1, 1), proj, "ALL", ne_10m_minor_islands, map, dat.Height);
            
            if (dat.Roads)
            {
                road_shape.Draw(Game1.Instance.spriteBatch, reg, new Vector2(1, 1), proj, "Road", road_data, map, dat.Height);
            }
            if (dat.Rivers)
            {
                ne_10m_rivers_europe_shape.Draw(Game1.Instance.spriteBatch, reg, new Vector2(1, 1), proj, "River", ne_10m_rivers_europe, map, dat.Height);
                ne_10m_rivers_lake_centerlines_shape.Draw(Game1.Instance.spriteBatch, reg, new Vector2(1, 1), proj, "River", ne_10m_rivers_lake_centerlines, map, dat.Height);
            }
            if (dat.Countries)
            {
                ne_10m_admin_0_boundary_lines_land_shape.Draw(Game1.Instance.spriteBatch, reg, new Vector2(1, 1), proj, "ALL", ne_10m_admin_0_boundary_lines_land, map, dat.Height);
            }
            if (dat.Cities)
            {
                ne_10m_populated_places_shape.Draw(Game1.Instance.spriteBatch, reg, new Vector2(1, 1), proj, "ALL", ne_10m_populated_places, map, dat.Height);
                ne_10m_populated_places_shape.DrawText(Game1.Instance.spriteBatch, font, reg, new Vector2(1, 1), proj, ne_10m_populated_places, Color.Black, dat.Height);
            }
            Game1.Instance.GraphicsDevice.SetRenderTarget(null);
            
        }

        public override void Update(float dt)
        {
            
        }

        public override void Draw()
        {
            float scalex = data.Width / 1000.0f;
            float scaley = data.Height / 1000.0f;
            int height = (int)((data.Height / scalex) + 0.5f);
            if (scaley > scalex)
                height = (int)((data.Height / scaley) + 0.5f);

            Rectangle dest = new Rectangle((1920 - 1000) / 2, (1000 - height) / 2, 1000, height);
            Game1.Instance.spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.NonPremultiplied);
            Game1.Instance.spriteBatch.Draw(generated_map,dest,Color.White);
            Game1.Instance.spriteBatch.End();

        }

        public override void SaveResults(string path)
        {
            String dir = Path.GetDirectoryName(path);
            String file = Path.GetFileNameWithoutExtension(path);
            String dest = Path.Combine(dir, file + ".png");
            FileStream stream = new FileStream(dest, FileMode.Create);
            generated_map.SaveAsPng(stream, generated_map.Width, generated_map.Height);
            stream.Close();
        }
    }
}


