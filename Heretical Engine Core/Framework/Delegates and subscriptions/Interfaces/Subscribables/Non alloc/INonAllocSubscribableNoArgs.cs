namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for a non-allocating subscribable with no arguments
    /// </summary>
    public interface INonAllocSubscribableNoArgs
        : INonAllocSubscribable
    {
        /// <summary>
        /// Subscribes to the event with the given subscription
        /// </summary>
        /// <param name="subscription">The subscription to add</param>
        void Subscribe(ISubscription subscription);

        /// <summary>
        /// Unsubscribes from the event with the given subscription
        /// </summary>
        /// <param name="subscription">The subscription to remove</param>
        void Unsubscribe(ISubscription subscription);
    }
}