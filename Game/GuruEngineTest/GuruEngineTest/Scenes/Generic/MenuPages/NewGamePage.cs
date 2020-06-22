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
    public class NewGamePage:MenuPage
    {
        public NewGamePage()
        {
            Items.Add(new MainMenuButton(21, StringLocalizer.GetString(StringIDS.SinglePlayer), new Vector2(50, 400)));
            Items.Add(new MainMenuButton(22, StringLocalizer.GetString(StringIDS.MultiPlayer), new Vector2(50, 440)));
            Items.Add(new MainMenuButton(0, StringLocalizer.GetString(StringIDS.Back), new Vector2(50, 560)));
        }

        public override int DoAction(int id, out MenuPage newpage)
        {
            newpage = null;
            switch (id)
            {
                case 0:
                    newpage = new MainPage();
                    return 1;

                case 21:
                    newpage = new SinglePlayerPage();
                    return 1;
            }
            return 0;
        }
    }
}
