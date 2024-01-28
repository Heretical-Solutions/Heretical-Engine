namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a subscription to a publisher
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Gets a value indicating whether the subscription is active
        /// </summary>
        bool Active { get; }
    }
}