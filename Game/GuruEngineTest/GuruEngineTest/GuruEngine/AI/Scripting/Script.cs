using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.AI.Scripting
{
    public abstract class Script
    {
        public object Host;

        public abstract void CreateObjects();
        public abstract void StartAction(String name);
        public abstract void AbortAction(bool all);
        public abstract void UpdateScript(float dt);
        public abstract bool CheckBooleanState(String name);
    }
}
