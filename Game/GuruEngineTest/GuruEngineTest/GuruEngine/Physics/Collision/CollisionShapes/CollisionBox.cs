using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

using GuruEngine.Helpers;
using GuruEngine.Physics.Collision;



namespace GuruEngine.Physics.Collision.Shapes
{
	/// <summary>
	/// Defines a collision shape in the form of a cube.
	/// </summary>
	public class CollisionBox : CollisionShape
	{
		#region BoxEdge Declaration
	    
		internal struct BoxEdge
		{
			internal BoxCornerType CornerType0;
			internal BoxCornerType CornerType1;
		}

		#endregion

		/// <summary>
		/// Gets an array of <see cref="CollisionBox.BoxEdges"/> instances.
		/// </summary>
		/// <remarks>
		/// Used with <see cref="CollisionBox.Corners" /> to simplify box edge retrieval during collision detection.
		/// </remarks>
		internal static BoxEdge[] Edges = new BoxEdge[12]
			{ 
				new BoxEdge() { CornerType0 = BoxCornerType.RightBottomBack, CornerType1 = BoxCornerType.RightTopBack },
				new BoxEdge() { CornerType0 = BoxCornerType.RightBottomBack, CornerType1 = BoxCornerType.LeftBottomBack },
				new BoxEdge() { CornerType0 = BoxCornerType.RightBottomBack, CornerType1 = BoxCornerType.RightBottomFront },
				new BoxEdge() { CornerType0 = BoxCornerType.LeftBottomBack, CornerType1 = BoxCornerType.LeftTopBack }, 
				new BoxEdge() { CornerType0 = BoxCornerType.LeftBottomBack, CornerType1 = BoxCornerType.LeftBottomFront },
				new BoxEdge() { CornerType0 = BoxCornerType.RightBottomFront, CornerType1 = BoxCornerType.RightTopFront },
				new BoxEdge() { CornerType0 = BoxCornerType.RightBottomFront, CornerType1 = BoxCornerType.LeftBottomFront },
				new BoxEdge() { CornerType0 = BoxCornerType.RightTopBack, CornerType1 = BoxCornerType.LeftTopBack },
				new BoxEdge() { CornerType0 = BoxCornerType.RightTopBack, CornerType1 = BoxCornerType.RightTopFront },
				new BoxEdge() { CornerType0 = BoxCornerType.LeftTopBack, CornerType1 = BoxCornerType.LeftTopFront },
				new BoxEdge() { CornerType0 = BoxCornerType.RightTopFront, CornerType1 = BoxCornerType.LeftTopFront },
				new BoxEdge() { CornerType0 = BoxCornerType.LeftBottomFront, CornerType1 = BoxCornerType.LeftTopFront }
			};

		/// <summary>
		/// Gets or set the extents of the <see cref="CollisionBox"/>.
		/// </summary>
		public Vector3 Extents;

		/// <summary>
		/// Gets the corner positions of the <see cref="CollisionBox"/>.
		/// </summary>
		/// <remarks>
		/// Do not set this field directly.  The corners of the <see cref="CollisionBox"/> get
		/// calculated when moved.
		/// </remarks>
		public Vector3[] Corners = new Vector3[8];

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionBox"/> class.
		/// </summary>
		public CollisionBox() : base(CollisionShapeType.Box) { }

		#region CollisionShape Members
		
		internal override void CalculateInternals()
		{
			// Call inherited method.
			base.CalculateInternals();

			// Calculate the corner positions.
			Vector3 xAxis = MathsHelper.GetAxis(this.Orientation, MatrixAxis.X) * this.Extents.X;
			Vector3 yAxis = MathsHelper.GetAxis(this.Orientation, MatrixAxis.Y) * this.Extents.Y;
			Vector3 zAxis = MathsHelper.GetAxis(this.Orientation, MatrixAxis.Z) * this.Extents.Z;

			Vector3 temp0 = this.Position + xAxis;
			Vector3 temp1 = this.Position - xAxis;
			Vector3 temp2 = yAxis + zAxis;
			Vector3 temp3 = yAxis - zAxis;

			this.Corners[(int)BoxCornerType.RightTopFront] = temp0 + temp2;
			this.Corners[(int)BoxCornerType.RightTopBack] = temp0 + temp3;
			this.Corners[(int)BoxCornerType.RightBottomFront] = temp0 - temp2;
			this.Corners[(int)BoxCornerType.RightBottomBack] = temp0 - temp3;
			this.Corners[(int)BoxCornerType.LeftTopFront] = temp1 + temp2;
			this.Corners[(int)BoxCornerType.LeftTopBack] = temp1 + temp3;
			this.Corners[(int)BoxCornerType.LeftBottomFront] = temp1 - temp2;
			this.Corners[(int)BoxCornerType.LeftBottomBack] = temp1 - temp3;

		}

		protected override void CalculateAABB()
		{

			// Calculate extents based on rotation.
			float x = Math.Abs(this.Orientation.M11 * this.Extents.X) + Math.Abs(this.Orientation.M21 * this.Extents.Y) + Math.Abs(this.Orientation.M31 * this.Extents.Z);
			float y = Math.Abs(this.Orientation.M12 * this.Extents.X) + Math.Abs(this.Orientation.M22 * this.Extents.Y) + Math.Abs(this.Orientation.M32 * this.Extents.Z);
			float z = Math.Abs(this.Orientation.M13 * this.Extents.X) + Math.Abs(this.Orientation.M23 * this.Extents.Y) + Math.Abs(this.Orientation.M33 * this.Extents.Z);

			// Update the axis-aligned bounding box for this shape.
			this.AABB.Min.X = this.Position.X - x;
			this.AABB.Max.X = this.Position.X + x;
			this.AABB.Min.Y = this.Position.Y - y;
			this.AABB.Max.Y = this.Position.Y + y;
			this.AABB.Min.Z = this.Position.Z - z;
			this.AABB.Max.Z = this.Position.Z + z;
		}

		public override bool IntersectsWith(ref CollisionRay ray, out Vector3 position, out Vector3 normal, out float t)
		{
			// Initialize return values.
			position = Vector3.Zero;
			normal = Vector3.Zero;
			t = float.MaxValue;

			// Initialize minimum and maximum values.
			float minimum = float.MinValue;
			float maximum = float.MaxValue;

			// Initialize the minimum and maximum axis.
			int minimumAxis = 0;
			int maximumAxis = 0;

			// Get the vector between the start of the ray and the box's position.

			Vector3 difference = this.Position - ray.Position;

			// Iterate on axes.
			Vector3 axis;
			for (int i = 0; i < 3; i++)
			{
				// Get the current box axis and extent.

				axis = MathsHelper.GetAxis(this.Orientation, (MatrixAxis)i);

				float extent = MathsHelper.GetVector3Component(this.Extents, (VectorIndex)i);

				// Get the projection of the ray's vector and difference in positions along the axis.

				float a = Vector3.Dot(axis, ray.Vector);
				float b = Vector3.Dot(axis, difference);
				if (Math.Abs(a) < MathsHelper.Epsilon)
				{
					if (-b - extent > 0.0f || -b + extent < 0.0f)
					{
						return false;
					}
				}
				else
				{
					// Get the projection amount along the collision ray for the current axis.
					float t0 = (b + extent) / a;
					float t1 = (b - extent) / a;

					// Swap projections if necessary.
					if (t0 > t1)
					{
						float tmp = t0;
						t0 = t1; 
						t1 = tmp;
					}

					// Hold onto the best minimum.
					if (t0 > minimum)
					{
						minimum = t0;
						minimumAxis = i;
					}

					// Hold onto the best maximum.
					if (t1 < maximum)
					{
						maximum = t1;
						maximumAxis = i;
					}

					// Validate range.
					if (minimum > maximum)
					{
						return false;
					}

					// Validate the maximum value.
					if (maximum < 0.0f)
					{
						return false;
					}
				}
			}

			// Determine the winning axis.
			int bestAxis;
			if (minimum > 0.0f)
			{
				bestAxis = minimumAxis;
				t = minimum;
			}
			else
			{
				bestAxis = maximumAxis;
				t = maximum;
			}

			// Keep within valid range.
			t = MathHelper.Clamp(t, 0.0f, 1.0f);

			// Get the position and normal of the intersection.

			position = ray.GetPointAt(t);
			normal = MathsHelper.GetAxis(this.Orientation, (MatrixAxis)bestAxis);

			// Make sure the normal is pointing in the correct direction.
			if (Vector3.Dot(normal, ray.Vector) > 0.0f)
			{

				normal = -normal;
			}

			// An intersection occurred.
			return true;
		}

		protected internal override float GetVolume()
		{
			return (this.Extents.X * this.Extents.Y * this.Extents.Z) * 8.0f;
		}

		public override void CalculateMassProperties(float density, out float mass, out Matrix inertiaTensor)
		{
			// Get the mass.
			mass = density * this.GetVolume();

			// Calculate some values used below.
			float massFactor = (1.0f / 12.0f) * mass;
			float widthSquared = this.Extents.X * this.Extents.X * 4.0f;
			float heightSquared = this.Extents.Y * this.Extents.Y * 4.0f;
			float depthSquared = this.Extents.Z * this.Extents.Z * 4.0f;

			// Get the inertia tensor.

			inertiaTensor.M11 = massFactor * (heightSquared + depthSquared);
			inertiaTensor.M12 = 0.0f;
			inertiaTensor.M13 = 0.0f;
			inertiaTensor.M14 = 0.0f;

			inertiaTensor.M21 = 0.0f;
			inertiaTensor.M22 = massFactor * (widthSquared + depthSquared);
			inertiaTensor.M23 = 0.0f;
			inertiaTensor.M24 = 0.0f;

			inertiaTensor.M31 = 0.0f;
			inertiaTensor.M32 = 0.0f;
			inertiaTensor.M33 = massFactor * (widthSquared + heightSquared);
			inertiaTensor.M34 = 0.0f;

			inertiaTensor.M41 = 0.0f;
			inertiaTensor.M42 = 0.0f;
			inertiaTensor.M43 = 0.0f;
			inertiaTensor.M44 = 1.0f;
						
			// Adjust the inertia tensor by the offset.
			inertiaTensor = MathsHelper.TransferAxis(inertiaTensor, this.OffsetPosition, mass);
		}

		#endregion

		internal static void GetBoxCorners(ref Vector3 position, ref Matrix orientation, ref Vector3 extents, Vector3[] corners)
		{

			// Calculate the corner positions.
			Vector3 xAxis = MathsHelper.GetAxis(orientation, MatrixAxis.X) * extents.X;
			Vector3 yAxis = MathsHelper.GetAxis(orientation, MatrixAxis.Y) * extents.Y;
			Vector3 zAxis = MathsHelper.GetAxis(orientation, MatrixAxis.Z) * extents.Z;

			Vector3 temp0 = position + xAxis;
			Vector3 temp1 = position - xAxis;
			Vector3 temp2 = yAxis + zAxis;
			Vector3 temp3 = yAxis - zAxis;

			corners[(int)BoxCornerType.RightTopFront] = temp0 + temp2;
			corners[(int)BoxCornerType.RightTopBack] = temp0 + temp3;
			corners[(int)BoxCornerType.RightBottomFront] = temp0 - temp2;
			corners[(int)BoxCornerType.RightBottomBack] = temp0 - temp3;
			corners[(int)BoxCornerType.LeftTopFront] = temp1 + temp2;
			corners[(int)BoxCornerType.LeftTopBack] = temp1 + temp3;
			corners[(int)BoxCornerType.LeftBottomFront] = temp1 - temp2;
			corners[(int)BoxCornerType.LeftBottomBack] = temp1 - temp3;

		}

	}
}
