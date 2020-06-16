using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.SceneManagement;
using GuruEngine.InputDevices;
using GuruEngine.AI;
using GuruEngine.Simulation.Weapons.Ammunition;
using GuruEngine.Simulation.Weapons.AAA;
using GuruEngine.Assets;
using GuruEngine.World;
using GuruEngine.DebugHelpers;
using GuruEngine.Localization;
using GuruEngine.Localization.Strings;
using GuruEngine.Audio;

namespace GuruEngine
{
    public class Engine
    {
        public static Engine Instance;

        Renderer renderer;
        Settings settings;
        WeaponDataBase weps = new WeaponDataBase();
        AmmunitionDatabase ammo = new AmmunitionDatabase();
        AIManager ai = new AIManager();
        SceneManager sceneManager;
        AssetManager assetManager;
        InputDeviceManager inputDeviceManager;
        StringLocalizer stringLocaliser;
        AudioManager audioManager;
        
        bool loaded = false;

#if DEBUG
        LogHelper logger = new LogHelper();
#endif
#if PROFILE
        Profiler profiler = new Profiler();
        DebugRenderer debug_render = new DebugRenderer();
#endif

        public void Initialise(GraphicsDevice device, ContentManager content, IServiceProvider serviceProvider, bool ForwardRenderer)
        {
            Instance = this;
            renderer = new Renderer(device, ForwardRenderer);
            assetManager = new AssetManager(device, serviceProvider, "Content");
           
            sceneManager = new SceneManager(content);
            inputDeviceManager = new InputDeviceManager();
            ai = new AIManager();
            ammo = new AmmunitionDatabase();
            weps = new WeaponDataBase();
            audioManager = new AudioManager(content);
            settings = new Settings();
            stringLocaliser = new StringLocalizer(Settings.Language);
           

            renderer.Initialise();

            Task.Factory.StartNew(() =>
            {
                var gl = new UpdateLoop(renderer);
                gl.Loop();
            });

        }

        public void SetScene(Scene scene)
        {
            sceneManager.SetScene(scene);

            loaded = true;
        }

        public void ShutDown()
        {
            settings.Save();
            assetManager.Shutdown();
            inputDeviceManager.CloseDown();
        }

        public void UpdateScenes(float dt)
        {
            if (!loaded)
            {
                return;
            }
            inputDeviceManager.Update();
            sceneManager.Update(dt);
            audioManager.Update();
        }

        public void Draw(GameTime gameTime)
        {
            if (!loaded)
            {
                return;
            }
#if PROFILE
            Profiler.Start("Game render");
#endif
            sceneManager.Draw(gameTime);

#if PROFILE
            Profiler.End("Game render");
#endif

        }

        public void EndDraw(GameTime gameTime)
        {
#if PROFILE
            DebugRenderer.Render(spriteBatch);
            Profiler.Clear();
#endif
        }

        #region Static methods
        public static void UpdateAll(float dt)
        {
            Instance.UpdateScenes(dt);
            
        }

        public static void EndDrawFrame(GameTime gameTime)
        {
            Instance.EndDraw(gameTime);
        }
        #endregion
    }
}
