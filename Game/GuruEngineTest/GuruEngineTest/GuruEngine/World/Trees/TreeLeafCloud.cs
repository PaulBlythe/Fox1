using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using GuruEngine.Rendering.VertexDeclarations;
using GuruEngine.Rendering;

namespace GuruEngine.World.Trees
{
    public class TreeLeafCloud : IDisposable
    {
        private GraphicsDevice device;
        private VertexBuffer vbuffer;

        public VertexBuffer Vbuffer
        {
            get { return vbuffer; }
        }
        private IndexBuffer ibuffer;

        public IndexBuffer Ibuffer
        {
            get { return ibuffer; }
        }
        private VertexDeclaration vdeclaration;

        public VertexDeclaration Vdeclaration
        {
            get { return vdeclaration; }
        }
        private int numleaves;

        public int Numleaves
        {
            get { return numleaves; }
            set { numleaves = value; }
        }
        private BoundingSphere boundingSphere;

        /// <summary>
        /// A bounding sphere enclosing all the leaves at any camera angle.
        /// </summary>
        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
            set { boundingSphere = value; }
        }

        /// <summary>
        /// Creates a leaf cloud displaying the leaves on the specified tree skeleton.
        /// </summary>
        /// <param name="device">The graphics device.</param>
        /// <param name="skeleton">The tree skeleton whose leaves you want to display.</param>
        /// <remarks>
        /// The leaf cloud does not remember the skeleton that generated it. The skeleton may be changed
        /// without affecting previously generated leaf clouds.
        /// </remarks>
        public TreeLeafCloud(GraphicsDevice device, TreeSkeleton skeleton)
        {
            this.device = device;

            Init(skeleton);
        }

        private void Init(TreeSkeleton skeleton)
        {
            if (skeleton.Leaves.Count == 0)
                return;

            Matrix[] transforms = new Matrix[skeleton.Branches.Count];
            skeleton.CopyAbsoluteBranchTransformsTo(transforms);

            Vector3 center = Vector3.Zero;
            for (int i = 0; i < skeleton.Leaves.Count; i++)
            {
                center += transforms[skeleton.Leaves[i].ParentIndex].Translation;
            }
            center = center / (float)skeleton.Leaves.Count;

            LeafVertex[] vertices = new LeafVertex[skeleton.Leaves.Count * 4];
            short[] indices = new short[skeleton.Leaves.Count * 6];

            int vindex = 0;
            int iindex = 0;

            boundingSphere.Center = center;
            boundingSphere.Radius = 0.0f;

            foreach (TreeLeaf leaf in skeleton.Leaves)
            {
                // Get the position of the leaf
                Vector3 position = transforms[leaf.ParentIndex].Translation + transforms[leaf.ParentIndex].Up * skeleton.Branches[leaf.ParentIndex].Length;
                if (skeleton.LeafAxis != null)
                {
                    position += skeleton.LeafAxis.Value * leaf.AxisOffset;
                }

                // Orientation
                Vector2 right = new Vector2((float)Math.Cos(leaf.Rotation), (float)Math.Sin(leaf.Rotation));
                Vector2 up = new Vector2(-right.Y, right.X);

                // Scale vectors by size
                right = leaf.Size.X * right;
                up = leaf.Size.Y * up;

                // Choose a normal vector for lighting calculations
                float distanceFromCenter = Vector3.Distance(position, center);
                Vector3 normal = (position - center) / distanceFromCenter; // normalize the normal

                //                    0---1
                // Vertex positions:  | \ |
                //                    3---2
                int vidx = vindex;
                vertices[vindex++] = new LeafVertex(position, new Vector2(0, 0), -right + up, leaf.Color, leaf.BoneIndex, normal);
                vertices[vindex++] = new LeafVertex(position, new Vector2(1, 0), right + up, leaf.Color, leaf.BoneIndex, normal);
                vertices[vindex++] = new LeafVertex(position, new Vector2(1, 1), right - up, leaf.Color, leaf.BoneIndex, normal);
                vertices[vindex++] = new LeafVertex(position, new Vector2(0, 1), -right - up, leaf.Color, leaf.BoneIndex, normal);

                // Add indices
                indices[iindex++] = (short)(vidx);
                indices[iindex++] = (short)(vidx + 1);
                indices[iindex++] = (short)(vidx + 2);

                indices[iindex++] = (short)(vidx);
                indices[iindex++] = (short)(vidx + 2);
                indices[iindex++] = (short)(vidx + 3);

                // Update the bounding sphere
                float size = leaf.Size.Length() / 2.0f;
                boundingSphere.Radius = Math.Max(boundingSphere.Radius, distanceFromCenter + size);
            }

            // Create the vertex declaration
            vdeclaration = new VertexDeclaration(LeafVertex.VertexElements);

            // Create the buffers
            vbuffer = new VertexBuffer(device, vdeclaration, vertices.Length, BufferUsage.None);
            vbuffer.SetData<LeafVertex>(vertices);

            ibuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            ibuffer.SetData<short>(indices);

          
            // Remember the number of leaves
            numleaves = skeleton.Leaves.Count;
        }

        

        #region IDisposable Members

        public void Dispose()
        {
            vbuffer.Dispose();
            ibuffer.Dispose();
            vdeclaration.Dispose();
        }

        #endregion
    }
}
