namespace HereticalSolutions.Pools.Factories
{
	/// <summary>
    /// Represents a chain of non-allocating decorator pools.
    /// </summary>
    /// <typeparam name="T">The type of objects in the pools.</typeparam>
	public class NonAllocDecoratorPoolChain<T>
	{
	    /// <summary>
	    /// Gets or sets the topmost wrapper in the pool chain.
	    /// </summary>
		public INonAllocDecoratedPool<T> TopWrapper { get; private set; }

		/// <summary>
		/// Adds a new wrapper to the pool chain.
		/// </summary>
		/// <param name="newWrapper">The new wrapper to add.</param>
		/// <returns>The updated pool chain.</returns>
		public NonAllocDecoratorPoolChain<T> Add(INonAllocDecoratedPool<T> newWrapper)
		{
			TopWrapper = newWrapper;

			return this;
		}

		/// <summary>
		/// Adds a new wrapper to the pool chain and returns it through an out parameter.
		/// </summary>
		/// <param name="newWrapper">The new wrapper to add.</param>
		/// <param name="wrapperOut">The new wrapper that was added.</param>
		/// <returns>The updated pool chain.</returns>
		public NonAllocDecoratorPoolChain<T> Add(
			INonAllocDecoratedPool<T> newWrapper,
			out INonAllocDecoratedPool<T> wrapperOut)
		{
			TopWrapper = newWrapper;

			wrapperOut = newWrapper;

			return this;
		}
	}
}