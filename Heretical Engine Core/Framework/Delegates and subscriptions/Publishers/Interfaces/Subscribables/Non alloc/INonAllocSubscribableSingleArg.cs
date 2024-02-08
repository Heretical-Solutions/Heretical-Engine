namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for a non-allocating subscribable with a single argument
    /// </summary>
    public interface INonAllocSubscribableSingleArg
        : INonAllocSubscribable
    {
        /// <summary>
        /// Subscribes to the event with a specific value type and a generic subscription handler
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="subscription">The subscription handler</param>
        void Subscribe<TValue>(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription);

        /// <summary>
        /// Subscribes to the event with a specific value type and a non-generic subscription handler
        /// </summary>
        /// <param name="valueType">The type of the value</param>
        /// <param name="subscription">The subscription handler</param>
        void Subscribe(
            Type valueType,
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription);

        /// <summary>
        /// Unsubscribes from the event with a specific value type and a generic subscription handler
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="subscription">The subscription handler</param>
        void Unsubscribe<TValue>(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription);

        /// <summary>
        /// Unsubscribes from the event with a specific value type and a non-generic subscription handler
        /// </summary>
        /// <param name="valueType">The type of the value</param>
        /// <param name="subscription">The subscription handler</param>
        void Unsubscribe(
            Type valueType,
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription);

        /// <summary>
        /// Gets all subscriptions for a specific value type with a generic subscription handler
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <returns>An enumerable of subscription handlers</returns>
        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>>
            GetAllSubscriptions<TValue>();

        /// <summary>
        /// Gets all subscriptions for a specific value type with a non-generic subscription handler
        /// </summary>
        /// <param name="valueType">The type of the value</param>
        /// <returns>An enumerable of subscriptions</returns>
        IEnumerable<ISubscription> GetAllSubscriptions(Type valueType);

        /// <summary>
        /// Gets all subscriptions with a non-generic subscription handler
        /// </summary>
        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>>
            AllSubscriptions { get; }
    }
}