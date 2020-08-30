using System;
using System.Collections.Generic;

using GuruEngine.World;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine;
using GuruEngine.Rendering;
using GuruEngine.Rendering.Particles;
using GuruEngine.World.Items;
using GuruEngine.World.Developer;
using GuruEngine.SceneManagement;
using GuruEngine.Assets;
using GuruEngine.DebugHelpers;
using GuruEngine.ECS;
using GuruEngine.ECS.Components.Effects;
using GuruEngine.ECS.Components.World;
using GuruEngine.Cameras;

namespace GuruEngineTest.Scenes.Debug
{
    public class ParticleEditorScene : Scene
    {
        WorldState worldState;
        SpriteBatch batch;

        Rectangle LoadButton = new Rectangle(8, 809, 90, 29);
        Rectangle SaveButton = new Rectangle(338, 809, 90, 29);
        Rectangle ResetButton = new Rectangle(168, 809, 90, 29);
        Rectangle ChangeButton = new Rectangle(358, 19, 90, 29);

        Rectangle ChangeMinColor = new Rectangle(430, 54 + (16 * 30), 16, 16);
        Rectangle ChangeMaxColor = new Rectangle(430, 54 + (17 * 30), 16, 16);

        ParticleSettings settings = null;

        MouseState oldmousestate;

        DebugFileBrowser fb = null;
        DebugColourPicker cb = null;

        ParticleEmitterComponent p_emitter = null;

        GameObject gameObject = null;
        World world;

        public override void Init()
        {
            world = new World("ParticleEditor");
            worldState = new WorldState();
            batch = new SpriteBatch(Renderer.GetGraphicsDevice());
            
        }

        public override void Load(ContentManager Content)
        {
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

            worldState.camera.SetPosition(new Vector3(0,0,10));
        }

        public override void Update(float dt)
        {
            world.Update(dt);
            worldState.Update(dt);
            
        }

        public override void Draw(GameTime gt)
        {
            MouseState ms = Mouse.GetState();
            bool click = ((ms.LeftButton == ButtonState.Released) && (oldmousestate.LeftButton == ButtonState.Pressed));

            Renderer.GetCurrentRenderer().Draw(worldState, gt);

            batch.Begin();
            batch.FillRectangle(5, 5, 450, 980, Color.FromNonPremultiplied(128, 128, 128, 64));

            batch.DrawString(AssetManager.GetDebugFont(), "Texture", new Vector2(10, 20), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Max particles", new Vector2(10, 50), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Lifetime mS", new Vector2(10, 80), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Duration Randomness", new Vector2(10, 110), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Velocity Sensitivity", new Vector2(10, 140), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Min Horizontal V", new Vector2(10, 170), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Max Horizontal V", new Vector2(10, 200), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Min Vertical V", new Vector2(10, 230), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Max Vertical V", new Vector2(10, 260), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Gravity Sensitivity", new Vector2(10, 290), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "End Velocity", new Vector2(10, 320), Color.White);         
            batch.DrawString(AssetManager.GetDebugFont(), "Min Rotation", new Vector2(10, 350), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Max Rotation", new Vector2(10, 380), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Min Start Size", new Vector2(10, 410), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Max Start Size", new Vector2(10, 440), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Min End Size", new Vector2(10, 470), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Max End Size", new Vector2(10, 500), Color.White);

            batch.DrawString(AssetManager.GetDebugFont(), "Min Colour", new Vector2(10, 530), Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Max Colour", new Vector2(10, 560), Color.White);

            if (settings != null)
            {
                batch.DrawString(AssetManager.GetDebugFont(), settings.ShortTextureName, new Vector2(120, 20), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0}", settings.MaxParticles), new Vector2(320, 50), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0}", settings.LifeTime), new Vector2(320, 80), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.DurationRandomness), new Vector2(320, 110), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.EmitterVelocitySensitivity), new Vector2(320, 140), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MinHorizontalVelocity), new Vector2(320, 170), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MaxHorizontalVelocity), new Vector2(320, 200), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MinVerticalVelocity), new Vector2(320, 230), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MaxVerticalVelocity), new Vector2(320, 260), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.GravitySensitivity), new Vector2(320, 290), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.EndVelocity), new Vector2(320, 320), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MinRotateSpeed), new Vector2(320, 350), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MaxRotateSpeed), new Vector2(320, 380), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MinStartSize), new Vector2(320, 410), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MaxStartSize), new Vector2(320, 440), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MinEndSize), new Vector2(320, 470), Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), String.Format("{0:0.0000}", settings.MaxEndSize), new Vector2(320, 500), Color.White);

                batch.FillRectangle(new Rectangle(280, 532, 60, 26), settings.MinColor);
                batch.FillRectangle(new Rectangle(280, 562, 60, 26), settings.MaxColor);
            }

            if (ChangeButton.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(ChangeButton, Color.DarkGray);
            }
            batch.DrawString(AssetManager.GetDebugFont(), "Change", new Vector2(360, 20), Color.White);
            batch.DrawRectangle(ChangeButton, Color.White);

            if (LoadButton.Contains(ms.X,ms.Y))
            {
                batch.FillRectangle(LoadButton, Color.DarkGray);
                if (click)
                {
                    fb = new DebugFileBrowser(FilePaths.DataPath + "ParticleSystems", "*.xml");
                }
            }
            batch.DrawString(AssetManager.GetDebugFont(), "Load", new Vector2(25, 810), Color.White);
            batch.DrawRectangle(LoadButton, Color.White);

            if (SaveButton.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(SaveButton, Color.DarkGray);
            }
            batch.DrawString(AssetManager.GetDebugFont(), "Save", new Vector2(355, 810), Color.White);
            batch.DrawRectangle(SaveButton, Color.White);

            if (ResetButton.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(ResetButton, Color.DarkGray);
            }
            batch.DrawString(AssetManager.GetDebugFont(), "Reset", new Vector2(180, 810), Color.White);
            batch.DrawRectangle(ResetButton, Color.White);

            if (ChangeMaxColor.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(ChangeMaxColor, Color.DarkGray);
                if ((click) && (cb == null))
                {
                    cb = new DebugColourPicker(settings.MaxColor);
                }
            }         
            batch.DrawRectangle(ChangeMaxColor, Color.White);

            if (ChangeMinColor.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(ChangeMinColor, Color.DarkGray);
            }
            batch.DrawRectangle(ChangeMinColor, Color.White);

            Rectangle r = new Rectangle(280, 54, 16, 16);
            Rectangle r2 = new Rectangle(430, 54, 16, 16);
            for (int i = 0; i < 16; i++)
            {
                if (r.Contains(ms.X, ms.Y))
                {
                    batch.FillRectangle(r, Color.DarkGray);
                    if (click)
                    {
                        switch (i)
                        {
                            case 0:
                                settings.MaxParticles--;
                                break;

                            case 1:
                                settings.LifeTime = settings.LifeTime - 1.0f;
                                break;

                            case 2:
                                settings.DurationRandomness -=0.05f;
                                break;

                            case 3:
                                settings.EmitterVelocitySensitivity -= 0.05f;
                                break;

                            case 4:
                                settings.MinHorizontalVelocity -= 0.05f;
                                break;

                            case 5:
                                settings.MaxHorizontalVelocity -= 0.05f;
                                break;

                            case 6:
                                settings.MinVerticalVelocity -= 0.05f;
                                break;

                            case 7:
                                settings.MaxVerticalVelocity -= 0.05f;
                                break;

                            case 8:
                                settings.GravitySensitivity -= 0.05f;
                                break;

                            case 9:
                                settings.EndVelocity -= 0.05f;
                                break;

                            case 10:
                                settings.MinRotateSpeed -= 0.05f;
                                break;

                            case 11:
                                settings.MaxRotateSpeed -= 0.05f;
                                break;

                            case 12:
                                settings.MinStartSize -= 0.05f;
                                break;
                            case 13:
                                settings.MaxStartSize -= 0.05f;
                                break;
                            case 14:
                                settings.MinEndSize -= 0.05f;
                                break;
                            case 15:
                                settings.MaxEndSize -= 0.05f;
                                break;
                        }
                        Renderer.GetCurrentRenderer().current.GetParticleSystem(p_emitter.psystem).Reload();
                    }
                }

                if (r2.Contains(ms.X, ms.Y))
                {
                    batch.FillRectangle(r2, Color.DarkGray);
                    if (click)
                    {
                        
                        switch (i)
                        {
                            case 0:
                                settings.MaxParticles++;
                                break;
                            case 1:
                                settings.LifeTime = settings.LifeTime + 1.0f;
                                break;
                            case 2:
                                settings.DurationRandomness += 0.05f;
                                break;
                            case 3:
                                settings.EmitterVelocitySensitivity += 0.05f;
                                break;
                            case 4:
                                settings.MinHorizontalVelocity += 0.05f;
                                break;
                            case 5:
                                settings.MaxHorizontalVelocity += 0.05f;
                                break;
                            case 6:
                                settings.MinVerticalVelocity += 0.05f;
                                break;
                            case 7:
                                settings.MaxVerticalVelocity += 0.05f;
                                break;
                            case 8:
                                settings.GravitySensitivity += 0.05f;
                                break;
                            case 9:
                                settings.EndVelocity += 0.05f;
                                break;
                            case 10:
                                settings.MinRotateSpeed += 0.05f;
                                break;
                            case 11:
                                settings.MaxRotateSpeed += 0.05f;
                                break;
                            case 12:
                                settings.MinStartSize += 0.05f;
                                break;
                            case 13:
                                settings.MaxStartSize += 0.05f;
                                break;
                            case 14:
                                settings.MinEndSize += 0.05f;
                                break;
                            case 15:
                                settings.MaxEndSize += 0.05f;
                                break;
                        }
                        Renderer.GetCurrentRenderer().current.GetParticleSystem(p_emitter.psystem).Reload();
                    }
                }

                batch.DrawRectangle(r, Color.White);
                batch.DrawRectangle(r2, Color.White);
                r.Y += 30;
                r2.Y += 30;
            }


            #region File browser
            if (fb != null)
            {
                fb.Draw(batch);
                switch (fb.ResultCode)
                {
                    case 1:
                        fb = null;
                        break;

                    case 2:
                        if (fb.SelectedEntry != null)
                        {
                            if (gameObject == null)
                            {
                                gameObject = World.Instance.CreateEmptyGameObjectAt("EditorEmitter", Vector3.Zero);
                            }
                            settings = new ParticleSettings();
                            settings.Load(fb.SelectedEntry.Path);
                            if (gameObject.Components.ContainsKey("ParticleEmitter"))
                            {
                                gameObject.Components.Remove("ParticleEmitter");
                            }
                            p_emitter = new ParticleEmitterComponent();
                            p_emitter.Name = "ParticleEmitter";
                            p_emitter.psettings = settings;
                            p_emitter.host = (WorldTransform) gameObject.FindGameComponentByName("WorldTransform_1");
                            p_emitter.Load(null);
                            p_emitter.rate = 2;
                            p_emitter.Mode = "Continuous";

                            gameObject.Components.Add("ParticleEmitter", p_emitter);
                        }
                        fb = null;
                        break;
                }
            }
            #endregion

            if (cb != null)
            {
                cb.Draw(batch);
                switch (cb.return_code)
                {
                    case 1:
                        cb = null;
                        break;

                    case 2:
                        cb = null;
                        break;
                }
            }

            batch.End();

            oldmousestate = ms;
        }
    }
}
