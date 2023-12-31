using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Decorator pools

        // No XML comments for this region.

        #endregion

        #region Non alloc decorator pools

        public static NonAllocPoolWithRuntimeTimer<T> BuildNonAllocPoolWithRuntimeTimer<T>(
            INonAllocDecoratedPool<T> innerPool,
            ISynchronizationProvider synchronizationProvider,
            IFormatLogger logger)
        {
            return new NonAllocPoolWithRuntimeTimer<T>(
                innerPool,
                synchronizationProvider,
                logger);
        }
        
        #endregion
    }
}