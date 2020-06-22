using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using GuruEngineTest.Scenes.Helpers;
using GuruEngine.Localization.Strings;
using GuruEngineTest.Scenes.Generic.Helpers;

namespace GuruEngineTest.Scenes.Generic.MenuPages
{
    public class MainPage :MenuPage
    {
        public MainPage()
        {
            Items.Add(new MainMenuButton(1, StringLocalizer.GetString(StringIDS.NewGame), new Vector2(50, 400)));
            Items.Add(new MainMenuButton(2, StringLocalizer.GetString(StringIDS.LoadGame), new Vector2(50, 440)));
            Items.Add(new MainMenuButton(3, StringLocalizer.GetString(StringIDS.Options), new Vector2(50, 480)));
            Items.Add(new MainMenuButton(4, StringLocalizer.GetString(StringIDS.Exit), new Vector2(50, 520)));
   
        }

        public override int DoAction(int id , out MenuPage newpage)
        {
            newpage = null;
            switch (id)
            {
                case 1:
                    {
                        newpage = new NewGamePage();
                        return 1;
                    }
                case 3:
                    {
                        newpage = new OptionsPage();
                        return 1;
                    }

                case 4:
                    {
                        Game1.GameInstance.Exit();
                    }
                    break;


            }
            return 0;
        }
    }
}
