using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// A factory for creating timer metadata objects.
    /// </summary>
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Metadata

        /// <summary>
        /// Builds a new instance of <see cref="RuntimeTimerMetadata"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="RuntimeTimerMetadata"/>.</returns>
        public static RuntimeTimerMetadata BuildRuntimeTimerMetadata()
        {
            return new RuntimeTimerMetadata();
        }

        /// <summary>
        /// Builds a new instance of <see cref="MetadataAllocationDescriptor"/> for <see cref="IContainsRuntimeTimer"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="MetadataAllocationDescriptor"/>.</returns>
        public static MetadataAllocationDescriptor BuildRuntimeTimerMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsRuntimeTimer),
                ConcreteType = typeof(RuntimeTimerMetadata)
            };
        }

        #endregion
    }
}