using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision.Shapes
{
	/// <summary>
	/// Defines a connection for <see cref="CollisionWall"/> instances.
	/// </summary>
	public class CollisionWallConnection
	{
		/// <summary>
		/// Gets or sets the position of the <see cref="CollisionWallConnection"/>.
		/// </summary>
		/// <remarks>
		/// The y component of the vector is not used.
		/// </remarks>
		public Vector3 Position;
	}

	/// <summary>
	/// Defines a collision shape in the form of a wall of infinite height.
	/// </summary>
	public class CollisionWall : CollisionShape
	{
		private CollisionWallConnection start, end;

		/// <summary>
		/// Gets the normal of the collision wall.
		/// </summary>
		/// <remarks>
		/// Do not set this field directly.  Should be considered read-only.
		/// </remarks>
		public Vector3 Normal;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionWall"/> class.
		/// </summary>
		internal protected CollisionWall() : this(null, null) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionWall"/> class.
		/// </summary>
		/// <param name="start">The starting position of the <see cref="CollisionWall"/>.</param>
		/// <param name="end">The ending position of the <see cref="CollisionWall"/>.</param>
		public CollisionWall(CollisionWallConnection start, CollisionWallConnection end) : base(CollisionShapeType.Wall)
		{
			// Hold onto parameters.
			this.start = start;
			this.end = end;
		}

		/// <summary>
		/// Gets or sets the start position of the <see cref="CollisionWall"/>.
		/// </summary>
		[ContentSerializer(SharedResource = true)]
		public CollisionWallConnection Start 
		{ 
			get { return this.start; }
			internal set { this.start = value; }
		}

		/// <summary>
		/// Gets or sets the end position of the <see cref="CollisionWall"/>.
		/// </summary>
		[ContentSerializer(SharedResource = true)]
		public CollisionWallConnection End
		{
			get { return this.end; }
			internal set { this.end = value; }
		}

		#region CollisionShape Members

		internal override void CalculateInternals()
		{
			// A wall is infinite in height.  The y-components of the start and end position are 
			// insignificant.
			//this.Start.Y = 0.0f;
			//this.End.Y = 0.0f;

			// Calculate the normal of the wall.
			Vector3 temp = Vector3.Normalize(this.End.Position - this.Start.Position);
			this.Normal.X = -temp.Z;
			this.Normal.Y = 0.0f;
			this.Normal.Z = temp.X;

			// Call inherited method.
			base.CalculateInternals();
		}

		protected override void CalculateAABB()
		{

			this.AABB = MathsHelper.InfiniteBB;
		}

		private static float Signed2dTriArea(Vector2 a, Vector2 b, Vector2 c)
		{
			return (a.X - c.X) * (b.Y - c.Y) - (a.Y - c.Y) * (b.X - c.X);
		}

		public override bool IntersectsWith(ref CollisionRay ray, out Vector3 position, out Vector3 normal, out float t)
		{
			// Get the end point of the collection ray.

			Vector3 end = ray.GetEndPoint();

			Vector2 a = new Vector2(ray.Position.X,ray.Position.Y);
			Vector2 b = new Vector2(end.X,end.Y);

			Vector2 c = new Vector2(Start.Position.X, Start.Position.Y); 
			Vector2 d = new Vector2(End.Position.X, End.Position.Y); 

			float a1 = Signed2dTriArea(a, b, d);
			float a2 = Signed2dTriArea(a, b, c);
			if (a1 * a2 < 0.0f)
			{
				float a3 = Signed2dTriArea(c, d, a);
				float a4 = a3 + a2 - a1;

				if (a3 * a4 < 0.0f)
				{
					t = a3 / (a3 - a4);

					position = ray.GetPointAt(t);

					// TODO : May have to flip.
					normal = this.Normal;
					return true;
				}
			}

			// The ray does not cross the wall.
			position = Vector3.Zero;
			normal = Vector3.Zero;
			t = 0.0f;

			// There is no intersection.
			return false;
		}

		protected internal override float GetVolume()
		{
			return 0.0f;
		}

		public override void CalculateMassProperties(float density, out float mass, out Matrix inertiaTensor)
		{
			mass = 0.0f;
			inertiaTensor = MathsHelper.Zero;
		}

		#endregion
	}
}
