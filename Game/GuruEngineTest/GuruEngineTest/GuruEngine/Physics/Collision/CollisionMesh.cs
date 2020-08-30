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
    public class CollisionMesh
    {
        int nCollections;

        CollisionMeshPartCollection[] Collections;

        public CollisionMesh()
        {
            nCollections = 0;
        }

        public CollisionMesh(CollisionMesh other)
        {
            nCollections = other.nCollections;
            Collections = other.Collections;
        }

        /// <summary>
        /// Load the whole collision mesh
        /// </summary>
        /// <param name="filename"></param>
        public void Load(String filename)
        {
            if (!File.Exists(filename))
            {
                nCollections = 0;
                return;
            }
            using (BinaryReader b = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                nCollections = b.ReadInt32();
                Collections = new CollisionMeshPartCollection[nCollections];
                for (int i = 0; i < nCollections; i++)
                {
                    Collections[i] = new CollisionMeshPartCollection();
                    Collections[i].Load(b);
                }
            }
        }

#if DEBUG
        public void DrawCollisionMesh(Matrix world)
        {
            if (nCollections>0)
            {
                for (int i=0; i<nCollections; i++)
                {
                    Collections[i].Draw(world);
                }
            }
        }
#endif
    }
}
