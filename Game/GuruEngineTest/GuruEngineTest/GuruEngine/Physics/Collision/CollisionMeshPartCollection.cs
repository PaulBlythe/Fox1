using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.Physics.Collision
{
    public class CollisionMeshPartCollection
    {
        public int nParts;
        public CollisionMeshPart[] Blocks;

        public void Load(BinaryReader b)
        {
            nParts = b.ReadInt32();
            Blocks = new CollisionMeshPart[nParts];
            for (int i=0; i<nParts; i++)
            {
                Blocks[i] = new CollisionMeshPart();
                Blocks[i].Load(b);
            }
        }

#if DEBUG
        public void Draw(Matrix w)
        {
            for (int i=0; i<nParts; i++)
            {
                Blocks[i].Draw(w);
            }
        }
#endif
    }
}
