using System;

using HereticalSolutions.Collections;
using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;
using HereticalSolutions.Delegates.Pinging;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

namespace HereticalSolutions.Delegates.Factories
{
    /// <summary>
    /// Provides factory methods for creating instances of pingers and non-alloc pingers.
    /// </summary>
    public static partial class DelegatesFactory
    {
        #region Pinger
        
        /// <summary>
        /// Builds a new instance of <see cref="Pinger"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="Pinger"/>.</returns>
        public static Pinger BuildPinger()
        {
            return new Pinger();
        }

        #endregion
        
        #region Non alloc pinger
        
        /// <summary>
        /// Builds a new instance of <see cref="NonAllocPinger"/> with default allocation rules.
        /// </summary>
        /// <returns>A new instance of <see cref="NonAllocPinger"/>.</returns>
        public static NonAllocPinger BuildNonAllocPinger()
        {
            Func<IInvokableNoArgs> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableNoArgs>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableNoArgs>(
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

            return BuildNonAllocPinger(subscriptionsPool);
        }

        /// <summary>
        /// Builds a new instance of <see cref="NonAllocPinger"/> with custom allocation rules.
        /// </summary>
        /// <param name="initial">The initial allocation command descriptor.</param>
        /// <param name="additional">The additional allocation command descriptor.</param>
        /// <returns>A new instance of <see cref="NonAllocPinger"/>.</returns>
        public static NonAllocPinger BuildNonAllocPinger(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional)
        {
            Func<IInvokableNoArgs> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableNoArgs>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableNoArgs>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                initial,
                additional);

            return BuildNonAllocPinger(subscriptionsPool);
        }

        /// <summary>
        /// Builds a new instance of <see cref="NonAllocPinger"/> with a custom subscriptions pool.
        /// </summary>
        /// <param name="subscriptionsPool">The subscriptions pool to use.</param>
        /// <returns>A new instance of <see cref="NonAllocPinger"/>.</returns>
        public static NonAllocPinger BuildNonAllocPinger(INonAllocDecoratedPool<IInvokableNoArgs> subscriptionsPool)
        {
            var contents = ((IModifiable<INonAllocPool<IInvokableNoArgs>>)subscriptionsPool).Contents;
			
            return new NonAllocPinger(
                subscriptionsPool,
                contents);
        }
        
        #endregion
    }
}