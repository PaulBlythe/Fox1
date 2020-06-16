using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using GUITestbed.GUI.Widgets;

namespace GUITestbed.GUI
{
    public class GuiManager
    {
        public static GuiManager Instance;

        Effect button_effect;
        Effect glyph_effect;
        Texture2D verdana_small;

        public SpriteBatch sbatch;
        public GUIBatch batch;
        public GuiFont font;
        public GuiTheme Theme;
        public Rectangle Region;
        

        List<GuiItem> Items = new List<GuiItem>();
        List<String> pendingEvents = new List<string>();
        List<Dialog> dialogs = new List<Dialog>();
        List<SideBar> sidebars = new List<SideBar>();

        public GuiManager(GraphicsDevice dev, ContentManager Content, GuiTheme theme)
        {
            Region = new Rectangle(0, 0, dev.Viewport.Width, dev.Viewport.Height);

            Instance = this;
            Theme = theme;
            verdana_small = Content.Load<Texture2D>(theme.FontTexture);

            button_effect = Content.Load<Effect>("Shaders/Button");
            button_effect.Parameters["Projection"].SetValue(Matrix.CreateOrthographicOffCenter(0, dev.Viewport.Width, dev.Viewport.Height, 0, 0.1f, 10));
            batch = new GUIBatch(dev, button_effect);

            glyph_effect = Content.Load<Effect>("Shaders/SDFont2");
            glyph_effect.Parameters["Projection"].SetValue(Matrix.CreateOrthographicOffCenter(0, dev.Viewport.Width, dev.Viewport.Height, 0, 0.1f, 10));

            font = new GuiFont(theme.FontDescriptor, verdana_small, glyph_effect, dev, 0.4f);

            char ch = 'a';
            for (Keys a = Keys.A; a<=Keys.Z; a++)
            {
                keytranslation.Add(a, ch);
                ch++;
            }
            ch = '0';
            for (Keys a = Keys.D0; a <= Keys.D9; a++)
            {
                keytranslation.Add(a, ch);
                ch++;
            }
            keytranslation.Add(Keys.Space, ' ');
            keytranslation.Add(Keys.OemPeriod, '.');
            keytranslation.Add(Keys.OemMinus, '-');

            sbatch = new SpriteBatch(Game1.Instance.GraphicsDevice);
        }

        public void Update(float dt)
        {
            if (pendingEvents.Count>0)
            {
                String s = pendingEvents[0];
                pendingEvents.RemoveAt(0);
                foreach (GuiItem i in Items)
                    i.HandleEvent(s);
            }
            if (dialogs.Count > 0)
            {
                dialogs[dialogs.Count - 1].Update(dt);
            }
            else
            {
                foreach (SideBar i in sidebars)
                    i.Update(dt);

                foreach (GuiItem i in Items)
                    i.Update(dt);
            }
        }

        public void Draw()
        {
           

            batch.Begin(BlendState.NonPremultiplied);
            foreach (GuiItem i in Items)
                i.Draw(batch);
            foreach (SideBar i in sidebars)
                i.Draw(batch);
            foreach (Dialog d in dialogs)
                d.Draw(batch);
            batch.End();

            sbatch.Begin();
            foreach (GuiItem i in Items)
                i.Draw(sbatch);
            sbatch.End();

            font.Begin();
            foreach (GuiItem i in Items)
                i.Draw(font);
            foreach (SideBar i in sidebars)
                i.Draw(font);
            foreach (Dialog d in dialogs)
                d.Draw(font);
            font.End();
        }

        public void Add(GuiItem item)
        {
            Items.Add(item);
        }

        public void HandleEvent(String s)
        {
            pendingEvents.Add(s);
        }

        public void Add(Dialog d)
        {
            dialogs.Add(d);
        }

        public void Add(SideBar d)
        {
            sidebars.Add(d);
        }

        public void RemoveTopLevelDialog()
        {
            dialogs.RemoveAt(dialogs.Count - 1);
        }

        public String GetTopLevelDialogName()
        {
            return dialogs[dialogs.Count - 1].Name;
        }

        public String GetString(Keys[] keys)
        {
            String res = "";
            bool shift = false;
            char ch = '\t';
            for (int i=0; i<keys.Length; i++)
            {
                if (keytranslation.ContainsKey(keys[i]))
                {
                    ch = keytranslation[keys[i]];
                }
                else
                {
                    if ((keys[i] == Keys.LeftShift)||(keys[i]== Keys.RightShift))
                    {
                        shift = true;
                    }
                    
                }
            }
            if (ch != '\t')
            {
                if (shift)
                {
                    ch = Char.ToUpperInvariant(ch);
                }
                res += ch;
            }
            return res;
        }

        Dictionary<Keys, char> keytranslation = new Dictionary<Keys, char>();

    }
}
