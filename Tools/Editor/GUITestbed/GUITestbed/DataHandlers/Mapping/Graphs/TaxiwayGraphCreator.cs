using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GUITestbed.DataHandlers.Fox1.Objects;
using GUITestbed.World;

namespace GUITestbed.DataHandlers.Mapping.Graphs
{
    public class TaxiwayChain
    {
        public List<RunwayTaxiwayPath> Path = new List<RunwayTaxiwayPath>();
        public List<int> Indices = new List<int>();

        public bool Contains(int i)
        {
            return Indices.Contains(i);
        }

        public int FindStart(int i)
        {
            for (int j = 0; j < Path.Count; j++)
            {
                if (Path[j].Start == i)
                    return j;
            }
            return -1;
        }
        public int FindEnd(int i, int exclude)
        {
            for (int j = 0; j < Path.Count; j++)
            {
                if ((Path[j].End == i) && (j != exclude))
                    return j;
            }
            return -1;
        }

        public void Trace(int[] Connections)
        {
            System.Console.WriteLine("Chain start ");
            foreach (RunwayTaxiwayPath p in Path)
            {
                System.Console.WriteLine(String.Format("   {0} to {1}  -- {2} and {3}", p.Start, p.End, Connections[p.Start], Connections[p.End]));
            }
            System.Console.WriteLine("Chain end ");
        }

        /// <summary>
        /// Should only be called when paths contains a single path
        /// </summary>
        /// <param name="TaxiwayPaths"></param>
        /// <param name="n"></param>
        public void FindConnections(List<RunwayTaxiwayPath> TaxiwayPaths, int n)
        {
            RunwayTaxiwayPath exclude = Path[0];
            FindConnection(TaxiwayPaths, n, exclude);
        }

        private void FindConnection(List<RunwayTaxiwayPath> TaxiwayPaths, int n, RunwayTaxiwayPath exclude)
        {
            foreach(RunwayTaxiwayPath r in TaxiwayPaths )
            {
                if (r!=exclude)
                {
                    if (!Path.Contains(r))
                    {
                        if (r.Start == n)
                        {
                            Path.Add(r);
                            Indices.Add(r.End);
                            FindConnection(TaxiwayPaths, r.End, r);
                            return;
                        }
                    }
                }
            }
        }

    }

    public class TaxiwayGraphCreator
    {
        public List<TaxiwayChain> Chains = new List<TaxiwayChain>();

        int[] Connections;

        public TaxiwayGraphCreator(List<RunwayTaxiwayPath> TaxiwayPaths, RunwayTaxiwayPoint[] TaxiwayPoints, double Latitude, double Longitude)
        {
            int max = 0;
            foreach (RunwayTaxiwayPath r in TaxiwayPaths)
            {
                if (r.Start > max)
                    max = r.Start;

                if (r.End > max)
                    max = r.End;
            }

            Connections = new int[max + 1];
            for (int i = 0; i < max + 1; i++)
                Connections[i] = 0;

            foreach (RunwayTaxiwayPath r in TaxiwayPaths)
            {
                Connections[r.End]++;
                Connections[r.Start]++;
            }

            foreach (RunwayTaxiwayPath r in TaxiwayPaths)
            {
                // Find an endpoint
                if (Connections[r.Start] == 1)
                {
                    TaxiwayChain tc = new TaxiwayChain();
                    tc.Path.Add(r);
                    tc.Indices.Add(r.Start);
                    tc.Indices.Add(r.End);
                    tc.FindConnections(TaxiwayPaths, r.End);

                    Chains.Add(tc);
                }
               
            }

            System.Console.WriteLine("Unsorted ");
            foreach (TaxiwayChain tc in Chains)
                tc.Trace(Connections);

            //// Sort the chains
            //foreach (TaxiwayChain tc in Chains)
            //{
            //    SortChain(tc);
            //}
            //
            // Check for out of order start and ends
            //foreach (TaxiwayChain tc in Chains)
            //{
            //    int changes = 0;
            //    do
            //    {
            //        changes = 0;
            //        for (int i = 0; i < tc.Path.Count - 1; i++)
            //        {
            //            int start = tc.Path[i].Start;
            //            int endx = tc.Path[i].End;
            //            if (tc.Path[i + 1].End == endx)
            //            {
            //                Swap(tc.Path[i + 1]);
            //            }
            //        }
            //
            //    } while (changes > 0);
            //}

            // Sort the chains
            //foreach (TaxiwayChain tc in Chains)
            //{
            //    SortChain(tc);
            //}

            System.Console.WriteLine("Sorted ");
            //foreach (TaxiwayChain tc in Chains)
            //    tc.Trace(Connections);

        }


        void Swap(RunwayTaxiwayPath p)
        {
            int g = p.Start;
            p.Start = p.End;
            p.End = g;
        }

        /// <summary>
        /// Sort the chain into display order
        /// </summary>
        /// <param name="tc"></param>
        void SortChain(TaxiwayChain tc)
        {
            int changes = 0;
            do
            {
                changes = 0;
                for (int i = 0; i < tc.Path.Count - 1; i++)
                {
                    int endx = tc.Path[i].End;
                    if (tc.Path[i + 1].Start != endx)
                    {
                        int dd = tc.FindStart(endx);
                        if (dd != -1)
                        {
                            RunwayTaxiwayPath p1 = tc.Path[i + 1];
                            RunwayTaxiwayPath p2 = tc.Path[dd];
                            tc.Path[i + 1] = p2;
                            tc.Path[dd] = p1;

                            changes++;
                        }
                    }
                }

            } while (changes > 0);
        }

        int IsInChain(int j)
        {
            if (Chains.Count == 0)
                return -1;

            for (int i = 0; i < Chains.Count; i++)
            {
                if (Chains[i].Contains(j))
                    return i;
            }
            return -1;
        }
    }
}
