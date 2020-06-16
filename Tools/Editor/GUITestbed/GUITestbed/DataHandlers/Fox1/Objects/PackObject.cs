using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class PackObject
    {
        public Guid guid;
        public String Filename;

        public PackObject(Guid g, String f)
        {
            guid = g;
            Filename = f;
        }
    }
}
