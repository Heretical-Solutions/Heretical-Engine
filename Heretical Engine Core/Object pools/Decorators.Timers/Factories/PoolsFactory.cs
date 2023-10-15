using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Time;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Factory for creating decorator pools related to timers.
    /// </summary>
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Decorator pools

        // No XML comments for this region.

        #endregion

        #region Non alloc decorator pools

        /// <summary>
        /// Builds a non-allocating pool with runtime timer decorator.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="innerPool">The inner pool to decorate.</param>
        /// <param name="synchronizationProvider">The synchronization provider used by the timer.</param>
        /// <returns>A new instance of the NonAllocPoolWithRuntimeTimer decorator.</returns>
        public static NonAllocPoolWithRuntimeTimer<T> BuildNonAllocPoolWithRuntimeTimer<T>(
            INonAllocDecoratedPool<T> innerPool,
            ISynchronizationProvider synchronizationProvider)
        {
            return new NonAllocPoolWithRuntimeTimer<T>(innerPool, synchronizationProvider);
        }
        
        #endregion
    }
}