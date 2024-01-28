namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a publisher that does not require any arguments when publishing events
    /// </summary>
    public interface IPublisherNoArgs
    {
        /// <summary>
        /// Publishes an event without any arguments
        /// </summary>
        void Publish();
    }
}