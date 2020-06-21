using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectEditor.GameComponent
{
    public class GameComponentConnection
    {
        public String Parent;
        public String Type;
        public String Name;
        public String ConnectedTo = "";
        public List<String> AllConnections = new List<string>();

    }
}
