using System;
using System.Collections.Generic;
using System.Text;

namespace GuruEngine.World.Trees.Instructions
{
    /// <summary>
    /// Has a chance of performing a group of instructions; otherwise does nothing.
    /// </summary>
    public class Maybe : TreeBuilderInstruction
    {
        private List<TreeBuilderInstruction> instructions = new List<TreeBuilderInstruction>();
        private float chance;

        /// <summary>
        /// Probability that the instructions will be executed. Should be between 0.0f and 1.0f.
        /// </summary>
        public float Chance
        {
            get { return chance; }
            set { chance = value; }
        }


        public List<TreeBuilderInstruction> Instructions
        {
            get { return instructions; }
            set { instructions = value; }
        }

        public Maybe(float chance)
        {
            this.chance = chance;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder builder, Random rnd)
        {
            if (rnd.NextDouble() < chance)
            {
                foreach (TreeBuilderInstruction child in instructions)
                {
                    child.Execute(builder, rnd);
                }
            }
        }

        #endregion
    }
}
