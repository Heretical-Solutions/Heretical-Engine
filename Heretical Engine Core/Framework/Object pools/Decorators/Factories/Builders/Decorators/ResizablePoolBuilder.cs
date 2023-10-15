using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a builder for resizable pools.
    /// </summary>
    /// <typeparam name="T">The type of objects to store in the pool.</typeparam>
    public class ResizablePoolBuilder<T>
    {
        private Func<T> valueAllocationDelegate;

        private Func<MetadataAllocationDescriptor>[] metadataDescriptorBuilders;

        private AllocationCommandDescriptor initialAllocation;

        private AllocationCommandDescriptor additionalAllocation;

        private IAllocationCallback<T>[] callbacks;

        /// <summary>
        /// Initializes the pool builder.
        /// </summary>
        /// <param name="valueAllocationDelegate">The delegate used to allocate new objects.</param>
        /// <param name="metadataDescriptorBuilders">An array of delegates that build metadata allocation descriptors.</param>
        /// <param name="initialAllocation">The allocation command descriptor for the initial allocation.</param>
        /// <param name="additionalAllocation">The allocation command descriptor for additional allocations.</param>
        /// <param name="callbacks">An array of allocation callbacks.</param>
        public void Initialize(
            Func<T> valueAllocationDelegate,
            Func<MetadataAllocationDescriptor>[] metadataDescriptorBuilders,
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,
            IAllocationCallback<T>[] callbacks)
        {
            this.valueAllocationDelegate = valueAllocationDelegate;
            this.metadataDescriptorBuilders = metadataDescriptorBuilders;
            this.initialAllocation = initialAllocation;
            this.additionalAllocation = additionalAllocation;
            this.callbacks = callbacks;
        }

        /// <summary>
        /// Builds a resizable pool using the initialized configurations.
        /// </summary>
        /// <returns>The created resizable pool.</returns>
        public INonAllocDecoratedPool<T> BuildResizablePool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception("[ResizablePoolBuilder] BUILDER NOT INITIALIZED");

            #region Metadata initialization

            List<MetadataAllocationDescriptor> metadataDescriptorsList = new List<MetadataAllocationDescriptor>();

            foreach (var descriptorBuilder in metadataDescriptorBuilders)
            {
                if (descriptorBuilder != null)
                    metadataDescriptorsList.Add(descriptorBuilder());
            }

            var metadataDescriptors = metadataDescriptorsList.ToArray();

            #endregion

            #region Allocation callbacks initialization

            IAllocationCallback<T> callback = PoolsFactory.BuildCompositeCallback(
                callbacks);

            #endregion

            INonAllocDecoratedPool<T> result = PoolsFactory.BuildResizableNonAllocPoolWithAllocationCallback(
                valueAllocationDelegate,
                metadataDescriptors,
                initialAllocation,
                additionalAllocation,
                callback);

            valueAllocationDelegate = null;
            metadataDescriptorBuilders = null;
            initialAllocation = default(AllocationCommandDescriptor);
            additionalAllocation = default(AllocationCommandDescriptor);
            callbacks = null;

            return result;
        }

        /// <summary>
        /// Builds a supply and merge pool using the initialized configurations.
        /// </summary>
        /// <returns>The created supply and merge pool.</returns>
        public INonAllocDecoratedPool<T> BuildSupplyAndMergePool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception("[ResizablePoolBuilder] BUILDER NOT INITIALIZED");

            #region Metadata initialization

            List<MetadataAllocationDescriptor> metadataDescriptorsList = new List<MetadataAllocationDescriptor>();

            foreach (var descriptorBuilder in metadataDescriptorBuilders)
            {
                if (descriptorBuilder != null)
                    metadataDescriptorsList.Add(descriptorBuilder());
            }

            var metadataDescriptors = metadataDescriptorsList.ToArray();

            #endregion

            #region Allocation callbacks initialization

            IAllocationCallback<T> callback = PoolsFactory.BuildCompositeCallback(
                callbacks);

            #endregion

            INonAllocDecoratedPool<T> result = PoolsFactory.BuildSupplyAndMergePoolWithAllocationCallback(
                valueAllocationDelegate,
                metadataDescriptors,
                initialAllocation,
                additionalAllocation,
                callback);

            valueAllocationDelegate = null;
            metadataDescriptorBuilders = null;
            initialAllocation = default(AllocationCommandDescriptor);
            additionalAllocation = default(AllocationCommandDescriptor);
            callbacks = null;

            return result;
        }
    }
}