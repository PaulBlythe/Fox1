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


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.IsFullScreen = false;
            Window.IsBorderless = true;
            Window.Position = new Point(0, 0);
            IsMouseVisible = true;

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
            //engine.SetScene(new Scenes.MainMenu());
            //engine.SetScene(new Scenes.LoadingScene());
            //engine.SetScene(new Scenes.Gebug.CarrierTest());
            //engine.SetScene(new Scenes.Campaign.WWII.British.PilotRecord());
            //engine.SetScene(new Scenes.Debug.ParticleEditorScene());
            //engine.SetScene(new Scenes.Developer.AircraftPhysicsTest());
            engine.SetScene(new Scenes.Developer.ObjectTester());

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

           
            base.Draw(gameTime);
        }
    }
}
