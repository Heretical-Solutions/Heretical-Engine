using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.RandomGeneration;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Factory class for creating pools with decorators.
    /// </summary>
    public static partial class VariantsDecoratorsPoolsFactory
    {
        #region Decorator pools

        // No decorator pools for now

        #endregion

        #region Non alloc decorator pools

        public static NonAllocPoolWithVariants<T> BuildNonAllocPoolWithVariants<T>(
            IRepository<int, VariantContainer<T>> repository,
            IRandomGenerator generator,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocPoolWithVariants<T>>()
                ?? null;

            return new NonAllocPoolWithVariants<T>(
                repository,
                generator,
                logger);
        }

        #endregion
    }
}