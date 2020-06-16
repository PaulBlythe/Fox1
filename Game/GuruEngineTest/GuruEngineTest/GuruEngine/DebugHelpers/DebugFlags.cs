using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace GuruEngine.DebugHelpers
{
    public class DebugFlags
    {
        public static bool RenderProfileData;
       

        static DebugFlags()
        {
            RenderProfileData = true;
        }


    }
}
