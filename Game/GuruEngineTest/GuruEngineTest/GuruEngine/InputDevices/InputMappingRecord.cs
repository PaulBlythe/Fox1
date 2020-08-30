using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.InputDevices
{
    public class InputMappingRecord
    {
        public enum InputType
        {
            Button,
            Float,
            Int,
            Joystick,
            Wheel,
            Knob,
            Toggle,
            Total
        }

        public String ID;
        public InputType Type;
        public String Device;
        public String DeviceIdentifier;
        public String Description;
        public String Group;
    }
}
