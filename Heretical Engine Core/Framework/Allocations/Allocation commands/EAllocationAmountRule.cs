namespace HereticalSolutions.Allocations
{
    /// <summary>
    /// Specifies the allocation amount rules for an allocation command.
    /// </summary>
    public enum EAllocationAmountRule
    {
        /// <summary>
        /// Allocates zero amount.
        /// </summary>
        ZERO,
        
        /// <summary>
        /// Allocates an amount by adding one.
        /// </summary>
        ADD_ONE,
        
        /// <summary>
        /// Allocates an amount by doubling the current amount.
        /// </summary>
        DOUBLE_AMOUNT,
        
        /// <summary>
        /// Allocates a predefined amount.
        /// </summary>
        ADD_PREDEFINED_AMOUNT
    }
}