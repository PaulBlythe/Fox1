using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.Text;

namespace GuruEngine.Rendering.Gui
{
    public class GuiManager
    {
        public static GuiManager Instance;

        GraphicsDevice device;
        List<Gui> activeGuis = new List<Gui>();

        Texture2D guiTexture;
        Texture2D verdanaBold;
        Texture2D verdanaSmall;
        Effect effect;
        Effect fontshader;

        int lastWheel;
        GlyphBatch batch;
        Dictionary<FontStyle, SDFFont> Fonts = new Dictionary<FontStyle, SDFFont>();

        #region Configuration variables
        public int[] FrameSource = new int[5];
        public int[] ButtonSource = new int[12];
        public int[] SmallButtonSource = new int[12];
        public float GuiTextureSize;
        #endregion

        public enum GuiIdentifiers
        {
            MapPositionWidget,
            PauseMenuWidget,
            OptionsMenuWidget,
            DeveloperToolsWidget,
            Total
        }
        public enum FontStyle
        {
            Title,
            Medium,
            Small,
            Smaller,
            Total
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Device"></param>
        public GuiManager(GraphicsDevice Device)
        {
            device = Device;
            Instance = this;
            lastWheel = 0;
           
        }

        /// <summary>
        /// Load any textures and shaders
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            guiTexture = content.Load<Texture2D>(@"Textures\Gui\gui");
            GuiTextureSize = guiTexture.Width;
            effect = content.Load<Effect>(@"Shaders\2D\Sprite");
            verdanaBold = content.Load<Texture2D>("Textures/Fonts/VerdanaBold");
            fontshader = content.Load<Effect>("Shaders/2D/SDFFont2");
            verdanaSmall = content.Load<Texture2D>("Textures/Fonts/Verdana");
            LoadFonts();
            LoadConfigs();
            batch = new GlyphBatch(device,guiTexture,effect,Color.White);
        }

        /// <summary>
        /// System update
        /// </summary>
        /// <param name="dt"></param>
        public void Update(float dt)
        {
            MouseState ms = Mouse.GetState();
            Vector2 mp = new Vector2(ms.X, ms.Y);
            int mb = 0;
            if (ms.LeftButton == ButtonState.Pressed)
                mb += 1;
            if (ms.RightButton == ButtonState.Pressed)
                mb += 2;
            if (ms.MiddleButton == ButtonState.Pressed)
                mb += 4;

            int dw = ms.ScrollWheelValue - lastWheel;
            lastWheel = ms.ScrollWheelValue;

            foreach(Gui g in activeGuis)
            {
                g.Update(mp, mb, dt, dw);
            }
        }

        public void Draw()
        {
            batch.StartSprite(guiTexture, effect, Color.White);
            foreach(Gui g in activeGuis)
            {
                g.Draw(batch);
            }
            batch.End();

            foreach (Gui g in activeGuis)
            {
                g.Draw();
            }
        }

        /// <summary>
        /// Add a new gui 
        /// </summary>
        /// <param name="gui">ID of the new gui</param>
        public void AddGui(GuiIdentifiers gui, Vector4 position)
        {
            Gui g = new Gui();
            g.Load(gui, position);
            activeGuis.Add(g);
        }

        /// <summary>
        /// Remove a gui from the list
        /// </summary>
        /// <param name="gui">ID of the gui to remove</param>
        public void RemoveGui(GuiIdentifiers gui)
        {
            for (int i=0; i<activeGuis.Count; i++ )
            {
                if (activeGuis[i].ID == gui)
                {
                    activeGuis.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Remove all guis
        /// </summary>
        public void Clear()
        {
            foreach (Gui g in activeGuis)
            {
                g.UnLoad();
            }

            activeGuis.Clear();
        }

        #region Scaling
        /// <summary>
        /// Gui's are designed at 1920 by 1080. We need to scale them to fit the actual screen dimensions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector2 ScalePosition(float x, float y)
        {
            return new Vector2((x * device.Viewport.Width) / 1920.0f, (y * device.Viewport.Height) / 1080.0f);
        }

        public float ScaleWidth(float x)
        {
            return (x * device.Viewport.Width) / 1920.0f;
        }
        #endregion

        #region Configuration file handling
        void LoadConfigs()
        {
            #region Read in the frame definition
            using (TextReader reader = File.OpenText(@"Content\Gui\Frame.txt"))
            {
                String l = reader.ReadLine();
                String[] parts = l.Split(' ');
                FrameSource[0] = int.Parse(parts[0]);
                FrameSource[1] = int.Parse(parts[1]);

                l = reader.ReadLine();
                parts = l.Split(' ');
                FrameSource[2] = int.Parse(parts[0]);
                FrameSource[3] = int.Parse(parts[1]);

                l = reader.ReadLine();
                parts = l.Split(' ');
                FrameSource[4] = int.Parse(parts[0]);
            }
            #endregion

            #region Read in the button definition file
            using (TextReader reader = File.OpenText(@"Content\Gui\Button.txt"))
            {

                // Read in the button definition

                String l = reader.ReadLine();
                String[] parts = l.Split(' ');

                ButtonSource[0] = int.Parse(parts[0]);
                ButtonSource[1] = int.Parse(parts[1]);
                ButtonSource[2] = int.Parse(parts[2]);
                ButtonSource[3] = int.Parse(parts[3]);

                l = reader.ReadLine();
                parts = l.Split(' ');
                ButtonSource[4] = int.Parse(parts[0]);
                ButtonSource[5] = int.Parse(parts[1]);
                ButtonSource[6] = int.Parse(parts[2]);
                ButtonSource[7] = int.Parse(parts[3]);

                l = reader.ReadLine();
                parts = l.Split(' ');
                ButtonSource[8] = int.Parse(parts[0]);
                ButtonSource[9] = int.Parse(parts[1]);
                ButtonSource[10] = int.Parse(parts[2]);
                ButtonSource[11] = int.Parse(parts[3]);

            }
            #endregion

            #region Read in the small button definition file
            using (TextReader reader = File.OpenText(@"Content\Gui\SmallButton.txt"))
            {

                // Read in the button definition

                String l = reader.ReadLine();
                String[] parts = l.Split(' ');

                SmallButtonSource[0] = int.Parse(parts[0]);
                SmallButtonSource[1] = int.Parse(parts[1]);
                SmallButtonSource[2] = int.Parse(parts[2]);
                SmallButtonSource[3] = int.Parse(parts[3]);

                l = reader.ReadLine();
                parts = l.Split(' ');
                SmallButtonSource[4] = int.Parse(parts[0]);
                SmallButtonSource[5] = int.Parse(parts[1]);
                SmallButtonSource[6] = int.Parse(parts[2]);
                SmallButtonSource[7] = int.Parse(parts[3]);

                l = reader.ReadLine();
                parts = l.Split(' ');
                SmallButtonSource[8] = int.Parse(parts[0]);
                SmallButtonSource[9] = int.Parse(parts[1]);
                SmallButtonSource[10] = int.Parse(parts[2]);
                SmallButtonSource[11] = int.Parse(parts[3]);

            }
            #endregion
        }
        #endregion

        #region Font management
        public SDFFont GetFont(FontStyle style)
        {
            return Fonts[style];
        }

        /// <summary>
        /// Load in all the signed distance field fonts
        /// </summary>
        void LoadFonts()
        {
            float scale = device.Viewport.Width / 1920.0f;

            SDFFont f = new SDFFont("Fonts/VerdanaBold.txt");
            f.Setup(verdanaBold, fontshader, Color.White, scale * 0.25f);
            Fonts.Add(FontStyle.Medium, f);

            f = new SDFFont("Fonts/VerdanaBold.txt");
            f.Setup(verdanaBold, fontshader, Color.White, scale * 0.45f);
            Fonts.Add(FontStyle.Title, f);

            f = new SDFFont("Fonts/Verdana.txt");
            f.Setup(verdanaSmall, fontshader, Color.White, scale * 0.25f);
            Fonts.Add(FontStyle.Small, f);

            f = new SDFFont("Fonts/Verdana.txt");
            f.Setup(verdanaSmall, fontshader, Color.White, scale * 0.35f);

            Fonts.Add(FontStyle.Smaller, f);
        }
        #endregion

        #region Event handling
        List<GuiEventListener> listeners = new List<GuiEventListener>();

        public void BroadcastMessage(String msg)
        {
            foreach (GuiEventListener gev in listeners)
            {
                gev.HandleEvent(msg);
            }
        }

        public void RegisterListener(GuiEventListener target)
        {
            listeners.Add(target);
        }

        public void RemoveListener(GuiEventListener target)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                if (listeners[i] == target)
                {
                    listeners.RemoveAt(i);
                    return;
                }
            }
        }
        #endregion

    }
}
