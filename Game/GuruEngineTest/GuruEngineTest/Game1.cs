using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GuruEngine;
using GuruEngineTest.Scenes;
using GuruEngine.Assets;
using GuruEngine.World;
using GuruEngine.Maths;
using GuruEngine.Audio;
using GuruEngine.DebugHelpers;
using GuruEngine.Rendering;
using GuruEngine.Player.Records;
using GuruEngine.Player.Records.WWII.British;
using Microsoft.Xna.Framework.Content;

namespace GuruEngineTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static Game1 GameInstance;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Engine engine;
        public GameTime global_game_time;
        PlayerRecord playerRecord = new PlayerRecord();
        public String nextscene = "";
#if DEBUG
        FrameCounter framecounter;
#endif

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.IsFullScreen = false;
            graphics.SynchronizeWithVerticalRetrace = true;

            Window.IsBorderless = true;
            Window.Position = new Point(0, 0);
            IsMouseVisible = true;
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";
            GameInstance = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            engine = new Engine();
#if DEBUG
            framecounter = new FrameCounter();
#endif
            playerRecord.pilotsLog = new PlayerLog414();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

          
            engine.Initialise(GraphicsDevice, Content, Services, true);
            engine.SetScene(new Scenes.Debug.DebugSceneSelection());
            //engine.SetScene(new Scenes.MainMenu());
            //engine.SetScene(new Scenes.LoadingScene());
            //engine.SetScene(new Scenes.Gebug.CarrierTest());
            //engine.SetScene(new Scenes.Campaign.WWII.British.PilotRecord());
            //engine.SetScene(new Scenes.Debug.ParticleEditorScene());
            //engine.SetScene(new Scenes.Developer.AircraftPhysicsTest());
            //engine.SetScene(new Scenes.Developer.ObjectTester());

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            engine.ShutDown();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(nextscene != "")
            {
                Engine.SetScene(nextscene);
                nextscene = "";
            }
#if MULTI_THREADED
#else
            Engine.UpdateAll(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
#endif

#if DEBUG
            framecounter.Update(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
#endif

            global_game_time = gameTime;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            engine.Draw(gameTime);

#if DEBUG
            spriteBatch.Begin();
            Rectangle r = new Rectangle((1920 / 2) - 50, 5, 100, 32);
            spriteBatch.FillRectangle(r, Color.White);
            r.X += 2;
            r.Width -= 4;
            r.Y += 2;
            r.Height -= 4;
            spriteBatch.FillRectangle(r, Color.DarkBlue);

            if (!float.IsInfinity(framecounter.AverageFramesPerSecond))
            {
                String s = String.Format("{0:0.00}", framecounter.AverageFramesPerSecond);
                Vector2 sp = AssetManager.GetDebugFont().MeasureString(s);
                sp *= -0.5f;
                sp.X += (1920 / 2);
                sp.Y += 22;
                spriteBatch.DrawString(AssetManager.GetDebugFont(), s, sp, Color.White);
            }
            spriteBatch.End();
#endif

            base.Draw(gameTime);
        }
    }
}
