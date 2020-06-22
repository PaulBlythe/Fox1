using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngineTest.Scenes.Helpers;
using GuruEngineTest.Scenes.Generic.Helpers;

namespace GuruEngineTest.Scenes.Generic.MenuPages
{
    public abstract class MenuPage
    {
        public List<MainMenuWidget> Items = new List<MainMenuWidget>();

        public abstract int DoAction(int id, out MenuPage newpage);

        
    }
}
