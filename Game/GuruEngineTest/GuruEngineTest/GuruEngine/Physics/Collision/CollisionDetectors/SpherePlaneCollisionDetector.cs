using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

using GuruEngine.Physics.Collision.Shapes;

namespace GuruEngine.Physics.Collision.CollisionDetectors
{
	/// <summary>
	/// Detects collisions between a <see cref="CollisionSphere"/> and a <see cref="CollisionPlane"/>.
	/// </summary>
	public class SpherePlaneCollisionDetector : CollisionDetector
	{
		#region CollisionDetector Members

		public override void Detect(CollisionShape shape0, CollisionShape shape1, CollisionContext context)
		{
			// Typecast the shapes to their correct subclasses.
			CollisionSphere sphere = (CollisionSphere)shape0;
			CollisionPlane plane = (CollisionPlane)shape1;

			// Calculate the distance between the sphere's center and the plane.

			float distance = Vector3.Dot(plane.Normal, sphere.Position) + plane.Distance;
			// Determine if the sphere is close enough to the plane for a collision.
			if (distance > sphere.Radius + context.Tolerance)
			{
				return;
			}

			// Calculate the depth of the penetration due to the collision.
			float collisionDepth = sphere.Radius - distance;

			// Calculate the position of the collision.

			Vector3 collisionPosition = sphere.Position - plane.Normal * sphere.Radius;
			// Create the collision point.
			CollisionPoint point = CollisionPoint.Retrieve();

			point.RelativePosition0 = collisionPosition - shape0.CollisionGroup.Position;
			point.RelativePosition1 = collisionPosition - shape1.CollisionGroup.Position;
			point.Depth = collisionDepth;

			// Add the collision point to the collection.
			CollisionPointCollection collisionPoints = CollisionPointCollection.Retrieve();
			collisionPoints.Add(point);

			// Notify subscribers of the collision.
			context.OnCollided(shape0, shape1, ref plane.Normal, collisionPoints);

			// Recycle the collision point collection.
			CollisionPointCollection.Recycle(collisionPoints);
		}

		#endregion
	}
}
