using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Physics.Collision.Shapes;

namespace GuruEngine.Physics.Collision.CollisionDetectors
{
	/// <summary>
	/// Detects collisions between a <see cref="CollisionSphere"/> and a <see cref="CollisionMesh"/>.
	/// </summary>
	public class SphereMeshCollisionDetector : CollisionDetector
	{
		private static List<CollisionMeshTriangle> triangles = new List<CollisionMeshTriangle>(32);

		#region CollisionDetector Members

		public override void Detect(CollisionShape shape0, CollisionShape shape1, CollisionContext context)
		{
			// Typecast the shapes to their correct subclasses.
			CollisionSphere sphere = (CollisionSphere)shape0;
			GuruEngine.Physics.Collision.Shapes.CollisionMesh mesh = (GuruEngine.Physics.Collision.Shapes.CollisionMesh)shape1;

			// Attempt to get all triangles that intersect with the sphere's axis-aligned bounding box.
			SphereMeshCollisionDetector.triangles.Clear();
			if (mesh.GetIntersectingTriangles(ref sphere.AABB, SphereMeshCollisionDetector.triangles))
			{
				// Get radius totals.
				float radiusTotal = sphere.Radius + context.Tolerance;
				float radiusSquaredTotal = radiusTotal * radiusTotal;

				// Transform the sphere's position into the mesh's object space.

				Vector3 spherePosition = Vector3.Transform(sphere.Position, mesh.inverseTransform);

				// Initialize collision manifold.
				CollisionPointCollection collisionPoints = CollisionPointCollection.Retrieve();
				Vector3 collisionNormal = Vector3.Zero;

				// Iterate on the potentially intersecting triangles.
				for (int i = 0; i < SphereMeshCollisionDetector.triangles.Count; i++)
				{
					// Get the current triangle.
					CollisionMeshTriangle triangle = SphereMeshCollisionDetector.triangles[i];

					// Calculate the distance to the triangle's plane.

					float distanceToPlane = Vector3.Dot(triangle.Normal, spherePosition) + triangle.distance;
					// Determine if the sphere is close enough to the plane of triangle for a collision.  Early out test.
					if (/*distanceToPlane <= 0.0f || */distanceToPlane >= radiusTotal)
					{
						continue;
					}

					// Get the closet point to the triangle.  This will be the collision position if
					// it's within the radius of the sphere.

					Vector3 collisionPosition = triangle.GetClosestPoint(spherePosition);
                    // Get the distance between the sphere center and the closest point to the triangle.

                    float distanceSquared = Vector3.DistanceSquared(spherePosition, collisionPosition);

					// Determine if the sphere is close enough to the triangle for a collision.
					if (distanceSquared < radiusSquaredTotal)
					{
						// Transform the collision position into world space.
						collisionPosition = Vector3.Transform(collisionPosition, mesh.transform);

						// Calculate the depth of the penetration due to the collision.
						float distance = (float)Math.Sqrt(distanceSquared);
						float collisionDepth = sphere.Radius - distance;
												
						// Determine the normal of the collision.
						Vector3 normal = triangle.Normal;

						// Create the collision point.
						CollisionPoint point = CollisionPoint.Retrieve();

						point.RelativePosition0 = collisionPosition - shape0.CollisionGroup.Position;
						point.RelativePosition1 = collisionPosition - shape1.CollisionGroup.Position;
						point.Depth = collisionDepth;

						// Add the collision point to the collection.
						collisionPoints.Add(point);

						// Combine the normal of this collision to the total.

						collisionNormal += normal;
					}
				}

				if (collisionPoints.Count > 0)
				{
                    collisionNormal.Normalize();


					// Transform the collision normal into world space.
					collisionNormal = Vector3.TransformNormal(collisionNormal, mesh.transform);

					// Notify subscribers of the collision.
					context.OnCollided(sphere, mesh, ref collisionNormal, collisionPoints);
				}

				// Recycle the collision point collection and any triangles found.
				CollisionPointCollection.Recycle(collisionPoints);
				mesh.ReleaseIntersectingTriangles(SphereMeshCollisionDetector.triangles);
			}
		}

		#endregion
	}
}