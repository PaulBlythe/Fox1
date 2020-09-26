using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Rendering;

namespace GuruEngine.Assets
{
    public class MeshPart
    {
        public int ID;

        public FaceGroup[] facegroups;
        public ushort[] indices;
        public VertexPositionNormalTexture[] verts;
        public Matrix startworld;
        public VertexBuffer vbuffer;
        public IndexBuffer ibuffer;
        public Vector3 Size;

        /// <summary>
        /// Constructor reads in the MeshPart from a binary stream
        /// </summary>
        /// <param name="b"></param>
        public MeshPart(BinaryReader b)
        {
            int nf = b.ReadInt32();
            facegroups = new FaceGroup[nf];
            for (int i=0; i<nf; i++)
            {
                facegroups[i] = new FaceGroup(b);
            }

            int ni = b.ReadInt32();
            indices = new ushort[ni];
            for (int i = 0; i < ni; i++)
            {
                indices[i] = b.ReadUInt16();
            }

            nf = b.ReadInt32();
            verts = new VertexPositionNormalTexture[nf];

            float maxx = float.MinValue;
            float maxy = float.MinValue;
            float maxz = float.MinValue;

            float minx = float.MaxValue;
            float miny = float.MaxValue;
            float minz = float.MaxValue;

            for (int i = 0; i < nf; i++)
            {
                VertexPositionNormalTexture vp = new VertexPositionNormalTexture();

                float px = b.ReadSingle();
                float py = b.ReadSingle();
                float pz = b.ReadSingle();

                minx = Math.Min(px, minx);
                miny = Math.Min(py, miny);
                minz = Math.Min(pz, minz);

                maxx = Math.Max(px, maxx);
                maxy = Math.Max(py, maxy);
                maxz = Math.Max(pz, maxz);

                vp.Position = new Vector3(px, py, pz); 

                px = b.ReadSingle();
                py = b.ReadSingle();
                pz = b.ReadSingle();

                vp.Normal = new Vector3(px, py, pz);
                vp.Normal.Normalize();
                
                px = b.ReadSingle();
                py = b.ReadSingle();

                vp.TextureCoordinate = new Vector2(px, py);
                verts[i] = vp;
            }

            startworld = new Matrix();

            startworld.M11 = b.ReadSingle();
            startworld.M12 = b.ReadSingle();
            startworld.M13 = b.ReadSingle();
            startworld.M14 = b.ReadSingle();
            
            startworld.M21 = b.ReadSingle();
            startworld.M22 = b.ReadSingle();
            startworld.M23 = b.ReadSingle();
            startworld.M24 = b.ReadSingle();
            
            startworld.M31 = b.ReadSingle();
            startworld.M32 = b.ReadSingle();
            startworld.M33 = b.ReadSingle();
            startworld.M34 = b.ReadSingle();
            
            startworld.M41 = b.ReadSingle();
            startworld.M42 = b.ReadSingle();
            startworld.M43 = b.ReadSingle();
            startworld.M44 = b.ReadSingle();

            vbuffer = new VertexBuffer(Renderer.GetGraphicsDevice(), VertexPositionNormalTexture.VertexDeclaration, nf, BufferUsage.WriteOnly);
            vbuffer.SetData<VertexPositionNormalTexture>(verts);

            ibuffer = new IndexBuffer(Renderer.GetGraphicsDevice(), IndexElementSize.SixteenBits, ni, BufferUsage.WriteOnly);
            ibuffer.SetData<ushort>(indices);


            Size = new Vector3((maxx - minx) / 2.0f, (maxy - miny) / 2.0f, (maxz - minz) / 2.0f);
        }

        
    }
}
