using System;
using System.Collections.Generic;
using System.Text;

using GuruEngine.Helpers;

namespace GuruEngine.World.Trees.Instructions
{
    public class Backward : TreeBuilderInstruction
    {
        private float distance;
        private float distanceVariation;

        public float DistanceVariation
        {
            get { return distanceVariation; }
            set { distanceVariation = value; }
        }


        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public Backward(float distance, float variation)
        {
            this.distance = distance;
            this.distanceVariation = variation;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder builder, Random rnd)
        {
            builder.Backward(MathsHelper.Random(rnd, distance, distanceVariation));
        }

        #endregion
    }
}
