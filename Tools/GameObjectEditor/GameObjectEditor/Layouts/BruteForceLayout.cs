using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

using GameObjectEditor.GameComponent;

namespace GameObjectEditor.Layouts
{
    public class BruteForceLayout
    {
        public int MaxIterations = 300;
        public double ForceMultiplier = 100.25f;
        public double ForceDistanceLimit = 400.0f;
        public double IdealEdgeLength = 250.0f;
        
        Vector[] forces;
        List<GameComponentDescriptor> Vertices = new List<GameComponentDescriptor>();
        List<Region> Regions = new List<Region>();
        List<Edge> Edges = new List<Edge>();

        Dictionary<String, int> Atlas = new Dictionary<string, int>();

        public void AddVertex(GameComponentDescriptor vert)
        {
            Point p = vert.GetPosition();
            Size s = vert.GetSize();

            Region r = new Region(p.X, p.Y, s.Width, s.Height);
            Vertices.Add(vert);
            Regions.Add(r);

            Atlas.Add(vert.Name, Vertices.Count - 1);
        }

        

        public void Run()
        {
            FindEdges();

            double dl = ForceDistanceLimit * ForceDistanceLimit;
            forces = new Vector[Vertices.Count];
            for (int j = 0; j < MaxIterations; j++)
            {
                int count = 0;
                ClearForces();
                for (int i = 0; i < Vertices.Count; i++)
                {
                    for (int k = 0; k < Vertices.Count; k++)
                    {
                        if (i != k)
                        {
                            double d2 = Regions[i].DistanceSquared(Regions[k]);
                            if (d2<dl)
                            {
                                Vector d = Regions[i].Direction(Regions[k]);
                                forces[i] += d;
                                count++;
                            }
                        }
                    }
                }
                for (int i=0; i<Edges.Count; i++)
                {
                    int v1 = Edges[i].Index1;
                    int v2 = Edges[i].Index2;
                    double d2 = Regions[v1].DistanceSquared(Regions[v2]);
                    if (d2 > dl)
                    {
                        if (!double.IsInfinity(d2))
                        {
                            Vector d = Regions[v1].Direction(Regions[v2]);
                            Vector f = d * 0.25;

                            if ((double.IsNaN(f.X)) || (double.IsNaN(f.Y)))
                            {
                                throw new Exception("ettttt");
                            }
                            else
                            {
                                forces[v1] -= f;
                                forces[v2] += f;
                            }
                        }
                        count++;
                    }
                }

                if (count == 0)
                {
                    Apply();
                    return;
                }

                ApplyForces(ForceMultiplier);
            }
            Apply();

            HorizontalImproved();
            Apply();
            VerticalImproved();
            Apply();
        }

        private void FindEdges()
        {
            int i = 0;
            foreach(GameComponentDescriptor g in Vertices)
            {
                foreach (GameComponentConnection gcc in g.Connections)
                {
                    if (gcc.Name != "Root")
                    {
                        String c = gcc.ConnectedTo;
                        String[] parts = c.Split(':');

                        int k = Atlas[parts[0]];
                        Edge e = new Edge(i, k);
                        Edges.Add(e);
                    }
                }
                i++;
            }
        }

        private void ClearForces()
        {
            int nVerts = Vertices.Count;
            for (int i=0; i<nVerts; i++)
            {
                forces[i] = new Vector(0, 0);
            }
        }

        private void ApplyForces(double mul)
        {
            int nVerts = Vertices.Count;
            for (int i = 0; i < nVerts; i++)
            {
                Vector force = forces[i] * mul;
                Regions[i].Move(force);
            }
        }

        private void Apply()
        {
            int nVerts = Vertices.Count;
            for (int i = 0; i < nVerts; i++)
            {
                Vertices[i].SetPosition((float)Regions[i].X, (float)Regions[i].Y);
            }
        }


        private Vector Force(Region vi, Region vj)
        {
            var f = new Vector(0, 0);
            if (vi.Intersects(vj))
            {
                f.X = 20;
            }
            return f;
        }

        private Vector Force2(Region vi, Region vj)
        {
            var f = new Vector(0, 0);
           
            if (vi.Intersects(vj))
            {
                f.Y = 20;
            }
            return f;
        }

       

        protected void HorizontalImproved()
        {
            bool done = false;
            for (int i=0; i<MaxIterations; i++)
            {
                ClearForces();
                done = true;
                
                for (int j=0; j<Regions.Count; j++)
                {
                    for (int k = 0; k < Regions.Count; k++)
                    {
                        if (j != k)
                        {
                            int act = j;
                            Region r1 = Regions[j];
                            Region r2 = Regions[k];
                            if (r1.X >r2.X)
                            {
                                r2 = r1;
                                r1 = Regions[k];
                                act = k;
                            }
                            Vector f = Force(r1,r2);
                            if (f.Length > 0)
                            {
                                done = false;
                                forces[act] += f;
                            }
                        }
                    }
                }
                if (done)
                    return;
                ApplyForces(0.00025);
            }
        }

        protected void VerticalImproved()
        {
            bool done = false;
            for (int i = 0; i < MaxIterations; i++)
            {
                ClearForces();
                done = true;

                for (int j = 0; j < Regions.Count; j++)
                {
                    for (int k = 0; k < Regions.Count; k++)
                    {
                        if (j != k)
                        {
                            int act = j;
                            Region r1 = Regions[j];
                            Region r2 = Regions[k];
                            if (r1.Y > r2.Y)
                            {
                                r2 = r1;
                                r1 = Regions[k];
                                act = k;
                            }
                            Vector f = Force2(r1, r2);
                            if (f.Length > 0)
                            {
                                done = false;
                                forces[act] += f;
                            }
                        }
                    }
                }
                if (done)
                    return;
                ApplyForces(0.00025);
            }
        }


    }
}
