using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Represents a concrete shape for collision processing.
	/// </summary>
	public abstract class CollisionShape
	{
		internal int typeId;

		private static int instanceCount;
		internal int id;

		private CollisionGroup collisionGroup;

		/// <summary>
		/// Gets or sets an arbitrary category bit mask for collision filtering.
		/// </summary>
		/// <remarks>
		/// Only collision shapes with a matching <see cref="CollisionShape.CollisionFlag" /> bit will collide with this <see cref="CollisionShape" />.
		/// </remarks>
		public int CategoryFlags = int.MaxValue;

		/// <summary>
		/// Gets or sets an arbitrary collision bit mask for collision filtering.
		/// </summary>
		/// <remarks>
		/// This <see cref="CollisionShape" /> will only collide with collision shapes with a matching <see cref="CollisionShape.CategoryFlag" /> bit.
		/// </remarks>
		public int CollisionFlags = int.MaxValue;

		/// <summary>
		/// Gets the location of the <see cref="CollisionShape"/> in world coordinates.
		/// </summary>
		/// <remarks>
		/// Do not set this field directly.  Instead, use the <see cref="CollisionShape.OffsetPosition" /> field to
		/// adjust a <see cref="CollisionShape" /> position.
		/// </remarks>
		public Vector3 Position;

		/// <summary>
		/// Gets the orientation of the <see cref="CollisionShape"/> in world coordinates.
		/// </summary>
		/// <remarks>
		/// Do not set this field directly.  Instead, use the <see cref="CollisionShape.OffsetOrientation" /> field to
		/// adjust a <see cref="CollisionShape" /> orientation.
		/// </remarks>

		public Matrix Orientation = Matrix.Identity;
		/// <summary>
		/// Gets or sets the location of the <see cref="CollisionShape"/> in local coordinates.
		/// </summary>
		public Vector3 OffsetPosition;

		/// <summary>
		/// Gets or sets the orientation of the <see cref="CollisionShape"/> in local coordinates.
		/// </summary>

		public Matrix OffsetOrientation = Matrix.Identity;
		/// <summary>
		/// Gets the axis-aligned bounding box for the <see cref="CollisionShape"/>.
		/// </summary>
		/// <remarks>
		/// Do not set this field directly.  This field is calculated by the <see cref="CollisionShape.CalculateAABB" /> method.
		/// </remarks>
		public BoundingBox AABB = MathsHelper.InfiniteBB;
		
		/// <summary>
		/// Gets or sets the static friction of the collision shape in local coordinates.
		/// </summary>
		/// <remarks>
		/// The value should be set between 0.0 and 1.0.  Friction decreases as the value approaches zero.
		/// </remarks>
		public float StaticFriction = 0.5f;

		/// <summary>
		/// Gets or sets the dynamic friction of the <see cref="CollisionShape"/>.
		/// </summary>
		/// <remarks>
		/// The value should be set between 0.0 and 1.0.  Friction decreases as the value approaches zero.
		/// </remarks>
		public float DynamicFriction = 0.5f;

		/// <summary>
		/// Gets or sets the restitution or bounciness of the <see cref="CollisionShape"/>.
		/// </summary>
		/// <remarks>
		/// The value should be set between 0.0 and 1.0.  Bounciness decreases as the value approaches zero.
		/// </remarks>
		public float Restitution = 0.5f;

		/// <summary>
		/// Initilizes a new instance of the <see cref="CollisionShape"/> class.
		/// </summary>
		/// <remarks>
		/// For serialization with the <see cref="IntermediateSerializer"/>.
		/// </remarks>
		protected CollisionShape() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionShape" /> class.
		/// </summary>
		/// <param name="typeId">An <see cref="System.Int32"/> representing the shape type.</param>
		internal CollisionShape(CollisionShapeType typeId)
		{
			// Hold onto parameters.
			this.typeId = (int)typeId;

			// Assign the collision shape a unique id.
			this.id = Interlocked.Increment(ref CollisionShape.instanceCount);
		}

		/// <summary>
		/// Gets a unique id for the <see cref="CollisionShape"/>.
		/// </summary>
		public int Id
		{
			get { return this.id; }
		}

		/// <summary>
		/// Gets or sets the object that contains data about the <see cref="CollisionShape"/>.
		/// </summary>
		[ContentSerializerIgnore()]
		public object Tag { get; set; }

		/// <summary>
		/// Gets the associated <see cref="CollisionGroup" /> of the <see cref="CollisionShape"/>.
		/// </summary>
		[ContentSerializerIgnore()]
		public CollisionGroup CollisionGroup
		{
			get { return this.collisionGroup; }
			internal set { this.collisionGroup = value; }
		}
		
		/// <summary>
		/// Calculates any additional information about the <see cref="CollisionShape" />.
		/// </summary>
		internal virtual void CalculateInternals()
		{
			// Calculate the axis-aligned bound box of this collision shape.
			this.CalculateAABB();
		}

		/// <summary>
		/// Calculates the axis-aligned bounding box of the <see cref="CollisionShape" />.
		/// </summary>
		protected virtual void CalculateAABB()
		{
			// No calculation necessary if the axis-aligned bounding box is irrelevant.
		}

		/// <summary>
		/// Gets the volume of the <see cref="CollisionShape" />.
		/// </summary>
		/// <returns>
		/// Returns the volume of the <see cref="CollisionShape" />.
		/// </returns>
		protected internal abstract float GetVolume();
		
		/// <summary>
		/// Performs a ray cast intersection test against the <see cref="CollisionShape"/>.
		/// </summary>
		/// <param name="ray">The <see cref="CollisionRay" /> to test the intersection with.</param>
		/// <param name="position">Set to the location of the intersection if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero" />.</param>
		/// <param name="normal">Set to the surface direction of the intersection if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero" />.</param>
		/// <param name="t">Set to the fraction of the <see cref="CollisionRay.Vector" />  if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to zero.</param>
		/// <returns>
		/// Returns <c>true</c> if the specified <see cref="CollisionRay" /> intersects with the <see cref="CollisionShape" />.  Otherwise, returns <c>false</c>.
		/// </returns>
		public abstract bool IntersectsWith(ref CollisionRay ray, out Vector3 position, out Vector3 normal, out float t);
		
		/// <summary>
		/// Gets the mass and inertia tensor for the <see cref="CollisionShape"/>.
		/// </summary>
		/// <param name="density">The density of the <see cref="CollisionShape"/>.</param>
		/// <param name="mass">The mass of the <see cref="CollisionShape"/>.</param>
		/// <param name="inertiaTensor">The inertia tensor of the <see cref="CollisionShape"/>.</param>
		public abstract void CalculateMassProperties(float density, out float mass, out Matrix inertiaTensor);
	}
}
