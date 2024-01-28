namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for objects that can be subscribed to with multiple arguments. Arguments are passed as an array of objects
    /// </summary>
    public interface ISubscribableMultipleArgs
        : ISubscribable
    {
        /// <summary>
        /// Subscribes to the object with a delegate that takes an array of objects as arguments
        /// </summary>
        /// <param name="delegate">The delegate to subscribe</param>
        void Subscribe(Action<object[]> @delegate);
            
        /// <summary>
        /// Unsubscribes from the object with a delegate that takes an array of objects as arguments
        /// </summary>
        /// <param name="delegate">The delegate to unsubscribe</param>
        void Unsubscribe(Action<object[]> @delegate);

        /// <summary>
        /// Gets all the subscriptions to the object
        /// </summary>
        IEnumerable<Action<object[]>> AllSubscriptions { get; }
    }
}