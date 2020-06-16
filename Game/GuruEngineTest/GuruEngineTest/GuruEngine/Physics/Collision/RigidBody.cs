//#define ASSERT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Helpers;
using GuruEngine.DebugHelpers;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Represents the behaviors and/or states of a <see cref="RigidBody />.
	/// </summary>
	[Flags()]
	public enum RigidBodyFlags
	{
		/// <summary>
		/// No special behavior or state.
		/// </summary>
		None = 0,

		/// <summary>
		/// The <see cref="RigidBody"/> is disabled.
		/// </summary>
		Disabled = 1,

		/// <summary>
		/// The <see cref="RigidBody"/> can be auto disabled.
		/// </summary>
		AutoDisable = 2,

		/// <summary>
		/// The <see cref="RigidBody"/> is immovable.
		/// </summary>
		Static = 4,

		/// <summary>
		/// The <see cref="RigidBody" /> is unaffected by collisions.
		/// </summary>
		NoResponse = 8
	}

	/// <summary>
	/// Represents a single solid body of finite size.
	/// </summary>
	public class RigidBody
	{
		#region ConstraintCollection Declaration

		// Is this the correct association???  Will CollisionGroups ever need an association???
		/// <summary>
		/// A collection of <see cref="Constaint"/> instances.
		/// </summary>
		public sealed class ConstraintCollection : ReadOnlyCollection<Constraint>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ConstraintCollection"/> class.
			/// </summary>
			internal ConstraintCollection() : base(new List<Constraint>()) { }

			/// <summary>
			/// Exposes the <see cref="System.Collections.Generic.IList[T]" /> that the <see cref="ConstraintCollection"/> wraps.
			/// </summary>
			internal new IList<Constraint> Items
			{
				get { return base.Items; }
			}

			/// <summary>
			/// Marks all constraints as unsolved.
			/// </summary>
			internal void Reset()
			{
				for (int i = 0; i < this.Count; i++)
				{
					this.Items[i].satisfied = false;
				}
			}
		}

		#endregion

		private static int instanceCount = 0;
		private int id;

		/// <summary>
		/// Gets or sets the behavior flags of the <see cref="RigidBody"/>.
		/// </summary>
		public RigidBodyFlags Flags = RigidBodyFlags.AutoDisable;

		private CollisionGroup collisionGroup;
		private ConstraintCollection constraints = new ConstraintCollection();

		private float mass = 1.0f;
		internal float inverseMass = 1.0f;
        public Vector3 CofG;

		private Matrix inertiaTensor;
		private Matrix inverseInertiaTensor;
		internal Matrix inverseInertiaTensorWorld;
        public Vector3 currentAcceleration;

        /// <summary>
        /// Gets the location of the <see cref="RigidBody"/> in world coordinates.
        /// </summary>
        /// <remarks>
        /// Do not set this field directly.  Instead, use the <see cref="RigidBody.UpdateTransform" /> method.
        /// </remarks>
        public Vector3 Position;

		/// <summary>
		/// Gets the orientation of the <see cref="RigidBody"/> in world coordinates.
		/// </summary>
		/// <remarks>
		/// Do not set this field directly.  Instead, use the <see cref="RigidBody.UpdateTransform" /> method.
		/// </remarks>

		public Matrix Orientation = Matrix.Identity;

		/// <summary>
		/// Gets or sets the linear velocity of the <see cref="RigidBody"/> in world coordinates.
		/// </summary>
		/// <remarks>
		/// The velocity of the <see cref="RigidBody"/> is normally adjusted through integration and application of forces/impulses.
		/// A better but indirect approach to changing the velocity is applying a force or impulse using the exposed methods
		/// of the <see cref="RigidBody" /> instance.
		/// </remarks>
		public Vector3 Velocity;

		/// <summary>
		/// Gets or sets the angular velocity of the <see cref="RigidBody"/> in world coordinates.
		/// </summary>
		/// <remarks>
		/// The rotation of the rigid body is normally adjusted through integration and application of forces/impulses.
		/// A better but indirect approach to changing the rotation is applying torque or angular impulse using the exposed methods
		/// of the <see cref="RigidBody" /> instance.
		/// </remarks>
		public Vector3 Rotation;

		/// <summary>
		/// Gets or sets the instantaneous force of the <see cref="RigidBody"/> in world coordinates.
		/// </summary>
		public Vector3 Force;

		/// <summary>
		/// Gets or sets the instantaneous torque, i.e. angular force, of the <see cref="RigidBody"/> in world coordinates.
		/// </summary>
		public Vector3 Torque;

		/// <summary>
		/// Gets or sets the linear force applied at every time frame to the <see cref="RigidBody"/>.
		/// </summary>
		/// <remarks>
		/// Normally, this vector represents the force due to gravity in units of measurement per second.
		/// </remarks>
		public Vector3 Acceleration = Vector3.Down * 9.810f;

		/// <summary>
		/// Gets or sets the linear damping factor to apply to the <see cref="RigidBody"/> during integration.
		/// </summary>
		public float LinearDamping;

		/// <summary>
		/// Gets or sets the angular damping factor to apply to the <see cref="RigidBody"/> during integration.
		/// </summary>
		public float AngularDamping;

		private float inactiveTime;

		private float linearMotionThreshold;
		private float linearMotionThresholdSquared;
		private float angularMotionThreshold;
		private float angularMotionThresholdSquared;

		// TODO : Not used yet.
		private float velocityLimit;
		private float velocityLimitSquared;
		private float rotationLimit;
		private float rotationLimitSquared;

        public bool dirty = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="RigidBody" /> class.
		/// </summary>
		public RigidBody()
		{
			// Assign the rigid body a unique id.
			this.id = Interlocked.Increment(ref RigidBody.instanceCount);

			// Create the associated collision group collection.
			this.collisionGroup = new CollisionGroup(this);

			// Default some properties.
			this.DeactivationTime = 1.25f;
			this.LinearMotionThreshold = 0.6f;
			this.AngularMotionThreshold = (MathHelper.Pi / 8.0f);
		}

		/// <summary>
		/// Gets a unique id for the <see cref="RigidBody" />.
		/// </summary>
		public int Id
		{
			get { return this.id; }
		}

		/// <summary>
		/// Gets or sets the object that contains data about the <see cref="RigidBody"/>.
		/// </summary>
		[ContentSerializerIgnore()]
		public object Tag { get; set; }

		/// <summary>
		/// Gets or sets the total mass of the <see cref="RigidBody" />.
		/// </summary>
		/// <remarks>
		/// The mass of a <see cref="RigidBody" /> is calculated from the defined density, 
		/// associated <see cref="CollisionGroup" /> and it's collision shapes.
		/// </remarks>
		public float Mass
		{
			get { return this.mass; }
			set
			{
				// Hold onto value.
				this.mass = value;

				// Hold onto the mass's inverse.  Most calculation involving mass use the inverse.
				this.inverseMass = 1.0f / this.mass;
			}
		}

		/// <summary>
		/// Gets or sets the combined moment of inertia of the <see cref="RigidBody" />.
		/// </summary>
		/// <remarks>
		/// The inertia tensor of a <see cref="RigidBody" /> is calculated from the mass, 
		/// associated <see cref="CollisionGroup" /> and it's collision shapes.
		/// </remarks>
		public Matrix InertiaTensor
		{
			get { return this.inertiaTensor; }
			set
			{
				// Hold onto value.
				this.inertiaTensor = value;

				// Hold onto the inertia tensor's inverse.  Most calculation involving moment of inertia use the inverse.
				if (this.inertiaTensor == MathsHelper.Zero)
				{
					this.inverseInertiaTensor = MathsHelper.Zero;
				}
				else if (this.inertiaTensor.M11 == 0.0f || this.inertiaTensor.M22 == 0.0f || this.inertiaTensor.M33 == 0.0f)
				{
					this.inverseInertiaTensor.M11 = this.inertiaTensor.M11 == 0.0f ? this.inertiaTensor.M11 : 1.0f / this.inertiaTensor.M11;
					this.inverseInertiaTensor.M22 = this.inertiaTensor.M22 == 0.0f ? this.inertiaTensor.M22 : 1.0f / this.inertiaTensor.M22;
					this.inverseInertiaTensor.M33 = this.inertiaTensor.M33 == 0.0f ? this.inertiaTensor.M33 : 1.0f / this.inertiaTensor.M33;
				}
				else
				{
                    MathsHelper.Invert33(ref this.inertiaTensor, out this.inverseInertiaTensor);
				}
			}
		}

		/// <summary>
		/// Gets the associated <see cref="CollisionGroup" /> of the <see cref="RigidBody" />.
		/// </summary>
		public CollisionGroup CollisionGroup
		{
			get { return this.collisionGroup; }
		}

		/// <summary>
		/// Gets the associated <see cref="Constraint" /> instances of the <see cref="RigidBody" />.
		/// </summary>
		/// <remarks>
		/// Constraints are associated with an active <see cref="RigidBody"/> when added to 
		/// the <see cref="IPhysicsService.Constraints"/> collection.
		/// </remarks>
		[ContentSerializerIgnore()]
		public ConstraintCollection Constraints
		{
			get { return this.constraints; }
		}

		/// <summary>
		/// Gets or sets the time the <see cref="RigidBody" /> must remain under the specified movement thresholds in order
		/// to be considered for deactivation.
		/// </summary>
		/// <remarks>
		/// The value of the this property is considered only when the <see cref="RigidBody.Flags"/> field 
		/// contains <see cref="RigidBodyFlags.AutoDisable"/>.  The value represents seconds.
		/// </remarks>
		public float DeactivationTime { get; set; }

		/// <summary>
		/// Gets or sets the threshold for linear motion of the <see cref="RigidBody" />.
		/// </summary>
		/// <remarks>
		/// When motion of the <see cref="RigidBody"/> remains under this and the <see cref="RigidBody.AngularMotionThreshold"/> thresholds
		/// for the specified <see cref="RigidBody.DeactivationTime"/>, the <see cref="RigidBody"/>
		/// will be deactivated.
		/// </remarks>
		public float LinearMotionThreshold
		{
			get { return this.linearMotionThreshold; }
			set
			{
				// Hold onto the value.
				this.linearMotionThreshold = value;

				// Hold onto the square of the motion threshold.
				this.linearMotionThresholdSquared = value * value;
			}
		}

		/// <summary>
		/// Gets or sets the threshold for angular motion of the <see cref="RigidBody" />.
		/// </summary>
		/// <remarks>
		/// When motion of the <see cref="RigidBody"/> remains under this and the <see cref="RigidBody.LinearMotionThreshold"/> thresholds
		/// for the specified <see cref="RigidBody.DeactivationTime"/>, the <see cref="RigidBody"/>
		/// will be deactivated.
		/// </remarks>
		public float AngularMotionThreshold
		{
			get { return this.angularMotionThreshold; }
			set
			{
				// Hold onto the value.
				this.angularMotionThreshold = value;

				// Hold onto the square of the motion threshold.
				this.angularMotionThresholdSquared = value * value;
			}
		}

		/// <summary>
		/// Gets or sets the limit for velocity of the <see cref="RigidBody" />.
		/// </summary>
		public float VelocityLimit
		{
			get { return this.velocityLimit; }
			set
			{
				// Hold onto the value.
				this.velocityLimit = value;

				// Hold onto the square of the limit.
				this.velocityLimitSquared = value * value;
			}
		}

		/// <summary>
		/// Gets or sets the limit for rotation of the <see cref="RigidBody" />.
		/// </summary>
		public float RotationLimit
		{
			get { return this.rotationLimit; }
			set
			{
				// Hold onto the value.
				this.rotationLimit = value;

				// Hold onto the square of the limit.
				this.rotationLimitSquared = value * value;
			}
		}

		/// <summary>
		/// Integrates the velocities from the forces applied to the <see cref="RigidBody" />.
		/// </summary>
		/// <param name="elapsed">The simulation step's total elapsed time, in seconds.</param>
		public void IntegrateVelocity(float elapsed)
		{
			this.Assert();

			// Make sure the rigid body is enabled and not static.
			if ((this.Flags & (RigidBodyFlags.Disabled | RigidBodyFlags.Static)) != RigidBodyFlags.None)
			{
				// Exit.
				return;
			}

            if (elapsed != 0)
            {
                // Calculate the current linear acceleration from the applied forces and constant acceleration (gravity)
                // during this iteration.  Hold onto this value for collision resolution calculations.
                currentAcceleration = (this.Acceleration + this.Force * this.inverseMass) * elapsed;

                //System.Console.WriteLine(currentAcceleration.ToString() + "   force = " + Force.ToString());
                // Calculate the current angular acceleration from the applied torque during this iteration.
                Vector3 angularAcceleration = Vector3.TransformNormal(this.Torque * elapsed, this.inverseInertiaTensorWorld);

                // Update the linear and angular velocities from linear and angular accelerations, respectively.
                this.Velocity += currentAcceleration;
                this.Rotation += angularAcceleration;

                this.Assert();

                // Adjust the linear and angular velocities by the linear and angular dampings, respectively.
                this.Velocity *= MathHelper.Clamp(1.0f - this.LinearDamping * elapsed, 0.0f, 1.0f);
                this.Rotation *= MathHelper.Clamp(1.0f - this.AngularDamping * elapsed, 0.0f, 1.0f);



                this.Assert();
            }
		}

		/// <summary>
		/// Integrates the transformation from the velocities applied to the <see cref="RigidBody" />.
		/// </summary>
		/// <param name="elapsed">The simulation step's total elapsed time, in seconds.</param>
		public void IntegratePosition(float elapsed)
		{
			// Make sure the rigid body is enabled and not static.
			if ((this.Flags & (RigidBodyFlags.Disabled | RigidBodyFlags.Static)) != RigidBodyFlags.None)
			{
				// Exit.
				return;
			}

			// Update the position by the velocity.
			this.Position += this.Velocity * elapsed;

			// Update the orientation by the rotation.
			float length = this.Rotation.Length();
			if (length > MathsHelper.Epsilon)
			{
				this.Orientation *= Matrix.CreateFromAxisAngle(this.Rotation / length, length * elapsed);
                MathsHelper.Orthonormalize(ref this.Orientation);
			}

			// Calculate the inertia tensor in world coordinates.
			this.inverseInertiaTensorWorld = MathsHelper.ConvertBasis33(this.inverseInertiaTensor, this.Orientation);

			this.Assert();

			// Calculate all associated collision shape's internal data.
			this.collisionGroup.UpdateTransform(ref this.Position, ref this.Orientation);
		}

		/// <summary>
		/// Updates the <see cref="RigidBody" /> to the specified transformation.
		/// </summary>
		/// <remarks>
		/// Use the <see cref="RigidBody.UpdateTransform"/> method instead of setting the <see cref="RigidBody.Position" /> field or
		/// <see cref="RigidBody.Orientation"/> explicitly.
		/// </remarks>
		public void UpdateTransform()
		{
			// Call overloaded method.
			this.UpdateTransform(ref this.Position, ref this.Orientation);
		}

		/// <summary>
		/// Updates the <see cref="RigidBody" /> to the specified transformation.
		/// </summary>
		/// <param name="position">The location in world coordinates to where the <see cref="RigidBody"/> should be placed.</param>
		/// <remarks>
		/// Use the <see cref="RigidBody.UpdateTransform"/> method instead of setting the <see cref="RigidBody.Position" /> fields explicitly.
		/// </remarks>
		public void UpdateTransform(ref Vector3 position)
		{
			// Call overloaded method.
			this.UpdateTransform(ref position, ref this.Orientation);
		}

		/// <summary>
		/// Updates the <see cref="RigidBody" /> to the specified transformation.
		/// </summary>
		/// <param name="orientation">The orientation in world coordinates to place the <see cref="RigidBody"/>.</param>
		/// <remarks>
		/// Use the <see cref="RigidBody.UpdateTransform"/> method instead of setting the <see cref="RigidBody.Orientation" /> fields explicitly.
		/// </remarks>

		public void UpdateTransform(ref Matrix orientation)

		{
			// Call overloaded method.
			this.UpdateTransform(ref this.Position, ref orientation);
		}

		/// <summary>
		/// Updates the <see cref="RigidBody" /> to the specified transformation.
		/// </summary>
		/// <param name="position">The location in world coordinates to where the <see cref="RigidBody"/> should be placed.</param>
		/// <param name="orientation">The orientation in world coordinates to place the <see cref="RigidBody"/>.</param>
		/// <remarks>
		/// Use the <see cref="RigidBody.UpdateTransform"/> method instead of setting the <see cref="RigidBody.Position" /> and 
		/// <see cref="RigidBody.Orientation"/> fields explicitly.
		/// </remarks>

		public void UpdateTransform(ref Vector3 position, ref Matrix orientation)
		{
			// Make sure the rigid body is active.
			this.Activate();

			// Modify the current transformation of the rigid body.
			this.Position = position;
			this.Orientation = orientation;

			this.Assert();

			// Move all associated collision shapes as well.
			this.collisionGroup.UpdateTransform(ref this.Position, ref this.Orientation);
		}

		/// <summary>
		/// Activates the <see cref="RigidBody" /> from a disabled state.
		/// </summary>
		public void Activate()
		{
			if ((this.Flags & RigidBodyFlags.Disabled) == RigidBodyFlags.Disabled)
			{
				this.Flags &= ~RigidBodyFlags.Disabled;
				this.inactiveTime = 0.0f;
			}
		}

		/// <summary>
		/// Deactivates the <see cref="RigidBody" /> from an enabled state.
		/// </summary>
		public void Deactivate()
		{
			if ((this.Flags & RigidBodyFlags.Disabled) == RigidBodyFlags.None)
			{
				this.Flags |= RigidBodyFlags.Disabled;
				this.inactiveTime = this.DeactivationTime;

				// Clear the velocities as well.
				this.Velocity = Vector3.Zero;
				this.Rotation = Vector3.Zero;
			}
		}

		/// <summary>
		/// Updates the inactivity time based on movement and deactivates the <see cref="RigidBody"/> if necessary.
		/// </summary>
		/// <param name="elapsed">The simulation step's total elapsed time.</param>
		internal void AttemptDeactivation(float elapsed)
		{
			if ((this.Flags & RigidBodyFlags.AutoDisable) == RigidBodyFlags.AutoDisable)
			{
				// Determine if motion is under tolerances.

				if (this.Velocity.LengthSquared() > this.linearMotionThresholdSquared || this.Rotation.LengthSquared() > this.angularMotionThresholdSquared)

				{
					this.inactiveTime = 0.0f;
				}
				else
				{
					this.inactiveTime += elapsed;
				}

				// Check if the rigid body should be deactivated.
				if (this.inactiveTime >= this.DeactivationTime)
				{
					this.Deactivate();
				}
			}
		}

		/// <summary>
		/// Clears the applied force and torque of the <see cref="RigidBody" />.
		/// </summary>
		public void ClearForces()
		{
			// Clear the force and torque.  Normally happens after each integration but manual reasons exist.
			this.Force = Vector3.Zero;
			this.Torque = Vector3.Zero;
		}

		/// <summary>
		/// Applies a force in local coordinates to the <see cref="RigidBody" /> at the <see cref="RigidBody.Position"/>.
		/// </summary>
		/// <param name="force">The force, in local-coordinates to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyLocalForce(Vector3 force)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Add the specified force to the total.

			this.Force += Vector3.TransformNormal(force, this.Orientation);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies a force in local coordinates to the <see cref="RigidBody" /> at the <see cref="RigidBody.Position"/>.
		/// </summary>
		/// <param name="force">The force, in local-coordinates to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyLocalForce(ref Vector3 force)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Add the specified force to the total.

			this.Force += Vector3.TransformNormal(force, this.Orientation);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}
        /// <summary>
		/// Applies a force in local coordinates to the <see cref="RigidBody" /> at the <see cref="RigidBody.Position"/>.
		/// </summary>
		/// <param name="force">The force, in local-coordinates to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyLocalForce( Vector3 force, Vector3 position)
        {
            // Make sure the rigid body is not static.
            if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
            {
                return;
            }

            // Add the specified force to the total.
            Vector3 worldforce = Vector3.TransformNormal(force, this.Orientation);
            this.Force += worldforce;
            this.Torque += Vector3.Cross(position - CofG, worldforce);
            this.Assert();

            // Enable the rigid body.
            this.Activate();
        }

        /// <summary>
        /// Applies a force to the <see cref="RigidBody" /> at the <see cref="RigidBody.Position"/>.
        /// </summary>
        /// <param name="force">The force, in world-coordinates to apply to the <see cref="RigidBody"/>.</param>
        public void ApplyWorldForce(Vector3 force)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Add the specified force to the total.

			this.Force += force;
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies a force to the <see cref="RigidBody" /> at the <see cref="RigidBody.Position"/>.
		/// </summary>
		/// <param name="force">The force, in world-coordinates to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyWorldForce(ref Vector3 force)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Add the specified force to the total.

			this.Force += force;
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies a force to the <see cref="RigidBody" /> at the specified location in world-coordinates.
		/// </summary>
		/// <param name="force">The force, in world-coordinates to apply to the <see cref="RigidBody"/>.</param>
		/// <param name="position">The location, in local coordinates, to apply the force.</param>
		public void ApplyWorldForce(Vector3 force, Vector3 position)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

            // Apply the specified force at the specified location to the total.
			this.Force += force;
			this.Torque += Vector3.Cross(position - CofG, force);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies a force to the <see cref="RigidBody" /> at the specified location in world-coordinates.
		/// </summary>
		/// <param name="force">The force, in world-coordinates to apply to the <see cref="RigidBody"/>.</param>
		/// <param name="position">The location, in world-coordinates, to apply the force.</param>
		public void ApplyWorldForce(ref Vector3 force, ref Vector3 position)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Apply the specified force at the specified location to the total.

			this.Force += force;
			this.Torque += Vector3.Cross(position - CofG, force);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies additional torque to the <see cref="RigidBody" /> in world coordinates.
		/// </summary>
		/// <param name="torque">The torque to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyWorldTorque(Vector3 torque)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Add the specified torque to the total.

			this.Torque += torque;
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies additional torque to the <see cref="RigidBody" /> in world coordinates.
		/// </summary>
		/// <param name="torque">The torque to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyWorldTorque(ref Vector3 torque)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Add the specified torque to the total.

			this.Torque += torque;
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies an impulse to the <see cref="RigidBody" /> at the specified location in world-coordinates.
		/// </summary>
		/// <param name="impulse">The impulse, in world-coordinates, to apply to the <see cref="RigidBody"/>.</param>
		/// <param name="position">The location, in world-coordinates, to apply the impulse.</param>
		public void ApplyForceImpulse(Vector3 impulse, Vector3 position)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Adjust the velocities by the specified impulse.

			this.Velocity += impulse * this.inverseMass;
			this.Rotation += Vector3.TransformNormal(Vector3.Cross(position - this.Position, impulse), this.inverseInertiaTensorWorld);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies an impulse to the <see cref="RigidBody" /> at the <see cref="RigidBody.Position"/>.
		/// </summary>
		/// <param name="impulse">The impulse, in world-coordinates, to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyForceImpulse(Vector3 impulse)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Adjust the velocity by the specified impulse.

			this.Velocity += impulse * this.inverseMass;
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies an impulse to the <see cref="RigidBody" /> at the <see cref="RigidBody.Position"/>.
		/// </summary>
		/// <param name="impulse">The impulse, in world-coordinates, to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyForceImpulse(ref Vector3 impulse)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Adjust the velocity by the specified impulse.

			this.Velocity += impulse * this.inverseMass;
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies an impulse to the <see cref="RigidBody" /> at the specified location in world-coordinates.
		/// </summary>
		/// <param name="impulse">The impulse, in world-coordinates, to apply to the <see cref="RigidBody"/>.</param>
		/// <param name="position">The location, in world-coordinates, to apply the impulse.</param>
		public void ApplyForceImpulse(ref Vector3 impulse, ref Vector3 position)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Adjust the velocities by the specified impulse.

			this.Velocity += impulse * this.inverseMass;
			this.Rotation += Vector3.TransformNormal(Vector3.Cross(position - this.Position, impulse), this.inverseInertiaTensorWorld);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies impulsive torque to the <see cref="RigidBody"/> in world-coordinates.
		/// </summary>
		/// <param name="torque">The impulse, in world-coordinates, to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyTorqueImpulse(Vector3 torque)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Adjust the rotation by the specified impulse.

			this.Rotation += Vector3.TransformNormal(torque, this.inverseInertiaTensorWorld);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies impulsive torque to the <see cref="RigidBody"/> in world-coordinates.
		/// </summary>
		/// <param name="torque">The impulse, in world-coordinates, to apply to the <see cref="RigidBody"/>.</param>
		public void ApplyTorqueImpulse(ref Vector3 torque)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Adjust the rotation by the specified impulse.

			this.Rotation += Vector3.TransformNormal(torque, this.inverseInertiaTensorWorld);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Applies an impulse to the <see cref="RigidBody" /> at the specified offset from the position
		/// of the rigid body in world-coordinates.
		/// </summary>
		/// <param name="impulse">The impulse to apply to the <see cref="RigidBody"/>.</param>
		/// <param name="offset">The vector, in world-coordinates, from the the <see cref="RigidBody.Position"/>.</param>
		public void ApplyOffsetForceImpulse(ref Vector3 impulse, ref Vector3 offset)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Adjust the velocities by the specified impulse.

			this.Velocity += impulse * this.inverseMass;
			this.Rotation += Vector3.TransformNormal(Vector3.Cross(offset, impulse), this.inverseInertiaTensorWorld);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		// TODO : Remove or make internal, if able.  Seems too specialized.  I thought I was being clever until this.
		public void ApplyReverseOffsetForceImpulse(ref Vector3 impulse, ref Vector3 offset)
		{
			// Make sure the rigid body is not static.
			if ((this.Flags & RigidBodyFlags.Static) == RigidBodyFlags.Static)
			{
				return;
			}

			// Adjust the velocities by the specified impulse.

			this.Velocity -= impulse * this.inverseMass;
			this.Rotation += Vector3.TransformNormal(Vector3.Cross(impulse, offset), this.inverseInertiaTensorWorld);
			this.Assert();

			// Enable the rigid body.
			this.Activate();
		}

		/// <summary>
		/// Determines the mass and intertia tensor based on the specified density and associated <see cref="CollisionGroup"/>.
		/// </summary>
		/// <param name="density">The density of the <see cref="RigidBody"/>.</param>
		public void CalculateMassProperties(float density)
		{
			// Get the mass properties from the associated collision group.
			float mass;
			Matrix inertiaTensor;
			this.collisionGroup.CalculateMassProperties(density, out mass, out inertiaTensor);

			// Update the mass and interia tensor.
			this.Mass = mass;
			this.InertiaTensor = inertiaTensor;

			// Calculate the inertia tensor in world coordinates.

			this.inverseInertiaTensorWorld = MathsHelper.ConvertBasis33(this.inverseInertiaTensor, this.Orientation);
		}

        List<MassBlob> blobs = new List<MassBlob>();

        /// <summary>
        /// Calculate CofG and inertia tensor
        /// </summary>
        public void CalculateMassProperties()
        {
            inertiaTensor = MathsHelper.Zero;
            Mass = 0;
            Vector3 m = new Vector3(0, 0, 0);

            foreach (MassBlob b in blobs)
            {
                Mass += b.Mass;
                inertiaTensor += b.GetTensor();

                m += b.Mass * b.Position;
            }
            MathsHelper.Invert33(ref this.inertiaTensor, out this.inverseInertiaTensor);
            inverseInertiaTensorWorld = MathsHelper.ConvertBasis33(inverseInertiaTensor, Orientation);

            CofG = m / Mass;

            dirty = false;
        }

        public int AddPointMass(float mass, Vector3 position)
        {
            blobs.Add(new MassBlob(mass, position));
            dirty = true;
            return blobs.Count - 1;
        }

        /// <summary>
        /// Allows update of dynamic masses 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="m"></param>
        public void UpdatePointMass(int i, float m)
        {
            if (blobs[i].Mass != m)
            {
                blobs[i].Mass = m;
                dirty = true;
            }
        }
	}

	internal static class RigidBodyExtension
	{
		[System.Diagnostics.Conditional("ASSERT")]
		internal static void Assert(this RigidBody source)
		{
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Position.X));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Position.Y));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Position.Z));

			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M11));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M12));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M13));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M14));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M21));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M22));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M23));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M24));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M31));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M32));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M33));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M34));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M41));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M42));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M43));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Orientation.M44));

			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Velocity.X));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Velocity.Y));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Velocity.Z));

			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Rotation.X));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Rotation.Y));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Rotation.Z));

			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Force.X));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Force.Y));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Force.Z));

			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Torque.X));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Torque.Y));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.Torque.Z));

			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M11));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M12));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M13));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M14));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M21));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M22));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M23));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M24));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M31));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M32));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M33));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M34));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M41));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M42));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M43));
			System.Diagnostics.Debug.Assert(!float.IsNaN(source.inverseInertiaTensorWorld.M44));
		}
	}
}
