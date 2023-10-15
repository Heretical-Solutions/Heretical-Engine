namespace HereticalSolutions.Allocations
{
    /// <summary>
    /// Specifies the status of an allocation.
    /// </summary>
    public enum EAllocationStatus
    {
        /// <summary>
        /// Indicates that the allocation is free and available for use.
        /// </summary>
        FREE,

        /// <summary>
        /// Indicates that the allocation is currently being used.
        /// </summary>
        USED
    }
}