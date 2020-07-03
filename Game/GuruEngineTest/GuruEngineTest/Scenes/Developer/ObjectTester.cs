using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.World;
using GuruEngine.Rendering;
using GuruEngine.World.Items;
using GuruEngine.SceneManagement;
using GuruEngine.DebugHelpers;
using GuruEngine.ECS;
using GuruEngine.Cameras;
using GuruEngine.World.Developer;
using GuruEngine.ECS.Components;
using GuruEngine.Assets;
using GuruEngineTest.Scenes.Developer.Subs;

namespace GuruEngineTest.Scenes.Developer
{
    public class ObjectTester : Scene
    {
        WorldState worldState;
        World gameWorld;
        SpriteBatch batch;
        MouseState oldmousestate;
        SubScene sub;
        GameObject test_subject = null;

        public override void Load(ContentManager Content)
        {
            worldState = new WorldState();
            gameWorld = new World(@"WindTunnel");

            DebugCamera.SetPosition(new Vector3(20, 5, 0));
            DebugCamera.Instance.Yaw(MathHelper.ToRadians(90));
            switch (Renderer.GetSkyType())
            {
                case Skies.Traced:
                    {
                        WorldItem sky = new TracedSky();
                        worldState.AddWorldItem(sky);
                    }
                    break;
                default:
                    {
                        WorldItem sky = new Sky();
                        worldState.AddWorldItem(sky);
                    }
                    break;
            }
            WorldItem Stars = new Stars();
            worldState.AddWorldItem(Stars);

            WorldItem Planets = new Planets();
            worldState.AddWorldItem(Planets);

            WorldItem Moon = new Moon();
            worldState.AddWorldItem(Moon);

            WindTunnel tunnel = new WindTunnel();
            worldState.AddWorldItem(tunnel);

            sub = new LoadObject();
        }

        public override void Init()
        {
            batch = new SpriteBatch(Renderer.GetGraphicsDevice());
        }

        public override void Update(float dt)
        {
            gameWorld.Update(dt);
            worldState.Update(dt);
            if (sub != null)
            {
                sub.Update(dt);
                switch (sub.return_code)
                {
                    case 1:             // load game object
                        GameObjectManager.Instance.LoadGameObject(sub.result);
                        String name = Path.GetFileNameWithoutExtension(sub.result);
                        test_subject = GameObjectManager.Instance.CreateInstanceAt(name, new Vector3(0, 0, 0));
                        GameObjectManager.Instance.LoadContent(SceneManager.Instance.content);
                        
                        sub = null;
                        break;
                }
            }
        }

        public override void Draw(GameTime gt)
        {
            MouseState ms = Mouse.GetState();
            bool click = ((ms.LeftButton == ButtonState.Released) && (oldmousestate.LeftButton == ButtonState.Pressed));

            Renderer.GetCurrentRenderer().Draw(worldState, gt);

            if (sub != null)
                sub.Draw();

            oldmousestate = ms;
        }

        private void DrawButton(SpriteBatch batch, Rectangle r, MouseState ms, String s, bool click, int i)
        {
            if (r.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(r, Color.SlateBlue);
                
            }
            else
                batch.FillRectangle(r, Color.DarkSlateBlue);

            batch.DrawString(AssetManager.GetDebugFont(), s, new Vector2(r.X + 5, r.Y + 5), Color.White);
            batch.DrawRectangle(r, Color.White);
        }


    }
}
