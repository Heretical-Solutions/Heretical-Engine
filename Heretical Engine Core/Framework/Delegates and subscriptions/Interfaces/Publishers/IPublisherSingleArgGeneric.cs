namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a generic interface for publishing events with a single argument.
    /// </summary>
    /// <typeparam name="TValue">The type of the argument.</typeparam>
    public interface IPublisherSingleArgGeneric<TValue>
    {
        /// <summary>
        /// Publishes an event with the specified argument.
        /// </summary>
        /// <param name="value">The argument value.</param>
        void Publish(TValue value);
    }
}