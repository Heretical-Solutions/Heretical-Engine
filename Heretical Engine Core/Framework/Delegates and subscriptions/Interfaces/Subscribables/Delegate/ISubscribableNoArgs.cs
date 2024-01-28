namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for objects that can be subscribed to without any arguments
    /// </summary>
    public interface ISubscribableNoArgs
        : ISubscribable
    {
        /// <summary>
        /// Subscribes to the event with the specified delegate
        /// </summary>
        /// <param name="delegate">The delegate to subscribe</param>
        void Subscribe(Action @delegate);
        
        /// <summary>
        /// Unsubscribes from the event with the specified delegate
        /// </summary>
        /// <param name="delegate">The delegate to unsubscribe</param>
        void Unsubscribe(Action @delegate);

        /// <summary>
        /// Gets all the subscriptions for this object
        /// </summary>
        IEnumerable<Action> AllSubscriptions { get; }
    }
}