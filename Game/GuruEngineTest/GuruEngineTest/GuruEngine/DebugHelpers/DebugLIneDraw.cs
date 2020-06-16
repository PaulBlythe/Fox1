using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Assets;

namespace GuruEngine.DebugHelpers
{
    public class TextRecord
    {
        public String Text;
        public Vector3 Position;

        public TextRecord(String t, Vector3 p)
        {
            Text = t;
            Position = p;
        }
    }

    public class DebugLineDraw
    {
#if DEBUG
        List<VertexPositionColor> Verts = new List<VertexPositionColor>(120000);
        List<TextRecord> Texts = new List<TextRecord>();

        public static DebugLineDraw Instance;

        SpriteBatch batch = null;

        public DebugLineDraw()
        {
            Instance = this;
        }

        public void AddLine(Vector3 start, Vector3 end, Color colour)
        {
            lock (Verts)
            {
                VertexPositionColor v1 = new VertexPositionColor();
                v1.Position = start;
                v1.Color = colour;
                Verts.Add(v1);

                VertexPositionColor v2 = new VertexPositionColor();
                v2.Position = end;
                v2.Color = colour;
                Verts.Add(v2);
            }
        }

        public void AddText(String t, Vector3 p)
        {
            lock(Texts)
            {
                Texts.Add(new TextRecord(t, p));
            }
        }

        public void Draw(BasicEffect be)
        {
            if ((Verts.Count == 0) && (Texts.Count == 0))
                return;

            if (Verts.Count > 0)
            {
                VertexPositionColor[] vs;

                lock (Verts)
                {
                    vs = Verts.ToArray();
                    Verts.Clear();
                }

                be.VertexColorEnabled = true;
                be.CurrentTechnique.Passes[0].Apply();
                be.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vs, 0, vs.Length / 2);
            }

            if (Texts.Count > 0)
            {
                if (batch == null)
                {
                    batch = new SpriteBatch(be.GraphicsDevice);
                }
                lock (Texts)
                {
                    batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    foreach (TextRecord t in Texts)
                    {
                        Vector3 pos = be.GraphicsDevice.Viewport.Project(t.Position, be.Projection, be.View, Matrix.Identity);
                        Vector2 dp = new Vector2(pos.X, pos.Y);
                        batch.DrawString(AssetManager.GetDebugFont(), t.Text, dp, Color.White);
                    }
                    batch.End();
                    Texts.Clear();
                }
            }
        }


        public static void DrawLine(Vector3 start, Vector3 end, Color colour)
        {
            DebugLineDraw.Instance.AddLine(start, end, colour);
        }

        public static void DrawFrustrum(BoundingFrustum f, Matrix m)
        {
            Vector3[] corners = f.GetCorners();
            for (int i = 0; i < 8; i++)
                corners[i] = Vector3.Transform(corners[i], m);

            DebugLineDraw.Instance.AddLine(corners[0], corners[1], Color.Coral);
            DebugLineDraw.Instance.AddLine(corners[1], corners[2], Color.Coral);
            DebugLineDraw.Instance.AddLine(corners[2], corners[3], Color.Coral);
            DebugLineDraw.Instance.AddLine(corners[0], corners[3], Color.Coral);

            DebugLineDraw.Instance.AddLine(corners[0], corners[4], Color.Blue);
            DebugLineDraw.Instance.AddLine(corners[1], corners[5], Color.Blue);
            DebugLineDraw.Instance.AddLine(corners[2], corners[6], Color.Blue);
            DebugLineDraw.Instance.AddLine(corners[3], corners[7], Color.Blue);

            DebugLineDraw.Instance.AddLine(corners[4], corners[5], Color.Green);
            DebugLineDraw.Instance.AddLine(corners[5], corners[6], Color.Green);
            DebugLineDraw.Instance.AddLine(corners[6], corners[7], Color.Green);
            DebugLineDraw.Instance.AddLine(corners[4], corners[7], Color.Green);
        }

        public static void DrawTarget(Vector3 position, Color color, float size)
        {
            Vector3 p1 = new Vector3(size, 0, 0);
            Vector3 p2 = new Vector3(0, size, 0);
            Vector3 p3 = new Vector3(0, 0, size);

            DebugLineDraw.Instance.AddLine(position - p1, position + p1, color);
            DebugLineDraw.Instance.AddLine(position - p2, position + p2, color);
            DebugLineDraw.Instance.AddLine(position - p3, position + p3, color);
        }

        public static void DrawArrow(Vector3 position, Vector3 Direction, Color color, float size)
        {
            Vector3 p1 = Direction * size;
            Vector3 p2 = 0.25f * Vector3.Cross(Direction, Vector3.Right);
            Vector3 p3 = Direction * 0.25f;
            Vector3 p4 = 0.25f * Vector3.Cross(Direction, Vector3.Forward);

            DebugLineDraw.Instance.AddLine(position, position + p1, color);
            DebugLineDraw.Instance.AddLine(position + p1, position + p1 + p2 - p3, color);
            DebugLineDraw.Instance.AddLine(position + p1, position + p1 - p2 - p3, color);
            DebugLineDraw.Instance.AddLine(position + p1, position + p1 + p4 - p3, color);
            DebugLineDraw.Instance.AddLine(position + p1, position + p1 - p4 - p3, color);
        }

        public static void DrawText(String t, Vector3 p)
        {
            DebugLineDraw.Instance.AddText(t, p);
        }

        public static void DrawAll(BasicEffect be)
        {
            DebugLineDraw.Instance.Draw(be);
        }
#endif
    }
}
