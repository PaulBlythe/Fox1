using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using GuruEngineTest.Scenes.Helpers;
using GuruEngine.Localization.Strings;
using GuruEngineTest.Scenes.Generic.Helpers;

namespace GuruEngineTest.Scenes.Generic.MenuPages
{
    public class AudioOptionsPage: MenuPage
    {
        public AudioOptionsPage()
        {
            Items.Add(new MainMenuButton(-1, StringLocalizer.GetString(StringIDS.MusicVolume), new Vector2(50, 400)));
            Items.Add(new MainMenuButton(-1, StringLocalizer.GetString(StringIDS.SoundEffectVolume), new Vector2(50, 440)));
            Items.Add(new MainMenuButton(16, StringLocalizer.GetString(StringIDS.Back), new Vector2(50, 560)));

            Items.Add(new MainMenuSlider(-2, "", new Vector2(350, 400)));
            Items.Add(new MainMenuSlider(-3, "", new Vector2(350, 440)));

        }

        public override int DoAction(int id, out MenuPage newpage)
        {
            newpage = null;
            switch (id)
            {
                case 16:
                    {
                        newpage = new OptionsPage();
                        return 1;
                    }
            }

            return 0;
        }
    }
}
