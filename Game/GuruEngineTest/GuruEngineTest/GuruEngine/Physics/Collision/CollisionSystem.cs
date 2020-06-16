using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Allows for <see cref="CollisionGroup"/> filtering during a <see cref="CollisionSystem.IntersectsWith"/> method call.
	/// </summary>
	public interface ICollisionRayCallback
	{
		/// <summary>
		/// Determines if the specified <see cref="CollisionGroup"/> is a valid ray-test test case.
		/// </summary>
		/// <param name="collisionGroup"></param>
		/// <returns>Returns <c>true</c> if the specified <see cref="CollisionGroup"/> is to be test against.</returns>
		bool CanCompare(CollisionGroup collisionGroup);
	}

	/// <summary>
	/// Represents the method that will handle a <see cref="CollisionSystem.CollisionGroupAdded"/>, <see cref="CollisionSystem.CollisionGroupRemoved"/>
	/// or <see cref="CollisionGroupChanged"/> event.
	/// </summary>
	/// <param name="sender">The sender object from which the event came.</param>
	/// <param name="e">The <see cref="CollisionGroupEventArgs"/> data associated with the event.</param>
	public delegate void CollisionGroupEventHandler(object sender, CollisionGroupEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="CollisionSystem.CollisionGroupAdded"/>, <see cref="CollisionSystem.CollisionGroupRemoved"/>
	/// and <see cref="CollisionGroupChanged"/> events.
	/// </summary>
	public class CollisionGroupEventArgs : EventArgs
	{
		private static CollisionGroupEventArgs instance = new CollisionGroupEventArgs();

		private CollisionGroup collisionGroup;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionGroupEventArgs"/> class.
		/// </summary>
		internal CollisionGroupEventArgs() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionGroupEventArgs"/> class.
		/// </summary>
		/// <param name="collisionGroup">The <see cref="CollisionGroup"/> that is the source of the event.</param>
		public CollisionGroupEventArgs(CollisionGroup collisionGroup)
		{
			// Hold onto parameters.
			this.collisionGroup = collisionGroup;
		}

		/// <summary>
		/// Gets the <see cref="CollisionGroup"/> associated with the event.
		/// </summary>
		public CollisionGroup CollisionGroup
		{
			get { return this.collisionGroup; }
			internal set { this.collisionGroup = value; }
		}

		/// <summary>
		/// Gets the singleton instance of the <see cref="CollisionGroupEventArgs"/> class.
		/// <remarks>
		/// This method is used bt <see cref="CollisionSystem"/> instead of creating new instances of the <see cref="CollisionGroupEventArgs"/> class
		/// to prevent garbage collection.
		/// </remarks>
		/// </summary>
		/// <param name="collisionGroup">The <see cref="CollisionGroup"/> that is the source of the event.</param>
		/// <returns>
		/// Returns the singleton instance of the <see cref="CollisionGroupEventArgs"/> class
		/// with the <see cref="CollsionGroupEventArgs.CollisionGroup"/> set to the source of the event.
		/// </returns>
		internal static CollisionGroupEventArgs GetInstance(CollisionGroup collisionGroup)
		{
			CollisionGroupEventArgs.instance.CollisionGroup = collisionGroup;
			return CollisionGroupEventArgs.instance;
		}
	}

	/// <summary>
	/// Defines the comparison strategy for broad-phase collision processing.
	/// </summary>
	public abstract class CollisionSystem
	{
		#region CollisionGroupCollection Declaration

		/// <summary>
		/// A collection of <see cref="CollisionGroup"/> instances.
		/// </summary>
		public class CollisionGroupCollection : Collection<CollisionGroup>
		{
			private CollisionSystem collisionSystem;

			/// <summary>
			/// Initializes a new instance of the <see cref="CollisionGroupCollection"/> class.
			/// </summary>
			/// <param name="collisionSystem">The associated <see cref="CollisionSystem"/>.</param>
			internal CollisionGroupCollection(CollisionSystem collisionSystem)
			{
				// The collision system parameter must be specified.
				if (collisionSystem == null)
				{
					throw new ArgumentNullException("collisionSystem");
				}

				// Hold onto parameters.
				this.collisionSystem = collisionSystem;
			}

			protected override void ClearItems()
			{
				// Remove all references to the collision system from the collision groups.
				for (int i = 0; i < this.Count; i++)
				{
					this.Items[i].CollisionSystem = null;
					this.collisionSystem.OnCollisionGroupRemoved(this.Items[i]);
				}

				// Call inherited method.
				base.ClearItems();
			}

			protected override void InsertItem(int index, CollisionGroup item)
			{
				if (this.Contains(item))
				{
					throw new ArgumentException("item");
				}

				// Call inherited method.
				base.InsertItem(index, item);

				// Associate the item to the collision system.
				item.CollisionSystem = this.collisionSystem;
				this.collisionSystem.OnCollisionGroupAdded(item);
			}

			protected override void RemoveItem(int index)
			{
				// Remove the removed item's collision system association.
				this.Items[index].CollisionSystem = null;
				this.collisionSystem.OnCollisionGroupRemoved(this.Items[index]);

				//// Re-order for memory efficiency. 
				//CollisionGroup temp = this.Items[this.Count - 1];
				//this.Items[this.Count - 1] = item;
				//this.Items[index] = temp;

				// Call inherited method.
				base.RemoveItem(index);
				//base.RemoveItem(this.Count - 1);
			}

			protected override void SetItem(int index, CollisionGroup item)
			{
				// Not allowed to just set an item.
				throw new NotSupportedException();
			}
		}

		#endregion

		#region CollisionFlagsCollisionRayCallback Declaration

		/// <summary>
		/// Filters comparisons during intersection operations with a <see cref="CollisionRay"/>.
		/// </summary>
		internal struct CollisionFlagsCollisionRayCallback : ICollisionRayCallback
		{
			private int collisionFlags;

			/// <summary>
			/// Initializes a new instance of the <see cref="CategoryFlagsCollisionRayCallback"/> class.
			/// </summary>
			/// <param name="collisionFlags">The collision flags to use for comparison filtering.</param>
			internal CollisionFlagsCollisionRayCallback(int collisionFlags)
			{
				// Hold onto parameters.
				this.collisionFlags = collisionFlags;
			}

			#region ICollisionRayCallback Members

			bool ICollisionRayCallback.CanCompare(CollisionGroup collisionGroup)
			{
				return (collisionGroup.CategoryFlags & this.collisionFlags) != 0;
			}

			#endregion
		}

		#endregion

		private CollisionGroupCollection collisionGroups;

		/// <summary>
		/// Occurs when a <see cref="CollisionGroup"/> has been added.
		/// </summary>
		public event CollisionGroupEventHandler CollisionGroupAdded;

		/// <summary>
		/// Occurs when a <see cref="CollisionGroup"/> has been removed.
		/// </summary>
		public event CollisionGroupEventHandler CollisionGroupRemoved;

		/// <summary>
		/// Occurs when a <see cref="CollisionGroup"/> has been changed.
		/// </summary>
		public event CollisionGroupEventHandler CollisionGroupChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionSystem"/> class.
		/// </summary>
		public CollisionSystem()
		{
			// Create the collision group collection.
			this.collisionGroups = new CollisionGroupCollection(this);
		}

		/// <summary>
		/// Gets the <see cref="CollisionGroup"/> instances managed by the <see cref="CollisionSystem"/>.
		/// </summary>
		public CollisionGroupCollection CollisionGroups
		{
			get { return this.collisionGroups; }
		}

		/// <summary>
		/// Detects all collisions between the specified <see cref="RigidBody"/> and the registered <see cref="CollisionGroup"/> instances.
		/// </summary>
		/// <param name="rigidBody">The source <see cref="RigidBody"/>.</param>
		/// <param name="context">The <see cref="CollisionContext"/> to use for collision detection.</param>
		public abstract void DetectCollisions(RigidBody rigidBody, CollisionContext context);

		/// <summary>
		/// Detects all collisions between the specified <see cref="RigidBody"/> instances and the registered <see cref="CollisionGroup"/> instances.
		/// </summary>
		/// <param name="rigidBodies">The source <see cref="RigidBody"/> instances.</param>
		/// <param name="context">The <see cref="CollisionContext"/> to use for collision detection.</param>
		public void DetectCollisions(IList<RigidBody> rigidBodies, CollisionContext context)
		{
			// Iterate on the rigid bodies.
			for (int i = 0; i < rigidBodies.Count; i++)
			{
				// Make sure the rigid body is not disabled or static.
				if ((rigidBodies[i].Flags & (RigidBodyFlags.Disabled | RigidBodyFlags.Static)) != RigidBodyFlags.None)
				{
					continue;
				}

				// Attempt to detect collision with the current rigid body.
				this.DetectCollisions(rigidBodies[i], context);
			}
		}

		/// <summary>
		/// Detects all collisions between the two <see cref="CollisionGroup"/> instances.
		/// </summary>
		/// <param name="collisionGroup0">The first <see cref="CollisionGroup"/>.</param>
		/// <param name="collisionGroup1">The second <see cref="CollisionGroup"/>.</param>
		/// <param name="context">The <see cref="CollisionContext"/> to use for collision detection.</param>
		protected void DetectCollisions(CollisionGroup collisionGroup0, CollisionGroup collisionGroup1, CollisionContext context)
		{
			// Determine comparison validity.
			if (!CollisionSystem.CanCompare(collisionGroup0, collisionGroup1))
			{
				return;
			}

			// Prevent duplicate tests by suppressing reversal comparison.
			if (collisionGroup1.RigidBody != null && (collisionGroup1.RigidBody.Flags & (RigidBodyFlags.Disabled | RigidBodyFlags.Static)) == RigidBodyFlags.None)
			{
				if (collisionGroup0.Id > collisionGroup1.Id)
				{
					return;
				}
			}

			// Determine intersection.

			if (MathsHelper.BoundingBoxContains(collisionGroup0.AABB,collisionGroup1.AABB, context.Tolerance) == ContainmentType.Disjoint)
			{
				return;
			}

			// Iterate on the rigid body's collision shapes.
			for (int i = 0; i < collisionGroup0.CollisionShapes.Count; i++)
			{
				// Iterate on the current collision group's collision shapes.
				for (int j = 0; j < collisionGroup1.CollisionShapes.Count; j++)
				{
					//Determine comparison validity.
					if (!CollisionSystem.CanCompare(collisionGroup0.CollisionShapes[i], collisionGroup1.CollisionShapes[j]))
					{
						continue;
					}

					// Detect collisions between the two collision shapes.
					CollisionDetector.DetectCollisions(collisionGroup0.CollisionShapes[i], collisionGroup1.CollisionShapes[j], context);
				}
			}
		}

		/// <summary>
		/// Performs a ray cast intersection test against the <see cref="CollisionGroup"/> instances of the <see cref="CollisionSystem"/>.
		/// </summary>
		/// <param name="ray">The <see cref="CollisionRay" /> to test the intersection with.</param>
		/// <param name="callback">The <see cref="ICollisionRayCallback"/> to use for test filtering.</param>
		/// <param name="collisionGroup">Set to the closest intersecting <see cref="CollisionGroup"/>.</param>
		/// <param name="position">Set to the location of the intersection if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero" />.</param>
		/// <param name="normal">Set to the surface direction of the intersection if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero" />.</param>
		/// <param name="t">Set to the fraction of the <see cref="CollisionRay.Vector" />  if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to zero.</param>
		/// <returns>Returns <c>true</c> if the specified <see cref="CollisionRay" /> intersects with the <see cref="CollisionShape" />.  Otherwise, returns <c>false</c>.</returns>
		public virtual bool IntersectsWith(ref CollisionRay ray, ICollisionRayCallback callback, out CollisionGroup collisionGroup, out Vector3 position, out Vector3 normal, out float t)
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

            BoundingBox aabb = localRay.GetBoundingBox();

			// Iterate on all registered collision groups.
			for (int i = 0; i < this.CollisionGroups.Count; i++)
			{
				// Get the current collision group.
				CollisionGroup localGroup = this.CollisionGroups[i];

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

		/// <summary>
		/// Performs a ray cast intersection test against the <see cref="CollisionGroup"/> instances of the <see cref="CollisionSystem"/>.
		/// </summary>
		/// <param name="ray">The <see cref="CollisionRay" /> to test the intersection with.</param>
		/// <param name="collisionGroup">Set to the closest intersecting <see cref="CollisionGroup"/>.</param>
		/// <param name="position">Set to the location of the intersection if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero" />.</param>
		/// <param name="normal">Set to the surface direction of the intersection if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero />.</param>
		/// <param name="t">Set to the fraction of the <see cref="CollisionRay.Vector" />  if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to zero.</param>
		/// <returns>Returns <c>true</c> if the specified <see cref="CollisionRay" /> intersects with the <see cref="CollisionShape" />.  Otherwise, returns <c>false</c>.</returns>
		public bool IntersectsWith(ref CollisionRay ray, out CollisionGroup collisionGroup, out Vector3 position, out Vector3 normal, out float t)
		{
			// Call overloaded method.
			return this.IntersectsWith(ref ray, null, out collisionGroup, out position, out normal, out t);
		}

		/// <summary>
		/// Performs a ray cast intersection test against the <see cref="CollisionGroup"/> instances of the <see cref="CollisionSystem"/>.
		/// </summary>
		/// <param name="ray">The <see cref="CollisionRay" /> to test the intersection with.</param>
		/// <param name="collisionFlags">The collision flags to use for comparison filtering.</param>
		/// <param name="collisionGroup">Set to the closest intersecting <see cref="CollisionGroup"/>.</param>
		/// <param name="position">Set to the location of the intersection if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero" />.</param>
		/// <param name="normal">Set to the surface direction of the intersection if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to <see cref="Microsoft.Xna.Framework.Vector3.Zero />.</param>
		/// <param name="t">Set to the fraction of the <see cref="CollisionRay.Vector" />  if an interection occurs with the <see cref="CollisionShape" />. Otherwise, it is set to zero.</param>
		/// <returns>Returns <c>true</c> if the specified <see cref="CollisionRay" /> intersects with the <see cref="CollisionShape" />.  Otherwise, returns <c>false</c>.</returns>
		public bool IntersectsWith(ref CollisionRay ray, int collisionFlags, out CollisionGroup collisionGroup, out Vector3 position, out Vector3 normal, out float t)
		{
			// Call overloaded method.
			return this.IntersectsWith(ref ray, new CollisionFlagsCollisionRayCallback(collisionFlags), out collisionGroup, out position, out normal, out t);
		}

		/// <summary>
		/// Invoked when a <see cref="CollisionGroup"/> is added to the <see cref="CollisionSystem"/>.
		/// <remarks>
		/// The method is invoked when a <see cref="RigidBody"/> is added to the <see cref="PhysicsComponent.RigidBodies"/> collection
		/// or when a <see cref="CollisionGroup"/> is added to the <see cref="CollisionSystem.CollisionGroups"/> collection.
		/// </remarks>
		/// </summary>
		/// <param name="collisionGroup">The <see cref="CollisionGroup"/> to add.</param>
		protected virtual void OnCollisionGroupAdded(CollisionGroup collisionGroup)
		{
			// Raise the added event if necessary.
			if (this.CollisionGroupAdded != null)
			{
				this.CollisionGroupAdded(this, CollisionGroupEventArgs.GetInstance(collisionGroup));
			}
		}

		/// <summary>
		/// Invoked when a <see cref="CollisionGroup"/> is removed from the <see cref="CollisionSystem"/>.
		/// <remarks>
		/// The method is invoked when a <see cref="RigidBody"/> is removed from the <see cref="PhysicsComponent.RigidBodies"/> collection
		/// or when a <see cref="CollisionGroup"/> is added to the <see cref="CollisionSystem.CollisionGroups"/> collection.
		/// </remarks>
		/// </summary>
		/// <param name="collisionGroup">The <see cref="CollisionGroup"/> to remove.</param>
		protected virtual void OnCollisionGroupRemoved(CollisionGroup collisionGroup)
		{
			// Raise the removed event if necessary.
			if (this.CollisionGroupRemoved != null)
			{
				this.CollisionGroupRemoved(this, CollisionGroupEventArgs.GetInstance(collisionGroup));
			}
		}

		/// <summary>
		/// Invoked when a <see cref="CollisionGroup"/> instance's transform properties have changed.
		/// </summary>
		/// <param name="collisionGroup">The <see cref="CollisionGroup"/> that changed.</param>
		internal protected virtual void OnCollisionGroupChanged(CollisionGroup collisionGroup)
		{
			// Raise the changed event if necessary.
			if (this.CollisionGroupChanged != null)
			{
				this.CollisionGroupChanged(this, CollisionGroupEventArgs.GetInstance(collisionGroup));
			}
		}

		/// <summary>
		/// Determines if <paramref name="collisionGroup0"/> can be compared to <paramref name="collisionGroup1"/> during
		/// collision detection.
		/// </summary>
		/// <param name="collisionGroup0">The first <see cref="CollisionGroup"/> in the comparison test.</param>
		/// <param name="collisionGroup1">The second <see cref="CollisionGroup"/> in the comparison test.</param>
		/// <returns>
		/// Returns <c>true</c> if the specified <see cref="CollisionGroup"/> instances are comparable during
		/// collision detection.  Otherwise, returns <c>false</c>.
		/// </returns>
		protected static bool CanCompare(CollisionGroup collisionGroup0, CollisionGroup collisionGroup1)
		{
			return
				// Collision groups should be different.
				collisionGroup0 != collisionGroup1 &&

				// Exclusion flags must be different and not zero.
				(collisionGroup0.ExclusionFlag == 0 || 
				 collisionGroup1.ExclusionFlag == 0 ||
				 collisionGroup0.ExclusionFlag != collisionGroup1.ExclusionFlag) &&

				// Category and collision flags should overlap.
				(collisionGroup0.CategoryFlags & collisionGroup1.CollisionFlags) != 0 &&
				(collisionGroup1.CategoryFlags & collisionGroup0.CollisionFlags) != 0;
		}

		/// <summary>
		/// Determines if <paramref name="shape0"/> can be compared to <paramref name="shape1"/> during
		/// collision detection.
		/// </summary>
		/// <param name="shape0">The first <see cref="CollisionShape"/> in the comparison test.</param>
		/// <param name="shape1">The second <see cref="CollisionShape"/> in the comparison test.</param>
		/// <returns>
		/// Returns <c>true</c> if the specified <see cref="CollisionShape"/> instances are comparable during
		/// collision detection.  Otherwise, returns <c>false</c>.
		/// </returns>
		internal protected static bool CanCompare(CollisionShape shape0, CollisionShape shape1)
		{
			return
				// Collision shapes should be different.
				shape0 != shape1 &&

				// Category and collision flags should overlap.
				(shape0.CategoryFlags & shape1.CollisionFlags) != 0 &&
				(shape1.CategoryFlags & shape0.CollisionFlags) != 0;
		}
	}

	
}
