using System;

namespace HereticalSolutions.Allocations
{
    /// <summary>
    /// Describes an allocation command, specifying the rule and amount for an allocation operation.
    /// </summary>
    [Serializable]
    public struct AllocationCommandDescriptor
    {
        /// <summary>
        /// The allocation amount rule for the allocation command.
        /// </summary>
        public EAllocationAmountRule Rule;

        /// <summary>
        /// The amount to allocate based on the allocation command.
        /// </summary>
        public int Amount;
    }
}