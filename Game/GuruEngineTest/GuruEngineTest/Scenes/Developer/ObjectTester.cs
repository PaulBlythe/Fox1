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
using GuruEngine.ECS.Components.Animators.Aircraft.Standard;
using GuruEngineTest.Scenes.Developer.Helpers;
using GuruEngine.ECS.Components.Artillery;
using GuruEngine.ECS.Components.Animators.Generic;
using GuruEngine.Simulation.Weapons.AAA;

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
        MouseState oms;
        List<AnimatedVar> Variables = new List<AnimatedVar>();
        Rectangle back;
        Rectangle tab;
        bool camera_has_focus = true;

        FlakGunnerComponent fg = null;
        ArtilleryGunnerComponent ag = null;
        List<Rectangle> buttons = new List<Rectangle>();
        List<String> button_text = new List<string>();
        List<int> Actions = new List<int>();

        AnimatedVar current = null;
        Random rand = new Random();


        public override void Load(ContentManager Content)
        {
            worldState = new WorldState(new OrbitCamera(Renderer.GetGraphicsDevice().Viewport.AspectRatio));
            gameWorld = new World(@"WindTunnel");

            worldState.camera.SetPosition(new Vector3(20, 5, 0));
            worldState.camera.Yaw(MathHelper.ToRadians(90));
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

            GroundPlane gp = new GroundPlane();
            worldState.AddWorldItem(gp);

            sub = new LoadObject();
        }

        public override void Init()
        {
            batch = new SpriteBatch(Renderer.GetGraphicsDevice());
            ID = "ObjectTester".GetHashCode();

            int x1 = Renderer.GetGraphicsDevice().Viewport.X + (Renderer.GetGraphicsDevice().Viewport.Width / 2);
            x1 -= 200;
            int y1 = Renderer.GetGraphicsDevice().Viewport.Y + Renderer.GetGraphicsDevice().Viewport.Height;
            y1 -= 100;
            back = new Rectangle(x1, y1, 400, 32);
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
                        test_subject = GameObjectManager.Instance.CreateInstanceAt(name, new Vector3(0, 1, 0));
                        GameObjectManager.Instance.LoadContent(SceneManager.Instance.content);
                        ScanForAnimators();
                        sub = null;
                        break;
                }
            }
            MouseState ms = Mouse.GetState();
            if ((ms.LeftButton == ButtonState.Released) && (oms.LeftButton == ButtonState.Pressed))
            {
                for (int i=0; i<Variables.Count; i++)
                {
                    if (Variables[i].Over(ms.X,ms.Y))
                    {
                        if (Variables[i].Tracked)
                        {
                            Variables[i].Tracked = false;
                            current = null;
                        }
                        else
                        {
                            for (int j=0; j<Variables.Count; j++)
                            {
                                Variables[j].Tracked = false;
                            }
                            Variables[i].Tracked = true;
                            current = Variables[i];
                            
                        }
                    }
                }
            }
            if ((current!=null)&& (ms.LeftButton == ButtonState.Pressed))
            {
                if (tab.Contains(ms.X,ms.Y))
                {
                    if (camera_has_focus)
                    {
                        camera_has_focus = false;
                    }
                    
                }
            }
            if (!camera_has_focus)
            {
                if (ms.LeftButton == ButtonState.Released)
                    camera_has_focus = true;
                else
                {
                    int x = ms.X - back.X;
                    if (x > back.Width)
                    {
                        x = back.Width;
                    }
                    if (x < 0)
                    {
                        x = 0;
                    }
                    float sx = (float)x / (float)back.Width;
                    current.SetValueScaled(sx);
                }
            }
            if (current!=null)
            {
                current.Broadcast();
            }

            oms = ms;
            worldState.camera.HasFocus = camera_has_focus;
            worldState.camera.Update(dt);
        }

        public override void Draw(GameTime gt)
        {
            MouseState ms = Mouse.GetState();
            bool click = ((ms.LeftButton == ButtonState.Released) && (oldmousestate.LeftButton == ButtonState.Pressed));

            Renderer.GetCurrentRenderer().Draw(worldState, gt);

            if (sub != null)
                sub.Draw();

           
            batch.Begin();
            foreach (AnimatedVar av in Variables)
            {
                if (av.Tracked)
                    batch.FillRectangle(av.Region, Color.SlateBlue);
                else
                    batch.FillRectangle(av.Region, Color.DarkSlateBlue);

                batch.DrawRectangle(av.Region, Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), av.Name, new Vector2(av.Region.X + 5, av.Region.Y + 2), Color.White);
            }


            batch.FillRectangle(back, Color.Black);
            batch.DrawRectangle(back, Color.Orange);
            if (current != null)
            {
                float rv = current.ScaleValue();
                rv *= back.Width;
                tab = new Rectangle(back.X + (int)rv - 8, back.Y - 2, 16, back.Height + 4);
                batch.FillRectangle(tab, Color.Cornsilk);
                batch.DrawRectangle(tab, Color.White);
            }

            for (int i=0; i<buttons.Count; i++)
            {
                DrawButton(batch, buttons[i], ms, button_text[i], click, i);
                if ((click)&&(buttons[i].Contains(ms.X,ms.Y)))
                {
                    switch (Actions[i])
                    {
                        case 1:
                            {
                                int maxp = (int)(fg.gun.MaxPitch - fg.gun.MinPitch);
                                float tp = rand.Next(maxp) + fg.gun.MinPitch;
                                float tr = WeaponDataBase.GetAAAWeapon(fg.gun.Round.GetHashCode()).AimMaxDistance / 2;
                                float ta = (float)(tr * Math.Tan(MathHelper.ToRadians(tp)));
                                int maxy = (int)(fg.gun.MaxYaw - fg.gun.MinYaw);
                                float tb = rand.Next(maxy) + fg.gun.MinYaw;
                                if (tb < 0)
                                    tb += 360;
                                if (tb > 360)
                                    tb -= 360;

                                fg.HandleEvent(String.Format("AttackArea:{0}:{1}:{2}",tb, -ta, tr));
                            }
                            break;

                        case 2:
                            {
                                int maxp = (int)(ag.gun.MaxPitch - ag.gun.MinPitch);
                                float tp = rand.Next(maxp) + ag.gun.MinPitch;
                                float tr = WeaponDataBase.GetArtilleryWeapon(ag.gun.Round.GetHashCode()).AimMaxDistance / 2;
                                int maxy = (int)(ag.gun.MaxYaw - ag.gun.MinYaw);
                                float tb = rand.Next(maxy) + ag.gun.MinYaw;
                                if (tb < 0)
                                    tb += 360;
                                if (tb > 360)
                                    tb -= 360;

                                ag.HandleEvent(String.Format("AttackArea:{0}:{1}:{2}", tb, 0, tr));
                            }
                            break;
                    }
                }
            }


            batch.End();
            oldmousestate = ms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="r"></param>
        /// <param name="ms"></param>
        /// <param name="s"></param>
        /// <param name="click"></param>
        /// <param name="i"></param>
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

        /// <summary>
        /// 
        /// </summary>
        private void ScanForAnimators()
        {
            Variables.Clear();
            Rectangle r = new Rectangle(10, 50, 200, 28);
            AircraftStateComponent astate = (AircraftStateComponent)test_subject.FindSingleComponentByType<AircraftStateComponent>();
            AntiAircraftArtilleryComponent aaastate = (AntiAircraftArtilleryComponent)test_subject.FindSingleComponentByType<AntiAircraftArtilleryComponent>();
            ArtilleryComponent arty = (ArtilleryComponent)test_subject.FindSingleComponentByType<ArtilleryComponent>();

            List<ECSGameComponent> comps = test_subject.FindGameComponentByType<AileronsAnimatorComponent>();
            if (comps.Count>0)
            {
                AnimatedVar av = new AnimatedVar();
                av.Name = "AileronPosition";
                av.Min = -1;
                av.Max = 1;
                av.Value = 0;
                av.Region = r;
                av.astate = astate;
                av.aaastate = aaastate;
                Variables.Add(av);
                r.X += 220;
            }

            List<ECSGameComponent> trans = test_subject.FindGameComponentByType<TranslateAnimatorComponent>();
            foreach (ECSGameComponent ecs in trans)
            {
                TranslateAnimatorComponent t = (TranslateAnimatorComponent)ecs;
                AnimatedVar av = new AnimatedVar();
                av.Min = 0;
                av.Max = 1;
                av.Value = 0;
                av.Name = t.ControlVariable;
                av.astate = astate;
                av.aaastate = aaastate;
                av.Region = r;
                Variables.Add(av);
                r.X += 220;
                if (r.X > Renderer.GetGraphicsDevice().Viewport.Width - 210)
                {
                    r.X = 10;
                    r.Y += 32;
                }
            }

            trans = test_subject.FindGameComponentByType<PitchAnimatorComponent>();
            foreach (ECSGameComponent ecs in trans)
            {
                PitchAnimatorComponent t = (PitchAnimatorComponent)ecs;
                AnimatedVar av = new AnimatedVar();
                av.Min = 0;
                av.Max = 1;
                av.Value = 0;
                av.Name = t.Control;
                av.astate = astate;
                av.aaastate = aaastate;
                av.Region = r;
                Variables.Add(av);
                r.X += 220;
                if (r.X > Renderer.GetGraphicsDevice().Viewport.Width - 210)
                {
                    r.X = 10;
                    r.Y += 32;
                }
            }

            trans = test_subject.FindGameComponentByType<AntiAircraftArtilleryComponent>();
            foreach (ECSGameComponent ecs in trans)
            {
                AntiAircraftArtilleryComponent t = (AntiAircraftArtilleryComponent)ecs;
                AnimatedVar av = new AnimatedVar();
                av.Min = t.MinPitch;
                av.Max = t.MaxPitch;
                av.Value = 0;
                av.Name = "GunPitch";
                av.astate = astate;
                av.aaastate = aaastate;
                av.Region = r;
                Variables.Add(av);
                r.X += 220;
                if (r.X > Renderer.GetGraphicsDevice().Viewport.Width - 210)
                {
                    r.X = 10;
                    r.Y += 32;
                }

                av = new AnimatedVar();
                av.Min = t.MinYaw;
                av.Max = t.MaxYaw;
                av.Value = 0;
                av.Name = "GunYaw";
                av.astate = astate;
                av.aaastate = aaastate;
                av.Region = r;
                Variables.Add(av);
                r.X += 220;
                if (r.X > Renderer.GetGraphicsDevice().Viewport.Width - 210)
                {
                    r.X = 10;
                    r.Y += 32;
                }

                av = new AnimatedVar();
                av.Min = 0;
                av.Max = 1;
                av.Value = 0;
                av.Name = "Recoil";
                av.astate = astate;
                av.aaastate = aaastate;
                av.Region = r;
                Variables.Add(av);
                r.X += 220;
                if (r.X > Renderer.GetGraphicsDevice().Viewport.Width - 210)
                {
                    r.X = 10;
                    r.Y += 32;
                }
            }

            trans = test_subject.FindGameComponentByType<ArtilleryComponent>();
            foreach (ECSGameComponent ecs in trans)
            {
                ArtilleryComponent t = (ArtilleryComponent)ecs;
                AnimatedVar av = new AnimatedVar();
                av.Min = t.MinPitch;
                av.Max = t.MaxPitch;
                av.Value = 0;
                av.Name = "GunPitch";
                av.astate = astate;
                av.arty = arty;
                av.Region = r;
                Variables.Add(av);
                r.X += 220;
                if (r.X > Renderer.GetGraphicsDevice().Viewport.Width - 210)
                {
                    r.X = 10;
                    r.Y += 32;
                }

                av = new AnimatedVar();
                av.Min = t.MinYaw;
                av.Max = t.MaxYaw;
                av.Value = 0;
                av.Name = "GunYaw";
                av.astate = astate;
                av.aaastate = aaastate;
                av.arty = arty;
                av.Region = r;
                Variables.Add(av);
                r.X += 220;
                if (r.X > Renderer.GetGraphicsDevice().Viewport.Width - 210)
                {
                    r.X = 10;
                    r.Y += 32;
                }

                av = new AnimatedVar();
                av.Min = 0;
                av.Max = 1;
                av.Value = 0;
                av.Name = "Recoil";
                av.astate = astate;
                av.aaastate = aaastate;
                av.arty = arty;
                av.Region = r;
                Variables.Add(av);
                r.X += 220;
                if (r.X > Renderer.GetGraphicsDevice().Viewport.Width - 210)
                {
                    r.X = 10;
                    r.Y += 32;
                }
            }

            fg = (FlakGunnerComponent) test_subject.FindSingleComponentByType<FlakGunnerComponent>();
            if (fg!=null)
            {
                Rectangle rf = new Rectangle(40, 200, 200, 32);
                buttons.Add(rf);
                button_text.Add("Flak attack");
                Actions.Add(1);

            }

            ag = (ArtilleryGunnerComponent)test_subject.FindSingleComponentByType<ArtilleryGunnerComponent>();
            if (ag !=null)
            {
                Rectangle rf = new Rectangle(40, 200, 200, 32);
                buttons.Add(rf);
                button_text.Add("Barrage");
                Actions.Add(2);
            }
        }


    }
}
