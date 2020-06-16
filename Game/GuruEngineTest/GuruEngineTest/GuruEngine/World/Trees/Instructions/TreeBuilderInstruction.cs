using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.World.Trees.Instructions
{
    public interface TreeBuilderInstruction
    {
        void Execute(TreeBuilder builder, Random rnd);
    }
}
