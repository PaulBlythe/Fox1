using System;
using System.Collections.Generic;
using System.Text;
using GuruEngine.Physics.Collision.CollisionDetectors;

namespace GuruEngine.Physics.Collision
{
	/// <summary>
	/// Defines the <see cref="CollisionShape"/> type for <see cref="CollisionDetector"/> determination.
	/// </summary>
	internal enum CollisionShapeType
	{
		None = 0,
		Plane = 1,
		Sphere = 2,
		Box = 4,
		Mesh = 8,
		Wall = 16,
		Triangle = 32 // TODO : Potentially causes hashing inefficiencies.
	}

	/// <summary>
	/// Defines the collision detection strategy between two <see cref="CollisionShape"/> instances.
	/// </summary>
	public abstract class CollisionDetector
	{
		#region CollisionDetectorKey Declaration

		/// <summary>
		/// Represents the key for looking up collision detectors.
		/// </summary>
		private struct CollisionDetectorKey
		{
			/// <summary>
			/// Gets or sets the first <see cref="CollisionShape"/> type id.
			/// </summary>
			internal int Type0;

			/// <summary>
			/// Gets or sets the second <see cref="CollisionShape"/> type id.
			/// </summary>
			internal int Type1;

			/// <summary>
			/// Initializes a new instance of the <see cref="CollisionDetectorKey"/> structure.
			/// </summary>
			/// <param name="type0">The first <see cref="CollisionShape"/> type id.</param>
			/// <param name="type1">The second <see cref="CollisionShape"/> type id.</param>
			internal CollisionDetectorKey(int type0, int type1)
			{
				// Hold onto parameters
				this.Type0 = type0;
				this.Type1 = type1;
			}

			public override int GetHashCode()
			{
				return this.Type0 + (this.Type1 << 16);
			}

			public override bool Equals(object obj)
			{
				if (obj is CollisionDetectorKey)
				{
					CollisionDetectorKey key = (CollisionDetectorKey)obj;
					return this.Type0 == key.Type0 && this.Type1 == key.Type1;
				}

				return false;
			}
		}

		#endregion

		#region CollisionDetectorKeyEqualityComparer Declaration

		/// <summary>
		/// Defines methods to support the comparison of <see cref="CollisionDetectorKey"/> instances.
		/// </summary>
		private class CollisionDetectorKeyEqualityComparer : IEqualityComparer<CollisionDetectorKey>
		{
			#region IEqualityComparer<CollisionDetectorKey> Members

			public bool Equals(CollisionDetectorKey x, CollisionDetectorKey y)
			{
				return x.Type0 == y.Type0 && x.Type1 == y.Type1;
			}

			public int GetHashCode(CollisionDetectorKey obj)
			{
				return obj.Type0 + (obj.Type1 << 16);
			}

			#endregion
		}

		#endregion

		private static Dictionary<CollisionDetectorKey, CollisionDetector> collisionDetectors = new Dictionary<CollisionDetectorKey, CollisionDetector>(new CollisionDetectorKeyEqualityComparer());

		/// <summary>
		/// Initializes the staic instance of the <see cref="CollisionDetector"/> class.
		/// </summary>
		static CollisionDetector()
		{
			// Get collision shape types for keying.
			int plane = (int)CollisionShapeType.Plane;
			int sphere = (int)CollisionShapeType.Sphere;
			int box = (int)CollisionShapeType.Box;
			int mesh = (int)CollisionShapeType.Mesh;
			
			// Index collision detectors on simple bitwise operation of type ids.
			CollisionDetector.collisionDetectors.Add(new CollisionDetectorKey(sphere, plane), new SpherePlaneCollisionDetector());
			CollisionDetector.collisionDetectors.Add(new CollisionDetectorKey(sphere, sphere), new SphereSphereCollisionDetector());
			CollisionDetector.collisionDetectors.Add(new CollisionDetectorKey(sphere, mesh), new SphereMeshCollisionDetector());
			CollisionDetector.collisionDetectors.Add(new CollisionDetectorKey(box, plane), new BoxPlaneCollisionDetector());
			CollisionDetector.collisionDetectors.Add(new CollisionDetectorKey(box, sphere), new BoxSphereCollisionDetector());
			CollisionDetector.collisionDetectors.Add(new CollisionDetectorKey(box, box), new BoxBoxCollisionDetector());
			CollisionDetector.collisionDetectors.Add(new CollisionDetectorKey(box, mesh), new BoxMeshCollisionDetector());
		}

		/// <summary>
		/// Delegates the collision processing to the correct <see cref="CollisionDetector"/> instance.
		/// </summary>
		/// <param name="shape0">The first <see cref="CollisionShape"/> involved in the collision.</param>
		/// <param name="shape1">The second <see cref="CollisionShape"/> involved in the collision.</param>
		/// <param name="context">The <see cref="CollisionContext"/> to use for collision detection.</param>
		internal static void DetectCollisions(CollisionShape shape0, CollisionShape shape1, CollisionContext context)
		{
			// Get the shapes' type ids.
			int type0 = shape0.typeId;
			int type1 = shape1.typeId;

			// If a valid collision detector is found for the collision shapes specified, use it.
			CollisionDetector collisionDetector = null;
			if (CollisionDetector.collisionDetectors.TryGetValue(new CollisionDetectorKey(type0, type1), out collisionDetector))
			{
				collisionDetector.Detect(shape0, shape1, context);
			}
			else if (CollisionDetector.collisionDetectors.TryGetValue(new CollisionDetectorKey(type1, type0), out collisionDetector))
			{
				collisionDetector.Detect(shape1, shape0, context);
			}
		}

		/// <summary>
		/// Determines if a collision occurred between the two specified <see cref="CollisionShape"/> instances.
		/// </summary>
		/// <param name="shape0">The first <see cref="CollisionShape"/> involved in the collision.</param>
		/// <param name="shape1">The second <see cref="CollisionShape"/> involved in the collision.</param>
		/// <param name="context">The <see cref="CollisionContext"/> to use for collision detection.</param>
		public abstract void Detect(CollisionShape shape0, CollisionShape shape1, CollisionContext context);
	}
}
