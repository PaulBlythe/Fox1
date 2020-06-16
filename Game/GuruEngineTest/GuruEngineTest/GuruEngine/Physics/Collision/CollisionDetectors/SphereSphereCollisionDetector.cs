using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

using GuruEngine.Physics.Collision.Shapes;

namespace GuruEngine.Physics.Collision.CollisionDetectors
{
	/// <summary>
	/// Detects collisions between two <see cref="CollisionSphere"/> instances.
	/// </summary>
	public class SphereSphereCollisionDetector : CollisionDetector
	{
		#region CollisionDetector Members

		public override void Detect(CollisionShape shape0, CollisionShape shape1, CollisionContext context)
		{
			// Typecast the shapes to their correct subclasses.
			CollisionSphere sphere0 = (CollisionSphere)shape0;
			CollisionSphere sphere1 = (CollisionSphere)shape1;

			// Determine the vector between the spheres.

			Vector3 difference = sphere0.Position - sphere1.Position;

			// Determine the distance of the vector, squared.
			float distanceSquared = difference.LengthSquared();

			// Get radius totals.
			float radiusTotal = sphere0.Radius + sphere1.Radius;
			float radiusToleranceTotal = radiusTotal + context.Tolerance;

			// Determine if the spheres are close enough for a collision.
			if (distanceSquared >= radiusToleranceTotal * radiusToleranceTotal)
			{
				return;
			}

			// Get the distance between the two spheres' centers.
			float distance = (float)Math.Sqrt(distanceSquared);
			if (distance <= 0.0f)
			{
				return;
			}

			// Calculate the normal of the collision.

			difference /= distance;

			// Calculate the depth of the penetration due to the collision.
			float collisionDepth = radiusTotal - distance;
            
			// Calculate the position of the collision.
			Vector3 collisionPosition = sphere1.Position + difference * (sphere1.Radius - collisionDepth * 0.5f);

			// Create the collision point.
			CollisionPoint point = CollisionPoint.Retrieve();
			point.RelativePosition0 = collisionPosition - shape0.CollisionGroup.Position;
			point.RelativePosition1 = collisionPosition - shape1.CollisionGroup.Position;
			point.Depth = collisionDepth;

			// Add the collision point to the collection.
			CollisionPointCollection collisionPoints = CollisionPointCollection.Retrieve();
			collisionPoints.Add(point);

			// Notify subscribers of the collision.
			context.OnCollided(shape0, shape1, ref difference, collisionPoints);

			// Recycle the collision point collection.
			CollisionPointCollection.Recycle(collisionPoints);
		}

		#endregion
	}
}
