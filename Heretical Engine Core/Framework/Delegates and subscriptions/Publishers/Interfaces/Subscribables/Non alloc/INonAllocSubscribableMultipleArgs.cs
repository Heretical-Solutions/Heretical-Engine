namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for a non-allocating subscribable with multiple arguments. Arguments are passed as an array of objects
    /// </summary>
    public interface INonAllocSubscribableMultipleArgs
        : INonAllocSubscribable
    {
        /// <summary>
        /// Subscribes a subscription handler to the non-allocating subscribable
        /// </summary>
        /// <param name="subscription">The subscription handler to subscribe.</param>
        void Subscribe(
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>
                subscription);

        /// <summary>
        /// Unsubscribes a subscription handler from the non-allocating subscribable
        /// </summary>
        /// <param name="subscription">The subscription handler to unsubscribe.</param>
        void Unsubscribe(
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>
                subscription);

        /// <summary>
        /// Gets all the subscriptions of the non-allocating subscribable
        /// </summary>
        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>>
                AllSubscriptions { get; }
    }
}