using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision.Shapes
{
	/// <summary>
	/// Defines a collision shape in the form of an infinite plane.
	/// </summary>
	public partial class CollisionPlane : CollisionShape
	{
		/// <summary>
		/// Gets or sets the normal of the collision plane.
		/// </summary>
		public Vector3 Normal;

		/// <summary>
		/// Gets or sets the distance along the normal the plane is from the origin.
		/// </summary>
		public float Distance;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionPlane"/> class.
		/// </summary>
		public CollisionPlane() : base(CollisionShapeType.Plane) { }

		#region CollisionShape Members

		public override bool IntersectsWith(ref CollisionRay ray, out Vector3 position, out Vector3 normal, out float t)
		{

			float distance = Vector3.Dot(this.Normal, ray.Vector);

			if (Math.Abs(distance) > MathsHelper.Epsilon)
			{
				t = -(Vector3.Dot(this.Normal, ray.Position) + this.Distance) / distance;
				if (t >= 0.0f && t <= 1.0f)
				{
					ray.GetPointAt(t, out position);
					normal = this.Normal;

					return true;
				}
			}
	        
			// The ray does not cross the plane.
			position = Vector3.Zero;
			normal = Vector3.Zero;
			t = 0.0f;

			// There is no intersection.
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
	}
}
