using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.Rendering._3D
{
    public class StaticMesh
    {
        public Effect effect;
        public VertexBuffer verts;
        public Matrix World;
        public Vector4 Colour;
        public Vector4 Ambient;
        public Vector4 Specular;
        public CullMode cullMode;
        public float Shininess;
        public Texture2D texture;
        public int nverts;
        public BoundingBox Bounds;

        RasterizerState stat;

        public StaticMesh()
        {
            stat = new RasterizerState();
            stat.CullMode = CullMode.None;
        }

        public void Draw(Camera c, Vector4 Ambient, Vector4 Sun, Vector4 sunpos)
        {
            GraphicsDevice device = Game1.Instance.GraphicsDevice;
            device.RasterizerState = stat;
            device.DepthStencilState = DepthStencilState.Default;
            device.BlendState = BlendState.AlphaBlend;

            if (effect.Parameters["WorldViewProjection"] != null)
            {
                Matrix wvp = World * c.View * c.Projection;
                effect.Parameters["WorldViewProjection"].SetValue(wvp);
            }
            if (effect.Parameters["World"] != null)
            {
                effect.Parameters["World"].SetValue(World);
            }
            if (effect.Parameters["View"] != null)
            {
                effect.Parameters["View"].SetValue(c.View);
            }
            if (effect.Parameters["Projection"] != null)
            {
                effect.Parameters["Projection"].SetValue(c.Projection);
            }
            if (effect.Parameters["WorldInverseTranspose"] != null)
            {
                effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Transpose(World)));
            }
            if (effect.Parameters["ViewVector"] != null)
            {
                effect.Parameters["ViewVector"].SetValue(c.Transform.Translation);
            }
            if (effect.Parameters["Texture"] != null)
            {
                effect.Parameters["Texture"].SetValue(texture);
            }
            if (effect.Parameters["Colour"] != null)
            {
                effect.Parameters["Colour"].SetValue(Colour * Sun);
            }
            if (effect.Parameters["AmbientColour"] != null)
            {
                effect.Parameters["AmbientColour"].SetValue(new Vector3(Ambient.X,Ambient.Y,Ambient.Z));
            }
            if (effect.Parameters["SpecularColor"] != null)
            {
                effect.Parameters["SpecularColor"].SetValue(Specular);
            }
            if (effect.Parameters["Shininess"] != null)
            {
                effect.Parameters["Shininess"].SetValue(Shininess);
            }
            if (effect.Parameters["SunDirection"] != null)
            {
                effect.Parameters["SunDirection"].SetValue(new Vector3(sunpos.X, sunpos.Y, sunpos.Z));
            }


            Game1.Instance.GraphicsDevice.SetVertexBuffer(verts);
            Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.Instance.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.Instance.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, nverts / 3);
            }
        }

        BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }
    }
}
