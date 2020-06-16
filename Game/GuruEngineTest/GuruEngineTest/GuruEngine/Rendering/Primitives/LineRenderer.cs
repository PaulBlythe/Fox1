using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Rendering;
using GuruEngine.Helpers;
using GuruEngine.Assets;

namespace GuruEngine.Rendering.Primitives
{
    public class LineRenderer
    {
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(AssetManager.GetSinglePixel(), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawBox(SpriteBatch batch, Rectangle r, Color color, float thickness = 1.0f)
        {
            Vector2 tl = new Vector2(r.X, r.Y);
            Vector2 tr = new Vector2(r.X + r.Width, r.Y);
            Vector2 bl = new Vector2(r.X, r.Y + r.Height);
            Vector2 br = new Vector2(r.X + r.Width, r.Y + r.Height);

            DrawLine(batch, tl, tr, color, thickness);
            DrawLine(batch, tr, br, color, thickness);
            DrawLine(batch, bl, br, color, thickness);
            DrawLine(batch, bl, tl, color, thickness);

        }

        public static void DrawRotatedBox(SpriteBatch batch, Rectangle r, Color color, float angle, float thickness = 1.0f)
        {
            Vector2 centre = new Vector2(r.X + (r.Width * 0.5f), r.Y + (r.Height * 0.5f));
            Vector2 tl = new Vector2(r.X, r.Y) - centre;
            Vector2 tr = new Vector2(r.X + r.Width, r.Y) - centre;
            Vector2 bl = new Vector2(r.X, r.Y + r.Height) - centre;
            Vector2 br = new Vector2(r.X + r.Width, r.Y + r.Height) - centre;

            tl = MathsHelper.Rotate2D(tl, angle);
            bl = MathsHelper.Rotate2D(bl, angle);
            tr = MathsHelper.Rotate2D(tr, angle);
            br = MathsHelper.Rotate2D(br, angle);

            DrawLine(batch, tl + centre, tr + centre, color, thickness);
            DrawLine(batch, tr + centre, br + centre, color, thickness);
            DrawLine(batch, bl + centre, br + centre, color, thickness);
            DrawLine(batch, bl + centre, tl + centre, color, thickness);

        }

        public static void DrawTarget(SpriteBatch batch, Rectangle r, Color color, float angle, float speed, float thickness = 1.0f)
        {
            Vector2 centre = new Vector2(r.X + (r.Width * 0.5f), r.Y + (r.Height * 0.5f));
            Vector2 tl = new Vector2(r.X, r.Y) - centre;
            Vector2 tr = new Vector2(r.X + r.Width, r.Y) - centre;
            Vector2 bl = new Vector2(r.X, r.Y + r.Height) - centre;
            Vector2 br = new Vector2(r.X + r.Width, r.Y + r.Height) - centre;
            Vector2 ds = new Vector2(centre.X, r.Y) - centre;
            Vector2 de = new Vector2(centre.X, r.Y - (speed / 50.0f)) - centre;

            tl = MathsHelper.Rotate2D(tl, angle);
            bl = MathsHelper.Rotate2D(bl, angle);
            tr = MathsHelper.Rotate2D(tr, angle);
            br = MathsHelper.Rotate2D(br, angle);
            ds = MathsHelper.Rotate2D(ds, angle);
            de = MathsHelper.Rotate2D(de, angle);


            DrawLine(batch, tl + centre, tr + centre, color, thickness);
            DrawLine(batch, tr + centre, br + centre, color, thickness);
            DrawLine(batch, bl + centre, br + centre, color, thickness);
            DrawLine(batch, bl + centre, tl + centre, color, thickness);
            DrawLine(batch, ds + centre, de + centre, color, thickness);

        }


        public static void DrawArtificialHorizon(SpriteBatch hud_img,
                                                 float yaw, float pitch, float roll,
                                                 int width, int center_gap, int tip_length,
                                                 float pitch_range_of_lens, int img_width, int image_height,
                                                 Color color, float line_width)
        {
            int elevon_pos_max = (int)(width * 7.0 / 8.0);
            int elevon_pos_min = (int)(width * 5.0 / 8.0);


            float px_per_deg = image_height / pitch_range_of_lens;
            int center_height = (int)(image_height / 2 - pitch * px_per_deg);

            int center_delta = center_height - image_height / 2;

            int left = (int)(img_width / 2 - (width * Math.Sin(MathHelper.Pi / 180.0 * (roll + 90))));
            int top = (int)(image_height / 2 - (width * Math.Cos(MathHelper.Pi / 180.0 * (roll + 90))) + center_delta);

            int bottom = (int)(image_height / 2 - (center_gap * Math.Cos(MathHelper.Pi / 180.0 * (roll + 90))) + center_delta);
            int right = (int)(img_width / 2 - (center_gap * Math.Sin(MathHelper.Pi / 180.0 * (roll + 90))));

            // draw the left half of the line
            DrawLine(hud_img, new Vector2(left, top), new Vector2(right, bottom), color, line_width);

            // draw the 90 degree tips on the lines
            int angle_left = (int)(left + Math.Sin(MathHelper.Pi / 180.0 * (roll + 180)) * tip_length);
            int angle_top = (int)(top + Math.Cos(MathHelper.Pi / 180.0 * (roll + 180)) * tip_length);

            DrawLine(hud_img, new Vector2(angle_left, angle_top), new Vector2(left, top), color, line_width);

            // draw the right half of the line now
            left = (int)(img_width / 2 + (center_gap * Math.Sin(MathHelper.Pi / 180.0 * (roll + 90))));
            top = (int)(image_height / 2 + (center_gap * Math.Cos(MathHelper.Pi / 180.0 * (roll + 90))) + center_delta);

            bottom = (int)(image_height / 2 + (width * Math.Cos(MathHelper.Pi / 180.0 * (roll + 90))) + center_delta);
            right = (int)(img_width / 2 + (width * Math.Sin(MathHelper.Pi / 180.0 * (roll + 90))));

            DrawLine(hud_img, new Vector2(left, top), new Vector2(right, bottom), color, line_width);

            // draw the right have tip
            angle_left = (int)(right + Math.Sin(MathHelper.Pi / 180.0 * (roll + 180)) * tip_length);
            angle_top = (int)(bottom + Math.Cos(MathHelper.Pi / 180.0 * (roll + 180)) * tip_length);

            DrawLine(hud_img, new Vector2(angle_left, angle_top), new Vector2(right, bottom), color, line_width);

        }

        public static void DrawLeftTag(SpriteBatch batch, int x, int y, int width, int height, Color color, float thickness = 1.0f)
        {
            int x1 = x + width - (height / 2);
            int x2 = x + width;
            int y2 = y + (height / 2);
            int y3 = y + height;

            DrawLine(batch, new Vector2(x, y), new Vector2(x, y3), color, thickness);
            DrawLine(batch, new Vector2(x, y), new Vector2(x1, y), color, thickness);
            DrawLine(batch, new Vector2(x1, y), new Vector2(x2, y2), color, thickness);
            DrawLine(batch, new Vector2(x2, y2), new Vector2(x1, y3), color, thickness);
            DrawLine(batch, new Vector2(x1, y3), new Vector2(x, y3), color, thickness);
        }

        public static void DrawRightTag(SpriteBatch batch, int x, int y, int width, int height, Color color, float thickness = 1.0f)
        {
            int x1 = x + (height / 2);
            int x2 = x + width;
            int y2 = y + (height / 2);
            int y3 = y + height;

            DrawLine(batch, new Vector2(x, y2), new Vector2(x1, y), color, thickness);      //   /
            DrawLine(batch, new Vector2(x1, y), new Vector2(x2, y), color, thickness);      //   -
            DrawLine(batch, new Vector2(x2, y), new Vector2(x2, y3), color, thickness);     //  |
            DrawLine(batch, new Vector2(x2, y3), new Vector2(x1, y3), color, thickness);    //   -
            DrawLine(batch, new Vector2(x1, y3), new Vector2(x, y2), color, thickness);     //   \
        }

        public static void DrawLeftLadder(SpriteBatch batch, SpriteFont font, int x, int y, int width, int height, float value, int minor, int major, int range, int divisor, Color color, float thickness = 1.0f)
        {
            Vector2 tr = new Vector2(x + (width / 2), y);
            Vector2 br = new Vector2(x + (width / 2), y + height);
            Vector2 cl = new Vector2(x + (width / 2) + 1, y + (height / 2));
            Vector2 cr = cl + Vector2.UnitX * (width / 2.0f);

            DrawLine(batch, tr, br, color, thickness);
            DrawLine(batch, cr, cl, color, thickness);

            float topspeed = value + (range * 0.5f);
      
            float value_per_px = (float)height / (float)range;

            int diff_from_minor_increment = (int)(Math.Round(topspeed)) % minor;
            int top_line_value = (int)(Math.Round(topspeed)) - diff_from_minor_increment;
            float float_diff_from_minor_increment = topspeed - top_line_value;
            int top_line_position = (int)(y + Math.Round(float_diff_from_minor_increment * value_per_px));
            int number_of_lines = (range / minor);
            int vertical_line_gap = (int)(height / number_of_lines);

            int is_minor;

            for (int i = 0; i < number_of_lines; i++)
            {

                int this_top = top_line_position + i * vertical_line_gap;
                int this_value = top_line_value - i * minor;
                if (this_value >= 0)
                {
                    if (this_value % major == 0)
                    {
                        is_minor = 0;
                    }
                    else
                    {
                        is_minor = 1;
                    }

                    // draw the line
                    if (this_value >= 0)
                        DrawLine(batch, new Vector2(x + (is_minor * 4), this_top), new Vector2(x + (width / 2), this_top), color, thickness);

                    // draw a label if this is a major
                    if ((is_minor == 0) && (this_value >= 0))
                    {
                        string this_label = String.Format("{0}", (int)(this_value / divisor));
                        Vector2 size = font.MeasureString(this_label);

                        int text_center = (int)(this_top - (size.Y / 2));

                        // don't draw if we're close to the centerline
                        if (Math.Abs(text_center - (y + (height / 2))) > size.Y * 1.5f)
                        {
                            Vector2 text_origin;

                            text_origin = new Vector2(x - size.X - 2, this_top - (size.Y / 2));

                            batch.DrawString(font, this_label, text_origin, color);
                        }
                    }
                }
            }

        }

        public static void DrawRightLadder(SpriteBatch batch, SpriteFont font, int x, int y, int width, int height, float value, int minor, int major, int range, int divisor, Color color, float thickness = 1.0f)
        {
            Vector2 tr = new Vector2(x + (width / 2), y);
            Vector2 br = new Vector2(x + (width / 2), y + height);
            Vector2 cl = new Vector2(x + (width / 2) + 1, y + (height / 2));
            Vector2 cr = cl - Vector2.UnitX * (width / 2.0f);

            DrawLine(batch, tr, br, color, thickness);
            DrawLine(batch, cr, cl, color, thickness);

            float topspeed = value + (range * 0.5f);

            float value_per_px = (float)height / (float)range;

            int diff_from_minor_increment = (int)(Math.Round(topspeed)) % minor;
            int top_line_value = (int)(Math.Round(topspeed)) - diff_from_minor_increment;
            float float_diff_from_minor_increment = topspeed - top_line_value;
            int top_line_position = (int)(y + Math.Round(float_diff_from_minor_increment * value_per_px));
            int number_of_lines = (range / minor);
            int vertical_line_gap = (int)(height / number_of_lines);

            int is_minor;

            for (int i = 0; i < number_of_lines; i++)
            {

                int this_top = top_line_position + i * vertical_line_gap;
                int this_value = top_line_value - i * minor;
                if (this_value >= 0)
                {
                    if (this_value % major == 0)
                    {
                        is_minor = 0;
                    }
                    else
                    {
                        is_minor = 1;
                    }

                    // draw the line
                    if (this_value >= 0)
                        DrawLine(batch, new Vector2(x + width - (is_minor * 4), this_top), new Vector2(x + (width / 2), this_top), color, thickness);

                    // draw a label if this is a major
                    if ((is_minor == 0) && (this_value >= 0))
                    {
                        string this_label = String.Format("{0:00.0}", ((float)this_value / divisor));
                        Vector2 size = font.MeasureString(this_label);

                        int text_center = (int)(this_top - (size.Y / 2));

                        // don't draw if we're close to the centerline
                        if (Math.Abs(text_center - (y + (height / 2))) > size.Y * 1.5f)
                        {
                            Vector2 text_origin;

                            text_origin = new Vector2(x + size.X + 2 + (width / 2), this_top - (size.Y / 2));

                            batch.DrawString(font, this_label, text_origin, color);
                        }
                    }
                }
            }

        }

        public static void DrawBottomLadder(SpriteBatch batch, SpriteFont font, int x, int y, int width, int height, float value, int minor, int major, int range, int divisor, Color color, float thickness = 1.0f)
        {
            Vector2 tr = new Vector2(x - (width / 2), y);
            Vector2 br = new Vector2(x + (width / 2), y);
            Vector2 cl = new Vector2(x , y);
            Vector2 cr = new Vector2(x, y - 8);

            DrawLine(batch, tr, br, color, thickness);
            DrawLine(batch, cr, cl, color, thickness);

            float xc = x - (width / 2);
            float pix_per_unit = (float)width / (float)range;
            float left_hdg = value - (range * 0.5f);
            if (left_hdg < 0)
                left_hdg += 360;

            int start_hdg = (int)(Math.Ceiling(left_hdg / minor) * minor);
            float skip_pixels = (start_hdg - left_hdg) * pix_per_unit;

            int number_of_lines = (range / minor);
            float vertical_line_gap = (width / number_of_lines);
            int is_minor;

            int top_line_position = (int)(xc + skip_pixels);

            for (int i = 0; i < number_of_lines; i++)
            {

                float this_top = top_line_position + (i * vertical_line_gap);
                int this_value = (start_hdg + i * minor) % 360;
                
                {
                    if (this_value % major == 0)
                    {
                        is_minor = 0;
                    }
                    else
                    {
                        is_minor = 1;
                    }

                    // draw the line
                    DrawLine(batch, new Vector2(this_top , y + (height / 2) - (is_minor * 4)), new Vector2(this_top, y ), color, thickness);

                    // draw a label if this is a major
                    if (is_minor == 0) 
                    {
                        string this_label = String.Format("{0:00}", (int)(this_value / divisor));
                        Vector2 size = font.MeasureString(this_label);

                        int text_center = (int)(this_top - (size.X / 2));

                        // don't draw if we're close to the centerline
                        if (Math.Abs(text_center - x)  > size.X * 1.5f)
                        {
                            Vector2 text_origin;

                            text_origin = new Vector2(this_top - (size.X / 2), y + 4 + (size.Y / 2));

                            batch.DrawString(font, this_label, text_origin, color);
                        }
                    }
                }
            }

        }

    }
}
