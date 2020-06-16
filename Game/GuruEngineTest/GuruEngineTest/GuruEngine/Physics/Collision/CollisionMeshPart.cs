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

using GuruEngine.DebugHelpers;

namespace GuruEngine.Physics.Collision
{
    public class CollisionMeshPart
    {
        public String Name;
        public ushort[] Indices;
        public VertexPosition[] Verts;

        /// <summary>
        /// Load the part from the binary data
        /// </summary>
        /// <param name="b"></param>
        public void Load(BinaryReader b)
        {
            Name = b.ReadString();
            int count = b.ReadInt32();
            Verts = new VertexPosition[count];
            for (int i=0; i<count; i++)
            {
                float x = b.ReadSingle();
                float y = b.ReadSingle();
                float z = b.ReadSingle();
                Verts[i] = new VertexPosition(new Vector3(x, y, z));
            }

            count = b.ReadInt32();
            Indices = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                Indices[i] = b.ReadUInt16();
            }

        }

#if DEBUG
        public void Draw(Matrix world)
        {
            //if (Name.Equals("xcf1"))
            {
                for (int i = 0; i < Indices.Length; i += 3)
                {
                    ushort v1 = Indices[i];
                    ushort v2 = Indices[i + 1];
                    ushort v3 = Indices[i + 2];

                    Vector3 vert1 = Vector3.Transform(Verts[v1].Position, world);
                    Vector3 vert2 = Vector3.Transform(Verts[v2].Position, world);
                    Vector3 vert3 = Vector3.Transform(Verts[v3].Position, world);


                    DebugLineDraw.DrawLine(vert1, vert2, Color.Red);
                    DebugLineDraw.DrawLine(vert2, vert3, Color.Red);
                    DebugLineDraw.DrawLine(vert3, vert1, Color.Red);

                }
            }
           
        }
#endif


    }
}
