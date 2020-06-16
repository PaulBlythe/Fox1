using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision.Shapes
{
	/// <summary>
	/// Defines a collision shape in the form of a sphere.
	/// </summary>
	public partial class CollisionSphere : CollisionShape
	{
		/// <summary>
		/// Gets or set the radius of the collision sphere.
		/// </summary>
		public float Radius;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionSphere"/> class.
		/// </summary>
		public CollisionSphere() : base(CollisionShapeType.Sphere) { }

		#region CollisionShape Members

		protected override void CalculateAABB()
		{
			// Calcluate the axis-aligned bounding box of this sphere.
			this.AABB.Min.X = this.Position.X - this.Radius;
			this.AABB.Max.X = this.Position.X + this.Radius;
			this.AABB.Min.Y = this.Position.Y - this.Radius;
			this.AABB.Max.Y = this.Position.Y + this.Radius;
			this.AABB.Min.Z = this.Position.Z - this.Radius;
			this.AABB.Max.Z = this.Position.Z + this.Radius;
		}

		public override bool IntersectsWith(ref CollisionRay ray, out Vector3 position, out Vector3 normal, out float t)
		{
			// Get the vector difference between the start of the collision ray and the center
			// of the sphere.


			Vector3 difference = ray.Position - this.Position;

			// Get radius totals.
			float radiusSquared = this.Radius * this.Radius;

			// Hold onto the length of the collision ray.
			float lengthSquared = ray.Vector.LengthSquared();
			if (lengthSquared < radiusSquared)
			{
				position = Vector3.Zero;
				normal = Vector3.Zero;
				t = 0.0f;

				return false;
			}

			float b = Vector3.Dot( difference, ray.Vector);

			// Determine if the collision ray intersects the sphere.
			float differenceSquared = difference.LengthSquared();
			float discriminant = (b * b) - lengthSquared * (differenceSquared - radiusSquared);
			if (discriminant < 0.0f)
			{
				position = Vector3.Zero;
				normal = Vector3.Zero;
				t = 0.0f;

				return false;
			}

			// Get the distance from the collision ray to the sphere.
			discriminant = (float)Math.Sqrt(discriminant);
			float distance0 = (-b - discriminant) / lengthSquared;
			float distance1 = (-b + discriminant) / lengthSquared;
			if (distance0 > 1.0f || distance1 < 0.0f)
			{
				position = Vector3.Zero;
				normal = Vector3.Zero;
				t = 0.0f;

				return false;
			}

			// Keep value valid.
			t = Math.Max(distance0, 0.0f);

			// Get the position of the intersection.
			ray.GetPointAt(t, out position);

			// Get the normal of the intersection.

			normal = position - this.Position;
            normal.Normalize();

			// Intersection found.
			return true;
		}

		protected internal override float GetVolume()
		{
			return (4.0f / 3.0f) * MathHelper.Pi * this.Radius * this.Radius * this.Radius;
		}

		public override void CalculateMassProperties(float density, out float mass, out Matrix inertiaTensor)
		{
			// Get the mass.
			mass = density * this.GetVolume();
			
			// Calculate some values used below
			float coefficient = (2.0f / 5.0f) * mass * this.Radius;

			// Get the inertia tensor.

			inertiaTensor.M11 = coefficient;
			inertiaTensor.M12 = 0.0f;
			inertiaTensor.M13 = 0.0f;
			inertiaTensor.M14 = 0.0f;

			inertiaTensor.M21 = 0.0f;
			inertiaTensor.M22 = coefficient;
			inertiaTensor.M23 = 0.0f;
			inertiaTensor.M24 = 0.0f;

			inertiaTensor.M31 = 0.0f;
			inertiaTensor.M32 = 0.0f;
			inertiaTensor.M33 = coefficient;
			inertiaTensor.M34 = 0.0f;

			inertiaTensor.M41 = 0.0f;
			inertiaTensor.M42 = 0.0f;
			inertiaTensor.M43 = 0.0f;
			inertiaTensor.M44 = 1.0f;

			// Adjust the inertia tensor by the offset.
			inertiaTensor = MathsHelper.TransferAxis(inertiaTensor, this.OffsetPosition, mass);
		}

		#endregion
	}
}
