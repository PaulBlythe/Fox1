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
    public class DisplayOptions:MenuPage
    {
        public DisplayOptions()
        {
            Items.Add(new MainMenuButton(-1, StringLocalizer.GetString(StringIDS.Renderer), new Vector2(50, 400)));
            Items.Add(new MainMenuSwitch(StringLocalizer.GetString(StringIDS.Deffered), StringLocalizer.GetString(StringIDS.Forward), -1, new Vector2(550, 400)));
            Items.Add(new MainMenuButton(1, StringLocalizer.GetString(StringIDS.Back), new Vector2(50, 560)));
        }

        public override int DoAction(int id, out MenuPage newpage)
        {
            newpage = null;
            switch(id)
            {
                case 1:
                    {
                        newpage = new OptionsPage();
                        return 1;
                    }
            }
            return 0;
        }
    }
}
