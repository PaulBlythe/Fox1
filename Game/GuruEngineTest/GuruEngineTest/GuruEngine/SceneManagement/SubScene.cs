using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.SceneManagement
{
    public abstract class SubScene
    {
        public abstract void Update(float dt);
        public abstract void Draw();

        public String result = "";
        public int return_code = 0;
    }
}
