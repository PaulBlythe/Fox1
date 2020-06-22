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
    public class OptionsPage:MenuPage
    {
        public OptionsPage()
        {
            Items.Add(new MainMenuButton(11, StringLocalizer.GetString(StringIDS.Display), new Vector2(50, 400)));
            Items.Add(new MainMenuButton(12, StringLocalizer.GetString(StringIDS.Audio), new Vector2(50, 440)));
            Items.Add(new MainMenuButton(13, StringLocalizer.GetString(StringIDS.Language), new Vector2(50, 480)));
            Items.Add(new MainMenuButton(14, StringLocalizer.GetString(StringIDS.Network), new Vector2(50, 520)));
            Items.Add(new MainMenuButton(15, StringLocalizer.GetString(StringIDS.Back), new Vector2(50, 560)));
        }

        public override int DoAction(int id, out MenuPage newpage)
        {
            newpage = null;
            switch (id)
            {
                case 11:
                    {
                        newpage = new DisplayOptions();
                        return 1;
                    }
                case 12:
                    {
                        newpage = new AudioOptionsPage();
                        return 1;
                    }
                case 15:
                    {
                        newpage = new MainPage();
                        return 1;
                    }
            }

            return 0;
        }
    }
}
