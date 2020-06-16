using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.AI.Scripting
{
    public class ScriptManager
    {
        public static ScriptManager Instance;

        public ScriptManager()
        {
            Instance = this;
        }

        public Script GetScript(ScriptClasses type, String name)
        {
            switch (type)
            {
                case ScriptClasses.Aircraft:
                    return GetAircraftScript(name);
            }
            return null;
        }

        #region Aircraft scripts
        Script GetAircraftScript(String name)
        {
            switch (name)
            {
                case "SpitfireIXc":
                    return new Scripts.Aircraft.British.Fighter.SpitfireIXc();
            }
            return null;
        }
        #endregion
    }
}
