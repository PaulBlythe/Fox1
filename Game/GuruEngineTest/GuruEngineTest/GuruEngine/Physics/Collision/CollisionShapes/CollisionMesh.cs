using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using GuruEngine.Helpers;



namespace GuruEngine.Physics.Collision.Shapes
{
	/// <summary>
	/// Defines a collision shape in the form of a mesh.
	/// </summary>
	public class CollisionMesh : CollisionShape 
	{
		private CollisionMeshProxy proxy;

		private List<CollisionMeshTriangle> triangles = new List<CollisionMeshTriangle>(32);

		internal Matrix transform = Matrix.Identity;
		internal Matrix inverseTransform = Matrix.Identity;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionMesh"/> class.
		/// </summary>
		/// <param name="proxy">The <see cref="CollisionMeshProxy"/> that will be used to retrieve information about the mesh.</param>
		public CollisionMesh(CollisionMeshProxy proxy) : base(CollisionShapeType.Mesh)
		{
			// The proxy parameter must be specified.
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}

			// Hold onto parameters.
			this.proxy = proxy;
		}

		/// <summary>
		/// Returns the associated <see cref="CollisionMeshProxy"/> of the <see cref="CollisionMesh"/>.
		/// </summary>
		public CollisionMeshProxy Proxy
		{
			get { return this.proxy; }
		}

		#region CollisionShape Members

		internal override void CalculateInternals()
		{
			// Get the inverse of this shape's transform.

			this.transform = this.Orientation * Matrix.CreateTranslation(this.Position);
			this.inverseTransform = Matrix.Invert(this.transform);

			// Call inherited method.
			base.CalculateInternals();
		}

		protected override void CalculateAABB()
		{
			// Transform the proxy's axis-aligned bounding box to world-space.

			Vector3 center = MathsHelper.GetBoundingBoxCentre(this.proxy.AABB);
			Vector3 extents = MathsHelper.GetBoundingBoxExtents(this.proxy.AABB);

			float x = Math.Abs(this.Orientation.M11 * extents.X) + Math.Abs(this.Orientation.M21 * extents.Y) + Math.Abs(this.Orientation.M31 * extents.Z);
			float y = Math.Abs(this.Orientation.M12 * extents.X) + Math.Abs(this.Orientation.M22 * extents.Y) + Math.Abs(this.Orientation.M32 * extents.Z);
			float z = Math.Abs(this.Orientation.M13 * extents.X) + Math.Abs(this.Orientation.M23 * extents.Y) + Math.Abs(this.Orientation.M33 * extents.Z);

			// Update the axis-aligned bounding box for this shape.
			this.AABB.Min.X = this.Position.X + center.X - x;
			this.AABB.Max.X = this.Position.X + center.X + x;
			this.AABB.Min.Y = this.Position.Y + center.Y - y;
			this.AABB.Max.Y = this.Position.Y + center.Y + y;
			this.AABB.Min.Z = this.Position.Z + center.Z - z;
			this.AABB.Max.Z = this.Position.Z + center.Z + z;
		}

		public override bool IntersectsWith(ref CollisionRay ray, out Vector3 position, out Vector3 normal, out float t)
		{
			// Initialize return values.
			position = Vector3.Zero;
			normal = Vector3.Zero;
			t = float.MaxValue;

			// Copy and transform ray into mesh-space.
			CollisionRay localRay;

			localRay.Position = Vector3.Transform(ray.Position, this.inverseTransform);
			localRay.Vector = Vector3.TransformNormal(ray.Vector, this.inverseTransform);

			// Attempt to get all triangles that intersect with the collision ray.
			this.triangles.Clear();
			if (this.proxy.GetIntersectingTriangles(ref localRay, this.triangles))
			{
				// Working storage.
				Vector3 localPosition;
				float localT;

				// Iterate on the potentially intersecting triangles.
				for (int i = 0; i < this.triangles.Count; i++)
				{
					// Get the current triangle.
					CollisionMeshTriangle triangle = this.triangles[i];

					// Determine intersection with the current triangle.
					if (triangle.IntersectsWith(ref localRay, out localPosition, out localT))
					{
						// Hold onto results.
						position = localPosition;
						normal = triangle.Normal;
						t = localT;

						// Adjust the ray's length and try again.

						localRay.Vector *= t;

					}
				}

				// Recycle triangles.
				this.ReleaseIntersectingTriangles(this.triangles);
			}

			// An intersection was found. Transform the results into world-space.
			if (t <= 1.0f)
			{

				position = Vector3.Transform(position, this.transform);
				normal = Vector3.TransformNormal(normal, this.transform);

				return true;
			}

			// An intersection was not found.
			return false;
		}

		protected internal override float GetVolume()
		{
			return 0.0f;
		}

		public override void CalculateMassProperties(float density, out float mass, out Matrix inertiaTensor)
		{
			mass = 0.0f;
			inertiaTensor = MathsHelper.Zero;
		}

		#endregion

		/// <summary>
		/// Gets all <see cref="CollisionTriangle"/> instances from the <see cref="CollisionMesh"/> whose bounds
		/// intersect with the specified <see cref="Microsoft.Xna.Framework.BoundingBox"/>.
		/// </summary>
		/// <param name="aabb">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
		/// <param name="result">The collection to store all colliding triangles from the <see cref="CollisionMesh"/>.</param>
		/// <returns><b>true</b> if the specified <paramref name="aabb"/> collides with any triangle from the <see cref="CollisionMesh"/>.</returns>
		internal bool GetIntersectingTriangles(ref BoundingBox aabb, List<CollisionMeshTriangle> result)
		{
			// Transform the axis-aligned bounding box to object space.

			BoundingBox localAABB;

			Vector3 center = MathsHelper.GetBoundingBoxCentre(aabb);
            Vector3 extents = MathsHelper.GetBoundingBoxExtents(aabb);

			float x = center.X - this.Position.X;
			float y = center.Y - this.Position.Y;
			float z = center.Z - this.Position.Z;

			center.X = x * this.Orientation.M11 + y * this.Orientation.M12 + z * this.Orientation.M13;
			center.Y = x * this.Orientation.M21 + y * this.Orientation.M22 + z * this.Orientation.M23;
			center.Z = x * this.Orientation.M31 + y * this.Orientation.M32 + z * this.Orientation.M33;

			x = Math.Abs(this.Orientation.M11 * extents.X) + Math.Abs(this.Orientation.M12 * extents.Y) + Math.Abs(this.Orientation.M13 * extents.Z);
			y = Math.Abs(this.Orientation.M21 * extents.X) + Math.Abs(this.Orientation.M22 * extents.Y) + Math.Abs(this.Orientation.M23 * extents.Z);
			z = Math.Abs(this.Orientation.M31 * extents.X) + Math.Abs(this.Orientation.M32 * extents.Y) + Math.Abs(this.Orientation.M33 * extents.Z);

			// Update the axis-aligned bounding box for this shape.
			localAABB.Min.X = center.X - x;
			localAABB.Max.X = center.X + x;
			localAABB.Min.Y = center.Y - y;
			localAABB.Max.Y = center.Y + y;
			localAABB.Min.Z = center.Z - z;
			localAABB.Max.Z = center.Z + z;

			// Use the proxy to determine the intersecting triangles.
			return this.proxy.GetIntersectingTriangles(ref localAABB, result);
		}

		/// <summary>
		/// Release use of the <see cref="CollisionMeshTriangle"/> instances returned from a call to <see cref="CollisionMeshProxy.GetIntersectingTriangles"/>.
		/// </summary>
		/// <param name="triangles">The collection of <see cref="CollisionMeshTriangle"/> instances to release.</param>
		internal void ReleaseIntersectingTriangles(IList<CollisionMeshTriangle> triangles)
		{
			// Delegate to proxy.
			this.proxy.ReleaseIntersectingTriangles(triangles);
		}
	}
}
