namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for a publisher that can publish multiple arguments.
    /// </summary>
    public interface IPublisherMultipleArgs
    {
        /// <summary>
        /// Publishes the specified values.
        /// </summary>
        /// <param name="values">The values to be published.</param>
        void Publish(object[] values);
    }
}