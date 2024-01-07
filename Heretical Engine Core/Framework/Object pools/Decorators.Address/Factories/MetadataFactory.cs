using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// A factory for creating <see cref="AddressMetadata"/> objects and <see cref="MetadataAllocationDescriptor"/> objects.
    /// </summary>
    public static partial class AddressDecoratorsPoolsFactory
    {
        #region Metadata

        /// <summary>
        /// Constructs a new <see cref="AddressMetadata"/> object.
        /// </summary>
        /// <returns>A new instance of <see cref="AddressMetadata"/>.</returns>
        public static AddressMetadata BuildAddressMetadata()
        {
            return new AddressMetadata();
        }

        /// <summary>
        /// Constructs a new <see cref="MetadataAllocationDescriptor"/> object with the binding type set to <see cref="IContainsAddress"/> and the concrete type set to <see cref="AddressMetadata"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="MetadataAllocationDescriptor"/> with the binding type set to <see cref="IContainsAddress"/> and the concrete type set to <see cref="AddressMetadata"/>.</returns>
        public static MetadataAllocationDescriptor BuildAddressMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsAddress),
                ConcreteType = typeof(AddressMetadata)
            };
        }

        #endregion
    }
}