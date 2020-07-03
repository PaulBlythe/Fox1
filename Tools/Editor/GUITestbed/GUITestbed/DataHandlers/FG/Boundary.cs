using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GUITestbed.DataHandlers.FG
{
    public class Boundary
    {
        public String Name;
        public List<LineNode> Nodes = new List<LineNode>();
        public List<PointSequence> chains = new List<PointSequence>();

        public Boundary(String definition)
        {
            definition = Parser.Skip(definition);
            Name = definition;
        }

        public void ConvertToChain(double clat, double clon, float altitude)
        {
            chains = APTHelper.LineNodeListToChain(Nodes);
            foreach (PointSequence p in chains)
            {
                p.Verts = APTHelper.PointSequenceToVertices(p, clat, clon, altitude);
                //draws.Add(GeometryHelper.ConvertChaintoDrawableStrip(p, 4, Game1.Instance.White, p.Closed));
            }
        }
        
    }
}
