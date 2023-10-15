using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Repositories;

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

        /// <summary>
        /// Builds a non-allocating decorator pool with address.
        /// </summary>
        /// <typeparam name="T">The type of item in the pool.</typeparam>
        /// <param name="repository">The repository to store the non-allocating decorated pool.</param>
        /// <param name="level">The level of the non-allocating pool.</param>
        /// <returns>A new instance of the non-allocating pool with address.</returns>
        public static NonAllocPoolWithAddress<T> BuildNonAllocPoolWithAddress<T>(
            IRepository<int, INonAllocDecoratedPool<T>> repository,
            int level)
        {
            return new NonAllocPoolWithAddress<T>(repository, level);
        }
        
        #endregion
    }
}