using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.FG
{
    public class LinearFeature
    {
        public String Name;
        public List<LineNode> Nodes = new List<LineNode>();
        public List<PointSequence> chains = new List<PointSequence>();

        public LinearFeature(String definition)
        {
            definition = Parser.Skip(definition);
            Name = definition;
        }

        //public void ConvertToChain(double clat, double clon, float altitude)
        //{
        //    chains = APTHelper.LineNodeListToChain(Nodes);
        //    foreach (PointSequence p in chains)
        //    {
        //        p.Verts = APTHelper.PointSequenceToVertices(p, clat, clon, altitude);
        //
        //        int lt = p.Points[0].linetype;
        //        Texture2D tex = Game1.Instance.White;
        //        if (Game1.Instance.LineTextures.ContainsKey(lt))
        //        {
        //            tex = Game1.Instance.LineTextures[lt];
        //        }
        //        draws.Add(GeometryHelper.ConvertChaintoDrawableStrip(p, 0.3f, tex, p.Closed));
        //
        //    }
        //}

        
    }
}
