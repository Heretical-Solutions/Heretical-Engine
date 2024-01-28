namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a publisher that can publish multiple arguments. Arguments are passed as an array of objects
    /// </summary>
    public interface IPublisherMultipleArgs
    {
        /// <summary>
        /// Publishes the specified values
        /// </summary>
        /// <param name="values">The values to be published</param>
        void Publish(object[] values);
    }
}