using HereticalSolutions.Allocations;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public class ResizablePoolBuilder<T>
    {
        private readonly ILoggerResolver loggerResolver;

        private readonly ILogger logger;

        private Func<T> valueAllocationDelegate;

        private Func<MetadataAllocationDescriptor>[] metadataDescriptorBuilders;

        private AllocationCommandDescriptor initialAllocation;

        private AllocationCommandDescriptor additionalAllocation;

        private IAllocationCallback<T>[] callbacks;

        public ResizablePoolBuilder(
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
        {
            this.loggerResolver = loggerResolver;

            this.logger = logger;
        }

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
                throw new Exception(
                    logger.TryFormat<ResizablePoolBuilder<T>>(
                        "BUILDER NOT INITIALIZED"));

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

            IAllocationCallback<T> callback = null;

            if (callbacks != null)
            {
                callback = PoolsFactory.BuildCompositeCallback(callbacks);
            }

            #endregion

            INonAllocDecoratedPool<T> result;

            if (callback == null)
            {
                result = PoolsFactory.BuildResizableNonAllocPool(
                    valueAllocationDelegate,
                    metadataDescriptors,
                    initialAllocation,
                    additionalAllocation,
                    loggerResolver);
            }
            else
            {
                result = PoolsFactory.BuildResizableNonAllocPoolWithAllocationCallback(
                    valueAllocationDelegate,
                    metadataDescriptors,
                    initialAllocation,
                    additionalAllocation,
                    callback,
                    loggerResolver);
            }

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
                throw new Exception(
                    logger.TryFormat<ResizablePoolBuilder<T>>(
                        "BUILDER NOT INITIALIZED"));

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

            IAllocationCallback<T> callback = null;

            if (callbacks != null)
            {
                callback = PoolsFactory.BuildCompositeCallback(callbacks);
            }

            #endregion

            INonAllocDecoratedPool<T> result;

            if (callback == null)
            {
                result = PoolsFactory.BuildSupplyAndMergePool(
                    valueAllocationDelegate,
                    metadataDescriptors,
                    initialAllocation,
                    additionalAllocation,
                    loggerResolver);
            }
            else
            {
                result = PoolsFactory.BuildSupplyAndMergePoolWithAllocationCallback(
                    valueAllocationDelegate,
                    metadataDescriptors,
                    initialAllocation,
                    additionalAllocation,
                    callback,
                    loggerResolver);
            }

            valueAllocationDelegate = null;

            metadataDescriptorBuilders = null;

            initialAllocation = default(AllocationCommandDescriptor);

            additionalAllocation = default(AllocationCommandDescriptor);
            
            callbacks = null;

            return result;
        }
    }
}