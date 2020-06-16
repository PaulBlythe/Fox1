using System.Threading;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Represents the behaviors and/or states of a <see cref="Constraint" />.
	/// </summary>
	public enum ConstraintFlags
	{
		/// <summary>
		/// No special behavior or state.
		/// </summary>
		None = 0,

		/// <summary>
		/// The <see cref="Constraint"/> is disabled.
		/// </summary>
		Disabled = 1,

		/// <summary>
		/// The <see cref="Constraint"/> is registered with the <see cref="IPhysicsService"/>.
		/// </summary>
		/// <remarks>
		/// Do not include this value manually.  It is managed by the <see cref="IPhysicsService"/>.
		/// </remarks>
		Registered = 2
	}

	/// <summary>
	/// Represents a constaint in the <see cref="IPhysicsService"/>.
	/// </summary>
	public abstract partial class Constraint
	{
		private static int instanceCount;
		internal int id;

		/// <summary>
		/// Gets or sets the behavior flags of the <see cref="Constraint"/>.
		/// </summary>
		public ConstraintFlags Flags;

		internal protected bool satisfied;

		/// <summary>
		/// Initializes a new instance of the <see cref="Constraint" /> class.
		/// </summary>
		protected Constraint()
		{
			// Assign the collision shape a unique id.
			this.id = Interlocked.Increment(ref Constraint.instanceCount);
		}

		/// <summary>
		/// Gets a unique id for the <see cref="Constraint"/>.
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
		/// Called when a <see cref="Constraint"/> is added to the <see cref="IPhysicsService"/>.
		/// </summary>
		internal protected virtual void OnAdded() 
		{ 
			// Enable the constraint.
			this.Flags |= ConstraintFlags.Registered;
		}

		/// <summary>
		/// Called when a <see cref="Constraint"/> is removed from the <see cref="IPhysicsService"/>.
		/// </summary>
		internal protected virtual void OnRemoved() 
		{ 
			// Disable the constraint.
			this.Flags &= ~ConstraintFlags.Registered;
		}

		/// <summary>
		/// Called before an attempt to solve the <see cref="Constraint"/> is made.
		/// </summary>
		/// <param name="elapsed">The simulation step's total elapsed time.</param>
		internal protected abstract void PreStep(float elapsed);

		/// <summary>
		/// Attempts to solve the <see cref="Constraint"/>.
		/// </summary>
		/// <param name="elapsed">The simulation step's total elapsed time.</param>
		/// <returns><b>true</b>, if the <see cref="Constraint"/> was enforced. <b>false</b>, otherwise.</returns>
		internal protected abstract bool Solve(float elapsed);
	}
}
