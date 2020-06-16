using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;


namespace GuruEngine.Physics.Collision.Shapes
{
	/// <summary>
	/// Defines a contract to retrieve information for a heightmap.
	/// </summary>
	public interface IHeightmapCollisionMeshCallback
	{
		/// <summary>
		/// Gets the size of the heightmap.
		/// </summary>
		Point Size { get; }

		/// <summary>
		/// Gets the sampling height at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>A <see cref="Single"/> representing the sample at the location specified.</returns>
		float GetSample(int x, int y);
	}

	/// <summary>
	/// Represents an abstraction from the triangle information of a heightmap.
	/// </summary>
	public sealed class HeightmapCollsionMeshProxy : CollisionMeshProxy
	{
		private ObjectPool<CollisionMeshTriangle> pool;

		private IHeightmapCollisionMeshCallback callback;
		private Vector3 scale;


		/// <summary>
		/// Initializes a new instance of the <see cref="HeightmapCollsionMeshProxy"/> class.
		/// </summary>
		/// <param name="callback">A callback to retrieve information about the heightmap.</param>
		/// <param name="scale">The scale of the heightmap.</param>
		public HeightmapCollsionMeshProxy(IHeightmapCollisionMeshCallback callback, Vector3 scale)
		{
			// The callback parameter must be specified.
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			if (scale.X <= 0.0f || scale.Y <= 0.0f || scale.Z <= 0.0f)
			{
				throw new ArgumentOutOfRangeException("scale");
			}

			// Hold onto parameters.
			this.callback = callback;
			this.scale = scale;

			// Create the triangle object pool.
			this.pool = new ObjectPool<CollisionMeshTriangle>(8, () => new CollisionMeshTriangle(this));

			// Initialize the collision mesh proxy.
			this.Initialize();
		}

		#region CollsionMeshProxy Members

		public override void GetExtendedTriangle(CollisionMeshTriangle triangle, out CollisionTriangleEx result)
		{
			// Get the size of the heightmap.
			Point size = this.callback.Size;

			// Get half the size of the heightmap.
			Point halfSize = new Point()
			{
				X = size.X >> 1,
				Y = size.Y >> 1
			};

			// Get the coordinates of the first vertex and determine the vertex's position.
			int x = triangle.Index0 >> 16;
			int y = triangle.Index0 & 0xFF;

			result.Vertex0.X = (x - halfSize.X) * this.scale.X;
			result.Vertex0.Y = this.callback.GetSample(x, y) * this.scale.Y;
			result.Vertex0.Z = (y - halfSize.Y) * this.scale.Z;

			// Get the coordinates of the second vertex and determine the vertex's position.
			x = triangle.Index1 >> 16;
			y = triangle.Index1 & 0xFF;

			result.Vertex1.X = (x - halfSize.X) * this.scale.X;
			result.Vertex1.Y = this.callback.GetSample(x, y) * this.scale.Y;
			result.Vertex1.Z = (y - halfSize.Y) * this.scale.Z;

			// Get the coordinates of the third vertex and determine the vertex's position.
			x = triangle.Index2 >> 16;
			y = triangle.Index2 & 0xFF;

			result.Vertex2.X = (x - halfSize.X) * this.scale.X;
			result.Vertex2.Y = this.callback.GetSample(x, y) * this.scale.Y;
			result.Vertex2.Z = (y - halfSize.Y) * this.scale.Z;
		}

		public override bool GetIntersectingTriangles(ref BoundingBox aabb, IList<CollisionMeshTriangle> result)
		{
			// Determine the bounds of the height map section that the shape is inline with.
			int minX = (int)(Math.Floor(aabb.Min.X / this.scale.X));
			int maxX = (int)(Math.Floor(aabb.Max.X / this.scale.X)) + 1;
			int minY = (int)(Math.Floor(aabb.Min.Z / this.scale.Z));
			int maxY = (int)(Math.Floor(aabb.Max.Z / this.scale.Z)) + 1;

			// Get the size of the heightmap.
			Point size = this.callback.Size;

			// Get half the size of the heightmap.
			Point halfSize = new Point()
			{
				X = size.X >> 1,
				Y = size.Y >> 1
			};

			// Clamp bounds to height map sampling bounds.
			minX = Math.Max(minX + halfSize.X, 0);
			maxX = Math.Min(maxX + halfSize.X, size.X - 1);
			minY = Math.Max(minY + halfSize.Y, 0);
			maxY = Math.Min(maxY + halfSize.Y, size.Y - 1);

			for (int x = minX; x < maxX; x++)
			{
				for (int y = minY; y < maxY; y++)
				{
					// Get two triangles from the pool.
					CollisionMeshTriangle triangle0 = this.pool.Retrieve();
					CollisionMeshTriangle triangle1 = this.pool.Retrieve();

					// Encode the indices of the triangles and recalculate 
					// extra information.
					triangle0.Index0 = (x << 16) + y;
					triangle0.Index1 = (x << 16) + (y + 1);
					triangle0.Index2 = ((x + 1) << 16) + y;
					triangle0.CalculateInternals();

					triangle1.Index0 = ((x + 1) << 16) + y;
					triangle1.Index1 = (x << 16) + (y + 1);
					triangle1.Index2 = ((x + 1) << 16) + (y + 1);
					triangle1.CalculateInternals();

					// Add the triangles to the result.
					result.Add(triangle0);
					result.Add(triangle1);
				}
			}

			return result.Count > 0;
		}

		// TODO : This is naive.  Then again, so is most of my code.  Re-evaluate.
		public override bool GetIntersectingTriangles(ref CollisionRay ray, IList<CollisionMeshTriangle> result)
		{
            // Get the axis-aligned bounding box that encompasses the collision ray.

            BoundingBox aabb = ray.GetBoundingBox();

			// Call overloaded method.
			return this.GetIntersectingTriangles(ref aabb, result);
		}

		public override void ReleaseIntersectingTriangles(IList<CollisionMeshTriangle> triangles)
		{
			// Recycle the used triangles.
			for (int i = 0; i < triangles.Count; i++)
			{
				this.pool.Recycle(triangles[i]);
			}
		}

		#endregion

		private void Initialize()
		{
			// Get the size of the heightmap.
			Point size = this.callback.Size;

			// Calculate the axis-align bounding box.
			this.AABB = MathsHelper.EmptyBB;
			for (int x = 0; x < size.X; x++)
			{
				for (int y = 0; y < size.Y; y++)
				{
					// Get the sample.
					float sample = this.callback.GetSample(x, y);

					// Set the y-extents.
					this.AABB.Min.Y = Math.Min(this.AABB.Min.Y, sample);
					this.AABB.Max.Y = Math.Max(this.AABB.Max.Y, sample);
				}
			}

			// Get half the size of the heightmap.
			Point halfSize = new Point()
			{
				X = size.X >> 1,
				Y = size.Y >> 1
			};

			// Set minimum/maximum x/z extents.
			this.AABB.Min.X = -halfSize.X;
			this.AABB.Min.Z = -halfSize.Y;

			// Set maximum x/z extents.
			this.AABB.Max.X = halfSize.X;
			this.AABB.Max.Z = halfSize.Y;

			// Apply scaling.
			this.AABB.Min *= this.scale;
			this.AABB.Max *= this.scale;
		}
	}

}
