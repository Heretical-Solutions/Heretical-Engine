using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class IDDecoratorsPoolsFactory
    {
        #region Decorator pools

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
            string id,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocPoolWithID<T>>()
                ?? null;

            return new NonAllocPoolWithID<T>(
                innerPool,
                id,
                logger);
        }

        #endregion
    }
}