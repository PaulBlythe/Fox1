using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.Rendering.Lighting.Photon
{

    public class Segment
    {
        public Segment(Ray ray, double tstart, double tstop)
        {
            if (tstart< -1000)
                tstart = -1000;
            if (tstop  >  1000)
                tstop  =  1000;
            a = ray.Position + ray.Direction * (float)tstart;
            b = ray.Position + ray.Direction * (float)tstop;
        }

        public Vector3 a;
        public Vector3 b;
    };

    public class RayTree
    {

        // when activated, these function calls store the segments of the tree
        public static void AddMainSegment(Ray ray, double tstart, double tstop)
        {
            if (!activated) return;
            main_segments.Add(new Segment(ray, tstart, tstop));
        }
        public static void AddShadowSegment(Ray ray, double tstart, double tstop)
        {
            if (!activated) return;
            shadow_segments.Add(new Segment(ray, tstart, tstop));
        }
        public static void AddReflectedSegment(Ray ray, double tstart, double tstop)
        {
            if (!activated) return;
            reflected_segments.Add(new Segment(ray, tstart, tstop));
        }

        public static void paint()
        {
            paintHelper(new Vector4(0.7f, 0.7f, 0.7f, 0.3f), 
                        new Vector4(0.1f, 0.9f, 0.1f, 0.3f),
                        new Vector4(0.9f, 0.1f, 0.1f, 0.3f),
                        new Vector4(0.1f, 0.1f, 0.9f, 0.3f));
        }


        static void Activate()
        {
            Clear();
            activated = true;
        }
        static void Deactivate()
        {
            activated = false;
        }

        static void paintHelper(Vector4 m,Vector4 s,Vector4 r,Vector4 t)
        {
            List<VertexPositionColor> verts = new List<VertexPositionColor>();
            foreach (Segment seg in main_segments)
            {
                VertexPositionColor v1 = new VertexPositionColor();
                v1.Color = Color.FromNonPremultiplied(m);
                v1.Position = seg.a;
                verts.Add(v1);

                VertexPositionColor v2 = new VertexPositionColor();
                v2.Color = Color.FromNonPremultiplied(m);
                v2.Position = seg.b;
                verts.Add(v2);
            }
            foreach (Segment seg in shadow_segments)
            {
                VertexPositionColor v1 = new VertexPositionColor();
                v1.Color = Color.FromNonPremultiplied(s);
                v1.Position = seg.a;
                verts.Add(v1);

                VertexPositionColor v2 = new VertexPositionColor();
                v2.Color = Color.FromNonPremultiplied(s);
                v2.Position = seg.b;
                verts.Add(v2);
            }
            foreach (Segment seg in reflected_segments)
            {
                VertexPositionColor v1 = new VertexPositionColor();
                v1.Color = Color.FromNonPremultiplied(r);
                v1.Position = seg.a;
                verts.Add(v1);

                VertexPositionColor v2 = new VertexPositionColor();
                v2.Color = Color.FromNonPremultiplied(r);
                v2.Position = seg.b;
                verts.Add(v2);
            }

            VertexPositionColor[] vs = verts.ToArray();

            if (effect == null)
            {
                effect = new BasicEffect(Game1.Instance.GraphicsDevice);
                effect.LightingEnabled = false;
                effect.TextureEnabled = false;
                effect.DiffuseColor = Color.White.ToVector3();
            }
            Game1.Instance.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vs, 0, verts.Count / 2);
            }
        }

        static void Clear()
        {
            main_segments.Clear();
            shadow_segments.Clear();
            reflected_segments.Clear();
        }

        static bool activated;
        static List<Segment> main_segments = new List<Segment>();
        static List<Segment> shadow_segments = new List<Segment>();
        static List<Segment> reflected_segments = new List<Segment>();
        public static BasicEffect effect = null;
        
    }
}
