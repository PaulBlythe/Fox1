using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class PhotonTree
    {
        const int MAX_PHOTONS_BEFORE_SPLIT = 100;
        const int MAX_DEPTH = 18;

        private static Mutex mutex = new Mutex();

        BoundingBox bbox;
        PhotonTree child1 = null;
        PhotonTree child2 = null;
        int depth = 0;
        int split_axis = 0;
        double split_value;
        List<Photon> photons = new List<Photon>();

        public PhotonTree(BoundingBox inbox, int indepth)
        {
            bbox = inbox;
            depth = indepth;
        }

        public Vector3 getMin()
        {
            return bbox.Min;
        }

        public Vector3 getMax()
        {
            return bbox.Max;
        }

        public bool overlaps(BoundingBox other)
        {
            return bbox.Intersects(other);
        }

        public int getDepth()
        {
            return depth;
        }

        public bool isLeaf()
        {
            if ((child1 == null) && (child2 == null))
                return true;
            return false;
        }

        public PhotonTree getChild1()
        {
            return child1;
        }

        public PhotonTree getChild2()
        {
            return child2;
        }

        public List<Photon> getPhotons()
        {
            return photons;
        }

        public void CollectPhotonsInBox(BoundingBox bb, List<Photon> inphotons)
        {
            List<PhotonTree> todo = new List<PhotonTree>();
            todo.Add(this);
            while (todo.Count > 0)
            {
                PhotonTree node = todo[todo.Count-1];
                todo.RemoveAt(todo.Count - 1);

                if (!node.overlaps(bb)) continue;
                if (node.isLeaf())
                {
                    // if this cell overlaps & is a leaf, add all of the photons into the master list
                    // NOTE: these photons may not be inside of the query bounding box
                    List<Photon> photons2 = node.getPhotons();
                    int num_photons = photons2.Count;
                    for (int i = 0; i < num_photons; i++)
                    {
                        photons.Add(photons2[i]);
                    }
                }
                else
                {
                    // if this cell is not a leaf, explore both children
                    todo.Add(node.getChild1());
                    todo.Add(node.getChild2());
                }
            }
        }

        public void AddPhoton(Photon p)
        {
            mutex.WaitOne();
            AddPhoton2(p);
        }

        public void AddPhoton2(Photon p)
        {
            if (isLeaf())
            {
                photons.Add(p);
                if (photons.Count > MAX_PHOTONS_BEFORE_SPLIT && depth < MAX_DEPTH)
                {
                    SplitCell();
                }
            }
            else
            {
                // this cell is not a leaf node
                // decide which subnode to recurse into
                if (split_axis == 0)
                {
                    if (p.position.X < split_value)
                        child1.AddPhoton2(p);
                    else
                        child2.AddPhoton2(p);
                }
                else if (split_axis == 1)
                {
                    if (p.position.Y < split_value)
                        child1.AddPhoton2(p);
                    else
                        child2.AddPhoton2(p);
                }
                else
                {
                    if (p.position.Z < split_value)
                        child1.AddPhoton2(p);
                    else
                        child2.AddPhoton2(p);
                }
            }
        }

        public bool PhotonInCell(Photon p)
        {
            return (bbox.Contains(p.position) == ContainmentType.Contains);
        }

        public void Clear()
        {
            photons.Clear();
            child1 = null;
            child2 = null;
        }

        void SplitCell()
        {
            Vector3 min = bbox.Min;
            Vector3 max = bbox.Max;
            double dx = max.X - min.X;
            double dy = max.Y - min.Y;
            double dz = max.Z - min.Z;
            // split this cell in the middle of the longest axis
            Vector3 min1, min2, max1, max2;
            if (dx >= dy && dx >= dz)
            {
                split_axis = 0;
                split_value = min.X + dx / 2.0;
                min1 = new Vector3(min.X, min.Y, min.Z);
                max1 = new Vector3((float)split_value, max.Y, max.Z);
                min2 = new Vector3((float)split_value, min.Y, min.Z);
                max2 = new Vector3(max.X, max.Y, max.Z);
            }
            else if (dy >= dx && dy >= dz)
            {
                split_axis = 1;
                split_value = min.Y + dy / 2.0;
                min1 = new Vector3(min.X, min.Y, min.Z);
                max1 = new Vector3(max.X, (float)split_value, max.Z);
                min2 = new Vector3(min.X, (float)split_value, min.Z);
                max2 = new Vector3(max.X, max.Y, max.Z);
            }
            else
            {
                split_axis = 2;
                split_value = min.z() + dz / 2.0;
                min1 = new Vector3(min.X, min.Y, min.Z);
                max1 = new Vector3(max.X, max.Y, (float)split_value);
                min2 = new Vector3(min.X, min.Y, (float)split_value);
                max2 = new Vector3(max.X, max.Y, max.Z);
            }
            // create two new children
            child1 = new PhotonTree(new BoundingBox(min1, max1), depth + 1);
            child2 = new PhotonTree(new BoundingBox(min2, max2), depth + 1);
            int num_photons = photons.Count;
            
            // add all the photons to one of those children
            for (int i = 0; i < num_photons; i++)
            {
                AddPhoton2(photons[i]);
            }
        }
    }
}
