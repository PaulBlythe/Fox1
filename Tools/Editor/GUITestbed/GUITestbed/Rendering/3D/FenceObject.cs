using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.Rendering._3D
{
    public class FenceObject
    {
        Texture2D Wire;
        Texture2D Post;
        Effect fx;

        List<VertexPositionNormalTexture> PostVertices = new List<VertexPositionNormalTexture>();
        List<VertexPositionNormalTexture> FenceVertices = new List<VertexPositionNormalTexture>();

        VertexPositionNormalTexture[] drawPosts;
        VertexPositionNormalTexture[] drawFences;

        public RasterizerState rs;

        public FenceObject()
        {
            rs = new RasterizerState();
            rs.CullMode = CullMode.None;

        }

        public void Load()
        {
            Wire = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/wirefence");
            Post = Game1.Instance.Content.Load<Texture2D>("Textures/Airport/concrete");
            fx = ShaderManager.GetEffect("Shaders/DiffuseFog");
        }

        public void AddFence(Vector3 p1, Vector3 p2)
        {
            Vector3 direction = p2 - p1;
            float length = direction.Length();
            direction.Normalize();
            Vector3 normal = Vector3.Cross(direction, Vector3.Right);

            VertexPositionNormalTexture v = new VertexPositionNormalTexture();
            v.Position = p1;
            v.TextureCoordinate = new Vector2(0, 0);
            v.Normal = normal;
            FenceVertices.Add(v);

            v = new VertexPositionNormalTexture();
            v.Position = p1 + new Vector3(0, 5, 0);
            v.TextureCoordinate = new Vector2(0, 5);
            v.Normal = normal;
            FenceVertices.Add(v);

            v = new VertexPositionNormalTexture();
            v.Position = p2 + new Vector3(0, 5, 0);
            v.TextureCoordinate = new Vector2(length, 5);
            v.Normal = normal;
            FenceVertices.Add(v);

            FenceVertices.Add(v);
            v = new VertexPositionNormalTexture();
            v.Position = p2 + new Vector3(0, 0, 0);
            v.TextureCoordinate = new Vector2(length, 0);
            v.Normal = normal;
            FenceVertices.Add(v);

            v = new VertexPositionNormalTexture();
            v.Position = p1;
            v.TextureCoordinate = new Vector2(0, 0);
            v.Normal = normal;
            FenceVertices.Add(v);


        }

        public void AddPost(Vector3 p1)
        {
            for (int i=0; i< mesh01_indices.Length; i++)
            {
                int p = mesh01_indices[i];
                VertexPositionNormalTexture v = new VertexPositionNormalTexture();
                v.Position = p1 + mesh01_coords[p];
                v.Normal = mesh01_normals[p];
                v.TextureCoordinate = mesh01_texcoords[p];
                PostVertices.Add(v);
            }
        }

        public void Finalise()
        {
            drawFences = FenceVertices.ToArray();
            drawPosts = PostVertices.ToArray();

            PostVertices.Clear();
            FenceVertices.Clear();
        }

        public void UpdateLighting(Vector4 Ambient, Vector4 Sun, Vector4 sunpos)
        {
            fx.Parameters["AmbientColour"].SetValue(new Vector3(Ambient.X, Ambient.Y, Ambient.Z));
            fx.Parameters["SunDirection"].SetValue(new Vector3(sunpos.X, sunpos.Y, sunpos.Z));
            fx.Parameters["FogColour"].SetValue(new Vector3(Sun.X, Sun.Y, Sun.Z));
            fx.Parameters["FogStart"].SetValue(18000.0f);
            fx.Parameters["FogEnd"].SetValue(42000.0f);
        }

        public void Draw(Camera camera)
        {
            Game1.Instance.GraphicsDevice.RasterizerState = rs;

            fx.Parameters["Texture"].SetValue(Wire);
            fx.Parameters["Projection"].SetValue(camera.Projection);
            fx.Parameters["View"].SetValue(camera.View);
            fx.Parameters["World"].SetValue(Matrix.Identity);
            fx.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.Identity)));
            fx.Parameters["ViewVector"].SetValue(camera.Transform.Translation);

            foreach (EffectPass pass in fx.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, drawFences, 0, drawFences.Length / 3);
            }

            fx.Parameters["Texture"].SetValue(Post);
            foreach (EffectPass pass in fx.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, drawPosts, 0, drawPosts.Length / 3);
            }
        }

        Vector3 [] mesh01_coords = new Vector3[]
        {
            new Vector3(-0.1f, 0, -0.1f),
            new Vector3(0.1f, 0, -0.1f),
            new Vector3(0.1f, 5, -0.1f),
            new Vector3(-0.1f, 5, -0.1f),
            new Vector3(-0.1f, 0, 0.1f),
            new Vector3(-0.1f, 5, 0.1f),
            new Vector3(0.1f, 5, 0.1f),
            new Vector3(0.1f, 0, 0.1f),
            new Vector3(-0.1f, 0, -0.1f),
            new Vector3(-0.1f, 5, -0.1f),
            new Vector3(-0.1f, 5, 0.1f),
            new Vector3(-0.1f, 0, 0.1f),
            new Vector3(0.1f, 0, -0.1f),
            new Vector3(0.1f, 0, 0.1f),
            new Vector3(0.1f, 5, 0.1f),
            new Vector3(0.1f, 5, -0.1f),
            new Vector3(-0.1f, 5, -0.1f),
            new Vector3(0.1f, 5, -0.1f),
            new Vector3(0.1f, 5, 0.1f),
            new Vector3(-0.1f, 5, 0.1f),
            new Vector3(-0.1f, 0, -0.1f),
            new Vector3(-0.1f, 0, 0.1f),
            new Vector3(0.1f, 0, 0.1f),
            new Vector3(0.1f, 0, -0.1f)
        };

        Vector3 [] mesh01_normals = new Vector3[]
        {
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, -1, 0)
        };

        Vector2 [] mesh01_texcoords = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 0)
        };

        int [] mesh01_indices = new int[]
        {
            0, 2, 1,
            0, 3, 2,
            4, 6, 5,
            4, 7, 6,
            8, 10, 9,
            8, 11, 10,
            12, 14, 13,
            12, 15, 14,
            16, 18, 17,
            16, 19, 18,
            20, 22, 21,
            20, 23, 22
        };
    }
}
