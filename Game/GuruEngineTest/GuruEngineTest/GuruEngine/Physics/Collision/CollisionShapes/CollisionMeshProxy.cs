using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;

using GuruEngine.Helpers;



namespace GuruEngine.Physics.Collision.Shapes
{
	/// <summary>
	/// Represents an abstraction from the triangle information of a <see cref="CollisionMesh"/>.
	/// </summary>
	/// <remarks>
	/// One instance of this class may be used for multiple <see cref="CollisionMesh"/> instances positioned in
	/// different locations and orientations in 3D space.
	/// </remarks>
	public abstract class CollisionMeshProxy
	{
		/// <summary>
		/// Gets the axis-aligned bounding box for the <see cref="CollisionMeshProxy"/>.
		/// </summary>
		/// <remarks>
		/// This field should be treated as readonly.  It is set at creation time of a <see cref="CollisionMeshProxy"/>.
		/// </remarks>
		public BoundingBox AABB = MathsHelper.EmptyBB;

		/// <summary>
		/// Gets an instance of the <see cref="CollisionTriangleEx"/> representing the specified <see cref="CollisionMeshTriangle"/>.
		/// </summary>
		/// <param name="triangle">The <see cref="CollisionMeshTriangle"/> on which to get extended information.</param>
		/// <param name="result">A <see cref="CollisionTriangleEx"/> containing the vertices of the <see cref="CollisionMeshTriangle"/>.</param>
		public abstract void GetExtendedTriangle(CollisionMeshTriangle triangle, out CollisionTriangleEx result);

		/// <summary>
		/// Gets all <see cref="CollisionTriangle"/> instances from the <see cref="CollisionMeshProxy"/> whose bounds
		/// intersect with the specified <see cref="Microsoft.Xna.Framework.BoundingBox"/>.
		/// </summary>
		/// <param name="aabb">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
		/// <param name="result">The collection to store all colliding triangles from the <see cref="CollisionMeshProxy"/>.</param>
		/// <returns><b>true</b> if the specified <paramref name="aabb"/> collides with any triangle from the <see cref="CollisionMeshProxy"/>.</returns>
		public abstract bool GetIntersectingTriangles(ref BoundingBox aabb, IList<CollisionMeshTriangle> result);

		/// <summary>
		/// Gets all <see cref="CollisionTriangle"/> instances from the <see cref="CollisionMeshProxy"/> whose bounds
		/// intersect with the specified <see cref="Microsoft.Xna.Framework.BoundingBox"/>.
		/// </summary>
		/// <param name="aabb">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
		/// <param name="result">The collection to store all colliding triangles from the <see cref="CollisionMeshProxy"/>.</param>
		/// <returns><b>true</b> if the specified <paramref name="aabb"/> collides with any triangle from the <see cref="CollisionMeshProxy"/>.</returns>
		public abstract bool GetIntersectingTriangles(ref CollisionRay ray, IList<CollisionMeshTriangle> result);

		/// <summary>
		/// Release use of the <see cref="CollisionMeshTriangle"/> instances returned from a call to <see cref="CollisionMeshProxy.GetIntersectingTriangles"/>.
		/// </summary>
		/// <param name="triangles">The collection of <see cref="CollisionMeshTriangle"/> instances to release.</param>
		public abstract void ReleaseIntersectingTriangles(IList<CollisionMeshTriangle> triangles);
	}

	// TODO : Not used yet.
	/// <summary>
	/// Represents a collection of <see cref="CollisionMeshTriangle"/> instances.
	/// </summary>
	public sealed class CollisionMeshTriangleCollection : Collection<CollisionMeshTriangle>
	{
		private static ObjectPool<CollisionMeshTriangleCollection> pool = new ObjectPool<CollisionMeshTriangleCollection>(8);

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionMeshTriangleCollection"/> class.
		/// </summary>
		public CollisionMeshTriangleCollection() { }

		/// <summary>
		/// Gets a <see cref="CollisionMeshTriangleCollection" /> instance from the pool.
		/// </summary>
		/// <returns>
		/// Returns a fresh <see cref="CollisionMeshTriangleCollection"/> instance.
		/// </returns>
		internal static CollisionMeshTriangleCollection Retrieve()
		{
			// Return an unused collision mesh triangle collection from the pool.
			return CollisionMeshTriangleCollection.pool.Retrieve();
		}

		/// <summary>
		/// Returns a <see cref="CollisionMeshTriangleCollection"/> instance to the pool.
		/// </summary>
		/// <param name="triangles">The <see cref="CollisionMeshTriangleCollection"/> to recycle.</param>
		internal static void Recycle(CollisionMeshTriangleCollection triangles)
		{
			// Uninitialize.
			triangles.Clear();

			// Put the collision mesh triangle collection back in the pool for later use.
			CollisionMeshTriangleCollection.pool.Recycle(triangles);
		}
	}

	/// <summary>
	/// Represents one triangle in a <see cref="CollisionMeshProxy"/> instance.
	/// </summary>
	public sealed class CollisionMeshTriangle
	{
		internal CollisionMeshProxy proxy;

		/// <summary>
		/// Gets or sets the index to the first vertex position of the <see cref="CollisionMeshTriangle"/>.
		/// </summary>
		public int Index0;

		/// <summary>
		/// Gets or sets the index to the second vertex position of the <see cref="CollisionMeshTriangle"/>.
		/// </summary>
		public int Index1;

		/// <summary>
		/// Gets or sets the index to the third vertex position of the <see cref="CollisionMeshTriangle"/>.
		/// </summary>
		public int Index2;

		/// <summary>
		/// Gets the normal of the <see cref="CollisionMeshTriangle"/>.
		/// </summary>
		/// <remarks>
		/// This value should be considered read-only.
		/// </remarks>
		public Vector3 Normal;

		internal float distance;

		/// <summary>
		/// Initializes a new instances of the <see cref="CollisionMeshTriangle"/> class.
		/// </summary>
		/// <param name="proxy">The associated <see cref="CollisionMeshProxy"/>.</param>
		internal CollisionMeshTriangle(CollisionMeshProxy proxy)
		{
			// Hold onto parameters.
			this.proxy = proxy;
		}

		/// <summary>
		/// Initializes a new instances of the <see cref="CollisionMeshTriangle"/> class.
		/// </summary>
		/// <param name="proxy">The associated <see cref="CollisionMeshProxy"/>.</param>
		/// <param name="index0">The index to the first vertex position.</param>
		/// <param name="index1">The index to the second vertex position.</param>
		/// <param name="index2">The index to the third vertex position.</param>
		internal CollisionMeshTriangle(CollisionMeshProxy proxy, int index0, int index1, int index2)
		{
			// Hold onto parameters.
			this.proxy = proxy;

			// Hold onto parameters.
			this.Index0 = index0;
			this.Index1 = index1;
			this.Index2 = index2;

			// Calculate the normal and distance from the origin.
			this.CalculateInternals();
		}

		/// <summary>
		/// Gets the closest point on the <see cref="CollisionMeshTriangle"/> to the specified <paramref name="point"/>.
		/// </summary>
		/// <param name="point">The source point.</param>
		/// <returns>A <see cref="Microsoft.Xna.Framework.Vector3"/> representing the closest point on the <see cref="CollisionMeshTriangle"/> to <paramref name="point"/>.</returns>
		public Vector3 GetClosestPoint(Vector3 point)
		{
			// Call overloaded method.
			Vector3 result;
			this.GetClosestPoint(ref point, out result);
			return result;
		}

		/// <summary>
		/// Gets the closest point on the <see cref="CollisionMeshTriangle"/> to the specified <paramref name="point"/>.
		/// </summary>
		/// <param name="point">The source point.</param>
		/// <param name="result">The closest point on the <see cref="CollisionMeshTriangle"/> to <paramref name="point"/>.</param>
		public void GetClosestPoint(ref Vector3 point, out Vector3 result)
		{

			// Get the positions of the vertices.
			CollisionTriangleEx triangle;
			this.proxy.GetExtendedTriangle(this, out triangle);

			// Get the 1st and 2nd edges.
			Vector3 edge0 = triangle.Vertex1 - triangle.Vertex0;
			Vector3 edge1 = triangle.Vertex2 - triangle.Vertex0;

			// Determine if the point is in the vertex region outside of the first vertex.
			Vector3 difference0 = point - triangle.Vertex0;
			float dot0 = Vector3.Dot(edge0, difference0);
			float dot1 = Vector3.Dot(edge1, difference0);
			if (dot0 <= 0.0f && dot1 <= 0.0f)
			{
				// Barycentric coordinates (1,0,0).
				result = triangle.Vertex0; 
				return;
			}

			// Determine if the point is in the vertex region outside of the second vertex.
			Vector3 difference1 = point - triangle.Vertex1;
			float dot2 = Vector3.Dot(edge0, difference1);
			float dot3 = Vector3.Dot(edge1, difference1);
			if (dot2 >= 0.0f && dot3 <= dot2)
			{
				// Barycentric coordinates (0,1,0).
				result = triangle.Vertex1; 
				return;
			}

			// Determine if the point is in the edge region of the first and second vertices.
			// If so, return the projection of the point onto said edge.
			float vc = dot0 * dot3 - dot2 * dot1;
			if (vc <= 0.0f && dot0 >= 0.0f && dot2 <= 0.0f)
			{
				float v0 = dot0 / (dot0 - dot2);

				// Barycentric coordinates (1 - v, v, 0).
				result = triangle.Vertex0 + (edge0 * v0);
				return;
			}

			// Determine if the point is in the vertex region outside of the third vertex.
			Vector3 difference2 = point - triangle.Vertex2;
			float dot4 = Vector3.Dot(edge0, difference2);
			float dot5 = Vector3.Dot(edge1, difference2);
			if (dot5 >= 0.0f && dot4 <= dot5)
			{
				// Barycentric coordinates (0, 0, 1).
				result = triangle.Vertex2; 
				return;
			}

			// Determine if the point is in the edge region of the first and third vertices.
			// If so, return the projection of the point onto said edge.
			float vb = dot4 * dot1 - dot0 * dot5;
			if (vb <= 0.0f && dot1 >= 0.0f && dot5 <= 0.0f)
			{
				float w0 = dot1 / (dot1 - dot5);

				// Barycentric coordinates (1 - w, 0, w).
				result = triangle.Vertex0 + (edge1 * w0); 
				return;
			}

			// Determine if the point is in the edge region of the second and third vertices.
			// If so, return the projection of the point onto said edge.
			float va = dot2 * dot5 - dot4 * dot3;
			if (va <= 0.0f && (dot3 - dot2) >= 0.0f && (dot4 - dot5) >= 0.0f)
			{
				float w0 = (dot3 - dot2) / ((dot3 - dot2) + (dot4 - dot5));

				// Barycentric coordinates (0, 1 - w, w).
				result = triangle.Vertex1 + (triangle.Vertex2 - triangle.Vertex1) * w0; 
				return;
			}

			// The point is within the face region. Calculate it's barycentric coordinates (u, v, w).
			float denominator = 1.0f / (va + vb + vc);
			float v = vb * denominator;
			float w = vc * denominator;

			// = u*a + v*b + w*c, u = va * denomator = 1.0f - v - w
			result = triangle.Vertex0 + edge0 * v + edge1 * w; 

		}

		// TODO : Reverse edges.
		/// <summary>
		/// Performs a ray cast intersection test against the <see cref="CollisionMeshTriangle"/>.
		/// </summary>
		/// <param name="ray">The <see cref="CollisionRay" /> to test the intersection with.</param>
		/// <param name="position">Set to the location of the intersection if an interection occurs with the <see cref="CollisionMeshTriangle" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero" />.</param>
		/// <param name="t">Set to the fraction of the <see cref="CollisionRay.Vector" />  if an interection occurs with the <see cref="CollisionMeshTriangle" />. Otherwise, it is set to zero.</param>
		/// <returns>
		/// Returns <c>true</c> if the specified <see cref="CollisionRay" /> intersects with the <see cref="CollisionMeshTriangle" />.  Otherwise, returns <c>false</c>.
		/// </returns>
		public bool IntersectsWith(ref CollisionRay ray, out Vector3 position, out float t)
		{
			// Initialize results.
			position = Vector3.Zero;
			t = 0.0f;
			
			// Get the positions of the vertices.
			CollisionTriangleEx triangle;
			this.proxy.GetExtendedTriangle(this, out triangle);

			// Get the 1st and 2nd edges.
			Vector3 edge0 = triangle.Vertex1 - triangle.Vertex0;
			Vector3 edge1 = triangle.Vertex2 - triangle.Vertex0;

			// Get the vector of the collision ray.			
			Vector3 vector = ray.Position - ray.GetEndPoint();

			// Calculate the non-unitized triangle normal. 
			Vector3 normal = Vector3.Cross(edge0, edge1);

			// Calculate the denominator and check if the collision ray is pointing towards
			// the triangle.
			float denominator = Vector3.Dot(vector, normal);
			if (denominator <= 0.0f)
			{
				return false;
			}

			// Calculate intersection with triangle's plane.
			Vector3 difference0 = ray.Position - triangle.Vertex0;
			t = Vector3.Dot(difference0, normal);
			if (t < 0.0f || t > denominator)
			{
				return false;
			}

			// Calculate the barycentric coordinates and determine if they are within the bounds
			// of the triangle.
			Vector3 e = Vector3.Cross(vector, difference0);

			float v = Vector3.Dot(edge1, e);
			if (v < 0.0f || v > denominator)
			{
				return false;
			}

			float w = -Vector3.Dot(edge0, e);
			if (w < 0.0f || v + w > denominator)
			{
				return false;
			}

			// Calculate the last barycentric coordinate component.
			float inverseDenominator = 1.0f / denominator;
			t *= inverseDenominator;
			v *= inverseDenominator;
			w *= inverseDenominator;
			//float u = 1.0f - v - w;

			// Determine the position of the intersection using the barycenteric coordinates.

			position = triangle.Vertex0 + edge0 * v + edge1 * w;

			// An intersection occurred.
			return true;
		}

		/// <summary>
		/// Calculates any additional information about the <see cref="CollisionMeshTriangle"/>.
		/// </summary>
		internal void CalculateInternals()
		{
			// Get the positions of the vertices.
			CollisionTriangleEx triangle;
			this.proxy.GetExtendedTriangle(this, out triangle);

			// Calculate the normal of this triangle.
			Vector3 edge0 = triangle.Vertex0 - triangle.Vertex1;
			Vector3 edge1 = triangle.Vertex0 - triangle.Vertex2;
			this.Normal = Vector3.Normalize(Vector3.Cross(edge0, edge1));

			// Calculate the distance from the origin.
			this.distance = -Vector3.Dot(this.Normal, triangle.Vertex0);

		}
	}

	/// <summary>
	/// Holds the vertex position for a <see cref="CollisionMeshTriangle"/>.
	/// </summary>
	public struct CollisionTriangleEx
	{
		/// <summary>
		/// Gets or sets the first vertex position of the <see cref="CollisionTriangleEx"/>.
		/// </summary>
		public Vector3 Vertex0;

		/// <summary>
		/// Gets or sets the second vertex position of the <see cref="CollisionTriangleEx"/>.
		/// </summary>
		public Vector3 Vertex1;

		/// <summary>
		/// Gets or sets the third vertex position of the <see cref="CollisionTriangleEx"/>.
		/// </summary>
		public Vector3 Vertex2;
	}
}
