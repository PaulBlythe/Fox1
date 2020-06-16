using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GuruEngine.Core;
using GuruEngine.Helpers;

namespace GuruEngine.Physics.Collision.CollisionSystems
{
	/// <summary>
	/// Defines a sweep and prune strategy for broad-phase collision processing.
	/// </summary>
	public class SweepAndPruneCollisionSystem : CollisionSystem
	{
		#region SweepMarker Declaration

		/// <summary>
		/// Represents a start or end marker for the bounds of a <see cref="CollisionGroup"/> for one axis.
		/// </summary>
		internal sealed class SweepMarker
		{
			private static ObjectPool<SweepMarker> pool = new ObjectPool<SweepMarker>(256);

			internal SweepAndPruneNode node;
			internal CollisionGroup CollisionGroup;
			internal float Position;
			internal bool IsUpperMarker;

			/// <summary>
			/// Gets a <see cref="SweepMarker" /> instance from the pool.
			/// </summary>
			/// <returns>Returns a fresh <see cref="SweepMarker"/> instance.</returns>
			internal static SweepMarker Retrieve()
			{
				// Return an unused sweep marker from the pool.
				return SweepMarker.pool.Retrieve();
			}

			/// <summary>
			/// Returns a <see cref="SweepMarker"/> instance to the pool.
			/// </summary>
			/// <param name="sweepMarker">The <see cref="SweepMarker"/> to recycle.</param>
			internal static void Recycle(SweepMarker sweepMarker)
			{
				// Uninitialize.
				sweepMarker.CollisionGroup = null;
				sweepMarker.node = null;

				// Put the sweep marker back in the pool for later use.
				SweepMarker.pool.Recycle(sweepMarker);
			}
		}

		#endregion

		#region SweepAndPruneNode Declaration

		/// <summary>
		/// Holds the sweep intervals and overlaps for a <see cref="CollisionGroup"/>.
		/// </summary>
		internal sealed class SweepAndPruneNode
		{
			private static ObjectPool<SweepAndPruneNode> pool = new ObjectPool<SweepAndPruneNode>(128);

			internal CollisionGroup collisionGroup;

			internal int[] lowerIndices = new int[3];
			internal int[] upperIndices = new int[3];

			internal List<SweepAndPruneNode> overlappingNodes = new List<SweepAndPruneNode>(16);

			/// <summary>
			/// Gets a <see cref="SweepAndPruneNode" /> instance from the pool.
			/// </summary>
			/// <param name="collisionGroup">The associated <see cref="CollisionGroup"/>.</param>
			/// <returns>Returns a fresh <see cref="SweepAndPruneNode"/> instance.</returns>
			internal static SweepAndPruneNode Retrieve(CollisionGroup collisionGroup)
			{
				// Gets an unused sweep and prune node from the pool.
				SweepAndPruneNode node = SweepAndPruneNode.pool.Retrieve();
				node.collisionGroup = collisionGroup;

				// Return the node.
				return node;
			}

			/// <summary>
			/// Returns a <see cref="SweepAndPruneNode"/> instance to the pool.
			/// </summary>
			/// <param name="node">The <see cref="SweepAndPruneNode"/> to recycle.</param>
			internal static void Recycle(SweepAndPruneNode node)
			{
				// Uninitialize.
				node.collisionGroup = null;
				node.overlappingNodes.Clear();

				// Put the sweep and prune node in the pool for later use.
				SweepAndPruneNode.pool.Recycle(node);
			}

			/// <summary>
			/// Determines if the specified <see cref="SweepAndPruneNode"/> instances overlap.
			/// </summary>
			/// <param name="node0">The first <see cref="SweepAndPruneNode"/>.</param>
			/// <param name="node1">The second <see cref="SweepAndPruneNode"/>.</param>
			/// <param name="ignoreAxis">The axis to ignore when comparing sweep intervals.</param>
			/// <returns><b>true</b> if the nodes overlap. Otherwise, <b>false</b>.</returns>
			internal static bool Overlaps(SweepAndPruneNode node0, SweepAndPruneNode node1, int ignoreAxis)
			{
				for (int axis = 0; axis < 3; axis++)
				{
					if (axis != ignoreAxis)
					{
						if (node0.upperIndices[axis] < node1.lowerIndices[axis] ||
							node1.upperIndices[axis] < node0.lowerIndices[axis])
						{
							return false;
						}
					}
				}

				return true;
			}
		}

		#endregion

		private List<SweepAndPruneNode> nodes = new List<SweepAndPruneNode>(128);
		private List<SweepMarker>[] sweepMarkers = new List<SweepMarker>[3];

		/// <summary>
		/// Initializes a new instance of the <see cref="SweepAndPruneCollisionSystem"/> class.
		/// </summary>
		public SweepAndPruneCollisionSystem() : base()
		{
			// Add a dummy node to keep the infinite bounds.
			SweepAndPruneNode node = SweepAndPruneNode.Retrieve(null);
			this.nodes.Add(node);

			// Iterate on x, y, z.
			for (int axis = 0; axis < 3; axis++)
			{
				// Create the sweep markers collection for this axis.
				this.sweepMarkers[axis] = new List<SweepMarker>();

				// Create the start marker for the interval of the infinite bounds.
				this.sweepMarkers[axis].Add(SweepMarker.Retrieve());
				this.sweepMarkers[axis][0].node = node;
				this.sweepMarkers[axis][0].CollisionGroup = null;
				this.sweepMarkers[axis][0].Position = float.MinValue;

				// Create the end marker for the interval of the infinite bounds.
				this.sweepMarkers[axis].Add(SweepMarker.Retrieve());
				this.sweepMarkers[axis][1].node = node;
				this.sweepMarkers[axis][1].CollisionGroup = null;
				this.sweepMarkers[axis][1].Position = float.MaxValue;
				this.sweepMarkers[axis][1].IsUpperMarker = true;

				// Update the sweep and prune node's marker indices.
				node.lowerIndices[axis] = 0;
				node.upperIndices[axis] = 1;
			}
		}

		#region CollisionSystem Members

		public override void DetectCollisions(RigidBody rigidBody, CollisionContext context)
		{
			// Get the assoicated collision groups sweep and prune node.
			SweepAndPruneNode node = (SweepAndPruneNode)rigidBody.CollisionGroup.internalTag;
			if (node != null)
			{
				// Iterate on the sweep and prune node's overlapping nodes.
				for (int i = 0; i < node.overlappingNodes.Count; i++)
				{
					// Detect collisions.
					this.DetectCollisions(rigidBody.CollisionGroup, node.overlappingNodes[i].collisionGroup, context);
				}
			}
		}

		protected override void OnCollisionGroupAdded(CollisionGroup collisionGroup)
		{
			// Get a new node and associate it with the collision group.
			SweepAndPruneNode node = SweepAndPruneNode.Retrieve(collisionGroup);
			collisionGroup.internalTag = node;

			// Add the new node to the collection.
			this.nodes.Add(node);

			// There should be twice as many sweep markers as there are nodes for each dimension.
			int markerCount = this.nodes.Count << 1;

			// Iterate on x, y, z.
			for (int axis = 0; axis < 3; axis++)
			{
				// Check if more sweep markers need to be added.
				if (this.sweepMarkers[axis].Count < markerCount)
				{
					this.sweepMarkers[axis].Add(SweepMarker.Retrieve());
					this.sweepMarkers[axis].Add(SweepMarker.Retrieve());
				}

				// Two more markers have been added to this axis' sweep.  Update the upper marker
				// index for the sweep that spans infinitely to reflect this.
				this.nodes[0].upperIndices[axis] += 2;

				// ...and move the marker as well.  The index and upper sweep marker are now in sync
				// and the new sweep markers are just inside the infinite bounds.
				SweepMarker temp = this.sweepMarkers[axis][markerCount - 1];
				this.sweepMarkers[axis][markerCount - 1] = this.sweepMarkers[axis][markerCount - 3];
				this.sweepMarkers[axis][markerCount - 3] = temp;

				// Match this axis' lower marker index with the new marker.
				node.lowerIndices[axis] = markerCount - 3;

				// Initialize the new lower sweep marker.
				this.sweepMarkers[axis][markerCount - 3].node = node;
				this.sweepMarkers[axis][markerCount - 3].CollisionGroup = collisionGroup;
                this.sweepMarkers[axis][markerCount - 3].Position = MathsHelper.GetVector3Component(collisionGroup.AABB.Min, (VectorIndex)axis);
				this.sweepMarkers[axis][markerCount - 3].IsUpperMarker = false;

				// Match this axis' upper marker index with the new marker.
				node.upperIndices[axis] = markerCount - 2;

				// Initialize the new upper sweep marker.
				this.sweepMarkers[axis][markerCount - 2].node = node;
				this.sweepMarkers[axis][markerCount - 2].CollisionGroup = collisionGroup;
				this.sweepMarkers[axis][markerCount - 2].Position = MathsHelper.GetVector3Component(collisionGroup.AABB.Max , (VectorIndex)axis);
				this.sweepMarkers[axis][markerCount - 2].IsUpperMarker = true;
			}

			// Sort the new lower and upper markers to the correct positions in the axis sweeps.
			this.SortLowerDown(0, node.lowerIndices[0], false);
			this.SortUpperDown(0, node.upperIndices[0], false);
			this.SortLowerDown(1, node.lowerIndices[1], false);
			this.SortUpperDown(1, node.upperIndices[1], false);
			this.SortLowerDown(2, node.lowerIndices[2], true);
			this.SortUpperDown(2, node.upperIndices[2], true);

			// Call inherited method.
			base.OnCollisionGroupAdded(collisionGroup);
		}

		protected override void OnCollisionGroupRemoved(CollisionGroup collisionGroup)
		{
			// Get the collision group's assocated sweep and prune node.
			SweepAndPruneNode node = (SweepAndPruneNode)collisionGroup.internalTag;
			if (node != null)
			{
				// Remove the associated node from all the other nodes' overlap collection.
				for (int i = 0; i < node.overlappingNodes.Count; i++)
				{
					node.overlappingNodes[i].overlappingNodes.Remove(node);
				}

				// Remove the associated node from the collection.
				this.nodes.Remove(node);

				// Recalculate the sweep marker count for each dimension.
				int markerCount = this.nodes.Count << 1;

				// Update the upper marker indices of each dimension to exclude the removed sweep markers.
				this.nodes[0].upperIndices[0] -= 2;
				this.nodes[0].upperIndices[1] -= 2;
				this.nodes[0].upperIndices[2] -= 2;

				// Iterate on x, y, z.
				for (int axis = 0; axis < 3; axis++)
				{
					// Get the sweep markers for the current axis.
					List<SweepMarker> axisMarkers = this.sweepMarkers[axis];

					// Update the position of the removing node's upper marker to
					// sort it to the end of the list.
					int upperIndex = node.upperIndices[axis];
					axisMarkers[upperIndex].Position = float.MaxValue;

					// Sort the upper bounds.
					this.SortUpperUp(axis, upperIndex, false);

					// Update the position of the removing node's lower marker to
					// sort it th the end of the list.
					int lowerIndex = node.lowerIndices[axis];
					axisMarkers[lowerIndex].Position = float.MaxValue;

					// Sort the lower bounds.
					this.SortLowerUp(axis, lowerIndex, false);

					// Update the last valid marker to encompass all sweeps.
					axisMarkers[markerCount - 1].CollisionGroup = null;
					axisMarkers[markerCount - 1].Position = float.MaxValue;
				}

				// Recycle the sweep and prune node.
				SweepAndPruneNode.Recycle(node);
				collisionGroup.internalTag = null;
			}

			// Call inherited method.
			base.OnCollisionGroupRemoved(collisionGroup);
		}

		protected internal override void OnCollisionGroupChanged(CollisionGroup collisionGroup)
		{
			// Get the associated sweep and prune node from the collision group.
			SweepAndPruneNode node = (SweepAndPruneNode)collisionGroup.internalTag;
			if (node != null)
			{
				// Iterate on x, y, z.
				for (int axis = 0; axis < 3; axis++)
				{
					// Get the lower and upper marker indices for the current axis from the node.
					int lowerIndex = node.lowerIndices[axis];
					int upperIndex = node.upperIndices[axis];

					// Get the new lower and upper bounds for the current axis for the collision group.
					float newLower = MathsHelper.GetVector3Component(collisionGroup.AABB.Min, (VectorIndex)axis);
					float newUpper = MathsHelper.GetVector3Component(collisionGroup.AABB.Max, (VectorIndex)axis);

					// Get the change in the lower and upper bounds from the current values.
					float lowerDifference = newLower - this.sweepMarkers[axis][lowerIndex].Position;
					float upperDifference = newUpper - this.sweepMarkers[axis][upperIndex].Position;

					// Update the the lower and upper markers values.
					this.sweepMarkers[axis][lowerIndex].Position = newLower;
					this.sweepMarkers[axis][upperIndex].Position = newUpper;

					// Sort the new lower and upper markers back into place.
					if (lowerDifference < 0.0f)
					{
						this.SortLowerDown(axis, lowerIndex, true);
					}
					if (upperDifference > 0.0f)
					{
						this.SortUpperUp(axis, upperIndex, true);
					}
					if (lowerDifference > 0.0f)
					{
						this.SortLowerUp(axis, lowerIndex, true);
					}
					if (upperDifference < 0.0f)
					{
						this.SortUpperDown(axis, upperIndex, true);
					}
				}
			}

			// Call inherited method.
			base.OnCollisionGroupChanged(collisionGroup);
		}

		#endregion

		/// <summary>
		/// Moves the lower marker down the list into the correct position.
		/// </summary>
		/// <param name="axis">The axis whose sweeps will be updated.</param>
		/// <param name="index">The index of the lower marker to move into position.</param>
		/// <param name="updateOverlaps">Determines if overlap updating should occur.</param>
		private void SortLowerDown(int axis, int index, bool updateOverlaps)
		{
			// Get the sweep markers for the axis being operated on.
			List<SweepMarker> axisMarkers = this.sweepMarkers[axis];

			// Get the current and previous sweep markers from the specified index.
			SweepMarker currentMarker = axisMarkers[index];
			SweepMarker previousMarker = axisMarkers[index - 1];

			// Process until marker specified by the index is in position. 
			while (currentMarker.Position < previousMarker.Position)
			{
				// Determine if overlaps occur because of the marker movement.
				if (previousMarker.IsUpperMarker)
				{
					if (updateOverlaps && SweepAndPruneNode.Overlaps(currentMarker.node, previousMarker.node, axis))
					{
						if (!currentMarker.node.overlappingNodes.Contains(previousMarker.node))
						{
							currentMarker.node.overlappingNodes.Add(previousMarker.node);
						}

						if (!previousMarker.node.overlappingNodes.Contains(currentMarker.node))
						{
							previousMarker.node.overlappingNodes.Add(currentMarker.node);
						}
					}

					// Update the previous marker's sweep and prune node's upper index to reflect 
					// the marker's movement upwards.
					previousMarker.node.upperIndices[axis]++;
				}
				else
				{
					// Update the previous marker's sweep and prune node's lower index to reflect 
					// the marker's movement upwards.
					previousMarker.node.lowerIndices[axis]++;
				}

				// Update the current marker's sweep and prune node's lower index to reflect 
				// the marker's movement downwards.
				currentMarker.node.lowerIndices[axis]--;

				// Exchange marker positions in the axis' sweep markers.
				axisMarkers[index] = previousMarker;
				axisMarkers[index - 1] = currentMarker;

				// Get the next iteration's current and previous markers.
				index--;
				currentMarker = axisMarkers[index];
				previousMarker = axisMarkers[index - 1];
			}
		}

		/// <summary>
		/// Moves the lower marker up the list into the correct position.
		/// </summary>
		/// <param name="axis">The axis whose sweeps will be updated.</param>
		/// <param name="index">The index of the lower marker to move into position.</param>
		/// <param name="updateOverlaps">Determines if overlap updating should occur.</param>
		private void SortLowerUp(int axis, int index, bool updateOverlaps)
		{
			// Get the sweep markers for the axis being operated on.
			List<SweepMarker> axisMarkers = this.sweepMarkers[axis];

			// Get the current and next sweep markers from the specified index.
			SweepMarker currentMarker = axisMarkers[index];
			SweepMarker nextMarker = axisMarkers[index + 1];

			// Process until marker specified by the index is in position.
			while (nextMarker.CollisionGroup != null && currentMarker.Position >= nextMarker.Position)
			{
				if (nextMarker.IsUpperMarker)
				{
					// Determine if overlaps don't exist anymore because of the marker movement.
					if (updateOverlaps)
					{
						currentMarker.node.overlappingNodes.Remove(nextMarker.node);
						nextMarker.node.overlappingNodes.Remove(currentMarker.node);
					}

					// Update the next marker's sweep and prune node's upper index to reflect 
					// the marker's movement downwards.
					nextMarker.node.upperIndices[axis]--;
				}
				else
				{
					// Update the next marker's sweep and prune node's lower index to reflect 
					// the marker's movement downwards.
					nextMarker.node.lowerIndices[axis]--;
				}

				// Update the current marker's sweep and prune node's lower index to reflect 
				// the marker's movement upwards.
				currentMarker.node.lowerIndices[axis]++;

				// Exchange marker positions in the axis' sweep markers.
				axisMarkers[index] = nextMarker;
				axisMarkers[index + 1] = currentMarker;

				// Get the next iteration's current and next markers.
				index++;
				currentMarker = axisMarkers[index];
				nextMarker = axisMarkers[index + 1];
			}
		}

		/// <summary>
		/// Moves the upper marker down the list into the correct position.
		/// </summary>
		/// <param name="axis">The axis whose sweeps will be updated.</param>
		/// <param name="index">The index of the upper marker to move into position.</param>
		/// <param name="updateOverlaps">Determines if overlap updating should occur.</param>
		private void SortUpperDown(int axis, int index, bool updateOverlaps)
		{
			// Get the sweep markers for the axis being operated on.
			List<SweepMarker> axisMarkers = this.sweepMarkers[axis];

			// Get the current and previous sweep markers from the specified index.
			SweepMarker currentMarker = axisMarkers[index];
			SweepMarker previousMarker = axisMarkers[index - 1];

			// Process until marker specified by the index is in position. 
			while (currentMarker.Position < previousMarker.Position)
			{
				if (!previousMarker.IsUpperMarker)
				{
					// Determine if overlaps don't exist anymore because of the marker movement.
					if (updateOverlaps)
					{
						currentMarker.node.overlappingNodes.Remove(previousMarker.node);
						previousMarker.node.overlappingNodes.Remove(currentMarker.node);
					}

					// Update the previous marker's sweep and prune node's lower index to reflect 
					// the marker's movement upwards.
					previousMarker.node.lowerIndices[axis]++; ;
				}
				else
				{
					// Update the previous marker's sweep and prune node's upper index to reflect 
					// the marker's movement upwards.
					previousMarker.node.upperIndices[axis]++;
				}

				// Update the current marker's sweep and prune node's upper index to reflect 
				// the marker's movement downwards.
				currentMarker.node.upperIndices[axis]--;

				// Exchange marker positions in the axis' sweep markers.
				axisMarkers[index] = previousMarker;
				axisMarkers[index - 1] = currentMarker;

				// Get the next iteration's current and previous markers.
				index--;
				currentMarker = axisMarkers[index];
				previousMarker = axisMarkers[index - 1];
			}
		}

		/// <summary>
		/// Moves the upper marker up the list into the correct position.
		/// </summary>
		/// <param name="axis">The axis whose sweeps will be updated.</param>
		/// <param name="index">The index of the upper marker to move into position.</param>
		/// <param name="updateOverlaps">Determines if overlap updating should occur.</param>
		private void SortUpperUp(int axis, int index, bool updateOverlaps)
		{
			// Get the sweep markers for the axis being operated on.
			List<SweepMarker> axisMarkers = this.sweepMarkers[axis];

			// Get the current and next sweep markers from the specified index.
			SweepMarker currentMarker = axisMarkers[index];
			SweepMarker nextMarker = axisMarkers[index + 1];

			// Process until marker specified by the index is in position.
			while (nextMarker.CollisionGroup != null && currentMarker.Position >= nextMarker.Position)
			{
				if (!nextMarker.IsUpperMarker)
				{
					// Determine if overlaps occur because of the marker movement.
					if (updateOverlaps && SweepAndPruneNode.Overlaps(currentMarker.node, nextMarker.node, axis))
					{
						if (!currentMarker.node.overlappingNodes.Contains(nextMarker.node))
						{
							currentMarker.node.overlappingNodes.Add(nextMarker.node);
						}

						if (!nextMarker.node.overlappingNodes.Contains(currentMarker.node))
						{
							nextMarker.node.overlappingNodes.Add(currentMarker.node);
						}
					}

					// Update the next marker's sweep and prune node's lower index to reflect 
					// the marker's movement downwards.
					nextMarker.node.lowerIndices[axis]--;
				}
				else
				{
					// Update the next marker's sweep and prune node's upper index to reflect 
					// the marker's movement downwards.
					nextMarker.node.upperIndices[axis]--;
				}

				// Update the current marker's sweep and prune node's upper index to reflect 
				// the marker's movement upwards.
				currentMarker.node.upperIndices[axis]++;

				// Exchange marker positions in the axis' sweep markers.
				axisMarkers[index] = nextMarker;
				axisMarkers[index + 1] = currentMarker;

				// Get the next iteration's current and next markers.
				index++;
				currentMarker = axisMarkers[index];
				nextMarker = axisMarkers[index + 1];
			}
		}

		private bool FindOverlap(int axis, float position, out int lowerIndex, out int upperIndex)
		{
			int count = 0;

			// Get the sweep markers for the axis being operated on.
			List<SweepMarker> axisMarkers = this.sweepMarkers[axis];

			lowerIndex = 0;
			while (lowerIndex < axisMarkers.Count && axisMarkers[lowerIndex].Position < position)
			{
				lowerIndex++;
				count++;
			}

			upperIndex = axisMarkers.Count - 1;
			while (upperIndex >= 0 && axisMarkers[upperIndex].Position > position)
			{
				upperIndex--;
				count++;
			}

			return count > 1;
		}
	}
}
