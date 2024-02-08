namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a publisher that can publish a single argument of generic type
    /// </summary>
    /// <typeparam name="TValue">The type of the argument to be published</typeparam>
    public interface IPublisherSingleArgGeneric<TValue>
    {
        /// <summary>
        /// Publishes the specified value
        /// </summary>
        /// <param name="value">The value to be published</param>
        void Publish(TValue value);
    }
}