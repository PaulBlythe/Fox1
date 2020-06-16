using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.SceneManagement;
using GuruEngine.Rendering;

namespace GuruEngineTest.Scenes
{
    public class LoadingScene : Scene
    {
        GraphicsDevice device;
        Effect backdrop;
        Texture2D Noise;
        Texture2D Aircraft;
        SpriteBatch batch;
        Rectangle fs;
        float time = 0;
        float progress; // 0-1

        Vector2 aircraft_pos = new Vector2();
        Vector2 origin = new Vector2(64, 24);
        Rectangle src = new Rectangle(0, 0, 128, 47);
        float rotation = 0;
        float width;
        float height;

        public override void Init()
        {
            device = Renderer.GetGraphicsDevice();
            batch = new SpriteBatch(device);
            fs = new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height);

            progress = 0;

            aircraft_pos.X = 67;
            aircraft_pos.Y = device.Viewport.Height - 80;

            width = device.Viewport.Width - 200;
            height = device.Viewport.Height - 80;
        }

        public override void Load(ContentManager Content)
        {
            Noise = Content.Load<Texture2D>(@"Textures\noise");
            backdrop = Content.Load<Effect>(@"Shaders\2D\Loading");
            Aircraft = Content.Load<Texture2D>(@"Textures\loading_aircraft");

            backdrop.Parameters["aspectratio"].SetValue(device.Viewport.AspectRatio);
            backdrop.Parameters["iResolution"].SetValue(new Vector2(device.Viewport.Width, device.Viewport.Height));
        }

        public override void Draw(GameTime gt)
        {
            batch.Begin(0, BlendState.Opaque, null, null, null, backdrop);
            batch.Draw(Noise, fs, Color.White);
            batch.End();

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);         
            batch.Draw(Aircraft, aircraft_pos, src, Color.White, rotation, origin, 1, SpriteEffects.None, 1);
            batch.End();
        }

        public override void Update(float dt)
        {
            time += dt;
            backdrop.Parameters["iTime"].SetValue(time);

            aircraft_pos.X = 67 + (width * progress);
            if ((progress > 0.15)&&(progress<0.25))
            {
                float t = (progress - 0.15f) * 10;
                float da = MathHelper.SmoothStep(0, 8, t);
                rotation = MathHelper.ToRadians(da);
            }
            if ((progress > 0.4)&&(progress<0.5))
            {
                float t = (progress - 0.4f) * 10;
                aircraft_pos.Y = MathHelper.SmoothStep(height, height-30, t);
            }

            if ((progress > 0.5) && (progress < 1.0))
            {
                float t = (progress - 0.5f) * 2;
                aircraft_pos.Y = MathHelper.SmoothStep(height-30, height - 250, t);
                rotation = MathHelper.ToRadians(MathHelper.SmoothStep(8, -12, t));
            }

            progress += 0.03f * dt;
        }
    }
}
