using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GuruEngine.InputDevices
{
    public class KeyboardButton
    {
        public String ID;
        public List<Keys> keys = new List<Keys>();
        public bool Down;
    }
}
