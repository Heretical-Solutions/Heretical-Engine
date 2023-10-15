namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents a pool of objects of type T.
    /// </summary>
    /// <typeparam name="T">The type of objects in the pool.</typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// Retrieves an object from the pool.
        /// </summary>
        /// <returns>The retrieved object.</returns>
        T Pop();

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="instance">The object to return to the pool.</param>
        void Push(T instance);
        
        /// <summary>
        /// Gets a value indicating whether the pool has free space.
        /// </summary>
        bool HasFreeSpace { get; }
    }
}