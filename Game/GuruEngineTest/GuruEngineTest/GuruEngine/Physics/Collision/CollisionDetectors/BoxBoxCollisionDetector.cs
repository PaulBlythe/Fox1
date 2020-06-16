using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GuruEngine.Helpers;
using GuruEngine.Physics.Collision.Shapes;

namespace GuruEngine.Physics.Collision.CollisionDetectors
{
	/// <summary>
	/// Detects collisions between two <see cref="CollisionBox"/> instances.
	/// </summary>
	public class BoxBoxCollisionDetector : CollisionDetector
	{
		private static List<Vector3> intersectionPoints = new List<Vector3>(16);
		private static Vector3[] corners = new Vector3[8];

		#region CollisionDetector Members

		public override void Detect(CollisionShape shape0, CollisionShape shape1, CollisionContext context)
		{
			// Typecast the shapes to their correct subclasses.
			CollisionBox box0 = (CollisionBox)shape0;
			CollisionBox box1 = (CollisionBox)shape1;

			// Determine the vector between the boxes' centers.

			Vector3 difference = box1.Position - box0.Position;

			// Initialize the depth information.
			float depth = float.MaxValue - 1.0f;
			int bestAxis = -1;

			// Iterate through the major axes of the first and second boxes.
			float tempDepth;
			Vector3 axis;
			Vector3 normal = Vector3.Zero;
			for (int i = 0; i < 3; i++)
			{
				// Use the current major axis of the first box's transform to determine penetration depth.
				axis = MathsHelper.GetAxis(box0.Orientation, (MatrixAxis)i);

				// Determine if there is separation for the current axis projection.
				if (BoxBoxCollisionDetector.GetPenetrationDepth(box0, box1, ref axis, ref difference, context.Tolerance, out tempDepth))
				{
					return;
				}

				// Determine if this is the best separating axis tested so far.
				if (tempDepth < depth)
				{
					normal = axis;
					depth = tempDepth;
					bestAxis = i;
				}

				// Use the current major axis of the second box's transform to determine penetration depth.

				axis = MathsHelper.GetAxis(box1.Orientation, (MatrixAxis)i);

				// Determine if there is separation for the current axis projection.
				if (BoxBoxCollisionDetector.GetPenetrationDepth(box0, box1, ref axis, ref difference, context.Tolerance, out tempDepth))
				{
					return;
				}

				// Determine if this is the best separating axis tested so far.
				if (tempDepth < depth)
				{
					normal = axis;
					depth = tempDepth;
					bestAxis = i + 3;
				}
			}

			// Iterate through major axes of the first box.
			Vector3 axis0, axis1;
			for (int i = 0; i < 3; i++)
			{
                // Get the major axis of first box for the current iteration.

                axis0 = MathsHelper.GetAxis(box0.Orientation, (MatrixAxis)i);

				// Iterate through major axes of the second box.
				for (int j = 0; j < 3; j++)
				{
                    // Get the major axis of second box for the current iteration.

                    axis1 = MathsHelper.GetAxis(box1.Orientation, (MatrixAxis)j);

					// Cross axes of both boxes to determine penetration depth.

					axis = Vector3.Cross(axis0, axis1);

					// Determine if there is separation for the current axis projection.
					if (BoxBoxCollisionDetector.GetPenetrationDepth(box0, box1, ref axis, ref difference, context.Tolerance, out tempDepth))
					{
						return;
					}

					// Determine if this is the best separating axis tested so far.
					if (tempDepth < depth)
					{
						normal = axis;
						depth = tempDepth;
						bestAxis = 6 + i * 3 + j;
					}
				}
			}

			// Make sure the axis is facing towards the first box.

			if (Vector3.Dot(difference, normal) > 0.0f)
			{
				normal = Vector3.Negate(normal);
			}
			// Get all the intersection points between the two collision boxes.
			BoxBoxCollisionDetector.intersectionPoints.Clear();
			if (depth > -MathsHelper.Epsilon)
			{
				BoxBoxCollisionDetector.GetIntersectionPoints(box0, box1);
				BoxBoxCollisionDetector.GetIntersectionPoints(box1, box0);
			}

			// Initialize the collision manifold.
			CollisionPointCollection collisionPoints = CollisionPointCollection.Retrieve();

			if (BoxBoxCollisionDetector.intersectionPoints.Count > 0)
			{				
				// Iterate on all intersection points.
				for (int i = 0; i < BoxBoxCollisionDetector.intersectionPoints.Count; i++)
				{
					// Create the collision point.
					CollisionPoint point = CollisionPoint.Retrieve();

					point.RelativePosition0 = BoxBoxCollisionDetector.intersectionPoints[i] - shape0.CollisionGroup.Position;
					point.RelativePosition1 = BoxBoxCollisionDetector.intersectionPoints[i] - shape1.CollisionGroup.Position;

					// TODO : Using the same depth for every collision point...not correct.
					point.Depth = depth;

					// Add the collision point to the collection.
					collisionPoints.Add(point);
				}
			}
			else
			{
				// Determine the location furthest along the axis causing the minimum penatration depth.
				Vector3 collisionPosition;
				if (bestAxis < 3)
				{
					// A face-to-corner collision has occurred.
					BoxBoxCollisionDetector.GetExtremePoint(box1, -normal, out collisionPosition);
				}
				else if (bestAxis < 6)
				{
					// A face-to-corner collision has occurred.
					BoxBoxCollisionDetector.GetExtremePoint(box0, normal, out collisionPosition);
				}
				else
				{
					// An edge-to-edge collision has occurred.  Determine which axes...again.
					bestAxis -= 6;
					int axisIndex0 = bestAxis / 3;
					int axisIndex1 = bestAxis % 3;

                    // Get the participating major axes of the two colliding boxes.

                    axis0 = MathsHelper.GetAxis(box0.Orientation, (MatrixAxis)axisIndex0);
                    axis1 = MathsHelper.GetAxis(box1.Orientation, (MatrixAxis)axisIndex1);


					// Determine the plane along the normal of the collision and the major axis 
					// of the second box.
					Vector3 planeNormal = Vector3.Cross(normal, axis1);
					
					// Make sure the contributing major axis of the first box and the plane are not parallel.
					// There's no intersection if this is the case.
					float distance = Vector3.Dot(axis0, planeNormal);
					if (Math.Abs(distance) < MathsHelper.Epsilon)
					{
						return;
					}

					// Get the locations on the boxes furthest along the axis causing the minimum penetration depth.
					Vector3 midPoint0;
					BoxBoxCollisionDetector.GetExtremePoint(box0, normal, out midPoint0);
					Vector3 midPoint1;
					BoxBoxCollisionDetector.GetExtremePoint(box1, -normal, out midPoint1);

					// Offset the plane so it passes through the support point of the second box.
					float planeDistance = Vector3.Dot(planeNormal, midPoint1);

					// Determine the point along the colliding edge of the first box.
					float t = (planeDistance - Vector3.Dot(midPoint0, planeNormal)) / distance;
					midPoint0 += axis0 * t;

					// Get the pericenter of the the two colliding edges.  This is the main contact location of the collision.
					collisionPosition = midPoint0 + normal * (depth * 0.5f);

				}

				// Create the collision point.
				CollisionPoint point = CollisionPoint.Retrieve();

				point.RelativePosition0 = collisionPosition - shape0.CollisionGroup.Position;
				point.RelativePosition1 = collisionPosition - shape1.CollisionGroup.Position;

				point.Depth = depth;

				// Add the collision point to the collection.
				collisionPoints.Add(point);
			}

			// Notify subscribers of the collision.
			context.OnCollided(shape0, shape1, ref normal, collisionPoints);

			// Recycle the collision point collection.
			CollisionPointCollection.Recycle(collisionPoints);
		}

		#endregion

		/// <summary>
		/// Gets the depth of penetration between the specified <see cref="CollisionBox"/> instances on the
		/// specified axis and unitizes it and the axis if valid.
		/// </summary>
		/// <param name="box0">The first <see cref="CollisionBox"/>.</param>
		/// <param name="box1">The second <see cref="CollisionBox"/>.</param>
		/// <param name="axis">A <see cref="Microsoft.Xna.Framework.Vector3"/> representing the axis to project the specified <see cref="CollisionBox"/> instances apon.  The result will be unitized if the <see cref="CollisionBox"/> instances overlap on the specified axis.</param>
		/// <param name="difference">The difference of the specified <see cref="CollisionBox"/> instances' positions.</param>
		/// <param name="collisionTolerance">The current <see cref="CollisionContext"/> collision tolerance.</param>
		/// <param name="depth">The unitized depth of overlap.</param>
		/// <returns>Returns <c>true</c> if there is no overlap.  Otherwise, it return <c>false</c>.</returns>
		private static bool GetPenetrationDepth(CollisionBox box0, CollisionBox box1, ref Vector3 axis, ref Vector3 difference, float collisionTolerance, out float depth)
		{

			// Check for the degenerate case.
			float lengthSquared = axis.LengthSquared();
			if (lengthSquared < MathsHelper.Epsilon)
			{
				depth = float.MaxValue;
				return false;
			}

			// Project the vector difference of the two centers to the specified axis.
			float distance = Vector3.Dot(difference, axis);


			// Get the sum of the boxes' extents projected onto the specified axis.  
			// Subtracting the projection from above gives the penetration depth.
			depth = 
				BoxBoxCollisionDetector.GetProjectionToAxis(box0, ref axis) +
				BoxBoxCollisionDetector.GetProjectionToAxis(box1, ref axis) -
				Math.Abs(distance);
			// There is no overlap.
			if (depth < -(collisionTolerance + MathsHelper.Epsilon))
			{
				return true;
			}

			// Unitize the axis and penetration depth.

			float inverseLength = 1.0f / (float)Math.Sqrt(lengthSquared);
			axis *= inverseLength;
			depth *= inverseLength;

			// An overlap exists.
			return false;
		}

		/// <summary>
		/// Gets half of the projection of the given <see cref="CollisionBox"/> along the given axis.
		/// </summary>
		/// <param name="box">The source <see cref="CollisionBox"/>.</param>
		/// <param name="axis">A <see cref="Microsoft.Xna.Framework.Vector3"/> representing the axis to project the box on.</param>
		/// <returns>Return half of <see cref="CollisionBox"/> instance's projection.</returns>
		private static float GetProjectionToAxis(CollisionBox box, ref Vector3 axis)
		{
			// Get the major axes of the collision box.
			Vector3 xAxis = MathsHelper.GetAxis(box.Orientation, MatrixAxis.X);
			Vector3 yAxis = MathsHelper.GetAxis(box.Orientation, MatrixAxis.Y);
			Vector3 zAxis = MathsHelper.GetAxis(box.Orientation, MatrixAxis.Z);

			// Projecting each extent to the specified axis along their respective axes and summing
			// the results gives us half of this box's total projection.  This is all that is needed
			// for box-to-box separating axis testing.
			return
				box.Extents.X * Math.Abs(Vector3.Dot(axis, xAxis)) +
				box.Extents.Y * Math.Abs(Vector3.Dot(axis, yAxis)) +
				box.Extents.Z * Math.Abs(Vector3.Dot(axis, zAxis));
		}

		/// <summary>
		/// Gets the furthest location on the specified <see cref="CollisionBox"/> along the specified axis.
		/// </summary>
		/// <param name="box">The source <see cref="CollisionBox"/>.</param>
		/// <param name="axis">The <see cref="Microsoft.Xna.Framework.Vector3"/> representing the axis.</param>
		/// <param name="result">A <see cref="Microsoft.Xna.Framework.Vector3"/> representing the furthest location on the <see cref="CollisionBox"/> along the specified axis."/></param>
		private static void GetExtremePoint(CollisionBox box, Vector3 axis, out Vector3 result)
		{
			// Initialize result.
			result = box.Position;

            {
                Vector3 axisTemp = MathsHelper.GetAxis(box.Orientation, (MatrixAxis)0);
                float dot = Vector3.Dot(axis, axisTemp);
                if (dot < -MathsHelper.Epsilon)
                {
                    result += axisTemp * box.Extents.X;
                }
                else if (dot >= MathsHelper.Epsilon)
                {
                    result -= axisTemp * box.Extents.X;
                }
            }
            {
                Vector3 axisTemp = MathsHelper.GetAxis(box.Orientation, (MatrixAxis)1);
                float dot = Vector3.Dot(axis, axisTemp);
                if (dot < -MathsHelper.Epsilon)
                {
                    result += axisTemp * box.Extents.Y;
                }
                else if (dot >= MathsHelper.Epsilon)
                {
                    result -= axisTemp * box.Extents.Y;
                }
            }
            {
                Vector3 axisTemp = MathsHelper.GetAxis(box.Orientation, (MatrixAxis)2);
                float dot = Vector3.Dot(axis, axisTemp);
                if (dot < -MathsHelper.Epsilon)
                {
                    result += axisTemp * box.Extents.Z;
                }
                else if (dot >= MathsHelper.Epsilon)
                {
                    result -= axisTemp * box.Extents.Z;
                }
            }

        }

		/// <summary>
		/// Gets the intersection points between the two <see cref="CollisionBox"/> instances.
		/// </summary>
		/// <param name="box0">The first <see cref="CollisionBox"/> involved in the intesection.</param>
		/// <param name="box1">The second <see cref="CollisionBox"/> involved in the intesection.</param>
		private static void GetIntersectionPoints(CollisionBox box0, CollisionBox box1)
		{
			// Transform the second box into the first box's local space.
			Matrix inverseOrientation = Matrix.Transpose(box0.Orientation);
			Vector3 position = Vector3.TransformNormal(box1.Position - box0.Position, inverseOrientation);
			Matrix orientation = box1.Orientation * inverseOrientation;

			// Get the corners of the translated box.
			CollisionBox.GetBoxCorners(ref position, ref orientation, ref box1.Extents, BoxBoxCollisionDetector.corners);

			// Iterate on all of a box's edges.
			int count = 0;
			for (int j = 0; j < CollisionBox.Edges.Length; j++)
			{
				// Determine the edge points for the current box edge.
				Vector3 edgePoint0 = BoxBoxCollisionDetector.corners[(int)CollisionBox.Edges[j].CornerType0];
				Vector3 edgePoint1 = BoxBoxCollisionDetector.corners[(int)CollisionBox.Edges[j].CornerType1];

				// Determine the intersections between the edge and world-aligned box's faces.
				count += BoxBoxCollisionDetector.GetIntersectionPoints(box0, ref edgePoint0, ref edgePoint1);

				// The most intersections that should occur between two boxes is 8.
				if (count >= 8)
				{
					break;
				}
			}
		}

		/// <summary>
		/// Gets the intersection points between a box-aligned edge and that world-aligned box's faces.
		/// </summary>
		/// <param name="box">The source <see cref="CollisionBox"/>.</param>
		/// <param name="edgePoint0">The location of the starting point of the box-aligned edge.</param>
		/// <param name="edgePoint1">The location of the eding point of the box-aligned edge.</param>
		/// <returns>Returns the number of intersection points between the <see cref="CollisionBox"/> and the defined edge.</returns>
		private static int GetIntersectionPoints(CollisionBox box, ref Vector3 edgePoint0, ref Vector3 edgePoint1)
		{
			// Get the edge's direction.

			Vector3 edgeDirection = (edgePoint1 - edgePoint0);
            edgeDirection.Normalize();

			// Iterate on each box extent.
			int count = 0;
			for (int i = 0; i < 3; i++)
			{
				// TODO : Remove magic number.
				// Don't attempt to find an intersection when the edge and current box face are parallel.
				if (Math.Abs(MathsHelper.GetVector3Component(edgeDirection, (VectorIndex)i)) < 0.0009f)
				{
					continue;
				}

				// Determine the other extent component indices.
				int i1 = (i + 1) % 3;
				int i2 = (i + 2) % 3;

				// Get the offset of the box's face using the current extent.
				float offset = -MathsHelper.GetVector3Component(box.Extents, (VectorIndex)i);

				// Iterate on each side of the current box face.
				for (int j = 0; j < 2; j++)
				{
					// Determine the edge points' matching component distances.
					float distance0 = MathsHelper.GetVector3Component(edgePoint0, (VectorIndex)i) - offset;
					float distance1 = MathsHelper.GetVector3Component(edgePoint1, (VectorIndex)i) - offset;

					float t = float.MinValue;
					if (distance0 * distance1 < -MathsHelper.Epsilon)
					{
						t = -distance0 / (distance1 - distance0);
					}
					else if (Math.Abs(distance0) < MathsHelper.Epsilon)
					{
						t = 0.0f;
					}
					else if (Math.Abs(distance1) < MathsHelper.Epsilon)
					{
						t = 1.0f;
					}

					if (t >= 0.0f)
					{
						// Determine the intersection point using the distances along the edge.

						Vector3 point = edgePoint0 * (1.0f - t) + edgePoint1 * t;
						// Get the intersection points other components.
						float t1 = MathsHelper.GetVector3Component(point, (VectorIndex)i1);
						float t2 = MathsHelper.GetVector3Component(point, (VectorIndex)i2);

						// Get the other box extents.
						float offset1 = MathsHelper.GetVector3Component(box.Extents, (VectorIndex)i1) + MathsHelper.Epsilon;
						float offset2 = MathsHelper.GetVector3Component(box.Extents, (VectorIndex)i2) + MathsHelper.Epsilon;

						// Make sure the other component's of the intersection point are within the 
						// other extents of box.
						if (t1 > -offset1 && t1 < offset1 &&
							t2 > -offset2 && t2 < offset2)
						{
							// TODO : Move to after all intersection points are discovered.
							// Transform the intersection point to world coordinates.

							Vector3 position = box.Position + Vector3.TransformNormal(point, box.Orientation);

							// Add the intersection point to the collection.
							BoxBoxCollisionDetector.intersectionPoints.Add(position);

							// An edge can only intersect with a box two times at most.
							count++;
							if (count == 2)
							{
								return count;
							}
						}
					}

					// Flip the face offset.
					offset = -offset;
				}
			}

			return count;
		}
	}
}
