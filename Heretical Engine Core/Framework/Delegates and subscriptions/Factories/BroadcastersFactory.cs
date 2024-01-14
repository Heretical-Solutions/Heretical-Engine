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
            ILoggerResolver loggerResolver = null)
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>(
                    loggerResolver));
        }

        #endregion
        
        #region Broadcaster with repository

        public static BroadcasterWithRepository BuildBroadcasterWithRepository(
            IRepository<Type, object> broadcastersRepository,
            ILoggerResolver loggerResolver = null)
        {
            return BuildBroadcasterWithRepository(
                RepositoriesFactory.BuildDictionaryObjectRepository(
                    broadcastersRepository),
                loggerResolver);
        }
        
        public static BroadcasterWithRepository BuildBroadcasterWithRepository(
            IReadOnlyObjectRepository repository,
            ILoggerResolver loggerResolver = null)
        {
            IFormatLogger logger =
                loggerResolver?.GetLogger<BroadcasterWithRepository>()
                ?? null;

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
            ILoggerResolver loggerResolver = null)
        {
            IFormatLogger logger =
                loggerResolver?.GetLogger<BroadcasterGeneric<T>>()
                ?? null;

            return new BroadcasterGeneric<T>(logger);
        }

        #endregion
        
        #region Non alloc broadcaster multiple args
        
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            ILoggerResolver loggerResolver = null)
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
                loggerResolver);

            return BuildNonAllocBroadcasterMultipleArgs(
                subscriptionsPool);
        }

        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver = null)
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
                loggerResolver);

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
            ILoggerResolver loggerResolver = null)
        {
            return BuildNonAllocBroadcasterWithRepository(
                RepositoriesFactory.BuildDictionaryObjectRepository(
                    broadcastersRepository),
                loggerResolver);
        }
        
        /// <summary>
        /// Builds a non-allocating broadcaster with a repository.
        /// </summary>
        /// <param name="repository">The repository for the broadcasters.</param>
        /// <param name="logger">The logger to use for logging.</param>
        /// <returns>The built non-allocating broadcaster with a repository.</returns>
        public static NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository(
            IReadOnlyObjectRepository repository,
            ILoggerResolver loggerResolver = null)
        {
            IFormatLogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterWithRepository>()
                ?? null;

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
            ILoggerResolver loggerResolver = null)
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
                loggerResolver);

            return BuildNonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                loggerResolver);
        }

        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver = null)
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
                loggerResolver);

            return BuildNonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                loggerResolver);
        }
        
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            INonAllocDecoratedPool<ISubscription> subscriptionsPool,
            ILoggerResolver loggerResolver = null)
        {
            var contents = ((IModifiable<INonAllocPool<ISubscription>>)subscriptionsPool).Contents;
			
            IFormatLogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterGeneric<T>>()
                ?? null;

            return new NonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                contents,
                logger);
        }
        
        #endregion
    }
}