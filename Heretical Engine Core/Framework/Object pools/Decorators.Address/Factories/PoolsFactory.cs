using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Provides methods to build decorator pools for addresses.
    /// </summary>
    public static partial class AddressDecoratorsPoolsFactory
    {
        #region Decorator pools
        
        // No decorator pools are implemented in this section.
        
        #endregion

        #region Non alloc decorator pools

        public static NonAllocPoolWithAddress<T> BuildNonAllocPoolWithAddress<T>(
            IRepository<int, INonAllocDecoratedPool<T>> repository,
            int level,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocPoolWithAddress<T>>()
                ?? null;

            return new NonAllocPoolWithAddress<T>(
                repository,
                level,
                logger);
        }
        
        #endregion
    }
}