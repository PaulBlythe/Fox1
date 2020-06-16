using System;
using System.Collections.Generic;
using System.Text;

namespace GuruEngine.World.Trees.Instructions
{
    public class Bone : TreeBuilderInstruction
    {
        public int Delta { get; set; }

        public Bone(int delta)
        {
            Delta = delta;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder builder, Random rnd)
        {
            builder.Bone(Delta);
        }

        #endregion
    }
}
