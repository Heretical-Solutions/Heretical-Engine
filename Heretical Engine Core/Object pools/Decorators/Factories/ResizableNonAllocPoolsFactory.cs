using System;
using HereticalSolutions.Allocations;
using HereticalSolutions.Pools.Allocations;
using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Pools.GenericNonAlloc;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// A factory class for creating resizable non-alloc pools.
    /// </summary>
    public static partial class PoolsFactory
    {
        #region Resizable non alloc pool

        /// <summary>
        /// Builds a resizable non-alloc pool with an allocation callback.
        /// </summary>
        /// <typeparam name="T">The type of object in the pool.</typeparam>
        /// <param name="valueAllocationDelegate">A delegate that allocates an object of type T.</param>
        /// <param name="metadataDescriptors">An array of metadata descriptors.</param>
        /// <param name="initialAllocation">The descriptor for the initial allocation.</param>
        /// <param name="additionalAllocation">The descriptor for the additional allocation.</param>
        /// <param name="callback">The allocation callback.</param>
        /// <returns>The created resizable non-alloc pool.</returns>
        public static ResizableNonAllocPool<T> BuildResizableNonAllocPoolWithAllocationCallback<T>(
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,
            IAllocationCallback<T> callback)
        {
            ResizableNonAllocPool<T> resizableNonAllocPool = BuildResizableNonAllocPoolFromPackedArrayPool<T>(
                BuildPoolElementAllocationCommandWithCallback<T>(
                    initialAllocation,
                    valueAllocationDelegate,
                    metadataDescriptors,
                    callback),
                BuildPoolElementAllocationCommandWithCallback<T>(
                    additionalAllocation,
                    valueAllocationDelegate,
                    metadataDescriptors,
                    callback),
                valueAllocationDelegate);

            return resizableNonAllocPool;
        }

        /// <summary>
        /// Builds a resizable non-alloc pool.
        /// </summary>
        /// <typeparam name="T">The type of object in the pool.</typeparam>
        /// <param name="valueAllocationDelegate">A delegate that allocates an object of type T.</param>
        /// <param name="metadataDescriptors">An array of metadata descriptors.</param>
        /// <param name="initialAllocation">The descriptor for the initial allocation.</param>
        /// <param name="additionalAllocation">The descriptor for the additional allocation.</param>
        /// <returns>The created resizable non-alloc pool.</returns>
        public static ResizableNonAllocPool<T> BuildResizableNonAllocPool<T>(
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation)
        {
            ResizableNonAllocPool<T> resizableNonAllocPool = BuildResizableNonAllocPoolFromPackedArrayPool<T>(
                BuildPoolElementAllocationCommand<T>(
                    initialAllocation,
                    valueAllocationDelegate,
                    metadataDescriptors),
                BuildPoolElementAllocationCommand<T>(
                    additionalAllocation,
                    valueAllocationDelegate,
                    metadataDescriptors),
                valueAllocationDelegate);

            return resizableNonAllocPool;
        }

        /// <summary>
        /// Builds a resizable non-alloc pool from a packed array pool.
        /// </summary>
        /// <typeparam name="T">The type of object in the pool.</typeparam>
        /// <param name="initialAllocationCommand">The allocation command for the initial allocation.</param>
        /// <param name="resizeAllocationCommand">The allocation command for resizing the pool.</param>
        /// <param name="topUpAllocationDelegate">A delegate that allocates an object of type T.</param>
        /// <returns>The created resizable non-alloc pool.</returns>
        public static ResizableNonAllocPool<T> BuildResizableNonAllocPoolFromPackedArrayPool<T>(
            AllocationCommand<IPoolElement<T>> initialAllocationCommand,
            AllocationCommand<IPoolElement<T>> resizeAllocationCommand,
            Func<T> topUpAllocationDelegate)
        {
            var pool = BuildPackedArrayPool<T>(initialAllocationCommand);

            return new ResizableNonAllocPool<T>(
                pool,
                pool,
                ResizeNonAllocPool,
                resizeAllocationCommand,
                topUpAllocationDelegate);
        }

        /// <summary>
        /// Resizes a resizable non-alloc pool.
        /// </summary>
        /// <typeparam name="T">The type of object in the pool.</typeparam>
        /// <param name="pool">The resizable non-alloc pool to resize.</param>
        public static void ResizeNonAllocPool<T>(
            ResizableNonAllocPool<T> pool)
        {
            ResizePackedArrayPool(
                (PackedArrayPool<T>)pool.Contents,
                pool.ResizeAllocationCommand);
        }

        #endregion
    }
}