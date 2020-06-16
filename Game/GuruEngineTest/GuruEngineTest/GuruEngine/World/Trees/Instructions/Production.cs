using System;
using System.Collections.Generic;
using System.Text;

namespace GuruEngine.World.Trees.Instructions
{
    public class Production
    {
        private List<TreeBuilderInstruction> instructions = new List<TreeBuilderInstruction>();

        public List<TreeBuilderInstruction> Instructions
        {
            get { return instructions; }
            set { instructions = value; }
        }

        public void Execute(TreeBuilder builder, Random rnd)
        {
            foreach (TreeBuilderInstruction instruction in instructions)
            {
                instruction.Execute(builder, rnd);
            }
        }
    }
}
