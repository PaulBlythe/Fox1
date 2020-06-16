using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuruEngine.World.Trees.Instructions
{
    public enum CompareType
    {
        Less,
        Greater
    }

    public class RequireLevel : TreeBuilderInstruction
    {
        public List<TreeBuilderInstruction> Instructions { get; private set; }
        public int Level { get; set; }
        public CompareType Type { get; set; }

        public RequireLevel(int level, CompareType type)
        {
            Instructions = new List<TreeBuilderInstruction>();
            Level = level;
            Type = type;
        }

        public void Execute(TreeBuilder builder, Random rnd)
        {
            if (builder.Level >= Level && Type == CompareType.Greater || builder.Level <= Level && Type == CompareType.Less)
            {
                foreach (TreeBuilderInstruction instruction in Instructions)
                {
                    instruction.Execute(builder, rnd);
                }
            }
        }
    }
}
