using HereticalSolutions.Pools.Decorators;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Factory class for creating decorator pools with ID.
    /// </summary>
    public static class IDDecoratorsPoolsFactory
    {
        #region Decorator pools

        /// <summary>
        /// Builds a decorator pool with ID.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="innerPool">The inner pool to be decorated.</param>
        /// <param name="id">The ID of the pool.</param>
        /// <returns>A new instance of PoolWithID&lt;T&gt;.</returns>
        public static PoolWithID<T> BuildPoolWithID<T>(
            IDecoratedPool<T> innerPool,
            string id)
        {
            return new PoolWithID<T>(innerPool, id);
        }

        #endregion

        #region Non alloc decorator pools

        /// <summary>
        /// Builds a non-allocating decorator pool with ID.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="innerPool">The inner non-allocating pool to be decorated.</param>
        /// <param name="id">The ID of the pool.</param>
        /// <returns>A new instance of NonAllocPoolWithID&lt;T&gt;.</returns>
        public static NonAllocPoolWithID<T> BuildNonAllocPoolWithID<T>(
            INonAllocDecoratedPool<T> innerPool,
            string id)
        {
            return new NonAllocPoolWithID<T>(innerPool, id);
        }

        #endregion
    }
}