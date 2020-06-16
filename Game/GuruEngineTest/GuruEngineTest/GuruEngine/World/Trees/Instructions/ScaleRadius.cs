using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuruEngine.Helpers;

namespace GuruEngine.World.Trees.Instructions
{
    public class ScaleRadius : TreeBuilderInstruction
    {
        private float scale;
        private float variation;

        public float Variation
        {
            get { return variation; }
            set { variation = value; }
        }


        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public ScaleRadius(float scale, float variation)
        {
            this.scale = scale;
            this.variation = variation;
        }


        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder builder, Random rnd)
        {
            builder.ScaleRadius(MathsHelper.Random(rnd, scale, variation));
        }

        #endregion
    }
}
