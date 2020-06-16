using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Defines the contract for a collision callback.
	/// </summary>
	public interface ICollisionCallback
	{
		/// <summary>
		/// Notified when a collision between two <see cref="CollisionShape"/> instances occurs.
		/// </summary>
		/// <param name="shape0">The first <see cref="CollisionShape"/> involved in the collision.</param>
		/// <param name="shape1">The second <see cref="CollisionShape"/> involved in the collision.</param>
		/// <param name="normal">The direction of the collisio pointing towards <paramref name="shape0"/>.</param>
		/// <param name="points">A collection of <see cref="CollisionPoint"/> instances involved in the collision.</param>
		/// <returns>
		/// Return <c>true</c>if this is a valid collision.  Otherwise, return <c>false</c>.
		/// </returns>
		bool OnCollided(CollisionShape shape0, CollisionShape shape1, ref Vector3 normal, IList<CollisionPoint> points);
	}
}
