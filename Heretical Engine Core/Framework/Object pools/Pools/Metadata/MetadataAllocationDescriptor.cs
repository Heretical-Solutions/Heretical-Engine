using System;

namespace HereticalSolutions.Pools.Allocations
{
    /// <summary>
    /// Represents a descriptor for a metadata allocation.
    /// </summary>
    public class MetadataAllocationDescriptor
    {
        /// <summary>
        /// Gets or sets the type used for binding.
        /// </summary>
        public Type BindingType;

        /// <summary>
        /// Gets or sets the concrete type to be allocated.
        /// </summary>
        public Type ConcreteType;
    }
}