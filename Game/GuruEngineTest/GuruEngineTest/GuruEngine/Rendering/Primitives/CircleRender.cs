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

namespace GuruEngine.Rendering.Primitives
{
    public class CircleRender
    {

        public static void DrawCircle(float radius, Vector2 centre, int sides, int thickness, Color color, SpriteBatch batch)
        {
            List<Vector2> vectors = new List<Vector2>();

            float max = 2 * (float)Math.PI;
            float step = max / (float)sides;

            for (float theta = 0; theta < max; theta += step)
            {
                vectors.Add(centre + new Vector2(radius * (float)Math.Cos((double)theta), radius * (float)Math.Sin((double)theta)));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(centre + new Vector2(radius * (float)Math.Cos(0), radius * (float)Math.Sin(0)));

            if (vectors.Count < 2)
                return;

            for (int i = 1; i < vectors.Count; i++)
            {
                Vector2 vector1 = (Vector2)vectors[i - 1];
                Vector2 vector2 = (Vector2)vectors[i];

                LineRenderer.DrawLine(batch, vector1, vector2, color, thickness);
            }
        }
    }
}
