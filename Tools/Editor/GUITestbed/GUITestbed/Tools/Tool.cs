using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.Tools
{
    public abstract class Tool
    {
        public abstract void Update(float dt);
        public abstract void Draw();
        public abstract void SaveResults(String path);
    }
}
