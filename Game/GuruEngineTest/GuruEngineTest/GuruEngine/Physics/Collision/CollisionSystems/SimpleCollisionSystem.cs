using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Core;


namespace GuruEngine.Physics.Collision.CollisionSystems
{
	/// <summary>
	/// Defines a brute force strategy for broad-phase collision processing.
	/// </summary>
	public class SimpleCollisionSystem : CollisionSystem
	{
		#region CollisionSystem Members

		public override void DetectCollisions(RigidBody rigidBody, CollisionContext context)
		{
			// Iterate on the collision groups.
			for (int i = 0; i < this.CollisionGroups.Count; i++)
			{
				// Detect collisions.
				this.DetectCollisions(rigidBody.CollisionGroup, this.CollisionGroups[i], context);
			}
		}

		#endregion
	}
}
