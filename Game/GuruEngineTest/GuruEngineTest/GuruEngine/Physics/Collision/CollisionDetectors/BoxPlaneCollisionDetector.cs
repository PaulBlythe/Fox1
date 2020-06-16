using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using GuruEngine.Physics.Collision.Shapes;

namespace GuruEngine.Physics.Collision.CollisionDetectors
{
	/// <summary>
	/// Detects collisions between a <see cref="CollisionBox"/> and a <see cref="CollisionPlane"/>.
	/// </summary>
	public class BoxPlaneCollisionDetector : CollisionDetector
	{
		#region CollisionDetector Members

		public override void Detect(CollisionShape shape0, CollisionShape shape1, CollisionContext context)
		{
			// Typecast the shapes to their correct subclasses.
			CollisionBox box = (CollisionBox)shape0;
			CollisionPlane plane = (CollisionPlane)shape1;

			// Get the distance between the center of the box and the plane.

			float distance = Vector3.Dot(plane.Normal, box.Position) + plane.Distance;
			// Determine if the box is close enough to the plane for a collision.  This is an early out check.
			if (distance > box.Extents.Length() + context.Tolerance)
			{
				return;
			}

			// Initialize collision manifold.
			CollisionPointCollection collisionPoints = CollisionPointCollection.Retrieve();

			// Iterate on all the box's corners.
			for (int i = 0; i < box.Corners.Length; i++)
			{
				// Calculate the potential depth of the penetration of the corner of the box and the plane.

				float collisionDepth = -(Vector3.Dot(plane.Normal,  box.Corners[i]) + plane.Distance);
				// Determine if the corner of the box is close enough to the plane for a collision
				if (collisionDepth > -context.Tolerance)
				{
					// Create the collision point.
					CollisionPoint point = CollisionPoint.Retrieve();

					point.RelativePosition0 = box.Corners[i] - shape0.CollisionGroup.Position;
					point.RelativePosition1 = box.Corners[i] - shape1.CollisionGroup.Position;
					point.Depth = collisionDepth;

					// Add the collision point to the collection.
					collisionPoints.Add(point);

					// There can only be at most four collision points.
					if (collisionPoints.Count >= 4)
					{
						break;
					}
				}
			}

			// Notify subscribers of the collision.
			if (collisionPoints.Count > 0)
			{
				context.OnCollided(shape0, shape1, ref plane.Normal, collisionPoints);
			}

			// Recycle the collision point collection.
			CollisionPointCollection.Recycle(collisionPoints);
		}

		#endregion
	}
}

