using System;

using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Logging;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Delegates.Factories
{
    /// <summary>
    /// Factory class for creating delegates.
    /// </summary>
    public static partial class DelegatesFactory
    {
        #region Broadcaster multiple args

        /// <summary>
        /// Builds a broadcaster with multiple arguments.
        /// </summary>
        /// <returns>The built broadcaster with multiple arguments.</returns>
        public static BroadcasterMultipleArgs BuildBroadcasterMultipleArgs()
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>());
        }

        #endregion
        
        #region Broadcaster with repository

        /// <summary>
        /// Builds a broadcaster with a repository.
        /// </summary>
        /// <param name="broadcastersRepository">The repository for the broadcasters.</param>
        /// <param name="logger">The logger to use for logging.</param>
        /// <returns>The built broadcaster with a repository.</returns>
        public static BroadcasterWithRepository BuildBroadcasterWithRepository(
            IRepository<Type, object> broadcastersRepository,
            IFormatLogger logger)
        {
            return BuildBroadcasterWithRepository(
                RepositoriesFactory.BuildDictionaryObjectRepository(
                    broadcastersRepository),
                logger);
        }
        
        /// <summary>
        /// Builds a broadcaster with a repository.
        /// </summary>
        /// <param name="repository">The repository for the broadcasters.</param>
        /// <param name="logger">The logger to use for logging.</param>
        /// <returns>The built broadcaster with a repository.</returns>
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
        public static BroadcasterGeneric<T> BuildBroadcasterGeneric<T>()
        {
            return new BroadcasterGeneric<T>();
        }

        #endregion
        
        #region Non alloc broadcaster multiple args
        
        /// <summary>
        /// Builds a non-allocating broadcaster with multiple arguments.
        /// </summary>
        /// <returns>The built non-allocating broadcaster with multiple arguments.</returns>
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs()
        {
            Func<IInvokableMultipleArgs> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableMultipleArgs>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableMultipleArgs>(
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
                });

            return BuildNonAllocBroadcasterMultipleArgs(subscriptionsPool);
        }

        /// <summary>
        /// Builds a non-allocating broadcaster with multiple arguments.
        /// </summary>
        /// <param name="initial">The allocation command descriptor for the initial allocation.</param>
        /// <param name="additional">The allocation command descriptor for the additional allocation.</param>
        /// <returns>The built non-allocating broadcaster with multiple arguments.</returns>
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional)
        {
            Func<IInvokableMultipleArgs> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableMultipleArgs>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableMultipleArgs>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                initial,
                additional);

            return BuildNonAllocBroadcasterMultipleArgs(subscriptionsPool);
        }
        
        /// <summary>
        /// Builds a non-allocating broadcaster with multiple arguments.
        /// </summary>
        /// <param name="subscriptionsPool">The pool of subscriptions for the broadcaster.</param>
        /// <returns>The built non-allocating broadcaster with multiple arguments.</returns>
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            INonAllocDecoratedPool<IInvokableMultipleArgs> subscriptionsPool)
        {
            var contents = ((IModifiable<INonAllocPool<IInvokableMultipleArgs>>)subscriptionsPool).Contents;
			
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
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>()
        {
            Func<IInvokableSingleArgGeneric<T>> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableSingleArgGeneric<T>>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableSingleArgGeneric<T>>(
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
                });

            return BuildNonAllocBroadcasterGeneric(subscriptionsPool);
        }

        /// <summary>
        /// Builds a non-allocating generic broadcaster.
        /// </summary>
        /// <typeparam name="T">The type of the broadcast argument.</typeparam>
        /// <param name="initial">The allocation command descriptor for the initial allocation.</param>
        /// <param name="additional">The allocation command descriptor for the additional allocation.</param>
        /// <returns>The built non-allocating generic broadcaster.</returns>
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional)
        {
            Func<IInvokableSingleArgGeneric<T>> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableSingleArgGeneric<T>>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableSingleArgGeneric<T>>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                initial,
                additional);

            return BuildNonAllocBroadcasterGeneric(subscriptionsPool);
        }
        
        /// <summary>
        /// Builds a non-allocating generic broadcaster.
        /// </summary>
        /// <typeparam name="T">The type of the broadcast argument.</typeparam>
        /// <param name="subscriptionsPool">The pool of subscriptions for the broadcaster.</param>
        /// <returns>The built non-allocating generic broadcaster.</returns>
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            INonAllocDecoratedPool<IInvokableSingleArgGeneric<T>> subscriptionsPool)
        {
            var contents = ((IModifiable<INonAllocPool<IInvokableSingleArgGeneric<T>>>)subscriptionsPool).Contents;
			
            return new NonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                contents);
        }
        
        #endregion
    }
}