namespace HereticalSolutions.Pools
{
    /// <summary>
    /// A generic interface for non-allocated object pools.
    /// </summary>
    /// <typeparam name="T">The type of objects stored in the pool.</typeparam>
	public interface INonAllocPool<T>
	{
        /// <summary>
        /// Retrieves an object from the pool.
        /// </summary>
        /// <returns>The object retrieved from the pool.</returns>
		IPoolElement<T> Pop();

        /// <summary>
        /// Returns an object back to the pool.
        /// </summary>
        /// <param name="instance">The object to be returned to the pool.</param>
		void Push(IPoolElement<T> instance);

        /// <summary>
        /// Determines if the pool has any free space.
        /// </summary>
        /// <value><c>true</c> if the pool has free space; otherwise, <c>false</c>.</value>
		bool HasFreeSpace { get; }
	}
}