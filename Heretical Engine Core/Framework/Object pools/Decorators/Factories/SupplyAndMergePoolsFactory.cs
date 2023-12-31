using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools.Allocations;
using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Pools.GenericNonAlloc;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class PoolsFactory
    {
        #region Supply and merge pool

        public static SupplyAndMergePool<T> BuildSupplyAndMergePoolWithAllocationCallback<T>(
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,
            IAllocationCallback<T> callback,
            IFormatLogger logger)
        {
            Func<T> nullAllocation = AllocationsFactory.NullAllocationDelegate<T>;

            SupplyAndMergePool<T> supplyAndMergePool = BuildSupplyAndMergePool<T>(
                BuildPoolElementAllocationCommandWithCallback<T>(
                    initialAllocation,
                    valueAllocationDelegate,
                    metadataDescriptors,
                    callback),
                BuildPoolElementAllocationCommandWithCallback<T>(
                    additionalAllocation,
                    nullAllocation,
                    metadataDescriptors,
                    callback),
                valueAllocationDelegate,
                logger);

            return supplyAndMergePool;
        }

        public static SupplyAndMergePool<T> BuildSupplyAndMergePool<T>(
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,
            IFormatLogger logger)
        {
            Func<T> nullAllocation = AllocationsFactory.NullAllocationDelegate<T>;

            SupplyAndMergePool<T> supplyAndMergePool = BuildSupplyAndMergePool<T>(
                BuildPoolElementAllocationCommand<T>(
                    initialAllocation,
                    valueAllocationDelegate,
                    metadataDescriptors),
                BuildPoolElementAllocationCommand<T>(
                    additionalAllocation,
                    nullAllocation,
                    metadataDescriptors),
                valueAllocationDelegate,
                logger);

            return supplyAndMergePool;
        }

        public static SupplyAndMergePool<T> BuildSupplyAndMergePool<T>(
            AllocationCommand<IPoolElement<T>> initialAllocationCommand,
            AllocationCommand<IPoolElement<T>> appendAllocationCommand,
            Func<T> topUpAllocationDelegate,
            IFormatLogger logger)
        {
            var basePool = BuildPackedArrayPool<T>(
                initialAllocationCommand,
                logger);

            var supplyPool = BuildPackedArrayPool<T>(
                appendAllocationCommand,
                logger);

            return new SupplyAndMergePool<T>(
                basePool,
                supplyPool,
                supplyPool,
                supplyPool,
                appendAllocationCommand,
                MergePools,
                topUpAllocationDelegate);
        }

        public static void MergePools<T>(
            INonAllocPool<T> receiverArray,
            INonAllocPool<T> donorArray,
            AllocationCommand<IPoolElement<T>> donorAllocationCommand)
        {
            MergePackedArrayPools(
                (PackedArrayPool<T>)receiverArray,
                (PackedArrayPool<T>)donorArray,
                donorAllocationCommand);
        }

        #endregion
    }
}