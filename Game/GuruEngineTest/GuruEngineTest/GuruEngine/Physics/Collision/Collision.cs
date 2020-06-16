using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Represents one contact point between two <see cref="CollisionShape"/> instances
	/// in a collision manifold.
	/// </summary>
	public class CollisionPoint
	{
		private static ObjectPool<CollisionPoint> pool = new ObjectPool<CollisionPoint>(128);
#if DEBUG
		/// <summary>
		/// Gets or sets the position of the <see cref="CollisionPoint"/> in world-coordinates.
		/// </summary>
		public Vector3 Position;
#endif
		/// <summary>
		/// Gets or sets the relative position to the <see cref="CollisionPoint"/> 
		/// of the first <see cref="CollisionShape"/> in the associated <see cref="Collision"/>.
		/// </summary>
		public Vector3 RelativePosition0;

		/// <summary>
		/// Gets or sets the relative position to the <see cref="CollisionPoint"/> 
		/// of the second <see cref="CollisionShape"/> in the associated <see cref="Collision"/>.
		/// </summary>
		public Vector3 RelativePosition1;

		/// <summary>
		/// Gets or sets the penetration depth of the <see cref="CollisionPoint"/>.
		/// </summary>
		public float Depth;

		internal float velocityChange; // This will be the reciprocal during usage.
		internal float desiredVelocityChange;

		/// <summary>
		/// Initialize a new instance of the <see cref="CollisionPoint"/> class.
		/// </summary>
		public CollisionPoint() { }

		/// <summary>
		/// Gets a <see cref="CollisionPoint" /> instance from the pool.
		/// </summary>
		/// <returns>
		/// Returns a fresh <see cref="CollisionPoint"/> instance.
		/// </returns>
		internal static CollisionPoint Retrieve()
		{
			// Get an unused collision point from the pool.
			CollisionPoint point = CollisionPoint.pool.Retrieve();

			// Initialize other data to defaults.
			point.velocityChange = 0.0f;
			point.desiredVelocityChange = 0.0f;
			
			// Return the collision point.
			return point;
		}

		/// <summary>
		/// Returns a <see cref="CollisionPoint"/> instance to the pool.
		/// </summary>
		/// <param name="point">The <see cref="CollisionPoint"/> to recycle.</param>
		internal static void Recycle(CollisionPoint point)
		{
			// Put the collision point back in the pool for later use.
			CollisionPoint.pool.Recycle(point);
		}
	}

	/// <summary>
	/// Represents a collection of <see cref="CollisionPoint"/> instances.
	/// </summary>
	public class CollisionPointCollection : Collection<CollisionPoint>
	{
		private static ObjectPool<CollisionPointCollection> pool = new ObjectPool<CollisionPointCollection>(8);

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionPointCollection"/> class.
		/// </summary>
		public CollisionPointCollection() { }

		/// <summary>
		/// Gets a <see cref="CollisionPointCollection" /> instance from the pool.
		/// </summary>
		/// <returns>
		/// Returns a fresh <see cref="CollisionPointCollection"/> instance.
		/// </returns>
		internal static CollisionPointCollection Retrieve()
		{
			// Return an unused collision point collection from the pool.
			return CollisionPointCollection.pool.Retrieve();
		}

		/// <summary>
		/// Returns a <see cref="CollisionPointCollection"/> instance to the pool.
		/// </summary>
		/// <param name="collisionPoints">The <see cref="CollisionPointCollection"/> to recycle.</param>
		internal static void Recycle(CollisionPointCollection collisionPoints)
		{
			// Uninitialize.
			collisionPoints.Clear();

			// Put the collision point collection back in the pool for later use.
			CollisionPointCollection.pool.Recycle(collisionPoints);
		}
	}

	// TODO : Derive from Constraint???  They're close in functionality and usage.
	/// <summary>
	/// Represents a collision manifold between two <see cref="CollisionShape"/> instances.
	/// </summary>
	public class Collision
	{
		private static ObjectPool<Collision> pool = new ObjectPool<Collision>(128);

		/// <summary>
		/// Gets or sets the first <see cref="CollisionShape"/> involved in the collision.
		/// </summary>
		public CollisionShape Shape0;

		/// <summary>
		/// Gets or sets the second <see cref="CollisionShape"/> involved in the collision.
		/// </summary>
		public CollisionShape Shape1;

		/// <summary>
		/// Gets or sets the direction to the first (zero) <see cref="CollisionShape"/> from
		/// the second <see cref="CollisionShape"/>.
		/// </summary>
		public Vector3 Normal;

		private List<CollisionPoint> points = new List<CollisionPoint>(12);

		internal float staticFriction;
		internal float dynamicFriction;
		internal float restitution;

		internal bool satisfied;
	            
		/// <summary>
		/// Initializes a new instance of the <see cref="Collision"/> class.
		/// </summary>
		public Collision() { }

		/// <summary>
		/// Gets all the <see cref="CollisionPoint"/> instances involved in the collision.
		/// </summary>
		public List<CollisionPoint> Points
		{
			get { return this.points; }
		}

		/// <summary>
		/// Called before an attempt to resolve the <see cref="Collision"/> is made.
		/// </summary>
		/// <param name="elapsed">The simulation step's total elapsed time.</param>
		internal void PreStep(float elapsed)
		{
			// Reset satisfied flag.
			this.satisfied = false;

			// Get the rigid bodies involved in this collision for easier access.
			RigidBody rigidBody0 = this.Shape0.CollisionGroup.RigidBody;
			RigidBody rigidBody1 = this.Shape1.CollisionGroup.RigidBody;

			// TODO : Make parameter.
			// Relax the resolution over a few frames.
			const int relaxationCount = 12;
			float inverseElapsed = 1.0f / MathHelper.Max(relaxationCount * elapsed, MathsHelper.Epsilon);

			// Iterate on all the contact points of this collision.
			for (int i = 0; i < this.Points.Count; i++)
			{
				// Get the current contact point.
				CollisionPoint point = this.Points[i];

				// Determine the desired velocity change of this collision point.
				point.desiredVelocityChange = point.Depth * inverseElapsed;
								
				// Limit the desired velocity change in the direction of the collision normal.
				point.desiredVelocityChange = MathHelper.Clamp(point.desiredVelocityChange, 0.0f, 2.5f);

				// Determine the relative velocity change magnitude of the first rigid body at the collision position.
				point.velocityChange = 0.0f;
				if (rigidBody0 != null && (rigidBody0.Flags & RigidBodyFlags.Static) == RigidBodyFlags.None)
				{

					point.velocityChange = rigidBody0.inverseMass + Vector3.Dot(this.Normal, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(point.RelativePosition0, this.Normal), rigidBody0.inverseInertiaTensorWorld), point.RelativePosition0));
				}

				// Determine the relative velocity change magnitude of the first second body at the collision position.
				// Apply it the first rigid body's velocity change.
				if (rigidBody1 != null && (rigidBody1.Flags & RigidBodyFlags.Static) == RigidBodyFlags.None)
				{
					point.velocityChange += rigidBody1.inverseMass + Vector3.Dot(this.Normal, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(point.RelativePosition1, this.Normal), rigidBody1.inverseInertiaTensorWorld), point.RelativePosition1));
				}

				// Make sure the relative velocity change does not cause division issues when determining the 
				// magnitude of the impulse needed to resolve a collision along the collision normal.
				if (point.velocityChange < MathsHelper.Epsilon)
				{
					point.velocityChange = MathsHelper.Epsilon;
				}

				// Store the reciprocal.
				point.velocityChange = 1.0f / point.velocityChange;
			}
		}

		/// <summary>
		/// Attempts to solve the <see cref="Collision"/>.
		/// </summary>
		/// <param name="elapsed">The simulation step's total elapsed time.</param>
		/// <returns><b>true</b>, if the <see cref="Collision"/> was enforced. <b>false</b>, otherwise.</returns>
		internal bool Solve(float elapsed)
		{
			// Get the rigid bodies involved in this collision for easier access.
			RigidBody rigidBody0 = this.Shape0.CollisionGroup.RigidBody;
			RigidBody rigidBody1 = this.Shape1.CollisionGroup.RigidBody;

			// Assume the collision is satisfied at the start.
			this.satisfied = true;

			// Iterate through all the collision's contact points.
			bool result = false;
			for (int i = 0; i < this.Points.Count; i++)
			{
				// Get the current contact point.
				CollisionPoint point = this.Points[i];

				// Get the relative velocity to the collision point of the first rigid body.
				Vector3 temp = Vector3.Zero;
				if (rigidBody0 != null)
				{
					temp = rigidBody0.Velocity + Vector3.Cross(rigidBody0.Rotation, point.RelativePosition0);
				}

				// Get the relative velocity to the collision point of the second rigid body.  Apply it to the first.
				if (rigidBody1 != null)
				{
					temp -= rigidBody1.Velocity + Vector3.Cross(rigidBody1.Rotation, point.RelativePosition1);
				}

				// Determine the velocity magnitude in the direction of the collision normal.
				float normalVelocity = Vector3.Dot(temp, this.Normal);

				// Check if the rigid body is already heading towards resolving this collision point.
				if (normalVelocity > point.desiredVelocityChange)
				{
					continue;
				}

				// Apply the restitution coefficient.
				float restitutedNormalVelocity = -this.restitution * normalVelocity;
				if (restitutedNormalVelocity < 0.0009f)
				{
					restitutedNormalVelocity = point.desiredVelocityChange;
				}

				// Check threshold for minimum required velocity change. 
				float deltaVelocity = restitutedNormalVelocity - normalVelocity;
				if (deltaVelocity <= 0.0009f)
				{
					continue;
				}

				// Determine the magnitude of the impulse needed to resolve this collision point along
				// the collision normal.  The value stored in velocity change is the reciprocal.  This
				// is a division operation.
				float normalImpulse = deltaVelocity * point.velocityChange;

				// Determine the vector impulse.

				Vector3 impulse = this.Normal * normalImpulse;

				// Apply the impulse to the rigid bodies involved in this collision.
				if (rigidBody0 != null && (rigidBody0.Flags & RigidBodyFlags.NoResponse) == RigidBodyFlags.None)
				{
					rigidBody0.ApplyOffsetForceImpulse(ref impulse, ref point.RelativePosition0);
				}
				if (rigidBody1 != null && (rigidBody1.Flags & RigidBodyFlags.NoResponse) == RigidBodyFlags.None)
				{
					rigidBody1.ApplyReverseOffsetForceImpulse(ref impulse, ref point.RelativePosition1);
				}

				// This collision need to be resolved based on the current collision point.
				result = true;

				// Frictionless collisions don't require the following processing.
				if (this.staticFriction == 0.0f && this.dynamicFriction == 0.0f)
				{
					continue;
				}

				// Get the relative velocity to the collision point of the first rigid body again.  It has
				// changed because of the applied impulse above.
				Vector3 relativeVelocity = Vector3.Zero;
				if (rigidBody0 != null)
				{

					relativeVelocity = rigidBody0.Velocity + Vector3.Cross(rigidBody0.Rotation, point.RelativePosition0);
				}

				// Get the relative velocity to the collision point of the secong rigid body again.  It has
				// changed because of the applied impulse above.  Apply it the first rigid body's relative velocity.
				if (rigidBody1 != null)
				{
					relativeVelocity -= rigidBody1.Velocity + Vector3.Cross(rigidBody1.Rotation, point.RelativePosition1);
				}

				// Determine the velocity at the collision position perpendicular to the collision normal.  This
				// gives the surface velocity needed to apply friction.

				Vector3 perpendicularVelocity = relativeVelocity - (this.Normal * Vector3.Dot(relativeVelocity, this.Normal));
				float perpendicularSpeed = perpendicularVelocity.Length();
				if (perpendicularSpeed > 0.0009f)
				{
					// Get the direction at the collision position to apply friction.
					Vector3 frictionDirection = perpendicularVelocity / -perpendicularSpeed;

					// Determine the magnitude of an impulse necessary ro remove all relative velocity at the collision position of
					// the first rigid body.  This takes into account the rigid body's mass properties of course.
					float denominator = 0.0f;
					if (rigidBody0 != null && (rigidBody0.Flags & RigidBodyFlags.Static) == RigidBodyFlags.None)
					{
						denominator = rigidBody0.inverseMass + Vector3.Dot(frictionDirection, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(point.RelativePosition0, frictionDirection), rigidBody0.inverseInertiaTensorWorld), point.RelativePosition0));
					}

					// Determine the magnitude of an impulse necessary ro remove all relative velocity at the collision position of
					// the second rigid body.  Apply it to the first rigid body's magnitude.
					if (rigidBody1 != null && (rigidBody1.Flags & RigidBodyFlags.Static) == RigidBodyFlags.None)
					{

						denominator += rigidBody1.inverseMass + Vector3.Dot(frictionDirection, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(point.RelativePosition1, frictionDirection), rigidBody1.inverseInertiaTensorWorld), point.RelativePosition1));
					}

					if (denominator > MathsHelper.Epsilon)
					{
						// Determine the magnitude of the impulse needed to apply friction in the direction
						// of the surface (tangent) of the collision position.
						float frictionImpulse = perpendicularSpeed / denominator;

						// Determine if dynamic friction comes into play.
						if (frictionImpulse >= this.staticFriction * normalImpulse)
						{
							frictionImpulse = this.dynamicFriction * normalImpulse;
						}

						// Determine the vector friction impulse.

						frictionDirection *= frictionImpulse;
						// Apply the friction impulse to the rigid bodies involved in this collision.
						if (rigidBody0 != null && (rigidBody0.Flags & RigidBodyFlags.NoResponse) == RigidBodyFlags.None)
						{
							rigidBody0.ApplyOffsetForceImpulse(ref frictionDirection, ref point.RelativePosition0);
						}
						if (rigidBody1 != null && (rigidBody1.Flags & RigidBodyFlags.NoResponse) == RigidBodyFlags.None)
						{
							rigidBody1.ApplyReverseOffsetForceImpulse(ref frictionDirection, ref point.RelativePosition1);
						}
					}
				}
			}

			// The rigid bodies need another go at resolution.
			if (result)
			{
				if (rigidBody0 != null)
				{
					rigidBody0.CollisionGroup.Collisions.Reset();
					rigidBody0.Constraints.Reset();
				}
				if (rigidBody1 != null)
				{
					rigidBody1.CollisionGroup.Collisions.Reset();
					rigidBody1.Constraints.Reset();
				}

				// Keep this collision satisfied.
				this.satisfied = true;
			}

			// Return status of resolution.
			return result;
		}

		/// <summary>
		/// Gets a <see cref="Collision" /> instance from the pool.
		/// </summary>
		/// <returns>
		/// Returns a fresh <see cref="Collision"/> instance.
		/// </returns>
		internal static Collision Retrieve()
		{
			// Get an unused collision from the pool.
			return Collision.pool.Retrieve();
		}

		/// <summary>
		/// Returns a <see cref="Collision"/> instance to the pool.
		/// </summary>
		/// <param name="collision">The <see cref="Collision"/> to recycle.</param>
		internal static void Recycle(Collision collision)
		{
			// Uninitialize.
			collision.Shape0 = null;
			collision.Shape1 = null;

			// Put the collision back in the pool for later use.
			Collision.pool.Recycle(collision);

			// Recycle all associated collision points as well.
			for (int i = 0; i < collision.Points.Count; i++)
			{
				CollisionPoint.Recycle(collision.Points[i]);
			}
			collision.Points.Clear();
		}
	}
}
