
using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Pinging;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static partial class DelegatesFactory
    {
        #region Pinger
        
        public static Pinger BuildPinger()
        {
            return new Pinger();
        }

        #endregion
        
        #region Non alloc pinger
        
        public static NonAllocPinger BuildNonAllocPinger(
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

            return BuildNonAllocPinger(subscriptionsPool);
        }

        public static NonAllocPinger BuildNonAllocPinger(
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

            return BuildNonAllocPinger(subscriptionsPool);
        }

        public static NonAllocPinger BuildNonAllocPinger(
            INonAllocDecoratedPool<ISubscription> subscriptionsPool)
        {
            var contents = ((IModifiable<INonAllocPool<ISubscription>>)subscriptionsPool).Contents;
			
            return new NonAllocPinger(
                subscriptionsPool,
                contents);
        }
        
        #endregion
    }
}