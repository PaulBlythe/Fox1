using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.InputDevices
{
    public class HiHat
    {
        bool _up = false;
        bool _down = false;
        bool _right = false;
        bool _left = false;

        bool _bup = false;
        bool _bdown = false;
        bool _bleft = false;
        bool _bright = false;
        
        public bool Up
        {
            get { return _up; }
            set {
                    if (value)
                    {
                        if (!_up)
                            _bup = value;
                    }
                    _up = value;
                }
        }

        public bool Down
        {
            get { return _down; }
            set
            {
                if (value)
                {
                    if (!_down)
                        _bdown = value;
                }
                _down = value;
            }
        }
        public bool Left
        {
            get { return _left; }
            set
            {
                if (value)
                {
                    if (!_left)
                        _bleft = value;
                }
                _left = value;
            }
        }
        public bool Right
        {
            get { return _right; }
            set
            {
                if (value)
                {
                    if (!_right)
                        _bright = value;
                }
                _right = value;
            }
        }

        public bool BouncedUp
        {
            get
            {
                if (_bup)
                {
                    _bup = false;
                    return true;
                }
                return false;
            }
        }
        public bool BouncedDown
        {
            get
            {
                if (_bdown)
                {
                    _bdown = false;
                    return true;
                }
               
                return false;
            }
        }
        public bool BouncedLeft
        {
            get
            {
                if (_bleft)
                {
                    _bleft = false;
                    return true;
                }
               
                return false;
            }
        }
        public bool BouncedRight
        {
            get
            {
                if (_bright)
                {
                    _bright = false;
                    return true;
                }
               
                return false;
            }
        }

        public bool Is(HiHat other)
        {
            if (other.Up != _up)
                return false;
            if (other.Down != _down)
                return false;
            if (other.Left != _left)
                return false;
            if (other.Right != _right)
                return false;

            return true;
        }
        public void Set(HiHat other)
        {
            _up = other.Up;
            _down = other.Down;
            _left = other.Left;
            _right = other.Right;
        }
    }

    public class DebouncedButton
    {
        bool pressed = false;
        bool released = false;

        public bool Down
        {
            get
            {
                if (released)
                {
                    released = false;
                    return true;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    pressed = true;
                    released = false;
                }
                else
                {
                    released = pressed;
                    pressed = false;
                }

            }
        }

       
    }

    public abstract class InputDevice
    {
        public abstract bool Connect();
        public abstract bool Update();
        public abstract void CloseDown();

        public Dictionary<string, bool> Buttons = new Dictionary<string, bool>();
        public Dictionary<string, Vector2> Axes = new Dictionary<string, Vector2>();
        public Dictionary<string, HiHat> HiHats = new Dictionary<string, HiHat>();
        public Dictionary<string, int> POVs = new Dictionary<string, int>();
        public Dictionary<string, byte> Wheels = new Dictionary<string, byte>();
        public Dictionary<string, float> Throttles = new Dictionary<string, float>();
        public Dictionary<string, int> GenericInts = new Dictionary<string, int>();
        public Dictionary<String, float> GenericFloats = new Dictionary<string, float>();
        public Dictionary<string, DebouncedButton> DebouncedButtons = new Dictionary<string, DebouncedButton>();

        bool Locked = false;
        public void Lock()
        {
            Locked = true;
        }

        public void Unlock()
        {
            Locked = false;
        }

        public bool IsLocked()
        {
            return Locked;
        }
    }
}
