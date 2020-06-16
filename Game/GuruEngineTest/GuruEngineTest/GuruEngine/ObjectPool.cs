using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuruEngine
{
	/// <summary>
	/// A class that pools instances of the specified type for reuse.
	/// </summary>
	/// <typeparam name="T">The type of the items in the <see cref="ObjectPool"/>.</typeparam>
	public sealed class ObjectPool<T>
	{
		// TODO : This is slower than enforcing a type parameter constraint but this provides a more rebust solution.  Reevaluate performance.
		private static Func<T> defaultCreator = () => Activator.CreateInstance<T>();

		private Func<T> creator;
		private Stack<T> stack;

		/// <summary>
		/// Creates an instance of the <see cref="ObjectPool"/> class.
		/// </summary>
		public ObjectPool() : this(0) {	}

		/// <summary>
		/// Creates an instance of the <see cref="ObjectPool"/> class initialized with the specifed size.
		/// </summary>
		/// <param name="size">An <see cref="System.Int"/> specifying the number of initial items in the <see cref="Pool"/> instance.</param>
		public ObjectPool(int size) : this(size, null) 
		{ 
			// The type must have a public parameterless constructor.

			if (typeof(T).GetConstructor(Type.EmptyTypes) == null)
			{
				throw new ArgumentException("creator");
			}
		}
		
		/// <summary>
		/// Creates an instance of the <see cref="ObjectPool"/> class initialized with the specifed size.
		/// </summary>
		/// <param name="size">An <see cref="System.Int"/> specifying the number of initial items in the <see cref="Pool"/> instance.</param>
		/// <param name="creator">The method used to create new instance of <typeparamref name="T"/>.</param>
		public ObjectPool(int size, Func<T> creator)
		{
			// Hold onto parameters.
			this.creator = creator ?? ObjectPool<T>.defaultCreator;

			// Create the stack with the defined size.
			this.stack = new Stack<T>(size);

			// Create <size> number of items of the specified type.
			for (int i = 0; i < size; i++)
			{
				this.stack.Push(this.creator.Invoke());
			}
		}

		/// <summary>
		/// Retrieves an instances of <c>T</c> from the stack.
		/// </summary>
		/// <returns>Returns an instance of type <c>T</c>.</returns>
		public T Retrieve()
		{
			lock (this.stack)
			{
				// Make sure the stack is not empty.
				if (this.stack.Count > 0)
				{
					// Return the next available instance.
					return this.stack.Pop();
				}
			}

			// Return a new instance when the stack is empty.
			return this.creator.Invoke();
		}

		/// <summary>
		/// Release use of the specified instance of <c>T</c> and gives it back
		/// to the stack.
		/// </summary>
		/// <param name="item"></param>
		public void Recycle(T item)
		{
			lock (this.stack)
			{
				// Put the instance back on the stack.
				this.stack.Push(item);
			}
		}
	}
}
