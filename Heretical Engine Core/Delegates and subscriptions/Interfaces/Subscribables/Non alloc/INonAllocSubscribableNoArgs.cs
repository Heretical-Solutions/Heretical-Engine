namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a non-allocating subscribable event with no arguments.
    /// </summary>
    public interface INonAllocSubscribableNoArgs
    {
        /// <summary>
        /// Subscribes to the event.
        /// </summary>
        /// <param name="subscription">The subscription to be added.</param>
        void Subscribe(ISubscription subscription);

        /// <summary>
        /// Unsubscribes from the event.
        /// </summary>
        /// <param name="subscription">The subscription to be removed.</param>
        void Unsubscribe(ISubscription subscription);
    }
}