using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision.Shapes
{
	// TODO : Not finished.
	/// <summary>
	/// Defines a collision shape in the form of a triangle.
	/// </summary>
	public class CollisionTriangle : CollisionShape
	{
		/// <summary>
		/// Gets the first vertex of the <see cref="CollisionTriangle"/>.
		/// </summary>
		public Vector3 Position0;

		/// <summary>
		/// Gets the second vertex of the <see cref="CollisionTriangle"/>.
		/// </summary>
		public Vector3 Position1;

		/// <summary>
		/// Gets the third vertex of the <see cref="CollisionTriangle"/>.
		/// </summary>
		public Vector3 Position2;

		/// <summary>
		/// Gets the normal of the <see cref="CollisionTriangle"/>.
		/// </summary>
		/// <remarks>
		/// Do not set this field directly.  Should be considered read-only.
		/// </remarks>
		public Vector3 Normal;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionTriangle"/> class.
		/// </summary>
		public CollisionTriangle() : base(CollisionShapeType.Triangle) { }

		#region CollisionShape Members 

		internal override void CalculateInternals()
		{

			// Calculate the normal of this triangle.
			Vector3 edge0 = this.Position0 - this.Position1;
			Vector3 edge1 = this.Position0 - this.Position2;
			this.Normal = Vector3.Normalize(Vector3.Cross(edge0, edge1));


			// Call inherited method.
			base.CalculateInternals();
		}

		protected override void CalculateAABB()
		{
			// Calculate the axis-aligned bounding box for the specified triangle.

			this.AABB = new BoundingBox(
				Vector3.Min(this.Position0, Vector3.Min(this.Position1, this.Position2)),
				Vector3.Max(this.Position0, Vector3.Max(this.Position1, this.Position2)));

		}

		public override bool IntersectsWith(ref CollisionRay ray, out Vector3 position, out Vector3 normal, out float t)
		{
			// Initialize results.
			position = Vector3.Zero;
			normal = Vector3.Zero;
			t = 0.0f;

			// Get the 1st and 2nd edges.
			Vector3 edge0 = this.Position1 - this.Position0;
			Vector3 edge1 = this.Position2 - this.Position0;

			// Get the vector of the collision ray.			
			Vector3 vector = ray.Position - ray.GetEndPoint();

			// Calculate the non-unitized triangle normal. 
			Vector3 normal0 = Vector3.Cross(edge0, edge1);

			// Calculate the denominator and check if the collision ray is pointing towards
			// the triangle.
			float denominator = Vector3.Dot(vector, normal0);
			if (denominator <= 0.0f)
			{
				return false;
			}

			// Calculate intersection with triangle's plane.
			Vector3 difference0 = ray.Position - this.Position0;
			t = Vector3.Dot(difference0, normal0);
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

			position = this.Position0 + edge0 * v + edge1 * w;

			normal = this.Normal;

			// An intersection occurred.
			return true;
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
	}
}