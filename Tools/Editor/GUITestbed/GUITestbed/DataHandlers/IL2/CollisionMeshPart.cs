using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUITestbed.DataHandlers.IL2
{
    public class CollisionMeshPart
    {
        public String Type;
        public String Name;
        public int NFrames;
        public List<Vector3> Verts = new List<Vector3>();
        public List<int> NeiCount = new List<int>();
        public List<int> Neighbours = new List<int>();
        public List<short> Faces = new List<short>();
        public int TypeIntExt;

        public short[] indices;
        public VertexPositionColor[] verts;

        public CollisionMeshPart()
        {
            Type = "";
            NFrames = 0;
            TypeIntExt = 0;
        }

        public void AddVertex(double x, double y, double z)
        {
            Vector3 v = new Vector3((float)x, (float)y, (float)z);
            Verts.Add(v);

        }
    }

}
