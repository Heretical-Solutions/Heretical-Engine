using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools.Allocations;
using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Pools.GenericNonAlloc;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class PoolsFactory
    {
        #region Supply and merge pool

        /// <summary>
        /// Builds a supply and merge pool with allocation callback.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the pool.</typeparam>
        /// <param name="valueAllocationDelegate">The delegate used to allocate new objects.</param>
        /// <param name="metadataDescriptors">An array of metadata descriptors.</param>
        /// <param name="initialAllocation">The allocation descriptor for the initial allocation.</param>
        /// <param name="additionalAllocation">The allocation descriptor for additional allocations.</param>
        /// <param name="callback">The allocation callback.</param>
        /// <returns>A supply and merge pool.</returns>
        public static SupplyAndMergePool<T> BuildSupplyAndMergePoolWithAllocationCallback<T>(
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,
            IAllocationCallback<T> callback)
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
                valueAllocationDelegate);

            return supplyAndMergePool;
        }

        /// <summary>
        /// Builds a supply and merge pool.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the pool.</typeparam>
        /// <param name="valueAllocationDelegate">The delegate used to allocate new objects.</param>
        /// <param name="metadataDescriptors">An array of metadata descriptors.</param>
        /// <param name="initialAllocation">The allocation descriptor for the initial allocation.</param>
        /// <param name="additionalAllocation">The allocation descriptor for additional allocations.</param>
        /// <returns>A supply and merge pool.</returns>
        public static SupplyAndMergePool<T> BuildSupplyAndMergePool<T>(
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation)
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
                valueAllocationDelegate);

            return supplyAndMergePool;
        }

        /// <summary>
        /// Builds a supply and merge pool.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the pool.</typeparam>
        /// <param name="initialAllocationCommand">The allocation command for the initial allocation.</param>
        /// <param name="appendAllocationCommand">The allocation command for additional allocations.</param>
        /// <param name="topUpAllocationDelegate">The delegate used to top up allocations.</param>
        /// <returns>A supply and merge pool.</returns>
        public static SupplyAndMergePool<T> BuildSupplyAndMergePool<T>(
            AllocationCommand<IPoolElement<T>> initialAllocationCommand,
            AllocationCommand<IPoolElement<T>> appendAllocationCommand,
            Func<T> topUpAllocationDelegate)
        {
            var basePool = BuildPackedArrayPool<T>(initialAllocationCommand);
            var supplyPool = BuildPackedArrayPool<T>(appendAllocationCommand);

            return new SupplyAndMergePool<T>(
                basePool,
                supplyPool,
                supplyPool,
                supplyPool,
                appendAllocationCommand,
                MergePools,
                topUpAllocationDelegate);
        }

        /// <summary>
        /// Merges pools.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the pool.</typeparam>
        /// <param name="receiverArray">The receiving pool.</param>
        /// <param name="donorArray">The donor pool.</param>
        /// <param name="donorAllocationCommand">The allocation command for the donor pool.</param>
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