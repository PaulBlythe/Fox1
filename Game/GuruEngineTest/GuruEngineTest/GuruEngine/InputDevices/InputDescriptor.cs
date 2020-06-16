using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.InputDevices
{
    public class InputDescriptor
    {
        public String Device;
        public String Input;
        public InputDescriptorType Type;

        public InputDescriptor()
        {
            Device = Input = "";
            Type = InputDescriptorType.None;
        }
        public InputDescriptor(String device, String inputString, InputDescriptorType type)
        {
            Device = device;
            Input = inputString;
            Type = type;
        }
    }
}
