#define RANDOMIZE_COLLISIONS

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Represents the context in which collisions occur.
	/// </summary>
	public class CollisionContext
	{
		#region CollisionCollection Declaration

		/// <summary>
		/// A collection of <see cref="Collision"/> references.
		/// </summary>
		internal sealed class CollisionCollection : Collection<Collision>
		{
#if RANDOMIZE_COLLISIONS
			private Random random = new Random();
#endif
			/// <summary>
			/// Initializes a new instances of the <see cref="CollisionCollection"/> class.
			/// </summary>
			internal CollisionCollection() : base(new List<Collision>(512))	{ }
#if RANDOMIZE_COLLISIONS
			protected override void InsertItem(int index, Collision item)
			{
				if (this.Count > 0)
				{
					// Get a random index from the collection.
					int randomIndex = this.random.Next(this.Count);

					// Set the new collision at the random index.
					Collision collision = this.Items[randomIndex];
					this.Items[randomIndex] = item;

					// Call inherited method and insert previous item.
					base.InsertItem(index, collision);
				}
				else
				{
					// Call inherited method.
					base.InsertItem(index, item);
				}
			}
#endif
			protected override void RemoveItem(int index)
			{
				//// Re-order for memory efficiency. 
				//Collision temp = this.Items[this.Count - 1];
				//this.Items[this.Count - 1] = item;
				//this.Items[index] = temp;

				// Call inherited method.
				base.RemoveItem(index);
				//base.RemoveItem(this.Count - 1);
			}
		}

		#endregion

		private ICollisionCallback callback;

		internal CollisionCollection collisions = new CollisionCollection();

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionContext"/> class.
		/// </summary>
		/// <param name="callback">The callback used to notify subscribers of collisions.</param>
		public CollisionContext(ICollisionCallback callback)
		{
			// Hold onto parameters.
			this.callback = callback;

			// Default tolerance.
			this.Tolerance = 0.04f;
		}

		/// <summary>
		/// Gets or sets the collision tolerance when two <see cref="CollisionShape"/> instances 
		/// are being compared for collision detection.
		/// </summary>
		public float Tolerance { get; set; }

		/// <summary>
		/// Removes the restitution coefficient from all collisions.
		/// </summary>
		internal void RemoveRestitution()
		{
			// Remove restitution from all collisions.
			for (int i = 0; i < this.collisions.Count; i++)
			{
				this.collisions[i].restitution = 0.0f;
			}
		}

		/// <summary>
		/// Resets the collision context.
		/// </summary>
		/// <remarks>
		/// Recycles and clears the collision collection.
		/// </remarks>
		public void Reset()
		{
			// Recycle the collisions.
			for (int i = 0; i < this.collisions.Count; i++)
			{
				Collision.Recycle(this.collisions[i]);
			}

			// Clear the collision collection.
			this.collisions.Clear();
		}

		/// <summary>
		/// Notifies and processes a collision manifold between two <see cref="CollisionShape"/> instances.
		/// </summary>
		/// <param name="shape0">The first <see cref="CollisionShape"/> involved in the collision.</param>
		/// <param name="shape1">The second <see cref="CollisionShape"/> involved in the collision.</param>
		/// <param name="normal">The direction to the first (zero) <see cref="CollisionShape"/> from the second <see cref="CollisionShape"/>.</param>
		/// <param name="points">The <see cref="CollisionPoint"/> instances involved in the collision.</param>
		internal void OnCollided(CollisionShape shape0, CollisionShape shape1, ref Vector3 normal, CollisionPointCollection points)
		{
			// Get an used collision.
			Collision collision = Collision.Retrieve();

			// Hold onto parameters.
			collision.Shape0 = shape0;
			collision.Shape1 = shape1;
			collision.Normal = normal;
			for (int i = 0; i < points.Count; i++)
			{
				collision.Points.Add(points[i]);
#if DEBUG
				points[i].Position = collision.Shape0.CollisionGroup.Position + points[i].RelativePosition0;
#endif
			}

			// Store the combined restitution and friction coefficients.

			collision.restitution = shape0.Restitution * shape1.Restitution;
			collision.staticFriction = shape0.StaticFriction * shape1.StaticFriction;
			collision.dynamicFriction = shape0.DynamicFriction * shape1.DynamicFriction;

			// Store the collision for resolution.
			this.collisions.Add(collision);
		}

		// TODO : Move this up into OnCollided.  removal from the collision collection is not a good idea.
		/// <summary>
		/// Validates all detected collisions based on user notification and 
		/// the state of the <see cref="CollisionShape"/> instances involved.
		/// </summary>
		internal void Commit()
		{
			// Iterate on all collisions.
			for (int i = this.collisions.Count - 1; i >= 0; i--)
			{
				// Get the current collision.
				Collision collision = this.collisions[i];

				// Notify any subscribers of collision.
				if (this.callback != null && !this.callback.OnCollided(collision.Shape0, collision.Shape1, ref collision.Normal, collision.Points))
				{
					// Remove the collision.
					this.collisions.RemoveAt(i);
					Collision.Recycle(collision);

					continue;
				}
				// Sensors do not produce collisions.
				else if (((collision.Shape0.CollisionGroup.Flags | collision.Shape1.CollisionGroup.Flags) & CollisionGroupFlags.Sensor) == CollisionGroupFlags.Sensor)
				{
					// Remove the collision.
					this.collisions.RemoveAt(i);
					Collision.Recycle(collision);

					continue;
				}

				// Add the collision to the relevant collision groups.
				if (collision.Shape0.CollisionGroup.RigidBody != null)
				{
					collision.Shape0.CollisionGroup.Collisions.Add(collision);
				}
				if (collision.Shape1.CollisionGroup.RigidBody != null)
				{
					collision.Shape1.CollisionGroup.Collisions.Add(collision);
				}
			}
		}
	}
}
