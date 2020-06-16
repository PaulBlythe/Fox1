using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;
using GuruEngine.Physics.Collision.Shapes;

namespace GuruEngine.Physics.Collision.CollisionDetectors
{
	/// <summary>
	/// Detects collisions between a <see cref="CollisionBox"/> and a <see cref="CollisionMesh"/>.
	/// </summary>
	public class BoxMeshCollisionDetector : CollisionDetector
	{
		private static List<CollisionMeshTriangle> triangles = new List<CollisionMeshTriangle>(32);
		private static List<Vector3> intersectionPoints = new List<Vector3>(16);

		private CollisionBox transformedBox = new CollisionBox();

		#region CollisionDetector Members

		public override void Detect(CollisionShape shape0, CollisionShape shape1, CollisionContext context)
		{
			// Typecast the shapes to their correct subclasses.
			CollisionBox box = (CollisionBox)shape0;
			GuruEngine.Physics.Collision.Shapes.CollisionMesh mesh = (GuruEngine.Physics.Collision.Shapes.CollisionMesh)shape1;

			// Attempt to get all triangles that intersect with the box's axis-aligned bounding box.
			BoxMeshCollisionDetector.triangles.Clear();
			if (mesh.GetIntersectingTriangles(ref box.AABB, BoxMeshCollisionDetector.triangles))
			{
				// Get the radius of a sphere that encompasses the entire box.
				float radiusTotal = box.Extents.Length() + context.Tolerance;

				// Transform the box's position into the mesh's object space.

				Vector3 boxPosition = Vector3.Transform(box.Position, mesh.inverseTransform);

				// TODO : This has not been optimized yet.
				// Transform the box into the mesh's object space.

				Matrix inverseOrientation = Matrix.Transpose(mesh.Orientation);
				this.transformedBox.Position = Vector3.TransformNormal(box.Position - mesh.Position, inverseOrientation);
				this.transformedBox.Orientation = box.Orientation * inverseOrientation;
				this.transformedBox.Extents = box.Extents;
				this.transformedBox.CalculateInternals();

				// Initialize collision manifold.
				CollisionPointCollection collisionPoints = CollisionPointCollection.Retrieve();
				Vector3 collisionNormal = Vector3.Zero;
				float collisionDepth = float.MaxValue;//0.0f;

				// Clear the intersection point collection.
				BoxMeshCollisionDetector.intersectionPoints.Clear();

				// Iterate on the potentially intersecting triangles.
				for (int i = 0; i < BoxMeshCollisionDetector.triangles.Count; i++)
				{
					// Get the current triangle.
					CollisionMeshTriangle triangle = BoxMeshCollisionDetector.triangles[i];

					bool exit = false;

					// Calculate the distance to the triangle's plane.

                    float distanceToPlane = Vector3.Dot(triangle.Normal, boxPosition) + triangle.distance;

					// Determine if the box is close enough to the plane of triangle for a collision.  Early out test.
					if (/*distanceToPlane <= 0.0f || */distanceToPlane >= radiusTotal)
					{
						continue;
					}

					// Get the current triangle's vertices.
					CollisionTriangleEx triangleEx;
					mesh.Proxy.GetExtendedTriangle(triangle, out triangleEx);

					// Initialize the depth information.
					float depth = float.MaxValue - 1.0f;
					int bestAxis = -1; // TODO : Not used...yet???

					// Iterate through the major axes of the box.
					float tempDepth;

					Vector3 axis;
					Vector3 normal = Vector3.Zero;
					for (int j = 0; j < 3; j++)
					{
						// Use the current major axis of the box's transform to determine penetration depth.

						axis = MathsHelper.GetAxis(this.transformedBox.Orientation, (MatrixAxis)j);

						// Determine if there is separation for the current axis projection.
						if (BoxMeshCollisionDetector.GetPenetrationDepth(this.transformedBox, ref triangleEx, ref axis, context.Tolerance, out tempDepth))
						{
							exit = true;
							break;
						}

						// Determine if this is the best separating axis tested so far.
						if (tempDepth < depth)
						{
							normal = axis;
							depth = tempDepth;
							bestAxis = j;
						}
					}

					if (exit)
					{
						continue;
					}

					// Use the triangle's normal to determine penetration depth.
					if (BoxMeshCollisionDetector.GetPenetrationDepth(this.transformedBox, ref triangleEx, ref triangle.Normal, context.Tolerance, out tempDepth))
					{
						continue;
					}

					// Determine if this is the best separating axis tested so far.
					if (tempDepth < depth)
					{
						normal = triangle.Normal;
						depth = tempDepth;
						bestAxis = 3;
					}

					// Iterate through major axes of the box.
					Vector3 axis0, axis1;
					for (int j = 0; j < 3; j++)
					{
                        // Get the major axis of box for the current iteration.

                        axis0 = MathsHelper.GetAxis(this.transformedBox.Orientation, (MatrixAxis)j);

						// Iterate through edges of the triangle.
						for (int k = 0; k < 3; k++)
						{
							// Get the edge of the triangle.

							switch (k)
							{
								case 0:
									axis1 = triangleEx.Vertex1 - triangleEx.Vertex0;
									break;

								case 1:
									axis1 = triangleEx.Vertex2 - triangleEx.Vertex1;
									break;

								case 2:
									axis1 = triangleEx.Vertex0 - triangleEx.Vertex2;
									break;

								default:
									axis1 = Vector3.Zero;
									break;
							}

							// Cross axes of both boxes to determine penetration depth.

							axis = Vector3.Cross(axis0, axis1);

							// Determine if there is separation for the current axis projection.
							if (BoxMeshCollisionDetector.GetPenetrationDepth(this.transformedBox, ref triangleEx, ref axis, context.Tolerance, out tempDepth))
							{
								exit = true;
								break;
							}

							// Determine if this is the best separating axis tested so far.
							if (tempDepth < depth)
							{
								normal = axis;
								depth = tempDepth;
								bestAxis = 4 + j * 3 + k;
							}
						}

						if (exit)
						{
							break;
						}
					}

					if (exit)
					{
						continue;
					}

					normal = triangle.Normal;

					// Get all the intersection points between the collision box and current triangle.
					BoxMeshCollisionDetector.GetIntersectionPoints(this.transformedBox, triangle, ref triangleEx);		
					if (BoxMeshCollisionDetector.intersectionPoints.Count > 0)
					{
						// TODO : This can't be correct.  It just fixes some jitter because all collision points will
						// eventually resolve.  look into solving over n iterations in resolver.
						//collisionDepth += depth;
						collisionDepth = Math.Min(collisionDepth, depth);

						// Combine the normal of this collision to the total.

						collisionNormal += normal;
					}
				}

				// Iterate on all intersection points.
				for (int i = 0; i < BoxMeshCollisionDetector.intersectionPoints.Count; i++)
				{
					// Create the collision point.
					CollisionPoint point = CollisionPoint.Retrieve();

					// Transform the collision position into world space.
					Vector3 collisionPosition = BoxMeshCollisionDetector.intersectionPoints[i];

					collisionPosition = Vector3.Transform(collisionPosition, mesh.transform);

					// Transform the collision position into world space.

					point.RelativePosition0 = collisionPosition - shape0.CollisionGroup.Position;
					point.RelativePosition1 = collisionPosition - shape1.CollisionGroup.Position;

					// TODO : This can't be correct.  It makes no sense.  Same in box-box for that matter.
					point.Depth = collisionDepth / BoxMeshCollisionDetector.triangles.Count;

					// Add the collision point to the collection.
					collisionPoints.Add(point);
				}

				// Notify subscribers of the collision.
				if (collisionPoints.Count > 0)
				{
					// Transform normal into world space.

					collisionNormal = Vector3.TransformNormal(collisionNormal, mesh.transform);

                    // Unitize the combined collision normal.
                    collisionNormal.Normalize();


					context.OnCollided(shape0, shape1, ref collisionNormal, collisionPoints);
				}

				// Recycle the collision point collection and any triangles found.
				CollisionPointCollection.Recycle(collisionPoints);
				mesh.ReleaseIntersectingTriangles(BoxMeshCollisionDetector.triangles);
			}
		}

		#endregion

		/// <summary>
		/// Gets the depth of penetration between the specified <see cref="CollisionBox"/> and <see cref="CollisionTriangle"/> on the
		/// specified axis and unitizes it and the axis if valid.
		/// </summary>
		/// <param name="box">The source <see cref="CollisionBox"/>.</param>
		/// <param name="triangle">The source <see cref="CollisionTriangle"/>.</param>
		/// <param name="axis">A <see cref="Microsoft.Xna.Framework.Vector3"/> representing the axis to project the specified <see cref="CollisionBox"/> and <see cref="CollisionTriangle"/> apon.  The result will be unitized if the <see cref="CollisionBox"/> instances overlap on the specified axis.</param>
		/// <param name="collisionTolerance">The current <see cref="CollisionContext"/> collision tolerance.</param>
		/// <param name="depth">The unitized depth of overlap.</param>
		/// <returns>Returns <c>true</c> if there is no overlap.  Otherwise, it return <c>false</c>.</returns>
		private static bool GetPenetrationDepth(CollisionBox box, ref CollisionTriangleEx triangle, ref Vector3 axis, float collisionTolerance, out float depth)
		{
			// Check for the degenerate case.
			float lengthSquared = axis.LengthSquared();
			if (lengthSquared < MathsHelper.Epsilon)
			{
				depth = float.MaxValue;
				return false;
			}

			// Get the collision box projection range.

			float minimum0, maximum0;
			BoxMeshCollisionDetector.GetProjectionToAxis(box, ref axis, out minimum0, out maximum0);

			// Get the collision triangle projection range.

			float minimum1, maximum1;
			BoxMeshCollisionDetector.GetProjectionToAxis(ref triangle, ref axis, out minimum1, out maximum1);

			// The projections do not overlap.
			if (minimum0 > (maximum1 + collisionTolerance + MathsHelper.Epsilon) ||
				minimum1 > (maximum0 + collisionTolerance + MathsHelper.Epsilon))
			{
				depth = 0.0f;
				return true;
			}

			// Determine the depth from the projections.
			if (maximum0 > maximum1 && minimum1 > minimum0)
			{
				depth = Math.Min(maximum0 - minimum1, maximum1 - minimum0);
			}
			else if (maximum1 > maximum0 && minimum0 > minimum1)
			{
				depth = Math.Min(maximum1 - minimum0, maximum0 - minimum1);
			}
			else
			{
				depth = Math.Min(maximum0, maximum1) - Math.Max(minimum0, minimum1);
			}
			
			// Unitize the axis and penetration depth.

			float inverseLength = 1.0f / (float)Math.Sqrt(lengthSquared);

            axis *= inverseLength;
			depth *= inverseLength;

			// An overlap exists.
			return false;
		}

		/// <summary>
		/// Gets the projection of the given <see cref="CollisionBox"/> along the given axis.
		/// </summary>
		/// <param name="box">The source <see cref="CollisionBox"/>.</param>
		/// <param name="axis">A <see cref="Microsoft.Xna.Framework.Vector3"/> representing the axis to project the <see cref="CollisionBox"/> on.</param>
		/// <param name="minimum">The minimum projection value.</param>
		/// <param name="maximum">The maximum projection value.</param>

		private static void GetProjectionToAxis(CollisionBox box, ref Vector3 axis, out float minimum, out float maximum)
		{
            // Get the major axes of the collision box.
            Vector3 xAxis = MathsHelper.GetAxis(box.Orientation, MatrixAxis.X);
			Vector3 yAxis = MathsHelper.GetAxis(box.Orientation, MatrixAxis.Y);
			Vector3 zAxis = MathsHelper.GetAxis(box.Orientation, MatrixAxis.Z);

			// Projecting each extent to the specified axis along their respective axes and summing
			// the results gives us half of this box's total projection.
			float projection0 =
				box.Extents.X * Math.Abs(Vector3.Dot(axis, xAxis)) +
				box.Extents.Y * Math.Abs(Vector3.Dot(axis, yAxis)) +
				box.Extents.Z * Math.Abs(Vector3.Dot(axis, zAxis));

			// Project the position of the box to the axis and determine the range of the total projection.
			float projection1 = Vector3.Dot(box.Position, axis);
			minimum = projection1 - projection0;
			maximum = projection1 + projection0;
		}

		/// <summary>
		/// Gets the projection of the given <see cref="CollisionTriangleEx"/> along the given axis.
		/// </summary>
		/// <param name="triangle">The source <see cref="CollisionTriangleEx"/>.</param>
		/// <param name="axis">A <see cref="Microsoft.Xna.Framework.Vector3"/> representing the axis to project the <see cref="CollisionTriangleEx"/> on.</param>
		/// <param name="minimum">The minimum projection value.</param>
		/// <param name="maximum">The maximum projection value.</param>

		private static void GetProjectionToAxis(ref CollisionTriangleEx triangle, ref Vector3 axis, out float minimum, out float maximum)
		{
			float d0 = Vector3.Dot(triangle.Vertex0, axis);
			float d1 = Vector3.Dot(triangle.Vertex1, axis);
			float d2 = Vector3.Dot(triangle.Vertex2, axis);

			minimum = Math.Min(Math.Min(d0, d1), d2);
			maximum = Math.Max(Math.Max(d0, d1), d2);
		}

		/// <summary>
		/// Gets the intersection points between the a <see cref="CollisionBox"/> and <see cref="CollisionTriangle"/>.
		/// </summary>
		/// <param name="box">The <see cref="CollisionBox"/> involved in the intesection.</param>
		/// <param name="triangle">The <see cref="CollisionTriangle"/> involved in the intersection.</param>
		/// <param name="triangleEx">Extra information about the triangle involved in the intersection.</param>
		private static void GetIntersectionPoints(CollisionBox box, CollisionMeshTriangle triangle, ref CollisionTriangleEx triangleEx)
		{
			// TODO : Define corner threshholds elsewhere.
			const float MIN_T = 0.03f;
			const float MAX_T = 0.97f;

			// Initialize corner usage mask.
			int cornerMask = 0;

			// Working storage.
			Vector3 position, normal;
			float t;

			// Iterate on all of a box's edges.
			for (int i = 0; i < CollisionBox.Edges.Length; i++)
			{
				// Get the current corner types and their mask equivalents.
				BoxCornerType corner0 = CollisionBox.Edges[i].CornerType0;
				BoxCornerType corner1 = CollisionBox.Edges[i].CornerType1;
				int cornerMask0 = 1 << (int)corner0;
				int cornerMask1 = 1 << (int)corner1;

				// Determine the edge points for the current box edge.
				Vector3 edgePoint0 = box.Corners[(int)corner0];
				Vector3 edgePoint1 = box.Corners[(int)corner1];

				// Create a ray that represents the current box edge.
				CollisionRay ray0 = new CollisionRay(edgePoint0, edgePoint1 - edgePoint0);

				// Check for intersection with the triangle.
				if (triangle.IntersectsWith(ref ray0, out position, out t))
				{
					// Add the intersection to the collection.
					if (t < MIN_T) 
					{
						if ((cornerMask & cornerMask0) == 0)
						{
							cornerMask |= cornerMask0;
							BoxMeshCollisionDetector.intersectionPoints.Add(edgePoint0);
						}
					}
					else if (t > MAX_T)
					{
						if ((cornerMask & cornerMask1) == 0)
						{
							cornerMask |= cornerMask1;
							BoxMeshCollisionDetector.intersectionPoints.Add(edgePoint1);
						}
					}
					else
					{
						// Create a ray that represents the current box edge.
						Vector3 position0;
						CollisionRay ray1 = new CollisionRay(edgePoint1, edgePoint0 - edgePoint1);
						if (triangle.IntersectsWith(ref ray1, out position0, out t))
						{
							// Add the intersection to the collection.
							if (t < MIN_T)
							{
								if ((cornerMask & cornerMask1) == 0)
								{
									cornerMask |= cornerMask1;
									BoxMeshCollisionDetector.intersectionPoints.Add(edgePoint1);
								}
							}
							else if (t > MAX_T)
							{
								if ((cornerMask & cornerMask0) == 0)
								{
									cornerMask |= cornerMask0;
									BoxMeshCollisionDetector.intersectionPoints.Add(edgePoint0);
								}
							}
							else
							{
								// Use the mid-point of the two intersection points.

								position = (position + position0) * 0.5f;
								BoxMeshCollisionDetector.intersectionPoints.Add(position);
							}
						}
						else
						{
							// Add the intersection to the collection.
							if (t < MIN_T)
							{
								if ((cornerMask & cornerMask0) == 0)
								{
									cornerMask |= cornerMask0;
									BoxMeshCollisionDetector.intersectionPoints.Add(edgePoint0);
								}
							}
							else if (t > MAX_T)
							{
								if ((cornerMask & cornerMask1) == 0)
								{
									cornerMask |= cornerMask1;
									BoxMeshCollisionDetector.intersectionPoints.Add(edgePoint1);
								}
							}
							else
							{
								BoxMeshCollisionDetector.intersectionPoints.Add(position);
							}
						}
					}
				}
				else
				{
					// Create a collision ray that starts at the second edge point for the current edge
					// and test for intersection.
					CollisionRay ray1 = new CollisionRay(edgePoint1, edgePoint0 - edgePoint1);
					if (triangle.IntersectsWith(ref ray1, out position, out t))
					{
						// Add the intersection to the collection.
						if (t < MIN_T)
						{
							if ((cornerMask & cornerMask1) == 0)
							{
								cornerMask |= cornerMask1;
								BoxMeshCollisionDetector.intersectionPoints.Add(edgePoint1);
							}
						}
						else if (t > MAX_T)
						{
							if ((cornerMask & cornerMask0) == 0)
							{
								cornerMask |= cornerMask0;
								BoxMeshCollisionDetector.intersectionPoints.Add(edgePoint0);
							}
						}
						else
						{
							BoxMeshCollisionDetector.intersectionPoints.Add(position);
						}
					}
				}
			}
			
			// Iterate on all of a triangle's edges.
			for (int i = 0; i < 3; i++)
			{
				// Get the edge of the triangle.
				Vector3 edgePoint0;
				Vector3 edgePoint1;
				switch (i)
				{
					case 0:
						edgePoint0 = triangleEx.Vertex0;
						edgePoint1 = triangleEx.Vertex1;
						break;

					case 1:
						edgePoint0 = triangleEx.Vertex1;
						edgePoint1 = triangleEx.Vertex2;
						break;

					case 2:
						edgePoint0 = triangleEx.Vertex2;
						edgePoint1 = triangleEx.Vertex0;
						break;

					default:
						edgePoint0 = Vector3.Zero;
						edgePoint1 = Vector3.Zero;
						break;
				}

				// Create a collision ray that starts at the first edge point for the current edge
				// and test for intersection.
				CollisionRay ray0 = new CollisionRay(edgePoint0, edgePoint1 - edgePoint0);
				if (box.IntersectsWith(ref ray0, out position, out normal, out t))
				{
					// Create a collision ray that starts at the second edge point for the current edge
					// and test for intersection.
					Vector3 position0;
					CollisionRay ray1 = new CollisionRay(edgePoint1, edgePoint0 - edgePoint1);
					if (box.IntersectsWith(ref ray1, out position0, out normal, out t))
					{
						// Use the mid-point of the two intersection points.

						position = (position + position0) * 0.5f;
						BoxMeshCollisionDetector.intersectionPoints.Add(position);
					}
					else
					{
						// Add the intersection to the collection.
						BoxMeshCollisionDetector.intersectionPoints.Add(position);
					}
				}
				else
				{
					// Create a collision ray that starts at the second edge point for the current edge
					// and test for intersection.
					CollisionRay ray1 = new CollisionRay(edgePoint1, edgePoint0 - edgePoint1);
					if (box.IntersectsWith(ref ray1, out position, out normal, out t))
					{
						// Add the intersection to the collection.
						BoxMeshCollisionDetector.intersectionPoints.Add(position);
					}
				}
			}
		}
	}
}