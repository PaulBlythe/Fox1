using System;
using System.Collections.Generic;
using System.Text;

namespace GuruEngine.World.Trees.Instructions
{
    public class Level : TreeBuilderInstruction
    {
        private int deltaLevel;

        public int DeltaLevel
        {
            get { return deltaLevel; }
            set { deltaLevel = value; }
        }

        public Level(int deltaLevel)
        {
            this.deltaLevel = deltaLevel;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder builder, Random rnd)
        {
            builder.Level = builder.Level + deltaLevel;
        }

        #endregion
    }
}
