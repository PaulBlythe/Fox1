using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Assets;
using GuruEngine.World;
using GuruEngine.DebugHelpers;

namespace GuruEngine.DebugHelpers
{
    public abstract class GDMenuItem
    {
        public List<Rectangle> buttons = new List<Rectangle>();
        public List<String> text = new List<string>();
        public List<Vector2> textpositions = new List<Vector2>();

        public abstract GDMenuItem HandleEvent(int i);
    }

    #region Menus
    public class TopLevelMenu : GDMenuItem
    {
        public TopLevelMenu()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 65, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 115, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 165, 286, 32);
            buttons.Add(b);

            text.Add("Renderer");
            text.Add("Physics");
            text.Add("World");
            text.Add("Sound");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 70));
            textpositions.Add(new Vector2(18, 120));
            textpositions.Add(new Vector2(18, 170));
        }

        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    return new RendererTopMenu();
                case 1:
                    return new PhysicsTopLevelMenu();
                case 2:
                    return new WorldTopLevelMenu();
                case 3:
                    break;
            }
            return this;
        }
    }

    public class RendererTopMenu : GDMenuItem
    {
        public RendererTopMenu()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (1 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            text.Add("Display moon texture");
            text.Add("Display shadow texture");

            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (1 * 50)));
            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }
        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    DebugRenderSettings.RenderMoonTexture = !DebugRenderSettings.RenderMoonTexture;
                    break;
                case 1:
                    DebugRenderSettings.RenderShadowMap = !DebugRenderSettings.RenderShadowMap;
                    break;
                case 2:
                    return new TopLevelMenu();
            }
            return this;
        }
    }

    public class PhysicsTopLevelMenu : GDMenuItem
    {
        public PhysicsTopLevelMenu()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (1 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (2 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (3 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (4 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            text.Add("Display collision meshes");
            text.Add("Display hooks");
            text.Add("Display turrets");
            text.Add("Display aiming");
            text.Add("Display bullets");
            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (1 * 50)));
            textpositions.Add(new Vector2(18, 20 + (2 * 50)));
            textpositions.Add(new Vector2(18, 20 + (3 * 50)));
            textpositions.Add(new Vector2(18, 20 + (4 * 50)));
            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }

        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    DebugRenderSettings.RenderCollisionMeshes = !DebugRenderSettings.RenderCollisionMeshes;
                    break;
                case 1:
                    DebugRenderSettings.RenderHooks = !DebugRenderSettings.RenderHooks;
                    break;
                case 2:
                    DebugRenderSettings.RenderTurrets = !DebugRenderSettings.RenderTurrets;
                    break;
                case 3:
                    DebugRenderSettings.RenderAimPoints = !DebugRenderSettings.RenderAimPoints;
                    break;
                case 4:
                    DebugRenderSettings.RenderBullets = !DebugRenderSettings.RenderBullets;
                    break;
                case 5:
                    return new TopLevelMenu();

            }
            return this;
        }
    }

    public class WorldTopLevelMenu : GDMenuItem
    {
        public WorldTopLevelMenu()
        {
            Rectangle b = new Rectangle(15, 15, 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (1 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (2 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (3 * 50), 286, 32);
            buttons.Add(b);
            b = new Rectangle(15, 15 + (12 * 50), 286, 32);
            buttons.Add(b);

            text.Add("Advance time 1 hour");
            text.Add("Show clock");
            text.Add("Time * 10");
            text.Add("Time / 10");
            text.Add("Back");

            textpositions.Add(new Vector2(18, 20));
            textpositions.Add(new Vector2(18, 20 + (1 * 50)));
            textpositions.Add(new Vector2(18, 20 + (2 * 50)));
            textpositions.Add(new Vector2(18, 20 + (3 * 50)));
            textpositions.Add(new Vector2(18, 20 + (12 * 50)));
        }

        public override GDMenuItem HandleEvent(int i)
        {
            switch (i)
            {
                case 0:
                    WorldState.GetWorldState().GameTime = WorldState.GetWorldState().GameTime.AddHours(1);
                    break;
                case 1:
                    DebugRenderSettings.RenderClock = !DebugRenderSettings.RenderClock;
                    break;
                case 2:
                    WorldState.ChangeTimeStepMultiplier(10);
                    break;
                case 3:
                    WorldState.ChangeTimeStepMultiplier(1.0f / 10.0f);
                    break;
                case 4:
                    return new TopLevelMenu();
            }
            return this;
        }
    }

    #endregion

    public class GlobalDebugMenu
    {
        GDMenuItem current;

        public GlobalDebugMenu()
        {

        }

#if DEBUG
        bool _active = false;

        public bool Active
        {
            get { return _active; }
            set {
                _active = value;
                current = null ;
            }
        }

        Rectangle Screen = new Rectangle(10, 10, 300, 640);
        KeyboardState olds;
        MouseState oms;
        
        public void Draw(SpriteBatch batch)
        {
            KeyboardState ks = Keyboard.GetState();
            if ((ks.IsKeyDown(Keys.F12))&&(olds.IsKeyUp(Keys.F12)))
            {
                Active = !Active;
                current = new TopLevelMenu();

            }

            if (Active)
            {
                MouseState ms = Mouse.GetState();

                batch.Begin();
                batch.Draw(AssetManager.GetWhite(), Screen, Color.FromNonPremultiplied(0, 0, 0, 192));
                
                foreach (Rectangle r in current.buttons)
                {
                    if (r.Contains(ms.X,ms.Y))
                        batch.Draw(AssetManager.GetWhite(), r, Color.Black);
                    else
                        batch.Draw(AssetManager.GetWhite(), r, Color.DarkGray);
                }
                for (int i=0; i< current.text.Count; i++)
                {
                    batch.DrawString(AssetManager.GetDebugFont(), current.text[i], current.textpositions[i], Color.White);
                }
                batch.End();

                if (ms.LeftButton == ButtonState.Released)
                {
                    if (oms != null)
                    {
                        if (oms.LeftButton == ButtonState.Pressed)
                        {
                            for (int i=0; i< current.buttons.Count; i++)
                            {
                                if (current.buttons[i].Contains(ms.X,ms.Y))
                                {
                                    current = current.HandleEvent(i);
                                    break;
                                }
                            }
                            
                        }
                    }
                }
                oms = ms;
            }
            olds = ks;
        }
#endif
    }
}
