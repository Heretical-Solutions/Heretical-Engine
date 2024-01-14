using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a factory for creating variant metadata pools.
    /// </summary>
    public static partial class VariantsDecoratorsPoolsFactory
    {
        #region Metadata

        /// <summary>
        /// Builds a new instance of <see cref="VariantMetadata"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="VariantMetadata"/>.</returns>
        public static VariantMetadata BuildVariantMetadata()
        {
            return new VariantMetadata();
        }

        /// <summary>
        /// Builds a new instance of <see cref="MetadataAllocationDescriptor"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="MetadataAllocationDescriptor"/>.</returns>
        public static MetadataAllocationDescriptor BuildVariantMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsVariant),
                ConcreteType = typeof(VariantMetadata)
            };
        }

        #endregion
    }
}