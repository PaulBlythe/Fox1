using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;

namespace GuruEngine.World.Trees.Instructions
{
    public class Twist : TreeBuilderInstruction
    {
        private float angle;
        private float variation;

        public float Variation
        {
            get { return variation; }
            set { variation = value; }
        }


        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public Twist(float angle, float variation)
        {
            this.angle = MathHelper.ToRadians(angle);
            this.variation = MathHelper.ToRadians(variation);
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder crayon, Random rnd)
        {
            crayon.Twist(MathsHelper.Random(rnd, angle, variation));
        }

        #endregion
    }
}
