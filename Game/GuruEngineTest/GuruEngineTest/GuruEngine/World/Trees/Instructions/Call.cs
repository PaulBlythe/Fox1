using System;
using System.Collections.Generic;
using System.Text;

namespace GuruEngine.World.Trees.Instructions
{
    public class Call : TreeBuilderInstruction
    {
        private List<Production> productions = new List<Production>();

        private int deltaLevel;

        public int DeltaLevel
        {
            get { return deltaLevel; }
            set { deltaLevel = value; }
        }


        public List<Production> Productions
        {
            get { return productions; }
            set { productions = value; }
        }

        public Call(List<Production> productions, int deltaLevel)
        {
            this.productions = productions;
            this.deltaLevel = deltaLevel;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeBuilder builder, Random rnd)
        {
            if (productions.Count == 0)
                throw new InvalidOperationException("No productions exist for call.");

            if (builder.Level + deltaLevel < 0)
                return;

            builder.Level = builder.Level + deltaLevel;

            int i = rnd.Next(0, productions.Count);
            productions[i].Execute(builder, rnd);

            builder.Level = builder.Level - deltaLevel;
        }

        #endregion
    }
}
