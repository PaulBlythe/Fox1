using System;
using System.Collections.Generic;
using System.Linq;



namespace GuruEngine.AI
{
    public class AIManager
    {
        public static AIManager Instance;

        public AIManager()
        {
            Instance = this;
        }

        #region IFF code
        public static bool IsFriendly(int IFF1, int IFF2)
        {
            int t1 = (IFF1 >> 8);
            int t2 = (IFF2 >> 8);

            return (t1 == t2);

        }

        public static bool IsEnemy(int IFF1, int IFF2)
        {
            return !IsFriendly(IFF1, IFF2);
        }

        #endregion
    }
}
