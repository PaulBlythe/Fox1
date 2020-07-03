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
    public class AircraftPhysicsTest : Scene
    {
        WorldState worldState;
        World gameWorld;
        SpriteBatch batch;
        MouseState oldmousestate;
        SubScene sub;
        GameObject test_subject = null;
        AircraftStateComponent aircraft_state = null;

        Rectangle StartButton = new Rectangle(1210, 10, 200, 40);
        Rectangle StepButton = new Rectangle(1430, 10, 200, 40);
        Rectangle StopButton = new Rectangle(1650, 10, 200, 40);
        Rectangle ScriptWindow = new Rectangle(10, 60, 400, 700);
        Rectangle clipRect = new Rectangle(12, 62, 396, 696);
        Rectangle StartEngine = new Rectangle(10, 950, 200, 40);

        List<Rectangle> CheckBoxes = new List<Rectangle>();
        List<String> CheckText = new List<string>();
        List<bool> Checked = new List<bool>();

        bool paused = true;
        bool step = false;
        RasterizerState clipped;


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

            sub = new LoadAircraft();

            CheckBoxes.Add(new Rectangle(1700, 100, 32, 32));
            CheckText.Add("Fuel tanks");
            Checked.Add(false);
            CheckBoxes.Add(new Rectangle(1700, 140, 32, 32));
            CheckText.Add("Gears");
            Checked.Add(false);
            CheckBoxes.Add(new Rectangle(1700, 180, 32, 32));
            CheckText.Add("Masses");
            Checked.Add(false);
            CheckBoxes.Add(new Rectangle(1700, 220, 32, 32));
            CheckText.Add("Forces");
            Checked.Add(false);

            clipped = new RasterizerState();
            clipped.ScissorTestEnable = true;
        }

        public override void Init()
        {
            batch = new SpriteBatch(Renderer.GetGraphicsDevice());
        }

        public override void Update(float dt)
        {
            if (!step)
            {
                if (paused)
                    dt = 0;
            }
            else
            {
                step = false;
            }

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
                        test_subject = GameObjectManager.Instance.CreateInstanceAt(name, new Vector3(0, 3, 0));
                        GameObjectManager.Instance.LoadContent(SceneManager.Instance.content);
                        aircraft_state = (AircraftStateComponent)test_subject.FindSingleComponentByType<AircraftStateComponent>();
                        aircraft_state.SetVar("GearPosition", 1.0);
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

            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawButton(batch, StartButton, ms, "Start",click,0);
            DrawButton(batch, StepButton, ms, "Step",click,1);
            DrawButton(batch, StopButton, ms, "Stop",click,2);

            for (int i = 0; i < CheckBoxes.Count; i++)
            {
                Rectangle r = CheckBoxes[i];
                if (r.Contains(ms.X, ms.Y))
                {
                    batch.FillRectangle(r, Color.SlateBlue);
                    if (click)
                    {
                        Checked[i] = !Checked[i];
                        switch (i)
                        {
                            case 0:
                                DebugRenderSettings.DrawFuelTanks = !DebugRenderSettings.DrawFuelTanks;
                                break;
                            case 1:
                                DebugRenderSettings.DrawGears = !DebugRenderSettings.DrawGears;
                                break;
                            case 2:
                                DebugRenderSettings.DrawMasses = !DebugRenderSettings.DrawMasses;
                                break;
                            case 3:
                                DebugRenderSettings.DrawForces = !DebugRenderSettings.DrawForces;
                                break;
                        }
                    }
                }
                else
                    batch.FillRectangle(r, Color.DarkSlateBlue);

                batch.DrawRectangle(r, Color.White);

                if (Checked[i])
                {
                    Rectangle r2 = new Rectangle(r.X + 2, r.Y + 2, r.Width - 4, r.Height - 4);
                    batch.FillRectangle(r2, Color.SeaShell);
                }
                Vector2 pos = new Vector2(r.X + 40, r.Y + 5);
                batch.DrawString(AssetManager.GetDebugFont(), CheckText[i], pos, Color.White);

                
            }
            batch.FillRectangle(ScriptWindow, Color.DarkSlateBlue);
            batch.DrawRectangle(ScriptWindow, Color.White);

            if (test_subject != null)
            {
                DrawButton(batch, StartEngine, ms, "Start engine", click, 10);
            }

            batch.End();

            RasterizerState old = batch.GraphicsDevice.RasterizerState;

            batch.GraphicsDevice.RasterizerState = clipped;
            batch.GraphicsDevice.ScissorRectangle = clipRect;
            batch.Begin();
            Vector2 dpos = new Vector2(clipRect.X + 1, clipRect.Y + 5);
            DebugMessage[] messages = DebugMessageQueue.Instance.messages.ToArray();
            for (int i=0; i<DebugMessageQueue.Instance.messages.Count; i++)
            {
                batch.DrawString(AssetManager.GetDebugFont(), messages[i].Text, dpos, Color.Gray);
                dpos.Y += 25;
            }
            batch.End();
            batch.GraphicsDevice.RasterizerState = old;

            oldmousestate = ms;
        }

        private void DrawButton(SpriteBatch batch, Rectangle r, MouseState ms, String s, bool click, int i)
        {
            if (r.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(r, Color.SlateBlue);
                if (click)
                {
                    switch (i)
                    {
                        case 0:
                            paused = false;
                            break;
                        case 1:
                            step = true;
                            break;
                        case 2:
                            paused = true;
                            break;

                        ///==================================
                        /// Script events
                        ///==================================
                        case 10:
                            {
                                AircraftStateComponent ass = (AircraftStateComponent) test_subject.FindSingleComponentByType<AircraftStateComponent>();
                                ass.RunScript("Engine start");
                            }
                            break;
                    }
                }
            }
            else
                batch.FillRectangle(r, Color.DarkSlateBlue);

            batch.DrawString(AssetManager.GetDebugFont(), s, new Vector2(r.X + 5, r.Y + 5), Color.White);
            batch.DrawRectangle(r, Color.White);
        }

    }
}
