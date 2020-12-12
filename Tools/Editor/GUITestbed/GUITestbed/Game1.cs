using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GUITestbed.GUI;
using GUITestbed.GUI.Items;
using GUITestbed.Tools;
using GUITestbed.Rendering;
using GUITestbed.Rendering._3D;

namespace GUITestbed
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static string GISLocation = @"C:\Data\GIS\Natural_Earth_quick_start\";
        public static string SRTMLoocation = @"\\NFS1\FileStore\Data\SRTM\";

        public static Game1 Instance;
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        GuiManager gui;
        public Tool current = null;
        DebugLineDraw linedrawer;
        public DateTime GameDateTime;
        ShaderManager shaderManager;
        public SpriteFont debug_font;

        public Game1()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
            Content.RootDirectory = "Content";

            Window.IsBorderless = true;
            Window.Position = new Point(0, 0);
            IsMouseVisible = true;

            GameDateTime = new DateTime(1942, 6, 6, 12, 0, 0);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            shaderManager = new ShaderManager();
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
            gui = new GuiManager(GraphicsDevice, Content, new GuiTheme());
            gui.Add(new MainMenu());

            linedrawer = new DebugLineDraw();
            ShaderManager.Preload("Shaders/DiffuseFog");
            debug_font = Content.Load<SpriteFont>("debug");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            gui.Update(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            if (current!=null)
            {
                current.Update(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (current != null)
            {
                current.Draw();
            }
            gui.Draw();
            
           
            base.Draw(gameTime);
        }
    }
}
