using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static partial class DelegatesFactory
    {
        #region Broadcaster multiple args

        public static BroadcasterMultipleArgs BuildBroadcasterMultipleArgs(
            IFormatLogger logger)
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>(
                    logger));
        }

        #endregion
        
        #region Broadcaster with repository

        public static BroadcasterWithRepository BuildBroadcasterWithRepository(
            IRepository<Type, object> broadcastersRepository,
            IFormatLogger logger)
        {
            return BuildBroadcasterWithRepository(
                RepositoriesFactory.BuildDictionaryObjectRepository(
                    broadcastersRepository),
                logger);
        }
        
        public static BroadcasterWithRepository BuildBroadcasterWithRepository(
            IReadOnlyObjectRepository repository,
            IFormatLogger logger)
        {
            return new BroadcasterWithRepository(
                repository,
                logger);
        }

        #endregion
        
        #region Broadcaster generic

        /// <summary>
        /// Builds a generic broadcaster.
        /// </summary>
        /// <typeparam name="T">The type of the broadcast argument.</typeparam>
        /// <returns>The built generic broadcaster.</returns>
        public static BroadcasterGeneric<T> BuildBroadcasterGeneric<T>(
            IFormatLogger logger)
        {
            return new BroadcasterGeneric<T>(logger);
        }

        #endregion
        
        #region Non alloc broadcaster multiple args
        
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            IFormatLogger logger)
        {
            Func<ISubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<ISubscription>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<ISubscription>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                },
                logger);

            return BuildNonAllocBroadcasterMultipleArgs(
                subscriptionsPool);
        }

        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            IFormatLogger logger)
        {
            Func<ISubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<ISubscription>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<ISubscription>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                initial,
                additional,
                logger);

            return BuildNonAllocBroadcasterMultipleArgs(
                subscriptionsPool);
        }
        
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            INonAllocDecoratedPool<ISubscription> subscriptionsPool)
        {
            var contents = ((IModifiable<INonAllocPool<ISubscription>>)subscriptionsPool).Contents;
			
            return new NonAllocBroadcasterMultipleArgs(
                subscriptionsPool,
                contents);
        }
        
        #endregion
        
        #region Non alloc broadcaster with repository
        
        /// <summary>
        /// Builds a non-allocating broadcaster with a repository.
        /// </summary>
        /// <param name="broadcastersRepository">The repository for the broadcasters.</param>
        /// <param name="logger">The logger to use for logging.</param>
        /// <returns>The built non-allocating broadcaster with a repository.</returns>
        public static NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository(
            IRepository<Type, object> broadcastersRepository,
            IFormatLogger logger)
        {
            return BuildNonAllocBroadcasterWithRepository(
                RepositoriesFactory.BuildDictionaryObjectRepository(
                    broadcastersRepository),
                logger);
        }
        
        /// <summary>
        /// Builds a non-allocating broadcaster with a repository.
        /// </summary>
        /// <param name="repository">The repository for the broadcasters.</param>
        /// <param name="logger">The logger to use for logging.</param>
        /// <returns>The built non-allocating broadcaster with a repository.</returns>
        public static NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository(
            IReadOnlyObjectRepository repository,
            IFormatLogger logger)
        {
            return new NonAllocBroadcasterWithRepository(
                repository,
                logger);
        }
        
        #endregion
        
        #region Non alloc broadcaster generic
        
        /// <summary>
        /// Builds a non-allocating generic broadcaster.
        /// </summary>
        /// <typeparam name="T">The type of the broadcast argument.</typeparam>
        /// <returns>The built non-allocating generic broadcaster.</returns>
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            IFormatLogger logger)
        {
            Func<ISubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<ISubscription>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<ISubscription>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                },
                logger);

            return BuildNonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                logger);
        }

        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            IFormatLogger logger)
        {
            Func<ISubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<ISubscription>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<ISubscription>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                initial,
                additional,
                logger);

            return BuildNonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                logger);
        }
        
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            INonAllocDecoratedPool<ISubscription> subscriptionsPool,
            IFormatLogger logger)
        {
            var contents = ((IModifiable<INonAllocPool<ISubscription>>)subscriptionsPool).Contents;
			
            return new NonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                contents,
                logger);
        }
        
        #endregion
    }
}