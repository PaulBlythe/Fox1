using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.DataHandlers.IL2
{
    public class CollisionMesh
    {
        public int NBlocks;
        public List<CollisionMeshBlock> Blocks = new List<CollisionMeshBlock>();
        public int CurrentBlock;
        public int CurrentPart;

        public CollisionMesh()
        {
            NBlocks = 0;
            CurrentBlock = 0;
            CurrentPart = 0;
        }
    }
}
