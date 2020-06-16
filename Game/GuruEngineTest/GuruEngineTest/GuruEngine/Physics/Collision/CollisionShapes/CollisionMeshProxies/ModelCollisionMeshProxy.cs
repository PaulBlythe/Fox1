using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Helpers;



namespace GuruEngine.Physics.Collision.Shapes
{
	/// <summary>
	/// Represents an abstraction from the triangle information of a <see cref="Microsoft.Xna.Framework.Graphics.Model"/>.
	/// </summary>
	public sealed class ModelCollisionMeshProxy : CollisionMeshProxy
	{
		private static ObjectPool<Queue<BvhNode>> queuePool = new ObjectPool<Queue<BvhNode>>(4);

		#region BvhNode Declaration

		/// <summary>
		/// An abstract class representing a node in a bounding volume hierarchy.
		/// </summary>
		internal abstract class BvhNode
		{
			/// <summary>
			/// Gets the axis-aligned bounding box for the <see cref="BvhNode"/>.
			/// </summary>
			internal BoundingBox AABB;
		}

		#endregion

		#region BvhInnerNode Declaration

		/// <summary>
		/// Represents an inner node that encompasses other nodes in a bounding volume hierarchy.
		/// </summary>
		internal sealed class BvhInnerNode : BvhNode
		{
			#region BvhLeafNodeComparer Declaration

			/// <summary>
			/// A comparer used to sort <see cref="BvhLeafNode"/> instances by the specified vector
			/// component.
			/// </summary>
			internal struct BvhLeafNodeComparer : IComparer<BvhLeafNode>
			{
				private VectorIndex longestSide;

				/// <summary>
				/// Initializes a new instance of the <see cref="BvhLeafNodeComparer"/> structure.
				/// </summary>
				/// <param name="longestSide"></param>
				internal BvhLeafNodeComparer(VectorIndex longestSide)
				{
					// Hold onto values.
					this.longestSide = longestSide;
				}

				#region IComparer<BvhLeafNode> Members

				int IComparer<BvhLeafNode>.Compare(BvhLeafNode x, BvhLeafNode y)
				{
					// Get the component value of the axis-aligned bounding boxes of the specified leaf nodes.

					float x0 = MathsHelper.GetVector3Component(x.AABB.Max,this.longestSide);
					float y0 = MathsHelper.GetVector3Component(y.AABB.Max, this.longestSide);

                    // Get the result.
                    return x0 < y0 ? -1 : x0 > y0 ? 1 : 0;
				}

				#endregion
			}

			#endregion

			/// <summary>
			/// Gets the <see cref="BvhNode"/> for nearer volumes.
			/// </summary>
			internal BvhNode Node0;

			/// <summary>
			/// Gets the <see cref="BvhNode"/> for further volumes.
			/// </summary>
			internal BvhNode Node1;

			/// <summary>
			/// Initializes a new instance of the <see cref="BvhInnerNode"/> class.
			/// </summary>
			/// <param name="leafNodes">A collection of <see cref="BvhLeafNode"/> instances that are bound by the <see cref="BvhInnerNode"/> volume.</param>
			internal BvhInnerNode(List<BvhLeafNode> leafNodes)
			{
				// Determine the axis-aligned bounding box that encompasses the specified leaf nodes.
				this.AABB = MathsHelper.EmptyBB;
				for (int i = 0; i < leafNodes.Count; i++)
				{

					this.AABB = BoundingBox.CreateMerged(this.AABB, leafNodes[i].AABB);
				}

                // TODO : page 262.  Use spatial median or object/spatial hybrid???  Current uses object median.
                // Determine the component index of the axis-aligned bound box with the longest side
                // And sort the leaf nodes by that component's value.
                Vector3 l;
                MathsHelper.GetBoundingBoxSideLengths(ref this.AABB, out l);
                VectorIndex longestSide = MathsHelper.MaxComponentIndex(l);

				leafNodes.Sort(new BvhLeafNodeComparer(longestSide));

				// The child nodes are based on how many triangles are left to be partitioned.
				switch (leafNodes.Count)
				{
					case 1:
						System.Diagnostics.Debug.Assert(false);
						break;

					case 2:
						// Consider the first and second nodes as leafs.  No recursion necessary.
						this.Node0 = leafNodes[0];
						this.Node1 = leafNodes[1];

						break;

					case 3:
						// Consider the first node a leaf but process the other two leaf nodes further.
						this.Node0 = leafNodes[0];
						this.Node1 = new BvhInnerNode(leafNodes.GetRange(1, 2));

						break;

					default:
						// Split the leaf node collection in half and continue.
						int count = leafNodes.Count >> 1;
						this.Node0 = new BvhInnerNode(leafNodes.GetRange(0, count));
						this.Node1 = new BvhInnerNode(leafNodes.GetRange(count, leafNodes.Count - count));

						break;
				}
			}
		}

		#endregion

		#region BvhLeafNode Declaration

		/// <summary>
		/// Represents a leaf node in a bounding volume hierarchy that contains a <see cref="CollisionTriangle"/>.
		/// </summary>
		internal sealed class BvhLeafNode : BvhNode
		{
			/// <summary>
			/// Gets the associated <see cref="CollisionTriangle"/> for the <see cref="BvhLeafNode"/>.
			/// </summary>
			internal CollisionMeshTriangle Triangle;

			/// <summary>
			/// Initializes a new instance of the <see cref="BvhLeafNode"/> class.
			/// </summary>
			/// <param name="triangle">The associated <see cref="CollisionTriangle"/>.</param>
			internal BvhLeafNode(CollisionMeshTriangle triangle)
			{
				// Hold onto parameters.
				this.Triangle = triangle;

				// Get the vertex positions of the triangle.
				CollisionTriangleEx triangleEx;
				this.Triangle.proxy.GetExtendedTriangle(triangle, out triangleEx);

				// Calculate the axis-aligned bounding box for the specified triangle.

				this.AABB = new BoundingBox(
					Vector3.Min(triangleEx.Vertex0, Vector3.Min(triangleEx.Vertex1, triangleEx.Vertex2)),
					Vector3.Max(triangleEx.Vertex0, Vector3.Max(triangleEx.Vertex1, triangleEx.Vertex2)));
			}
		}

		#endregion

		private List<Vector3> positions = new List<Vector3>();
		private List<CollisionMeshTriangle> triangles = new List<CollisionMeshTriangle>();

		private BvhNode rootNode;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionMeshProxy"/> class.
		/// </summary>
		/// <param name="model">The <see cref="GuruEngine.Physics.Collision.CollisionMeshPart"/> to extract triange data from.</param>
		public ModelCollisionMeshProxy(CollisionMeshPart model)
		{
			// Get the positions and triangle indices from the model.
			this.Extract(model);

			// Create the bounding volume hierarchy.
			this.Initialize();
		}

		/// <summary>
		/// Gets the positions of the <see cref="CollisionMeshProxy"/>.
		/// </summary>
		private IList<Vector3> Positions
		{
			get { return this.positions; }
		}

		/// <summary>
		/// Gets the triangles that make up the <see cref="CollisionMeshProxy"/>.
		/// </summary>
		private IList<CollisionMeshTriangle> Triangles
		{
			get { return this.triangles; }
		}

		#region CollisionMeshProxy Members

		public override void GetExtendedTriangle(CollisionMeshTriangle triangle, out CollisionTriangleEx result)
		{

			// Get the current triangle's vertices.
			result.Vertex0 = this.Positions[triangle.Index0];
			result.Vertex1 = this.Positions[triangle.Index1];
			result.Vertex2 = this.Positions[triangle.Index2];
		}

		public override bool GetIntersectingTriangles(ref BoundingBox aabb, IList<CollisionMeshTriangle> result)
		{
			// There is no hierarchy.
			if (this.rootNode == null)
			{
				return false;
			}

			// Retrieve queue from the pool.
			Queue<BvhNode> queryQueue = ModelCollisionMeshProxy.queuePool.Retrieve();
			try
			{
				// Start with the root node.
				queryQueue.Enqueue(this.rootNode);

				// Iterate until the queue is empty.
				while (queryQueue.Count > 0)
				{
					// Get the next node.
					BvhNode node = queryQueue.Dequeue();

					// Determine if the node is an inner or leaf node.
					BvhInnerNode innerNode = node as BvhInnerNode;
					if (innerNode != null)
					{
						// TODO : Specify the tolerance as a parameter or include in aabb parameter.

						// Add the first node to the queue if an intersection occurred.
						if (MathsHelper.BoundingBoxContains(aabb,innerNode.Node0.AABB, 0.04f) != ContainmentType.Disjoint)
						{
							queryQueue.Enqueue(innerNode.Node0);
						}

						// Add the second node to the queue if an intersection occurred.
						if (MathsHelper.BoundingBoxContains(aabb,innerNode.Node1.AABB, 0.04f) != ContainmentType.Disjoint)
						{
							queryQueue.Enqueue(innerNode.Node1);
						}
					}
					else
					{
						// Add the associated triangle of the current node to the result.
						BvhLeafNode leafNode = (BvhLeafNode)node;
						result.Add(leafNode.Triangle);
					}
				}
			}
			finally
			{
				// Recycle the queue.
				ModelCollisionMeshProxy.queuePool.Recycle(queryQueue);
			}

			// Determine the return value.
			return result.Count > 0;
		}

		public override bool GetIntersectingTriangles(ref CollisionRay ray, IList<CollisionMeshTriangle> result)
		{
			// There is no hierarchy.
			if (this.rootNode == null)
			{
				return false;
			}

			// Retrieve queue from the pool.
			Queue<BvhNode> queryQueue = ModelCollisionMeshProxy.queuePool.Retrieve();
			try
			{
				// Start with the root node.
				queryQueue.Enqueue(this.rootNode);

				// Iterate until the queue is empty.
				while (queryQueue.Count > 0)
				{
					// Get the next node.
					BvhNode node = queryQueue.Dequeue();

					// Determine if the node is an inner or leaf node.
					BvhInnerNode innerNode = node as BvhInnerNode;
					if (innerNode != null)
					{
						// Add the first node to the queue if an intersection occurred.
						if (ray.Intersects(ref innerNode.Node0.AABB))
						{
							queryQueue.Enqueue(innerNode.Node0);
						}

						// Add the second node to the queue if an intersection occurred.
						if (ray.Intersects(ref innerNode.Node1.AABB))
						{
							queryQueue.Enqueue(innerNode.Node1);
						}
					}
					else
					{
						// Add the associated triangle of the current node to the result.
						BvhLeafNode leafNode = (BvhLeafNode)node;
						result.Add(leafNode.Triangle);
					}
				}
			}
			finally
			{
				// Recycle the queue.
				ModelCollisionMeshProxy.queuePool.Recycle(queryQueue);
			}

			// Determine the return value.
			return result.Count > 0;
		}

		public override void ReleaseIntersectingTriangles(IList<CollisionMeshTriangle> triangles)
		{
			// Do nothing.  The triangles do not need to be recycled.
		}

		#endregion

		/// <summary>
		/// Extracts the positions and triangle indices from the model.
		/// </summary>
		/// <param name="model">The <see cref="Microsoft.Xna.Framework.Graphics.Model"/> to extract triange data from.</param>
		private void Extract(CollisionMeshPart model)
		{
            // Extract the triangle information from the model.
            List<int> indices = new List<int>();
            for (int i=0; i<model.Indices.Length; i++)
            {
                indices.Add(model.Indices[i]);
            }

            positions = new List<Vector3>();
            for (int i=0; i<model.Verts.Length; i++)
            {
                positions.Add(model.Verts[i].Position);
            }



			// Create collision triangles from the indices.
			for (int i = 0; i < indices.Count; i += 3)
			{
				this.triangles.Add(new CollisionMeshTriangle(this, indices[i], indices[i + 1], indices[i + 2]));
			}
		}

		/// <summary>
		/// Creates the bounding volume hierarchy for speedier querying for intersections.
		/// </summary>
		private void Initialize()
		{
			if (this.triangles.Count > 0)
			{
				// Create a leaf node for each collision triangle.
				List<BvhLeafNode> leafNodes = new List<BvhLeafNode>();
				for (int i = 0; i < this.triangles.Count; i++)
				{
					leafNodes.Add(new BvhLeafNode(this.triangles[i]));
				}

				// Create the node hierarchy.
				this.rootNode = this.triangles.Count == 1 ? (BvhNode)leafNodes[0] : new BvhInnerNode(leafNodes);

				// Expose the axis-aligned bounding box of the entire mesh.
				this.AABB = this.rootNode.AABB;
			}
		}
	}

}
