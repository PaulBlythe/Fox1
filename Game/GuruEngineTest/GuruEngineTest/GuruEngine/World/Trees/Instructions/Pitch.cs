using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;

namespace GuruEngine.World.Trees.Instructions
{
    public class Pitch : TreeBuilderInstruction
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

        public Pitch(float angle, float variation)
        {
            this.angle = MathHelper.ToRadians(angle);
            this.variation = MathHelper.ToRadians(variation);
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder builder, Random rnd)
        {
            builder.Pitch(MathsHelper.Random(rnd, angle, variation));
        }

        #endregion
    }
}
