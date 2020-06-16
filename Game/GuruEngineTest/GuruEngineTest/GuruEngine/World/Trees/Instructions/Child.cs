using System;
using System.Collections.Generic;
using System.Text;

namespace GuruEngine.World.Trees.Instructions
{
    public class Child : TreeBuilderInstruction
    {
        private List<TreeBuilderInstruction> instructions = new List<TreeBuilderInstruction>();

        public List<TreeBuilderInstruction> Instructions
        {
            get { return instructions; }
            set { instructions = value; }
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder builder, Random rnd)
        {
            builder.PushState();
            foreach (TreeBuilderInstruction instruction in instructions)
            {
                instruction.Execute(builder, rnd);
            }
            builder.PopState();
        }

        #endregion
    }
}
