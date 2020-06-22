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
    public class SinglePlayerPage: MenuPage
    {
        public SinglePlayerPage()
        {
            Items.Add(new MainMenuButton(31, StringLocalizer.GetString(StringIDS.FreeFlight), new Vector2(50, 400)));
            Items.Add(new MainMenuButton(32, StringLocalizer.GetString(StringIDS.SingleMission), new Vector2(50, 440)));
            Items.Add(new MainMenuButton(33, StringLocalizer.GetString(StringIDS.Career), new Vector2(50, 480)));
            Items.Add(new MainMenuButton(34, StringLocalizer.GetString(StringIDS.Campaign), new Vector2(50, 520)));
            Items.Add(new MainMenuButton(1, StringLocalizer.GetString(StringIDS.Back), new Vector2(50, 560)));
        }

        public override int DoAction(int id, out MenuPage newpage)
        {
            newpage = null;
            switch (id)
            {
                case 1:
                    newpage = new NewGamePage();
                    return 1;

            }
            return 0;
        }
    }
}
