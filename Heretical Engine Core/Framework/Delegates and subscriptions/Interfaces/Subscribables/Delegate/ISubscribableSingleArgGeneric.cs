namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for a subscribable object that supports a single argument generic delegate
    /// </summary>
    /// <typeparam name="TValue">The type of the value passed as an argument to the delegate</typeparam>
    public interface ISubscribableSingleArgGeneric<TValue>
        : ISubscribable
    {
        /// <summary>
        /// Subscribes to the delegate with the specified action
        /// </summary>
        /// <param name="delegate">The action to subscribe</param>
        void Subscribe(Action<TValue> @delegate);
        
        /// <summary>
        /// Unsubscribes from the delegate with the specified action
        /// </summary>
        /// <param name="delegate">The action to unsubscribe</param>
        void Unsubscribe(Action<TValue> @delegate);

        /// <summary>
        /// Gets all the subscriptions for the delegate
        /// </summary>
        IEnumerable<Action<TValue>> AllSubscriptions { get; }
    }
}