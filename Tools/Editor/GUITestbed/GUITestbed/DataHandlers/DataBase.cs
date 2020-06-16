using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers
{
    public abstract class Database
    {
        public abstract String GetRecord(int index, String name);
        public abstract int GetCount();
    }
}
