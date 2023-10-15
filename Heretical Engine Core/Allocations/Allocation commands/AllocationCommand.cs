using System;

namespace HereticalSolutions.Allocations
{
    /// <summary>
    /// Represents an allocation command.
    /// </summary>
    /// <typeparam name="T">The type of object to allocate.</typeparam>
    public class AllocationCommand<T>
    {
        /// <summary>
        /// Gets or sets the allocation command descriptor.
        /// </summary>
        public AllocationCommandDescriptor Descriptor { get; set; }

        /// <summary>
        /// Gets or sets the delegate used for the allocation.
        /// </summary>
        public Func<T> AllocationDelegate { get; set; }
    }
}