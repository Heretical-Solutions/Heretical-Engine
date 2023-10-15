namespace HereticalSolutions.Collections.Managed
{
    /// <summary>
    /// Represents the possible states of an element in the buffer.
    /// </summary>
    public enum EBufferElementState
    {
        /// <summary>
        /// Indicates that the element is vacant and available for allocation.
        /// </summary>
        VACANT,

        /// <summary>
        /// Indicates that the element has been allocated for the producer to write a value.
        /// </summary>
        ALLOCATED_FOR_PRODUCER,

        /// <summary>
        /// Indicates that the element has been filled with a value by the producer and is ready for consumption.
        /// </summary>
        FILLED,

        /// <summary>
        /// Indicates that the element has been allocated for the consumer to read the value.
        /// </summary>
        ALLOCATED_FOR_CONSUMER
    }
}