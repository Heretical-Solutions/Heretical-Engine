namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Defines a generic interface for subscribing and unsubscribing handlers to an event with a single argument, without requiring the use of arrays or lists.
    /// </summary>
    /// <typeparam name="TValue">The type of the single argument.</typeparam>
    public interface INonAllocSubscribableSingleArgGeneric<TValue>
    {
        /// <summary>
        /// Subscribes a subscription handler to the event.
        /// </summary>
        /// <param name="subscription">The subscription handler to be added.</param>
        void Subscribe(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription);

        /// <summary>
        /// Unsubscribes a subscription handler from the event.
        /// </summary>
        /// <param name="subscription">The subscription handler to be removed.</param>
        void Unsubscribe(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription);
    }
}