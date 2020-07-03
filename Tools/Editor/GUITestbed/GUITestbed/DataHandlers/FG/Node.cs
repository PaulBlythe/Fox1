using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.FG
{
    public class Node
    {
        public Vector2D lo = null;
        public Vector2D pt = null;
        public Vector2D hi = null;

        public int linetype;
        public int lighting;
        public bool IsCurved = false;

        bool isSplit = false;

        public bool has_lo()
        {
            return lo != pt;
        }

        public bool has_hi()
        {
            return hi != pt;
        }

        public bool is_split()
        {
            return (new Vector2D(pt.X - lo.X, pt.Y - lo.Y) != new Vector2D(hi.X - pt.X, hi.Y - pt.Y));
        }

        public void set_split(bool split)
        {
            isSplit = split;
        }

        public void SetControlHi(Vector2D p)
        {
            hi = new Vector2D(pt, p);
            if (!isSplit)
            {
                lo = -hi;
            }
            IsCurved = true;
        }

        public void SetControlLo(Vector2D p)
        {
            lo = new Vector2D(pt, p);
            if (!isSplit)
            {
                hi = -lo;
            }
            IsCurved = true;
        }

        public bool GetControlHandleHi(out Vector2D res)
        {
            res = null;
            if (hi == null)
                return false;

            res = pt + hi;
            return true;
        }

        public bool GetControlHandleLo(out Vector2D res)
        {
            res = null;
            if (lo == null)
                return false;

            res = pt + lo;
            return true;
        }
    }
}
