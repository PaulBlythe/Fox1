using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;


namespace GUITestbed.Rendering.Lighting.Photon
{
    public class KDTree<Type>
    {
        List<KDTreeNode<Type>> mNodes = new List<KDTreeNode<Type>>();
        List<KDTreeNode<Type>> mBalanced = new List<KDTreeNode<Type>>();

        public KDTree()
        {
        }

        public void Clear()
        {
            mNodes.Clear();
            mBalanced.Clear();
        }

        public void Store(Vector3 point, Type value)
        {
            mNodes.Add(new KDTreeNode<Type>(value, point));
        }

        // Fixed Radius
        public uint Find(Vector3 p, float radius, List<KDTreeNode<Type>> nodes)
        {
            if (nodes != null)
            {
                Find(p, 1, radius, nodes);
                return (uint)nodes.Count();
            }
            List<KDTreeNode<Type>> local_nodes = new List<KDTreeNode<Type>>();
            Find(p, 1, radius, local_nodes);
            return (uint)local_nodes.Count();
        }

        // Nearest Neighbor search
        public void Find(Vector3 p, uint nb_elements, List<KDTreeNode<Type>> nodes, out float max_distance)
        {
            nodes.Clear();
            max_distance = float.MaxValue;

            if (mBalanced.Count() == 0)
                return;

            List<Pair<uint, float>> dist = new List<Pair<uint, float>>();

            max_distance = Find(p, 1, nb_elements,  nodes, max_distance, dist);
        }

        public KDTreeNode<Type> Find(Vector3 p)
        {
            return mBalanced[(int)Closest(p, 1, 1)];
        }

        public static void Resize(List<KDTreeNode<Type>> list, int sz, KDTreeNode<Type> c)
        {
            int cur = list.Count;
            if (sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if (sz > cur)
            {
                if (sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                    list.Capacity = sz;
                list.AddRange(Enumerable.Repeat(c, sz - cur));
            }
        }

        public void Balance()
        {
            if (mNodes.Count() == 0)
                return;

            List<KDTreeNode<Type>> aux = new List<KDTreeNode<Type>>(mNodes.Count() + 1);
            Resize(mBalanced , mNodes.Count() + 1, new KDTreeNode<Type>());

            int i;
            Vector3 bbmax = mNodes[0].Point;
            Vector3 bbmin = mNodes[0].Point;

            for (i = 1; !(mNodes.Count() == 0); i++, mNodes.RemoveAt(0))
            {
                aux[i] = mNodes[0];

                if (aux[i].Point.X < GetAxisValue(bbmin, KDTreeNode<Type>.Dimension.X)) bbmin.X = aux[i].Point.X;
                if (aux[i].Point.X > GetAxisValue(bbmax, KDTreeNode<Type>.Dimension.X)) bbmax.X = aux[i].Point.X;
                // Y dimension.                          
                if (aux[i].Point.Y < GetAxisValue(bbmin, KDTreeNode<Type>.Dimension.Y)) bbmin.Y = aux[i].Point.Y;
                if (aux[i].Point.Y > GetAxisValue(bbmax, KDTreeNode<Type>.Dimension.Y)) bbmax.Y = aux[i].Point.Y;
                // X dimension.                          
                if (aux[i].Point.Z < GetAxisValue(bbmin, KDTreeNode<Type>.Dimension.Z)) bbmin.Z = aux[i].Point.Z;
                if (aux[i].Point.Z > GetAxisValue(bbmax, KDTreeNode<Type>.Dimension.Z)) bbmax.Z = aux[i].Point.Z;
            }
            mNodes.Clear();

            BalanceSegment(mBalanced, aux, 1, 1, (mBalanced.Count() - 1), bbmin, bbmax);
        }

        public uint Size()
        {
            return (uint)mBalanced.Count();
        }

        public bool IsEmpty()
        {
            return (mBalanced.Count() == 0);
        }

        public KDTreeNode<Type> Get(uint idx) 
        {
            return mBalanced[(int)idx];
        }
      
        public void DumpToFile(String filename)
        {
            String s = "";
            foreach (KDTreeNode<Type> node in mBalanced)
            {
                s += node.Point.ToString();
                s += "\n";
            }
            File.WriteAllText(filename, s);
        }

        public static float GetAxisValue(Vector3 p, KDTreeNode<Type>.Dimension axis)
        {
            switch (axis)
            {
                case KDTreeNode<Type>.Dimension.X:
                    return p.X;
                case KDTreeNode<Type>.Dimension.Y:
                    return p.Y;
                case KDTreeNode<Type>.Dimension.Z:
                    return p.Z;
            }
            return 0;
        }

        public static KDTreeNode<Type>.Dimension LongestDimension(Vector3 a, Vector3 b)
        {
            KDTreeNode<Type>.Dimension longestDimension = KDTreeNode<Type>.Dimension.X;
            float maxDistance = Math.Abs(a.X - b.X);
            // Check if dimension Y is longer.
            if (Math.Abs(a.Y - b.Y) > maxDistance)
            {
                longestDimension = KDTreeNode<Type>.Dimension.Y;
                maxDistance = Math.Abs(a.Y - b.Y);
            }
            // Check if dimension Z is longer.
            if (Math.Abs(a.Z - b.Z) > maxDistance)
            {
                longestDimension = KDTreeNode<Type>.Dimension.Z;
            }
            return longestDimension;
        }

        public static void SetDimension(ref Vector3 v, KDTreeNode<Type>.Dimension axis, float value)
        {
            switch (axis)
            {
                case KDTreeNode<Type>.Dimension.X:
                    v.X = value;
                    break;

                case KDTreeNode<Type>.Dimension.Y:
                    v.Y = value;
                    break;

                case KDTreeNode<Type>.Dimension.Z:
                    v.Z = value;
                    break;
            }
        }

        public static void myswap(List<KDTreeNode<Type>> array, int a, int b)
        {
            KDTreeNode<Type> aux = array[a];
            array[a] = array[b];
            array[b] = aux;
        }

        public static void MedianSplit(List<KDTreeNode<Type>> p, int start,  int end, int median, KDTreeNode<Type>.Dimension axis)
        {
            int left = start;
            int right = end;

            while (right > left)
            {
                float v = GetAxisValue(p[right].Point,axis);

                int i = left - 1;
                int j = right;
                for (; ; )
                {
                    while (v > GetAxisValue(p[++i].Point, axis));
                    while (v < GetAxisValue(p[--j].Point, axis) && j > left) ;
                    if (i >= j)
                        break;
                    myswap(p, i, j);
                }

                myswap(p, i, right);
                if (i >= median)
                    right = i - 1;
                if (i <= median)
                    left = i + 1;
            }
        }

        public static void BalanceSegment(List<KDTreeNode<Type>> pbal, List<KDTreeNode<Type>> org, int index, int start, int end, Vector3 bbmin, Vector3 bbmax)
        {
            int median = 1;
            while ((4 * median) <= (end - start + 1))
                median += median;

            if ((3 * median) <= (end - start + 1))
            {
                median += median;
                median += start - 1;
            }
            else
                median = end - median + 1;

            // elegimos el eje más apropiado...
            KDTreeNode<Type>.Dimension axis = LongestDimension(bbmax,bbmin);

            // partimos el bloque de fotones por la mediana
            MedianSplit(org, start, end, median, axis);

            pbal[index] = org[median];
            pbal[index].Axis = axis;

            // y por último balanceamos recursivamente los bloques izquierdo y derecho
            if (median > start)
            {
                // balancear el segmento izquierdo
                if (start < median - 1)
                {
                    Vector3 newbbmax = bbmax;
                    SetDimension(ref newbbmax, axis, GetAxisValue(pbal[index].Point, axis));
                    BalanceSegment(pbal, org, 2 * index, start, median - 1, bbmin, newbbmax);
                }
                else
                {
                    pbal[2 * index] = org[start];
                }
            }

            if (median < end)
            {
                // balancear el segmento derecho
                if (median + 1 < end)
                {
                    Vector3 newbbmin = bbmin;
                    SetDimension(ref newbbmin, axis, GetAxisValue(pbal[index].Point, axis));
                    BalanceSegment(pbal, org, 2 * index + 1, median + 1, end, newbbmin, bbmax);
                }
                else
                {
                    pbal[2 * index + 1] = org[end];
                }
            }
        }

        uint Closest(Vector3 p, uint index, uint best)
        {
            uint sol = best;
            float distbest = Vector3.Distance(p , mBalanced[(int)best].Point);
            float aux;
            //We check if our node is better
            if ((aux = Vector3.Distance(mBalanced[(int)index].Point, p)) < distbest)
            {
                sol = index;
                distbest = aux;
            }
            //Now we check that this is not a leaf node
            if (index < ((mBalanced.Count() - 1) / 2))
            {
                float distaxis = 0;
                switch (mBalanced[(int)index].Axis)
                {
                    case KDTreeNode<Type>.Dimension.X:
                        distaxis = p.X - mBalanced[(int)index].Point.X;
                        break;
                    case KDTreeNode<Type>.Dimension.Y:
                        distaxis = p.Y - mBalanced[(int)index].Point.Y;
                        break;
                    case KDTreeNode<Type>.Dimension.Z:
                        distaxis = p.Z - mBalanced[(int)index].Point.Z;
                        break;

                }
                if (distaxis < 0.0) 
                {
                    uint candidate = Closest(p, 2 * index, sol);
                    if ((aux = Vector3.Distance(mBalanced[(int)candidate].Point , p)) < distbest)
                    {
                        sol = candidate;
                        distbest = aux;
                    }
                    if (distbest > Math.Abs(distaxis)) 
                    {
                        candidate = Closest(p, 2 * index + 1, sol);
                        if (Vector3.Distance(mBalanced[(int)candidate].Point , p) < distbest)
                        {
                            sol = candidate;
                        }
                    }
                }
                else 
                {
                    uint candidate = Closest(p, 2 * index + 1, sol);
                    if ((aux = Vector3.Distance(mBalanced[(int)candidate].Point, p)) < distbest)
                    {
                        sol = candidate;
                        distbest = aux;
                    }
                    if (distbest > Math.Abs(distaxis)) 
                    {
                        candidate = Closest(p, 2 * index, sol);
                        if (Vector3.Distance(mBalanced[(int)candidate].Point, p) < distbest)
                        {
                            sol = candidate;
                        }
                    }
                }
            }
            return sol;
        }

        void Find(Vector3 p, uint index, float radius, List<KDTreeNode<Type>> nodes)
        {
            //We check if our node enters
            if (Vector3.Distance(mBalanced[(int)index].Point,p ) < radius)
            {
                nodes.Add(mBalanced[(int)index]);
            }

            //Now we check that this is not a leaf node
            if (index < ((mBalanced.Count() - 1) / 2))
            {
                float distaxis = 0;
                switch (mBalanced[(int)index].Axis)
                {
                    case KDTreeNode<Type>.Dimension.X:
                        distaxis = p.X - mBalanced[(int)index].Point.X;
                        break;
                    case KDTreeNode<Type>.Dimension.Y:
                        distaxis = p.Y - mBalanced[(int)index].Point.Y;
                        break;
                    case KDTreeNode<Type>.Dimension.Z:
                        distaxis = p.Z - mBalanced[(int)index].Point.Z;
                        break;

                }

               
                if (distaxis < 0.0) 
                {
                    Find(p, 2 * index, radius, nodes);
                    if (radius > Math.Abs(distaxis)) 
                        Find(p, 2 * index + 1, radius, nodes);
                }
                else 
                {
                    Find(p, 2 * index + 1, radius, nodes);
                    if (radius > Math.Abs(distaxis)) 
                        Find(p, 2 * index, radius, nodes);
                }
            }
        }

        float Find(Vector3 p, uint index, uint nb_elements, List<KDTreeNode<Type>> nodes, float dist_worst, List<Pair<uint, float>> dist)
        {
            float aux;
            if ( (aux = Vector3.Distance(mBalanced[(int)index].Point , p)) < dist_worst)
            {
                UpdateHeapNodes(mBalanced[(int)index], aux, nb_elements, nodes, dist);
                dist_worst = (nodes.Count() < nb_elements) ? float.MaxValue : dist[0].B;
            }

            if (index < ((mBalanced.Count() - 1) / 2))
            {
                float distaxis = 0;
                switch (mBalanced[(int)index].Axis)
                {
                    case KDTreeNode<Type>.Dimension.X:
                        distaxis = p.X - mBalanced[(int)index].Point.X;
                        break;
                    case KDTreeNode<Type>.Dimension.Y:
                        distaxis = p.Y - mBalanced[(int)index].Point.Y;
                        break;
                    case KDTreeNode<Type>.Dimension.Z:
                        distaxis = p.Z - mBalanced[(int)index].Point.Z;
                        break;

                }

                if (distaxis < 0.0) 
                {
                    dist_worst = Find(p, 2 * index, nb_elements, nodes, dist_worst, dist);
                    if (dist_worst > Math.Abs(distaxis))
                        dist_worst = Find(p, 2 * index + 1, nb_elements, nodes, dist_worst, dist);
                }
                else 
                {
                    dist_worst = Find(p, 2 * index + 1, nb_elements, nodes, dist_worst, dist);
                    if (dist_worst > Math.Abs(distaxis))
                        dist_worst = Find(p, 2 * index, nb_elements, nodes, dist_worst, dist);
                }
            }
            return dist_worst;
        }

        void UpdateHeapNodes(KDTreeNode<Type> node, float distance, uint nb_elements, List<KDTreeNode<Type>> nodes, List<Pair<uint, float>> dist)
        {
            if (nodes.Count() < nb_elements)
            {
                dist.Add(new Pair<uint, float>((uint)nodes.Count(), distance));
                nodes.Add(node);

                if (nodes.Count() == nb_elements)
                {
                    dist.Sort(delegate (Pair<uint, float> x, Pair<uint, float> y)
                    {
                        return x.B.CompareTo(y.B);
                    });
                }
                //make_heap(dist.begin(), dist.end(), HeapComparison());
            }
            else
            {
                uint idx = dist[0].A;
                nodes[(int)idx] = node;

                // Pop removed element
                dist.RemoveAt(0);
                dist.Sort(delegate (Pair<uint, float> x, Pair<uint, float> y)
                {
                    return x.B.CompareTo(y.B);
                });
                //pop_heap(dist.begin(), dist.end(), HeapComparison());

                dist.RemoveAt(dist.Count-1);

                
                dist.Add( new Pair<uint,float>(idx, distance));
                dist.Sort(delegate (Pair<uint, float> x, Pair<uint, float> y)
                {
                    return x.B.CompareTo(y.B);
                });
            }
        }
    }
}
