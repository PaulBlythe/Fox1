using System;
using System.Collections.Generic;
using System.Text;
using GuruEngine.Helpers;

namespace GuruEngine.World.Trees.Instructions
{
    public class Scale : TreeBuilderInstruction
    {
        private float scale;

        private float variation;

        public float Variation
        {
            get { return variation; }
            set { variation = value; }
        }


        public float Value
        {
            get { return scale; }
            set { scale = value; }
        }

        public Scale(float scale, float variation)
        {
            this.scale = scale;
            this.variation = variation;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder builder, Random rnd)
        {
            builder.Scale(MathsHelper.Random(rnd, scale, variation));
        }

        #endregion
    }
}
