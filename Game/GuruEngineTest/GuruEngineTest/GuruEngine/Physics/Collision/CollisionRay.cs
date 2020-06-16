using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Represents a ray with a defined length used in collision tests.
	/// </summary>
	public struct CollisionRay
	{
		/// <summary>
		/// Represents an empty <see cref="CollisionRay"/> instance.
		/// </summary>
		public static readonly CollisionRay Empty = new CollisionRay(Vector3.Zero, Vector3.Zero);

		/// <summary>
		/// Gets or sets the start position of the <see cref="CollisionRay"/>.
		/// </summary>
		public Vector3 Position;

		/// <summary>
		/// Gets or sets the direction and magnitude of the <see cref="CollisionRay"/>.
		/// </summary>
		public Vector3 Vector;

		/// <summary>
		/// Initialize a new instance of the <see cref="CollisionRay"/> class.
		/// </summary>
		/// <param name="position">The starting location of the <see cref="CollisionRay"/>.</param>
		/// <param name="vector">The direction and maginitude of the <see cref="CollisionRay"/>.</param>
		public CollisionRay(ref Vector3 position, ref Vector3 vector)
		{
			// Hold onto parameters.
			this.Position = position;
			this.Vector = vector;
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="CollisionRay"/> class.
		/// </summary>
		/// <param name="position">The starting location of the <see cref="CollisionRay"/>.</param>
		/// <param name="vector">The direction and maginitude of the <see cref="CollisionRay"/>.</param>
		public CollisionRay(Vector3 position, Vector3 vector)
		{
			// Hold onto parameters.
			this.Position = position;
			this.Vector = vector;
		}

		/// <summary>
		/// Gets the point along the ray at the specified <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The length from the starting position along the <see cref="CollisionRay"/>.</param>
		/// <returns>Returns the point along the <see cref="CollisionRay"/> at <paramref name="t"/>.</returns>
		public Vector3 GetPointAt(float t)
		{

			Vector3 result;
			result = this.Position + this.Vector * t;
			return result;
		}

		/// <summary>
		/// Gets the point along the <see cref="CollisionRay"/> at the specified <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The length from the starting position along the <see cref="CollisionRay"/>.</param>
		/// <param name="result">The point along the <see cref="CollisionRay"/> at <paramref name="t"/>.</param>
		public void GetPointAt(float t, out Vector3 result)
		{
			result = this.Position + this.Vector * t;
		}

		/// <summary>
		/// Returns the end of the <see cref="CollisionRay"/>.
		/// </summary>
		/// <returns>Returns a <see cref="Microsoft.Xna.Framework.Vector3"/> representing the end of the collision ray.</returns>
		public Vector3 GetEndPoint()
		{

			Vector3 result;
			result = this.Position + this.Vector;
			return result;
		}

		/// <summary>
		/// Returns the end of the <see cref="CollisionRay"/>.
		/// </summary>
		/// <returns>Returns a <see cref="Microsoft.Xna.Framework.Vector3"/> representing the end of the collision ray.</returns>
		public void GetEndPoint(out Vector3 result)
		{

			result = this.Position + this.Vector;
		}

        public BoundingBox GetBoundingBox()
        {
            Vector3 minimum = Vector3.Min(Position, GetEndPoint());
            Vector3 maximum = Vector3.Max(Position, GetEndPoint());

            return new BoundingBox(minimum, maximum);
        }

		/// <summary>
		/// Checks whether the specified <see cref="Microsoft.Xna.Framework.BoundingBox"/> intersects the <see cref="CollisionRay"/>.
		/// </summary>
		/// <param name="aabb">The <see cref="Microsoft.Xna.Framework.BoundingBox"/> to check for intersection with.</param>
		/// <returns><b>true</b>, if the <see cref="CollisionRay"/> intersects with the <see cref="Microsoft.Xna.Framework.BoundingBox"/>.  Otherwise, <b>false</b>.</returns>
		public bool Intersects(ref BoundingBox aabb)
		{

            // Get the center and extents of the bounding box.
            Vector3 center = MathsHelper.GetBoundingBoxCentre(aabb);
            Vector3 extents = MathsHelper.GetBoundingBoxExtents(aabb);

			// Get the midpoint and half the vector length of the collision ray.
			Vector3 midPoint = this.GetPointAt(0.5f);
			Vector3 halfVector = midPoint - this.Position;

			// Translate the midpoint to the box's transform.
			midPoint = midPoint - center;

			// Apply the separating axis test on the axis-aligned bounding box and the collision ray.
			float adx = Math.Abs(halfVector.X);
			if (Math.Abs(midPoint.X) > extents.X + adx)
			{
				return false;
			}
			float ady = Math.Abs(halfVector.Y);
			if (Math.Abs(midPoint.Y) > extents.Y + ady)
			{
				return false;
			}
			float adz = Math.Abs(halfVector.Z);
			if (Math.Abs(midPoint.Z) > extents.Z + adz)
			{
				return false;
			}

			// Add in an epsilon term to counteract arithmetic errors when the collision ray is near parallel 
			// to a coordinate axis.
			adx += MathsHelper.Epsilon;
			ady += MathsHelper.Epsilon;
			adz += MathsHelper.Epsilon;

			// Test cross products of collision ray direction vector with coordinate axes.
			if (Math.Abs(midPoint.Y * halfVector.Z - midPoint.Z * halfVector.Y) > extents.Y * adz + extents.Z * ady)
			{
				return false;
			}
			if (Math.Abs(midPoint.Z * halfVector.X - midPoint.X * halfVector.Z) > extents.X * adz + extents.Z * adx)
			{
				return false;
			}
			if (Math.Abs(midPoint.X * halfVector.Y - midPoint.Y * halfVector.X) > extents.X * ady + extents.Y * adx)
			{
				return false;
			}

			// No separating axis found.  The collision ray intersects the axis-aligned bounding box.
			return true;
		}
	}
}
