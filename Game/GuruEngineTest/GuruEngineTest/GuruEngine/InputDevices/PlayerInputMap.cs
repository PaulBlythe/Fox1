using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.InputDevices
{
    public class PlayerInputMap
    {
        public static PlayerInputMap Instance;

        public String ThrottleDevice = "";
        public String JoystickDevice = "";
        public String RudderDevice = "";

        #region MFD input mapping
        public InputDescriptor MFDMasterInput = new InputDescriptor();
        public InputDescriptor MFDMasterSelectUp = new InputDescriptor();
        public InputDescriptor MFDMasterSelectDown = new InputDescriptor();
        public InputDescriptor MFDSlaveInput = new InputDescriptor();
        public InputDescriptor MFDSlaveSelectUp = new InputDescriptor();
        public InputDescriptor MFDSlaveSelectDown = new InputDescriptor();
        #endregion

        public PlayerInputMap()
        {
            Instance = this;
        }


    }
}
