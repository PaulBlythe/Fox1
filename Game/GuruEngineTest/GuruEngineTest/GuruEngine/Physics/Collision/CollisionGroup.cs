using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Represents the behaviors and/or states of a <see cref="CollisionGroup />.
	/// </summary>
	[Flags()]
	public enum CollisionGroupFlags
	{
		/// <summary>
		/// No special behavior or state.
		/// </summary>
		None = 0,

		/// <summary>
		/// The <see cref="CollisionGroup"/> is for collision notifications only.
		/// </summary>
		Sensor = 1
	}

	/// <summary>
	/// Manages and allows for a compound <see cref="CollisionShape"/>.
	/// </summary>
	public class CollisionGroup
	{
		#region CollisionShapeCollection Declaration

		/// <summary>
		/// A collection of <see cref="CollisionShape"/> instances.
		/// </summary>
		public sealed class CollisionShapeCollection : Collection<CollisionShape>
		{
			private CollisionGroup collisionGroup;

			/// <summary>
			/// Initializes a new instances of the <see cref="CollisionGroup.CollisionShapeCollection"/> class.
			/// </summary>
			/// <param name="collisionGroup">The associated <see cref="CollisionGroup"/></param>
			internal CollisionShapeCollection(CollisionGroup collisionGroup)
			{
				// The collision group parameter must be specified.
				if (collisionGroup == null)
				{
					throw new ArgumentNullException("collisionGroup");
				}

				// Hold onto parameters.
				this.collisionGroup = collisionGroup;
			}

			protected override void ClearItems()
			{
				// Remove all references to the collsion group from the collision shapes.
				for (int i = 0; i < this.Count; i++)
				{
					this.Items[i].CollisionGroup = null;
				}
                
				// Call inherited method.
				base.ClearItems();

				// Update the bounding box of all the collision shapes.
				this.collisionGroup.CalculateAABB();
			}

			protected override void InsertItem(int index, CollisionShape item)
			{
				if (item.CollisionGroup != null)
				{
					throw new ArgumentException("item");
				}

				// Call inherited method.
				base.InsertItem(index, item);

				// Associate the item to the collision group.
				item.CollisionGroup = this.collisionGroup;

				// Update the bounding box of all the collision shapes.
				this.collisionGroup.CalculateAABB();
			}

			protected override void RemoveItem(int index)
			{
				// Remove the removed item's collision group association.
				this.Items[index].CollisionGroup = null;

				//// Re-order for memory efficiency. 
				//CollisionShape temp = this.Items[this.Count - 1];
				//this.Items[this.Count - 1] = item;
				//this.Items[index] = temp;

				// Call inherited method.
				base.RemoveItem(index);
				//base.RemoveItem(this.Count - 1);

				// Update the bounding box of all the collision shapes.
				this.collisionGroup.CalculateAABB();
			}

			protected override void SetItem(int index, CollisionShape item)
			{
				// Not allowed to just set an item.
				throw new NotSupportedException();
			}
		}

		#endregion

		#region CollisionCollection Declaration

		/// <summary>
		/// A collection of <see cref="Collision"/> instances.
		/// </summary>
		public sealed class CollisionCollection : Collection<Collision>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="CollisionCollection"/> class.
			/// </summary>
			internal CollisionCollection() : base(new List<Collision>(16)) { }

			/// <summary>
			/// Marks all collisions as unsolved.
			/// </summary>
			internal void Reset()
			{
				// Mark all associated collisions as unsolved.
				for (int i = 0; i < this.Count; i++)
				{
					this.Items[i].satisfied = false;
				}
			}
		}

		#endregion

		private static int instanceCount;
		internal int id;

		/// <summary>
		/// Gets or sets the behavior flags of the <see cref="CollisionGroup"/>.
		/// </summary>
		public CollisionGroupFlags Flags;

		private RigidBody rigidBody;
		private CollisionSystem collisionSystem;

		private CollisionShapeCollection collisionShapes;

		/// <summary>
		/// Gets or sets an arbitrary category bit mask for collision filtering.
		/// </summary>
		/// <remarks>
		/// Only collision groups with a matching <see cref="CollisionGroup.CollisionFlag" /> bit will collide with this <see cref="CollisionGroup" />.
		/// </remarks>
		public int CategoryFlags = int.MaxValue;

		/// <summary>
		/// Gets or sets an arbitrary collision bit mask for collision filtering.
		/// </summary>
		/// <remarks>
		/// This <see cref="CollisionGroup" /> will only collide with collision groups with a matching <see cref="CollisionGroup.CategoryFlag" /> bit.
		/// </remarks>
		public int CollisionFlags = int.MaxValue;

		/// <summary>
		/// Gets or sets an arbitrary exclusion flag for collision filtering.
		/// </summary>
		/// <remarks>
		/// This <see cref="CollisionGroup"/> will only collide with other 
		/// <see cref="CollisionGroup"/> instances where the <see cref="CollisionGroup.ExclusionFlag"/> is different.
		/// </remarks>
		[ContentSerializer(Optional=true)]
		public int ExclusionFlag = 0;

		/// <summary>
		/// Gets or sets the location of the <see cref="CollisionGroup"/>.
		/// </summary>
		/// <remarks>
		/// When associated with a <see cref="RigidBody"/>, the position gets updated automatically during integration.
		/// </remarks>
		public Vector3 Position;

		/// <summary>
		/// Gets or sets the orientation of the <see cref="CollisionGroup"/>.
		/// </summary>
		/// <remarks>
		/// When associated with a <see cref="RigidBody"/>, the orientation gets updated automatically during integration.
		/// </remarks>
#if QUATERNION_ORIENTATION
		public Quaternion Orientation = Quaternion.Identity;
#else
		public Matrix Orientation = Matrix.Identity;
#endif
		/// <summary>
		/// Gets the axis-aligned bounding box of the <see cref="CollisionGroup"/>.
		/// </summary>
		/// <remarks>
		/// This field should not be set directly.  The axis-aligned bounding box of the <see cref="CollisionGroup"/>
		/// gets updated automatically during movement.
		/// </remarks>
		public BoundingBox AABB = MathsHelper.EmptyBB;
		
		private CollisionCollection collisions = new CollisionCollection();

		// Used in collision systems.
		internal object internalTag;

		/// <summary>
		/// Initialize a new instance of the <see cref="CollisionGroup"/> class.
		/// </summary>
		public CollisionGroup()	: this(null) {	}

		/// <summary>
		/// Initialize a new instance of the <see cref="CollisionGroup"/> class
		/// and associates the specified <see cref="RigidBody"/>.
		/// </summary>
		/// <param name="rigidBody">The <see cref="RigidBody"/> to associate.</param>
		internal CollisionGroup(RigidBody rigidBody)
		{
			// Assign the collision group a unique id.
			this.id = Interlocked.Increment(ref CollisionGroup.instanceCount);

			// Hold onto parameters.
			this.rigidBody = rigidBody;

			// Create the collision shapes collection.
			this.collisionShapes = new CollisionShapeCollection(this);
		}

		/// <summary>
		/// Gets a unique id for the <see cref="CollisionGroup"/>.
		/// </summary>
		public int Id
		{
			get { return this.id; }
		}

		/// <summary>
		/// Gets or sets the object that contains data about the <see cref="CollisionGroup"/>.
		/// </summary>
		[ContentSerializerIgnore()]
		public object Tag { get; set; }

		/// <summary>
		/// Gets the associated <see cref="RigidBody"/>.
		/// </summary>
		/// <remarks>
		/// Static <see cref="CollisionGroup"/>'s do not have an associated <see cref="RigidBody"/>.
		/// </remarks>
		[ContentSerializerIgnore()]
		public RigidBody RigidBody
		{
			get { return this.rigidBody; }
		}

		/// <summary>
		/// Gets the associated <see cref="CollisionSystem"/> for the <see cref="CollisionGroup"/>.
		/// </summary>
		[ContentSerializerIgnore()]
		public CollisionSystem CollisionSystem
		{
			get { return this.collisionSystem; }
			internal set { this.collisionSystem = value; }
		}

		/// <summary>
		/// Gets the <see cref="CollisionShape"/> instances associated with the <see cref="CollisionGroup"/>.
		/// </summary>
		public CollisionShapeCollection CollisionShapes
		{
			get { return this.collisionShapes; }
		}

		/// <summary>
		/// Gets the <see cref="Collision"/> instances associated with the <see cref="CollisionGroup"/>.
		/// </summary>
		[ContentSerializerIgnore()]
		public CollisionCollection Collisions
		{
			get { return this.collisions; }
		}

		/// <summary>
		/// Updates the transform of the <see cref="CollisionGroup"/> and all associated <see cref="CollisionShape"/> instances.
		/// </summary>
		public void UpdateTransform()
		{
			// Update all collision shapes by the specified transform.
			for (int i = 0; i < this.collisionShapes.Count; i++)
			{
				this.collisionShapes[i].CalculateInternals();
			}

			// Update the axis-aligned bounding box.
			this.CalculateAABB();

			// Notify the collision system of the change.
			if (this.collisionSystem != null)
			{
				this.collisionSystem.OnCollisionGroupChanged(this);
			}
		}

		/// <summary>
		/// Updates the transform of the <see cref="CollisionGroup"/> and all associated <see cref="CollisionShape"/> instances.
		/// </summary>
		/// <param name="position">A <see cref="Microsoft.Xna.Framework.Vector3" /> representing the new position of the <see cref="CollisionGroup" />.</param>
		public void UpdateTransform(ref Vector3 position)
		{
			// Call overloaded method.
			this.UpdateTransform(ref position, ref this.Orientation);
		}

		/// <summary>
		/// Updates the transform of the <see cref="CollisionGroup"/> and all associated <see cref="CollisionShape"/> instances.
		/// </summary>
#if QUATERNION_ORIENTATION
		/// <param name="orientation">A <see cref="Microsoft.Xna.Framework.Quaternion" /> representing the new orientation of the <see cref="CollisionGroup" />.</param>
		public void UpdateTransform(ref Quaternion orientation)
#else
		/// <param name="orientation">A <see cref="Microsoft.Xna.Framework.Matrix" /> representing the new orientation of the <see cref="CollisionGroup" />.</param>
		public void UpdateTransform(ref Matrix orientation)
#endif
		{
			// Call overloaded method.
			this.UpdateTransform(ref this.Position, ref orientation);
		}

		/// <summary>
		/// Updates the transform of the <see cref="CollisionGroup"/> and all associated <see cref="CollisionShape"/> instances.
		/// </summary>
		/// <param name="position">A <see cref="Microsoft.Xna.Framework.Vector3" /> representing the new position of the <see cref="CollisionGroup" />.</param>
#if QUATERNION_ORIENTATION
		/// <param name="orientation">A <see cref="Microsoft.Xna.Framework.Quaternion" /> representing the new orientation of the <see cref="CollisionGroup" />.</param>
		public void UpdateTransform(ref Vector3 position, ref Quaternion orientation)
#else
		/// <param name="orientation">A <see cref="Microsoft.Xna.Framework.Matrix" /> representing the new orientation of the <see cref="CollisionGroup" />.</param>
		public void UpdateTransform(ref Vector3 position, ref Matrix orientation)
#endif
		{
			// Hold onto parameters.
			this.Position = position;
			this.Orientation = orientation;

			// Update all collision shapes by the specified transform.
			for (int i = 0; i < this.collisionShapes.Count; i++)
			{
				// Get current collision shape.
				CollisionShape collisionShape = this.collisionShapes[i];

				// TODO : Move to CollisionShape.UpdateTransformBy

				collisionShape.Position = position + Vector3.Transform(collisionShape.OffsetPosition, orientation);
				collisionShape.Orientation = collisionShape.OffsetOrientation * orientation;
				collisionShape.CalculateInternals();
			}

			// Update the axis-aligned bounding box.
			this.CalculateAABB();

			// Notify the collision system of the change.
			if (this.collisionSystem != null)
			{
				this.collisionSystem.OnCollisionGroupChanged(this);
			}
		}

		/// <summary>
		/// Calculates the axis-aligned bounding box of the <see cref="CollisionGroup" />.
		/// </summary>
		internal void CalculateAABB()
		{
			this.AABB = MathsHelper.EmptyBB;
			for (int i = 0; i < this.collisionShapes.Count; i++)
			{

				this.AABB = BoundingBox.CreateMerged(this.AABB, this.collisionShapes[i].AABB);
			}
		}

		/// <summary>
		/// Performs an interection test against the <see cref="CollisionGroup"/>.
		/// </summary>
		/// <param name="ray">The <see cref="CollisionRay" /> to test the intersection with.</param>
		/// <param name="position">Set to the location of the intersection if an interection occurs with the <see cref="CollisionGroup" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero />.</param>
		/// <param name="normal">Set to the surface direction of the intersection if an interection occurs with the <see cref="CollisionGroup" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero />.</param>
		/// <param name="t">Set to the fraction of the <see cref="CollisionRay.Vector" />  if an interection occurs with the <see cref="CollisionGroup" />. Otherwise, it is set to zero.</param>
		/// <returns>
		/// Returns <c>true</c> if the specified <see cref="CollisionRay" /> intersects with the <see cref="CollisionGroup" />.  Otherwise, returns <c>false</c>.
		/// </returns>
		public bool IntersectsWith(ref CollisionRay ray, out Vector3 position, out Vector3 normal, out float t)
		{
			// Initialize results.
			position = Vector3.Zero;
			normal = Vector3.Zero;
			t = float.MaxValue;

			// Working storage.
			Vector3 localPosition;
			Vector3 localNormal;
			float localT;

			// Make copy of ray.
			CollisionRay localRay = ray;

			// Itererate on all collision shapes.
			for (int i = 0; i < this.collisionShapes.Count; i++)
			{
				// Determine intersection with the current collision shape.
				if (this.collisionShapes[i].IntersectsWith(ref localRay, out localPosition, out localNormal, out localT))
				{
					// Hold onto results.
					position = localPosition;
					normal = localNormal;
					t = localT;

					// Adjust the ray's length and try again.

					localRay.Vector *= t;
				}
			}

			// An intersection was found.
			return t <= 1.0f;
		}

		// TODO : Move to rigid body???
		// TODO : This isn't even remotely correct.
		public void CalculateMassProperties(float density, out float mass, out Matrix inertiaTensor)
		{
			// Initialize results.
			mass = 0.0f;
			inertiaTensor = MathsHelper.Zero;

			// Get the total weight of the collision shapes.
			float totalWeight = 0.0f;
			for (int i = 0; i < this.collisionShapes.Count; i++)
			{
				totalWeight += this.collisionShapes[i].GetVolume();
			}

			// Determine the mass, center of mass and total moment of inertia from all the collision shapes.
			for (int i = 0; i < this.collisionShapes.Count; i++)
			{
				// Get the weight of the current shape.
				float weight = this.collisionShapes[i].GetVolume();

				// Get the mass properties of the current shape.
				float tempMass;
				Matrix tempIntertiaTensor;
				this.collisionShapes[i].CalculateMassProperties(
					density * (weight / totalWeight), 
					out tempMass, 
					out tempIntertiaTensor);

				// Update totals.
				mass += tempMass;

				inertiaTensor += tempIntertiaTensor;
			}
		}
	}

	
}
