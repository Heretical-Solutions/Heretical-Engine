namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents a callback interface for allocation events in a pool.
    /// </summary>
    /// <typeparam name="T">The type of object being allocated.</typeparam>
    public interface IAllocationCallback<T>
    {
        /// <summary>
        /// Called when an object is allocated from the pool.
        /// </summary>
        /// <param name="poolElement">The allocated object.</param>
        void OnAllocated(IPoolElement<T> poolElement);
    }
}