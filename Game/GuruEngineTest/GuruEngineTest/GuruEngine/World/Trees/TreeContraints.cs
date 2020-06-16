using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.World.Trees
{
    public interface TreeContraints
    {
        /// <summary>
        /// Called whenever a branch is about to be created by the TreeCrayon.
        /// This may alter the crayon prior the creation of the branch, or it may prohibit the creation
        /// of the branch by returning false.
        /// </summary>
        /// <param name="builder">The builder about to create a branch.</param>
        /// <param name="distance">The distance of the branch to be created. May be altered.</param>
        /// <param name="radiusEndScale">Radius end scale of the branch to be created. May be altered.</param>
        /// <returns>True if the branch may be created, false it if should be cancelled.</returns>
        bool ConstrainForward(TreeBuilder builder, ref float distance, ref float radiusEndScale);
    }
}
