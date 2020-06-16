using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using GuruEngine.Physics.Collision.Shapes;

namespace GuruEngine.Physics.Collision.CollisionDetectors
{
	/// <summary>
	/// Detects collisions between a <see cref="CollisionBox"/> and a <see cref="CollisionSphere"/>.
	/// </summary>
	public class BoxSphereCollisionDetector : CollisionDetector
	{
		#region CollisionDetector Members

		public override void Detect(CollisionShape shape0, CollisionShape shape1, CollisionContext context)
		{
			// Typecast the shapes to their correct subclasses.
			CollisionBox box = (CollisionBox)shape0;
			CollisionSphere sphere = (CollisionSphere)shape1;

			// Get radius totals.
			float radiusTotal = sphere.Radius + context.Tolerance;

			// Transform the position of the sphere relative to the box.

			Vector3 position = Vector3.TransformNormal(sphere.Position - box.Position, Matrix.Transpose(box.Orientation));
			// Determine if a collision is possible with broad-phase check.
			if (Math.Abs(position.X) - radiusTotal > box.Extents.X ||
				Math.Abs(position.Y) - radiusTotal > box.Extents.Y ||
				Math.Abs(position.Z) - radiusTotal > box.Extents.Z)
			{
				return;
			}

			// Clamp each vector component of the sphere's relative position to the box's extents.
			Vector3 collisionPosition = new Vector3(
				MathHelper.Clamp(position.X, -box.Extents.X, box.Extents.X),
				MathHelper.Clamp(position.Y, -box.Extents.Y, box.Extents.Y),
				MathHelper.Clamp(position.Z, -box.Extents.Z, box.Extents.Z));

			// Determine the distance between the clamped position of sphere and sphere's relative position to the box, squared.

			float distanceSquared = Vector3.DistanceSquared(collisionPosition, position);

			// Determine if the sphere is close enough to the box for a collision.
			if (distanceSquared > radiusTotal * radiusTotal)
			{
				return;
			}

			// Transform the closest point to the box's local coordinates.

			collisionPosition = box.Position + Vector3.TransformNormal(collisionPosition, box.Orientation);
			// Determine the normal of the collision.
			Vector3 collisionNormal = collisionPosition - sphere.Position;
            collisionNormal.Normalize();

			// Calculate the depth of the penetration due to the collision.
			float collisionDepth = sphere.Radius - (float)Math.Sqrt(distanceSquared);

			// Create the collision point.
			CollisionPoint point = CollisionPoint.Retrieve();

			point.RelativePosition0 = collisionPosition - shape0.CollisionGroup.Position;
			point.RelativePosition1 = collisionPosition - shape1.CollisionGroup.Position;
			point.Depth = collisionDepth;

			// Add the collision point to the collection.
			CollisionPointCollection collisionPoints = CollisionPointCollection.Retrieve();
			collisionPoints.Add(point);

			// Notify subscribers of the collision.
			context.OnCollided(shape0, shape1, ref collisionNormal, collisionPoints);

			// Recycle the collision point collection.
			CollisionPointCollection.Recycle(collisionPoints);
		}

		#endregion
	}
}
