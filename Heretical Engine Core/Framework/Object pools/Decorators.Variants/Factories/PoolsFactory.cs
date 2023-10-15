using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.RandomGeneration;
using HereticalSolutions.Repositories;

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

        /// <summary>
        /// Builds a non-alloc pool with variants.
        /// </summary>
        /// <typeparam name="T">The type of objects stored in the pool.</typeparam>
        /// <param name="repository">The repository used for storing variant containers.</param>
        /// <param name="generator">The random generator used for generating variants.</param>
        /// <returns>A NonAllocPoolWithVariants instance.</returns>
        public static NonAllocPoolWithVariants<T> BuildNonAllocPoolWithVariants<T>(
            IRepository<int, VariantContainer<T>> repository,
            IRandomGenerator generator)
        {
            return new NonAllocPoolWithVariants<T>(repository, generator);
        }

        #endregion
    }
}