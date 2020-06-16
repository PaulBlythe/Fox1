using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GUITestbed.Rendering._3D
{
    public class PointLight
    {
        public Vector3 Position;
        public float Size;
        public Color Colour;

        public PointLight(Vector3 p, float s, Color c)
        {
            Position = p;
            Size = s;
            Colour = c;
        }
    }

    public class LightManager
    {
        public static LightManager Instance;
        public List<PointLight> PointLights = new List<PointLight>();
        Texture2D texture;
        BasicEffect effect;
        SpriteBatch batch;
        Matrix world;

        public LightManager()
        {
            Instance = this;
        }

        public void Load(GraphicsDevice dev, ContentManager c)
        {
            effect = new BasicEffect(dev);
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;
            effect.DiffuseColor = Color.White.ToVector3();
            effect.AmbientLightColor = Color.White.ToVector3();

            world = Matrix.CreateScale(1, -1, 1);
            batch = new SpriteBatch(dev);
            texture = c.Load<Texture2D>("Textures/light");
        }

        public static void AddPointLight(Vector3 p, float s, Color c)
        {
            PointLight pt = new PointLight(p, s, c);
            LightManager.Instance.PointLights.Add(pt);
        }

        public void Draw(Camera c)
        {
            effect.World = world;
            effect.View = Matrix.Identity;
            effect.Projection = c.Projection;

            Matrix mw = c.View * world;
            Vector2 Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            batch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, DepthStencilState.None, RasterizerState.CullNone, effect);
            for (int i = 0; i < PointLights.Count; i++)
            {
                Vector3 worldspace = PointLights[i].Position;
                Vector3 viewSpaceTextPosition = Vector3.Transform(worldspace, mw);
                batch.Draw(texture, new Vector2(viewSpaceTextPosition.X, viewSpaceTextPosition.Y), null, PointLights[i].Colour, 0.0f, Origin, PointLights[i].Size, SpriteEffects.None, viewSpaceTextPosition.Z);
            }
            batch.End();
        }
    }
}
