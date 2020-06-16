using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUITestbed.GUI
{
    public class GUIBatch
    {
        GraphicsDevice Device;
        Effect Shader;
        BlendState blend;
        RasterizerState stat = new RasterizerState();

        List<GUIVertex> Vertices = new List<GUIVertex>();

        float time = 0;
        Vector3 lightpos = new Vector3(1 ,- 0.314184f , 1.072981f);



        public GUIBatch(GraphicsDevice device, Effect shader)
        {
            Device = device;
            Shader = shader;
            stat.CullMode = CullMode.None;
        }

        public void Begin(BlendState bs)
        {
            blend = bs;
            Vertices.Clear();

            Shader.Parameters["LightDirection2"].SetValue(lightpos);
        }

        public void End()
        {
            Device.BlendState = blend;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = stat;
            GUIVertex[] v = Vertices.ToArray();

            foreach (EffectPass pass in Shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawUserPrimitives<GUIVertex>(PrimitiveType.TriangleList, v, 0, v.Length / 3);
            }
        }

        public void FillRectangle(Rectangle r, Color c)
        {
            Vector4 rv = new Vector4(r.X, r.Y, r.Width, r.Height);

            Vector3 tl = new Vector3(r.X, r.Y, -0.5f);
            Vector3 tr = new Vector3(r.X + r.Width, r.Y, -0.5f);
            Vector3 bl = new Vector3(r.X, r.Y + r.Height, -0.5f);
            Vector3 br = new Vector3(r.X + r.Width, r.Y + r.Height, -0.5f);

            GUIVertex v1 = new GUIVertex(tl, c, rv, new Vector2(0, 0));
            GUIVertex v2 = new GUIVertex(tr, c, rv, new Vector2(1, 0));
            GUIVertex v3 = new GUIVertex(br, c, rv, new Vector2(1, 1));
            GUIVertex v4 = new GUIVertex(bl, c, rv, new Vector2(0, 1));

            Vertices.Add(v1);
            Vertices.Add(v2);
            Vertices.Add(v3);

            Vertices.Add(v1);
            Vertices.Add(v3);
            Vertices.Add(v4);


        }
    }
}
