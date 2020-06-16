using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using GuruEngine.Core;


namespace GuruEngine.Physics.Collision.CollisionSystems
{
	/// <summary>
	/// Defines a octree strategy for broad-phase collision processing.
	/// </summary>
	public class OctreeCollisionSystem : CollisionSystem
	{
		#region OctreeNode Declaration

		/// <summary>
		/// Represents a single quadrant of space with eight equally sized child quadrants.
		/// </summary>
		internal sealed class OctreeNode
		{
			#region CollisionGroupCollection Declaration

			/// <summary>
			/// A collection of <see cref="CollisionGroup"/> instances.
			/// </summary>
			internal sealed class CollisionGroupCollection : Collection<CollisionGroup>
			{
				private OctreeNode octreeNode;

				/// <summary>
				/// Initializes a new instance of the <see cref="CollisionGroupCollection"/> class.
				/// </summary>
				/// <param name="octreeNode">The associated<see cref="QuadtreeNode"/>.</param>
				internal CollisionGroupCollection(OctreeNode octreeNode)
				{
					// Hold onto parameters.
					this.octreeNode = octreeNode;
				}

				protected override void ClearItems()
				{
					// Unassociated the collision groups from the octree node.
					for (int i = 0; i < this.Count; i++)
					{
						this.Items[i].internalTag = null;
					}

					// Update the counts all the way up the chain.
					OctreeNode node = this.octreeNode;
					do
					{
						node.totalCount -= this.Count;
						node = node.parent;
					}
					while (node != null);

					// Call inherited method.
					base.ClearItems();
				}

				protected override void InsertItem(int index, CollisionGroup item)
				{
					// Call inherited method.
					base.InsertItem(index, item);

					// Associate the collision group to the octree node.
					item.internalTag = this.octreeNode;

					// Update the counts all the way up the chain.
					OctreeNode node = this.octreeNode;
					do
					{
						node.totalCount++;
						node = node.parent;
					}
					while (node != null);
				}

				protected override void RemoveItem(int index)
				{
					// Get the remove item.
					CollisionGroup item = this.Items[index];

					// Unassociated the collision group from the octree node.
					item.internalTag = null;

					// Update the counts all the way up the chain.
					OctreeNode node = this.octreeNode;
					do
					{
						node.totalCount--;
						node = node.parent;
					}
					while (node != null);

					// Call inherited method.
					base.RemoveItem(index);
				}

				protected override void SetItem(int index, CollisionGroup item)
				{
					throw new NotSupportedException();
				}
			}

			#endregion

			internal OctreeNode parent;
			private OctreeNode[] nodes;

			private BoundingBox bounds;

			private CollisionGroupCollection collisionGroups;
			private int totalCount;

			/// <summary>
			/// Initializes a new instance of the <see cref="OctreeNode"/> class.
			/// </summary>
			/// <param name="parent">The parent <see cref="OctreeNode"/>.</param>
			/// <param name="bounds">A <see cref="BoundingBox"/> representing the size of the <see cref="OctreeNode"/>.</param>
			/// <param name="depth">The levels left to recursively create.</param>
			internal OctreeNode(OctreeNode parent, BoundingBox bounds, int depth)
			{
				// Create the collision group collection.
				this.collisionGroups = new CollisionGroupCollection(this);

				// Hold onto parameters.
				this.parent = parent;
				this.bounds = bounds;

				if (depth > 0)
				{
					// Decrement the depth.
					depth--;

					// Get half of the x, y and z extents.
					float x = (this.bounds.Max.X - this.bounds.Min.X) * 0.5f;
					float y = (this.bounds.Max.Y - this.bounds.Min.Y) * 0.5f;
					float z = (this.bounds.Max.Z - this.bounds.Min.Z) * 0.5f;

					// Create the child collection of nodes.
					this.nodes = new OctreeNode[8];
					this.nodes[0] = new OctreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X + x, this.bounds.Min.Y + y, this.bounds.Min.Z), new Vector3(this.bounds.Max.X, this.bounds.Max.Y, this.bounds.Max.Z - z)), depth);
					this.nodes[1] = new OctreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X, this.bounds.Min.Y + y, this.bounds.Min.Z), new Vector3(this.bounds.Max.X - x, this.bounds.Max.Y, this.bounds.Max.Z - z)), depth);
					this.nodes[2] = new OctreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X, this.bounds.Min.Y + y, this.bounds.Min.Z + z), new Vector3(this.bounds.Max.X - x, this.bounds.Max.Y, this.bounds.Max.Z)), depth);
					this.nodes[3] = new OctreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X + x, this.bounds.Min.Y + y, this.bounds.Min.Z + z), new Vector3(this.bounds.Max.X, this.bounds.Max.Y, this.bounds.Max.Z)), depth);

					this.nodes[4] = new OctreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X + x, this.bounds.Min.Y, this.bounds.Min.Z), new Vector3(this.bounds.Max.X, this.bounds.Max.Y - y, this.bounds.Max.Z - z)), depth);
					this.nodes[5] = new OctreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X, this.bounds.Min.Y, this.bounds.Min.Z), new Vector3(this.bounds.Max.X - x, this.bounds.Max.Y - y, this.bounds.Max.Z - z)), depth);
					this.nodes[6] = new OctreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X, this.bounds.Min.Y, this.bounds.Min.Z + z), new Vector3(this.bounds.Max.X - x, this.bounds.Max.Y - y, this.bounds.Max.Z)), depth);
					this.nodes[7] = new OctreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X + x, this.bounds.Min.Y, this.bounds.Min.Z + z), new Vector3(this.bounds.Max.X, this.bounds.Max.Y - y, this.bounds.Max.Z)), depth);
				}
				else
				{
					// Create an empty child collection for leaved nodes.
					this.nodes = new OctreeNode[0];
				}
			}

			/// <summary>
			/// Gets the collection of <see cref="CollisionGroup"/> instances within the bounds of the <see cref="OctreeNode"/>.
			/// </summary>
			internal CollisionGroupCollection CollisionGroups
			{
				get { return this.collisionGroups; }
			}

			/// <summary>
			/// Detects collisions with the specified <see cref="CollisionGroup"/> in the <see cref="OctreeNode"/> and
			/// it's children.
			/// </summary>
			/// <param name="collisionSystem">The associated<see cref="CollisionSystem"/>.</param>
			/// <param name="collisionGroup">The source <see cref="CollisionGroup"/>/</param>
			/// <param name="context">The <see cref="CollisionContext"/> to use for collision detection.</param>
			internal void DetectCollisions(OctreeCollisionSystem collisionSystem, CollisionGroup collisionGroup, CollisionContext context)
			{
				// Iterate through all of this node's associated collision groups.
				for (int i = 0; i < this.collisionGroups.Count; i++)
				{
					// Detect collisions.
					collisionSystem.DetectCollisions(collisionGroup, this.collisionGroups[i], context);
				}

				// Iterate through all child nodes.
				for (int i = 0; i < this.nodes.Length; i++)
				{
					// Check for a complete empty octree node hierarchy.
					if (this.nodes[i].totalCount == 0)
					{
						continue;
					}

					// Octree nodes with only one collision group can skip the containment check.
					if (!(this.nodes[i].totalCount == 1 && this.nodes[i].collisionGroups.Count > 0))
					{
						// Check for containment.
						if (collisionGroup.AABB.Max.X > this.nodes[i].bounds.Max.X ||
							collisionGroup.AABB.Min.X < this.nodes[i].bounds.Min.X ||
							collisionGroup.AABB.Max.Y > this.nodes[i].bounds.Max.Y ||
							collisionGroup.AABB.Min.Y < this.nodes[i].bounds.Min.Y ||
							collisionGroup.AABB.Max.Z > this.nodes[i].bounds.Max.Z ||
							collisionGroup.AABB.Min.Z < this.nodes[i].bounds.Min.Z)
						{
							continue;
						}
					}

					// Detect collisions with the current child octree node.
					this.nodes[i].DetectCollisions(collisionSystem, collisionGroup, context);
				}
			}

			/// <summary>
			/// Searches the quadtree for a <see cref="OctreeNode"/> that the specified <see cref="Microsoft.Xna.Framework.BoundinbBox"/> is contained within.
			/// </summary>
			/// <param name="aabb">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
			/// <returns>
			/// A <see cref="OctreeNode"/> that contains the specified <see cref="Microsoft.Xna.Framework.BoundingBox"/>.
			/// </returns>
			internal OctreeNode Search(ref BoundingBox aabb)
			{
				// Continue searching if the octree node encompasses the bounding box.
				if (this.Contains(ref aabb))
				{
					return this.SearchChild(ref aabb);
				}

				// Search the ancestors.
				if (this.parent != null)
				{
					return this.parent.Search(ref aabb);
				}

				// Return this octree node if the bounding box does is not complete contained in any
				// of the child or parent octree nodes.  Normally the root node.
				return this;
			}

			/// <summary>
			/// Searches the child <see cref="OctreeNode"/> for a <see cref="OctreeNode"/> that the specified <see cref="Microsoft.Xna.Framework.BoundinbBox"/> is contained within.
			/// </summary>
			/// <param name="aabb">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
			/// <returns>A <see cref="OctreeNode"/> that contains the specified <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</returns>
			private OctreeNode SearchChild(ref BoundingBox aabb)
			{
				// Iterate on child nodes.
				for (int i = 0; i < this.nodes.Length; i++)
				{
					// Continue searching if the current child encompasses the bounding box.
					if (this.nodes[i].Contains(ref aabb))
					{
						return this.nodes[i].SearchChild(ref aabb);
					}
				}

				// Return this octree node if the bounding box does is not complete contained in any
				// of the child octree nodes.
				return this;
			}

			/// <summary>
			/// Determines is a <see cref="Microsoft.Xna.Framework.BoundingBox"/> is completely contained 
			/// within the <see cref="OctreeNode."/>.
			/// </summary>
			/// <param name="aabb">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
			/// <returns>
			/// <b>true</b> if the <see cref="Microsoft.Xna.Framework.BoundingBox"/> is completely contained 
			/// within the <see cref="OctreeNode."/>.  Otherwise, <b>false</b>.
			/// </returns>
			private bool Contains(ref BoundingBox aabb)
			{
				return
					aabb.Min.X >= this.bounds.Min.X &&
					aabb.Max.X <= this.bounds.Max.X &&
					aabb.Min.Y >= this.bounds.Min.Y &&
					aabb.Max.Y <= this.bounds.Max.Y &&
					aabb.Min.Z >= this.bounds.Min.Z &&
					aabb.Max.Z <= this.bounds.Max.Z;
			}
		}

		#endregion

		private OctreeNode rootNode;

		/// <summary>
		/// Initializes a new instance of the <see cref="OctreeCollisionSystem"/> class.
		/// </summary>
		/// <param name="bounds">A <see cref="Microsoft.Xna.Framework.BoundingBox"/> representing the bounds of the world.</param>
		/// <param name="depth">The levels deep the <see cref="OctreeCollisionSystem"/> should be initialized to.</param>
		public OctreeCollisionSystem(BoundingBox bounds, int depth)
		{
			// Create the root node in the octree.
			this.rootNode = new OctreeNode(null, bounds, depth);
		}

		#region CollisionSystem Members

		public override void DetectCollisions(RigidBody rigidBody, CollisionContext context)
		{
			// Get the associated octree node of the rigid body's collision group.
			OctreeNode node = (OctreeNode)rigidBody.CollisionGroup.internalTag;
			if (node != null)
			{
				// Detect collisions starting from the associated octree node.
				node.DetectCollisions(this, rigidBody.CollisionGroup, context);

				// Iterate on the anscestors of the associated octree node.
				node = node.parent;
				while (node != null)
				{
					// Iterate on the current node's associated collision groups.
					for (int i = 0; i < node.CollisionGroups.Count; i++)
					{
						// Detect collisions (non-recursively).
						this.DetectCollisions(rigidBody.CollisionGroup, node.CollisionGroups[i], context);
					}

					// Next parent.
					node = node.parent;
				}
			}
		}

		public override bool IntersectsWith(ref CollisionRay ray, ICollisionRayCallback callback, out CollisionGroup collisionGroup, out Vector3 position, out Vector3 normal, out float t)
		{
			// TODO : Use octree information to narrow search.
			// Call inherited method.
			return base.IntersectsWith(ref ray, callback, out collisionGroup, out position, out normal, out t);
		}

		protected override void OnCollisionGroupAdded(CollisionGroup collisionGroup)
		{
			// Add the collision group to the octree node that it is contained within.
			this.rootNode.Search(ref collisionGroup.AABB).CollisionGroups.Add(collisionGroup);

			// Call inherited method.
			base.OnCollisionGroupAdded(collisionGroup);
		}

		protected override void OnCollisionGroupRemoved(CollisionGroup collisionGroup)
		{
			// Get the associated octree node of the collision group.
			OctreeNode node = (OctreeNode)collisionGroup.internalTag;
			if (node != null)
			{
				// Remove the collision group from the quadtree node.
				node.CollisionGroups.Remove(collisionGroup);
			}

			// Call inherited method.
			base.OnCollisionGroupRemoved(collisionGroup);
		}

		protected internal override void OnCollisionGroupChanged(CollisionGroup collisionGroup)
		{
			// Disabled rigid bodies don't move.
			if (collisionGroup.RigidBody == null || (collisionGroup.RigidBody.Flags & RigidBodyFlags.Disabled) == RigidBodyFlags.None)
			{
				// Get the associated octree node of the collision group.
				OctreeNode currentNode = (OctreeNode)collisionGroup.internalTag;
				if (currentNode != null)
				{
					// Find the octree node that contains the collision group's axis-aligned bounding box.
					OctreeNode foundNode = currentNode.Search(ref collisionGroup.AABB);
					if (foundNode != currentNode)
					{
						// Remove the collision group from it's old association and add it to the found octree node.
						currentNode.CollisionGroups.Remove(collisionGroup);
						foundNode.CollisionGroups.Add(collisionGroup);
					}
				}
			}

			// Call inherited method.
			base.OnCollisionGroupChanged(collisionGroup);
		}

		#endregion
	}
}
