namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for subscribing and unsubscribing multiple argument invokable actions with no allocation.
    /// </summary>
    public interface INonAllocSubscribableMultipleArgs
    {
        /// <summary>
        /// Subscribes a subscription handler to the invokable action.
        /// </summary>
        /// <param name="subscription">The subscription handler to be subscribed.</param>
        void Subscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription);

        /// <summary>
        /// Unsubscribes a subscription handler from the invokable action.
        /// </summary>
        /// <param name="subscription">The subscription handler to be unsubscribed.</param>
        void Unsubscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription);
    }
}