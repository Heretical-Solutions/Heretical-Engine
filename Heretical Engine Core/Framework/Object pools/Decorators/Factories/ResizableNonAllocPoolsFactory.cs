using HereticalSolutions.Allocations;

using HereticalSolutions.Metadata.Allocations;
using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Pools.GenericNonAlloc;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class PoolsFactory
    {
        #region Resizable non alloc pool

        public static ResizableNonAllocPool<T> BuildResizableNonAllocPoolWithAllocationCallback<T>(
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,
            IAllocationCallback<T> callback,
            ILoggerResolver loggerResolver = null)
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
                valueAllocationDelegate,
                loggerResolver);

            return resizableNonAllocPool;
        }

        public static ResizableNonAllocPool<T> BuildResizableNonAllocPool<T>(
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,
            ILoggerResolver loggerResolver = null)
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
                valueAllocationDelegate,
                loggerResolver);

            return resizableNonAllocPool;
        }

        public static ResizableNonAllocPool<T> BuildResizableNonAllocPoolFromPackedArrayPool<T>(
            AllocationCommand<IPoolElement<T>> initialAllocationCommand,
            AllocationCommand<IPoolElement<T>> resizeAllocationCommand,
            Func<T> topUpAllocationDelegate,
            ILoggerResolver loggerResolver = null)
        {
            var pool = BuildPackedArrayPool<T>(
                initialAllocationCommand,
                loggerResolver);

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