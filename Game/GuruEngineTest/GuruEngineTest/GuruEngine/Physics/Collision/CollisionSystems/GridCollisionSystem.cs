//#define GRIDNODE_RECYCLE_UNUSED

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GuruEngine.Core;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision.CollisionSystems
{
	/// <summary>
	/// Defines an infinite three-dimensional grid strategy for broad-phase collision processing.
	/// </summary>
	public class GridCollisionSystem : CollisionSystem
	{
		#region GridNode Declaration

		/// <summary>
		/// Represents a cell in the <see cref="GridCollsionSystem"/>.
		/// </summary>
		internal sealed class GridNode
		{
			private static ObjectPool<GridNode> pool = new ObjectPool<GridNode>(2000);

			private List<CollisionGroup> collisionGroups = new List<CollisionGroup>(4);

			/// <summary>
			/// Gets the collection of <see cref="CollisionGroup"/> instances associated with the <see cref="GridNode"/>.
			/// </summary>
			internal IList<CollisionGroup> CollisionGroups
			{
				get { return this.collisionGroups; }
			}

			/// <summary>
			/// Gets a <see cref="GridNode" /> instance from the pool.
			/// </summary>
			/// <returns>
			/// Returns a fresh <see cref="GridNode"/> instance.
			/// </returns>
			internal static GridNode Retrieve()
			{
				// Return an unused grid node instance.
				return GridNode.pool.Retrieve();
			}

			/// <summary>
			/// Returns a <see cref="GridNode"/> instance to the pool.
			/// </summary>
			/// <param name="gridNode">The <see cref="GridNode"/> instance to recycle</param>
			internal static void Recycle(GridNode gridNode)
			{
				// Uninitialize.
				gridNode.CollisionGroups.Clear();

				// Put the grid node back in the pool for later use.
				GridNode.pool.Recycle(gridNode);
			}
		}

		#endregion

		#region GridNodeRange Declaration

		/// <summary>
		/// Holds the cell positions of a bounding box of <see cref="GridNode"/> instances.
		/// </summary>
		internal sealed class GridNodeRange
		{
			private static ObjectPool<GridNodeRange> pool = new ObjectPool<GridNodeRange>(256);

			/// <summary>
			/// Gets or sets the minimum cell in the x-direction of the <see cref="GridNodeRange"/>.
			/// </summary>
			internal int MinX { get; set; }

			/// <summary>
			/// Gets or sets the maximum cell in the x-direction of the <see cref="GridNodeRange"/>.
			/// </summary>
			internal int MaxX { get; set; }

			/// <summary>
			/// Gets or sets the minimum cell in the y-direction of the <see cref="GridNodeRange"/>.
			/// </summary>
			internal int MinY { get; set; }

			/// <summary>
			/// Gets or sets the maximum cell in the y-direction of the <see cref="GridNodeRange"/>.
			/// </summary>
			internal int MaxY { get; set; }

			/// <summary>
			/// Gets or sets the minimum cell in the z-direction of the <see cref="GridNodeRange"/>.
			/// </summary>
			internal int MinZ { get; set; }

			/// <summary>
			/// Gets or sets the maximum cell in the z-direction of the <see cref="GridNodeRange"/>.
			/// </summary>
			internal int MaxZ { get; set; }

			/// <summary>
			/// Gets a <see cref="GridNodeRange" /> instance from the pool.
			/// </summary>
			/// <returns>
			/// Returns a fresh <see cref="GridNodeRange"/> instance.
			/// </returns>
			internal static GridNodeRange Retrieve()
			{
				// Return an unused grid node range instance.
				return GridNodeRange.pool.Retrieve();
			}

			/// <summary>
			/// Returns a <see cref="GridNodeRange"/> instance to the pool.
			/// </summary>
			/// <param name="range">The <see cref="GridNodeRange"/> instance to recycle</param>
			internal static void Recycle(GridNodeRange range)
			{
				// Put the grid node range back in the pool for later use.
				GridNodeRange.pool.Recycle(range);
			}
		}

		#endregion

		private const float LARGE_SIZE = float.MaxValue / 100.0f;

		private float cellSize;
		private float inverseCellSize;

		private List<CollisionGroup> largeCollisionGroups = new List<CollisionGroup>(16);
		private Dictionary<uint, GridNode> gridNodes = new Dictionary<uint, GridNode>(256);

		/// <summary>
		/// Initializes a new instance of the <see cref="GridCollisionSystem"/> class.
		/// </summary>
		/// <param name="cellSize">The size of cells in all dimensions of the <see cref="GridCollisionSystem"/>.</param>
		public GridCollisionSystem(float cellSize)
			: base()
		{
			// Hold onto parameters.
			this.cellSize = cellSize;

			// Hold onto the inverse of the cell size.
			this.inverseCellSize = 1.0f / this.cellSize;
		}

		#region CollisionSystem Members

		public override void DetectCollisions(RigidBody rigidBody, CollisionContext context)
		{
			// Get the range of grid node cells the rigid body's collision group intersects withs.
			int minX = (int)Math.Floor(rigidBody.CollisionGroup.AABB.Min.X * this.inverseCellSize);
			int maxX = (int)Math.Floor(rigidBody.CollisionGroup.AABB.Max.X * this.inverseCellSize);
			int minY = (int)Math.Floor(rigidBody.CollisionGroup.AABB.Min.Y * this.inverseCellSize);
			int maxY = (int)Math.Floor(rigidBody.CollisionGroup.AABB.Max.Y * this.inverseCellSize);
			int minZ = (int)Math.Floor(rigidBody.CollisionGroup.AABB.Min.Z * this.inverseCellSize);
			int maxZ = (int)Math.Floor(rigidBody.CollisionGroup.AABB.Max.Z * this.inverseCellSize);

			// Iterate on all intersecting cells.
			for (int x = minX; x <= maxX; x++)
			{
				for (int y = minY; y <= maxY; y++)
				{
					for (int z = minZ; z <= maxZ; z++)
					{
						// Get the key for the current overlapping cell.
						uint key = GridCollisionSystem.CalculateKey(x, y, z);

						// Get the grid node for the current key.
						GridNode node;
						if (this.gridNodes.TryGetValue(key, out node))
						{
							// Iterate on the current grid node's associated collision groups.
							for (int i = 0; i < node.CollisionGroups.Count; i++)
							{
								// Detect collisions.
								this.DetectCollisions(rigidBody.CollisionGroup, node.CollisionGroups[i], context);
							}
						}
					}
				}
			}

			// Check the large-sized collision groups too.
			for (int i = 0; i < this.largeCollisionGroups.Count; i++)
			{
				// Detect collisions.
				this.DetectCollisions(rigidBody.CollisionGroup, this.largeCollisionGroups[i], context);
			}
		}

		public override bool IntersectsWith(ref CollisionRay ray, ICollisionRayCallback callback, out CollisionGroup collisionGroup, out Vector3 position, out Vector3 normal, out float t)
		{
			// Initialize results.
			collisionGroup = null;
			position = Vector3.Zero;
			normal = Vector3.Zero;
			t = float.MaxValue;

			// Working storage.
			Vector3 localPosition;
			Vector3 localNormal;
			float localT;

			// Make copy of ray.
			CollisionRay localRay = ray;

            // Get the axis-aligned bound box that encompasses the collision ray.

            BoundingBox aabb = ray.GetBoundingBox();

			// Get the range of grid node cells the axis-aligned bounding box intersects.
			int minX = (int)Math.Floor(aabb.Min.X * this.inverseCellSize);
			int maxX = (int)Math.Floor(aabb.Max.X * this.inverseCellSize);
			int minY = (int)Math.Floor(aabb.Min.Y * this.inverseCellSize);
			int maxY = (int)Math.Floor(aabb.Max.Y * this.inverseCellSize);
			int minZ = (int)Math.Floor(aabb.Min.Z * this.inverseCellSize);
			int maxZ = (int)Math.Floor(aabb.Max.Z * this.inverseCellSize);

			// Iterate on all intersecting cells.
			for (int x = minX; x <= maxX; x++)
			{
				for (int y = minY; y <= maxY; y++)
				{
					for (int z = minZ; z <= maxZ; z++)
					{
						// Get the key for the current overlapping cell.
						uint key = GridCollisionSystem.CalculateKey(x, y, z);

						// Get the grid node for the current key.
						GridNode node;
						if (this.gridNodes.TryGetValue(key, out node))
						{
							// Iterate on the grid node's collision groups.
							for (int i = 0; i < node.CollisionGroups.Count; i++)
							{
								// Get the current collision group.
								CollisionGroup localGroup = node.CollisionGroups[i];

								// Use callback to determine eligibility.
								if (callback != null && !callback.CanCompare(localGroup))
								{
									continue;
								}

								// Check for intersection between the current collision group's bounds
								// and the collision ray's bounding box.

								if (!localGroup.AABB.Intersects(aabb))
								{
									continue;
								}

								// Determine intersections.
								if (localGroup.IntersectsWith(ref localRay, out localPosition, out localNormal, out localT))
								{
									// Update results.
									collisionGroup = localGroup;
									position = localPosition;
									normal = localNormal;
									t = localT;

									// Adjust ray's length and axis-aligned bounding box and try again.

									localRay.Vector *= t;
                                    aabb = localRay.GetBoundingBox();

								}
							}
						}
					}
				}
			}

			// Check the large-sized collision groups too.
			for (int i = 0; i < this.largeCollisionGroups.Count; i++)
			{
				// Get the current collision group.
				CollisionGroup localGroup = this.largeCollisionGroups[i];

				// Use callback to determine eligibility.
				if (callback != null && !callback.CanCompare(localGroup))
				{
					continue;
				}

				// Check for intersection between the current collision group's bounds
				// and the collision ray's bounding box.
#if REFERENCE
				bool intersects;
				localGroup.AABB.Intersects(ref aabb, out intersects);
				if (!intersects)
#else
				if (!localGroup.AABB.Intersects(aabb))
#endif
				{
					continue;
				}

				// Determine intersections.
				if (localGroup.IntersectsWith(ref localRay, out localPosition, out localNormal, out localT))
				{
					// Update results.
					collisionGroup = localGroup;
					position = localPosition;
					normal = localNormal;
					t = localT;

					// Adjust ray's length and axis-aligned bounding box and try again.

					localRay.Vector *= t;
                    aabb = localRay.GetBoundingBox();


				}
			}

			// Interection not found.
			if (t > 1.0f)
			{
				return false;
			}

			// Keep intersection within range.
			t = MathHelper.Clamp(t, 0.0f, 1.0f);

			// Intersection found.
			return true;
		}

		protected override void OnCollisionGroupAdded(CollisionGroup collisionGroup)
		{
			// Get a grid node range and associated it to the added collision group.
			GridNodeRange range = GridNodeRange.Retrieve();
			collisionGroup.internalTag = range;

            // Large-sized collision groups have issues.  Store them in a separate collection.
            Vector3 sl;
            MathsHelper.GetBoundingBoxSideLengths(ref collisionGroup.AABB, out sl);

            if (MathsHelper.MaxComponent(ref sl) >= GridCollisionSystem.LARGE_SIZE)
			{
				this.largeCollisionGroups.Add(collisionGroup);
				return;
			}

			// Get the range of grid nodes cell the collision group intersects.
			range.MinX = (int)Math.Floor((double)(collisionGroup.AABB.Min.X * this.inverseCellSize));
			range.MaxX = (int)Math.Floor((double)(collisionGroup.AABB.Max.X * this.inverseCellSize));
			range.MinY = (int)Math.Floor((double)(collisionGroup.AABB.Min.Y * this.inverseCellSize));
			range.MaxY = (int)Math.Floor((double)(collisionGroup.AABB.Max.Y * this.inverseCellSize));
			range.MinZ = (int)Math.Floor((double)(collisionGroup.AABB.Min.Z * this.inverseCellSize));
			range.MaxZ = (int)Math.Floor((double)(collisionGroup.AABB.Max.Z * this.inverseCellSize));

			// Iterate on all intersecting cells.
			for (int x = range.MinX; x <= range.MaxX; x++)
			{
				for (int y = range.MinY; y <= range.MaxY; y++)
				{
					for (int z = range.MinZ; z <= range.MaxZ; z++)
					{
						// Get the key for the current overlapping cell.
						uint key = GridCollisionSystem.CalculateKey(x, y, z);

						// Get the grid node for the current key.
						GridNode node;
						if (!this.gridNodes.TryGetValue(key, out node))
						{
							node = GridNode.Retrieve();
							this.gridNodes.Add(key, node);
						}

						// Add the collision group to the grid node.
						node.CollisionGroups.Add(collisionGroup);
					}
				}
			}

			// Call inherited method.
			base.OnCollisionGroupAdded(collisionGroup);
		}

		protected override void OnCollisionGroupRemoved(CollisionGroup collisionGroup)
		{
			// Get the associated grid node range.
			GridNodeRange range = (GridNodeRange)collisionGroup.internalTag;
			if (range != null)
			{
				// Unassociate grid node range.
				collisionGroup.internalTag = null;

				// Iterate on all intersecting cells.
				for (int x = range.MinX; x <= range.MaxX; x++)
				{
					for (int y = range.MinY; y <= range.MaxY; y++)
					{
						for (int z = range.MinZ; z <= range.MaxZ; z++)
						{
							// Get the key for the current overlapping cell.
							uint key = GridCollisionSystem.CalculateKey(x, y, z);

							// Remove the collision group from the grid node.
							GridNode node;
							if (this.gridNodes.TryGetValue(key, out node))
							{
								node.CollisionGroups.Remove(collisionGroup);
#if GRIDNODE_RECYCLE_UNUSED
								// Attempt to recycle the grid node.
								if (node.CollisionGroups.Count == 0)
								{
									this.gridNodes.Remove(node);
									GridNode.Recycle(node);
								}
#endif
							}
						}
					}
				}

				// Remove from large collision groups if necessary.
				this.largeCollisionGroups.Remove(collisionGroup);

				// Recycle the grid node range.
				GridNodeRange.Recycle(range);
			}

			// Call inherited method.
			base.OnCollisionGroupRemoved(collisionGroup);
		}

		internal protected override void OnCollisionGroupChanged(CollisionGroup collisionGroup)
		{
			// Get the associated grid node range.
			GridNodeRange range = (GridNodeRange)collisionGroup.internalTag;
			if (range != null)
			{
#if FINISHED
				// Get the range of grid node cells the collision group intersects.
				int minX = (int)Math.Floor(collisionGroup.AABB.Min.X * this.inverseCellSize);
				int maxX = (int)Math.Floor(collisionGroup.AABB.Max.X * this.inverseCellSize);
				int minY = (int)Math.Floor(collisionGroup.AABB.Min.Y * this.inverseCellSize);
				int maxY = (int)Math.Floor(collisionGroup.AABB.Max.Y * this.inverseCellSize);
				int minZ = (int)Math.Floor(collisionGroup.AABB.Min.Z * this.inverseCellSize);
				int maxZ = (int)Math.Floor(collisionGroup.AABB.Max.Z * this.inverseCellSize);

				// Determine the intersect range of cells between before and after the change.
				int intersectMinX = Math.Max(minX, range.MinX);
				int intersectMaxX = Math.Min(maxX, range.MaxX);
				int intersectMinY = Math.Max(minY, range.MinY);
				int intersectMaxY = Math.Min(maxY, range.MaxY);
				int intersectMinZ = Math.Max(minZ, range.MinZ);
				int intersectMaxZ = Math.Min(maxZ, range.MaxZ);
#else
				// Iterate on all intersecting cells.
				for (int x = range.MinX; x <= range.MaxX; x++)
				{
					for (int y = range.MinY; y <= range.MaxY; y++)
					{
						for (int z = range.MinZ; z <= range.MaxZ; z++)
						{
							// Get the key for the current overlapping cell.
							uint key = GridCollisionSystem.CalculateKey(x, y, z);

							// Remove the collision group from the grid node.
							GridNode node;
							if (this.gridNodes.TryGetValue(key, out node))
							{
								node.CollisionGroups.Remove(collisionGroup);
#if GRIDNODE_RECYCLE_UNUSED
								// Attempt to recycle the grid node.
								if (node.CollisionGroups.Count == 0)
								{
									this.gridNodes.Remove(node);
									GridNode.Recycle(node);
								}
#endif
							}
						}
					}
				}

				// Get the range of grid nodes cell the collision group intersects.
				range.MinX = (int)Math.Floor(collisionGroup.AABB.Min.X * this.inverseCellSize);
				range.MaxX = (int)Math.Floor(collisionGroup.AABB.Max.X * this.inverseCellSize);
				range.MinY = (int)Math.Floor(collisionGroup.AABB.Min.Y * this.inverseCellSize);
				range.MaxY = (int)Math.Floor(collisionGroup.AABB.Max.Y * this.inverseCellSize);
				range.MinZ = (int)Math.Floor(collisionGroup.AABB.Min.Z * this.inverseCellSize);
				range.MaxZ = (int)Math.Floor(collisionGroup.AABB.Max.Z * this.inverseCellSize);

				// Iterate on all intersecting cells.
				for (int x = range.MinX; x <= range.MaxX; x++)
				{
					for (int y = range.MinY; y <= range.MaxY; y++)
					{
						for (int z = range.MinZ; z <= range.MaxZ; z++)
						{
							// Get the key for the current overlapping cell.
							uint key = GridCollisionSystem.CalculateKey(x, y, z);

							// Get the grid node for the current key.
							GridNode node;
							if (!this.gridNodes.TryGetValue(key, out node))
							{
								node = GridNode.Retrieve();
								this.gridNodes.Add(key, node);
							}

							// Add the collision group to the grid node.
							node.CollisionGroups.Add(collisionGroup);
						}
					}
				}
#endif
			}

			// Call inherited method.
			base.OnCollisionGroupChanged(collisionGroup);
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		private static uint CalculateKey(int x, int y, int z)
		{
			const uint h1 = 0x8da6b343;
			const uint h2 = 0xd8163841;
			const uint h3 = 0xcb1ab31f;
			const uint size = 0xFFFF;

			uint n = ((uint)x * h1) + ((uint)y * h2) + ((uint)z * h3);
			n = n % size;
			if (n < 0)
			{
				n += size;
			}

			return n;
		}
	}
}
