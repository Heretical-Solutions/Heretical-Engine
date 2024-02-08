namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a non-allocating subscribable interface with a single generic argument
    /// </summary>
    /// <typeparam name="TValue">The type of the value being subscribed to</typeparam>
    public interface INonAllocSubscribableSingleArgGeneric<TValue>
        : INonAllocSubscribable
    {
        /// <summary>
        /// Subscribes a subscription handler to this subscribable
        /// </summary>
        /// <param name="subscription">The subscription handler to subscribe</param>
        void Subscribe(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription);

        /// <summary>
        /// Unsubscribes a subscription handler from this subscribable
        /// </summary>
        /// <param name="subscription">The subscription handler to unsubscribe</param>
        void Unsubscribe(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription);

        /// <summary>
        /// Gets all the subscriptions currently registered with this subscribable
        /// </summary>
        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>>
                AllSubscriptions { get; }
    }
}