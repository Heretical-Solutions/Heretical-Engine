namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a publisher with no arguments.
    /// </summary>
    public interface IPublisherNoArgs
    {
        /// <summary>
        /// Publishes an event with no arguments.
        /// </summary>
        void Publish();
    }
}