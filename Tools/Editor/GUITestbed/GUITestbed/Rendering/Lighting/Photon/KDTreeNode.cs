using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class KDTreeNode<Type>
    {
        public enum Dimension { X, Y, Z, NO_DIM };
        public Dimension Axis;
        public Vector3 Point;
        public Type Value;

        public KDTreeNode()
        {
            Axis = Dimension.NO_DIM;
        }

        public KDTreeNode(Type value, Vector3 point, Dimension dim)
        {
            Value = value;
            Point = point;
            Axis = dim;
        }

        public KDTreeNode(Type value, Vector3 point)
        {
            Value = value;
            Point = point;
            Axis = Dimension.NO_DIM;
        }


    }
}
