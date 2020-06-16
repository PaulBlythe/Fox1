using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace GuruEngine.Physics.Collision.CollisionSystems
{
	/// <summary>
	/// Defines a quad-tree strategy for broad-phase collision processing.
	/// </summary>
	public class QuadtreeCollisionSystem : CollisionSystem
	{
		#region QuadtreeNode Declaration

		/// <summary>
		/// Represents a single quadrant of space with four equally sized child quadrants.
		/// </summary>
		internal sealed class QuadtreeNode
		{
			#region CollisionGroupCollection Declaration

			/// <summary>
			/// A collection of <see cref="CollisionGroup"/> instances.
			/// </summary>
			internal sealed class CollisionGroupCollection : Collection<CollisionGroup>
			{
				private QuadtreeNode quadtreeNode;

				/// <summary>
				/// Initializes a new instance of the <see cref="CollisionGroupCollection"/> class.
				/// </summary>
				/// <param name="quadtreeNode">The associated<see cref="QuadtreeNode"/>.</param>
				internal CollisionGroupCollection(QuadtreeNode quadtreeNode)
				{
					// Hold onto parameters.
					this.quadtreeNode = quadtreeNode;
				}

				protected override void ClearItems()
				{
					// Unassociated the collision groups from the quadtree node.
					for (int i = 0; i < this.Count; i++)
					{
						this.Items[i].internalTag = null;
					}

					// Update the counts all the way up the chain.
					QuadtreeNode node = this.quadtreeNode;
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

					// Associate the collision group to the quadtree node.
					item.internalTag = this.quadtreeNode;

					// Update the counts all the way up the chain.
					QuadtreeNode node = this.quadtreeNode;
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

					// Unassociated the collision group from the quadtree node.
					item.internalTag = null;

					// Update the counts all the way up the chain.
					QuadtreeNode node = this.quadtreeNode;
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

			internal QuadtreeNode parent;
			private QuadtreeNode[] nodes;

			private BoundingBox bounds;

			private CollisionGroupCollection collisionGroups;
			private int totalCount;

			/// <summary>
			/// Initializes a new instance of the <see cref="QuadtreeNode"/> class.
			/// </summary>
			/// <param name="parent">The parent <see cref="QuadtreeNode"/>.</param>
			/// <param name="bounds">A <see cref="BoundingBox"/> representing the size of the <see cref="QuadtreeNode"/>.</param>
			/// <param name="depth">The levels left to recursively create.</param>
			internal QuadtreeNode(QuadtreeNode parent, BoundingBox bounds, int depth)
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

					// Get half of the x and z extents.
					float x = (this.bounds.Max.X - this.bounds.Min.X) * 0.5f;
					float z = (this.bounds.Max.Z - this.bounds.Min.Z) * 0.5f;

					// Create the child collection of nodes.
					this.nodes = new QuadtreeNode[4];
					this.nodes[0] = new QuadtreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X + x, this.bounds.Min.Y, this.bounds.Min.Z), new Vector3(this.bounds.Max.X, this.bounds.Max.Y, this.bounds.Max.Z - z)), depth);
					this.nodes[1] = new QuadtreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X, this.bounds.Min.Y, this.bounds.Min.Z), new Vector3(this.bounds.Max.X - x, this.bounds.Max.Y, this.bounds.Max.Z - z)), depth);
					this.nodes[2] = new QuadtreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X, this.bounds.Min.Y, this.bounds.Min.Z + z), new Vector3(this.bounds.Max.X - x, this.bounds.Max.Y, this.bounds.Max.Z)), depth);
					this.nodes[3] = new QuadtreeNode(this, new BoundingBox(new Vector3(this.bounds.Min.X + x, this.bounds.Min.Y, this.bounds.Min.Z + z), new Vector3(this.bounds.Max.X, this.bounds.Max.Y, this.bounds.Max.Z)), depth);
				}
				else
				{
					// Create an empty child collection for leaved nodes.
					this.nodes = new QuadtreeNode[0];
				}
			}

			/// <summary>
			/// Gets the collection of <see cref="CollisionGroup"/> instances within the bounds of the <see cref="QuadtreeNode"/>.
			/// </summary>
			internal CollisionGroupCollection CollisionGroups
			{
				get { return this.collisionGroups; }
			}

			/// <summary>
			/// Detects collisions with the specified <see cref="CollisionGroup"/> in the <see cref="QuadtreeNode"/> and
			/// it's children.
			/// </summary>
			/// <param name="collisionSystem">The associated<see cref="CollisionSystem"/>.</param>
			/// <param name="collisionGroup">The source <see cref="CollisionGroup"/>/</param>
			/// <param name="context">The <see cref="CollisionContext"/> to use for collision detection.</param>
			internal void DetectCollisions(QuadtreeCollisionSystem collisionSystem, CollisionGroup collisionGroup, CollisionContext context)
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
					// Get the current child node.
					QuadtreeNode node = this.nodes[i];

					// Check for a complete empty quadtree node hierarchy.
					if (node.totalCount == 0)
					{
						continue;
					}

					// Quadtree nodes with only one collision group can skip the containment check.
					if (!(node.totalCount == 1 && node.collisionGroups.Count > 0))
					{
						// Check for containment.
						if (collisionGroup.AABB.Max.X > node.bounds.Max.X ||
							collisionGroup.AABB.Min.X < node.bounds.Min.X ||
							collisionGroup.AABB.Max.Z > node.bounds.Max.Z ||
							collisionGroup.AABB.Min.Z < node.bounds.Min.Z)
						{
							continue;
						}
					}

					// Detect collisions with the current child quadtree node.
					node.DetectCollisions(collisionSystem, collisionGroup, context);
				}
			}

			/// <summary>
			/// Searches the quadtree for a <see cref="QuadtreeNode"/> that the specified <see cref="Microsoft.Xna.Framework.BoundinbBox"/> is contained within.
			/// </summary>
			/// <param name="aabb">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
			/// <returns>
			/// A <see cref="QuadtreeNode"/> that contains the specified <see cref="Microsoft.Xna.Framework.BoundingBox"/>.
			/// </returns>
			internal QuadtreeNode Search(ref BoundingBox aabb)
			{
				// Continue searching if the quadtree node encompasses the bounding box.
				if (this.Contains(ref aabb))
				{
					return this.SearchChild(ref aabb);
				}

				// Search the ancestors.
				if (this.parent != null)
				{
					return this.parent.Search(ref aabb);
				}

				// Return this quadtree node if the bounding box does is not complete contained in any
				// of the child or parent quadtree nodes.  Normally the root node.
				return this;
			}

			/// <summary>
			/// Searches the child <see cref="QuadtreeNode"/> for a <see cref="QuadtreeNode"/> that the specified <see cref="Microsoft.Xna.Framework.BoundinbBox"/> is contained within.
			/// </summary>
			/// <param name="aabb">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
			/// <returns>A <see cref="QuadtreeNode"/> that contains the specified <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</returns>
			private QuadtreeNode SearchChild(ref BoundingBox aabb)
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

				// Return this quadtree node if the bounding box does is not complete contained in any
				// of the child quadtree nodes.
				return this;
			}

			/// <summary>
			/// Determines is a <see cref="Microsoft.Xna.Framework.BoundingBox"/> is completely contained 
			/// within the <see cref="QuadtreeNode."/>.
			/// </summary>
			/// <param name="aabb">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
			/// <returns>
			/// <b>true</b> if the <see cref="Microsoft.Xna.Framework.BoundingBox"/> is completely contained 
			/// within the <see cref="QuadtreeNode."/>.  Otherwise, <b>false</b>.
			/// </returns>
			private bool Contains(ref BoundingBox aabb)
			{
				return
					aabb.Min.X >= this.bounds.Min.X &&
					aabb.Max.X <= this.bounds.Max.X &&
					aabb.Min.Z >= this.bounds.Min.Z &&
					aabb.Max.Z <= this.bounds.Max.Z;
			}
		}

		#endregion

		private QuadtreeNode rootNode;

		/// <summary>
		/// Initializes a new instance of the <see cref="QuadtreeCollisionSystem"/> class.
		/// </summary>
		/// <param name="bounds">A <see cref="Microsoft.Xna.Framework.BoundingBox"/> representing the bounds of the world.</param>
		/// <param name="depth">The levels deep the <see cref="QuadtreeCollisionSystem"/> should be initialized to.</param>
		public QuadtreeCollisionSystem(BoundingBox bounds, int depth)
		{
			// Create the root node in the quad-tree.
			this.rootNode = new QuadtreeNode(null, bounds, depth);
		}

		#region CollisionSystem Members

		public override void DetectCollisions(RigidBody rigidBody, CollisionContext context)
		{
			// Get the associated quadtree node of the rigid body's collision group.
			QuadtreeNode node = (QuadtreeNode)rigidBody.CollisionGroup.internalTag;
			if (node != null)
			{
				// Detect collisions starting from the associated quadtree node.
				node.DetectCollisions(this, rigidBody.CollisionGroup, context);

				// Iterate on the anscestors of the associated quadtree node.
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
			// TODO : Use quadtree information to narrow search.
			// Call inherited method.
			return base.IntersectsWith(ref ray, callback, out collisionGroup, out position, out normal, out t);
		}

		protected override void OnCollisionGroupAdded(CollisionGroup collisionGroup)
		{
			// Add the collision group to the quadtree node that it is contained within.
			this.rootNode.Search(ref collisionGroup.AABB).CollisionGroups.Add(collisionGroup);

			// Call inherited method.
			base.OnCollisionGroupAdded(collisionGroup);
		}

		protected override void OnCollisionGroupRemoved(CollisionGroup collisionGroup)
		{
			// Get the associated quadtree node of the collision group.
			QuadtreeNode node = (QuadtreeNode)collisionGroup.internalTag;
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
				// Get the associated quadtree node of the collision group.
				QuadtreeNode currentNode = (QuadtreeNode)collisionGroup.internalTag;
				if (currentNode != null)
				{
					// Find the quadtree node that contains the collision group's axis-aligned bounding box.
					QuadtreeNode foundNode = currentNode.Search(ref collisionGroup.AABB);
					if (foundNode != currentNode)
					{
						// Remove the collision group from it's old association and add it to the found quadtree node.
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
