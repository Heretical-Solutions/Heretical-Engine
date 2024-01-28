namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for a subscribable object with a single argument
    /// </summary>
    public interface ISubscribableSingleArg
        : ISubscribable
    {
        /// <summary>
        /// Subscribes to the event with a delegate that takes a single argument of type TValue
        /// </summary>
        /// <typeparam name="TValue">The type of the argument</typeparam>
        /// <param name="delegate">The delegate to subscribe</param>
        void Subscribe<TValue>(Action<TValue> @delegate);

        /// <summary>
        /// Subscribes to the event with a delegate that takes a single argument of the specified valueType
        /// </summary>
        /// <param name="valueType">The type of the argument</param>
        /// <param name="delegate">The delegate to subscribe</param>
        void Subscribe(
            Type valueType,
            object @delegate);

        /// <summary>
        /// Unsubscribes from the event with a delegate that takes a single argument of type TValue
        /// </summary>
        /// <typeparam name="TValue">The type of the argument</typeparam>
        /// <param name="delegate">The delegate to unsubscribe</param>
        void Unsubscribe<TValue>(Action<TValue> @delegate);

        /// <summary>
        /// Unsubscribes from the event with a delegate that takes a single argument of the specified valueType
        /// </summary>
        /// <param name="valueType">The type of the argument</param>
        /// <param name="delegate">The delegate to unsubscribe</param>
        void Unsubscribe(
            Type valueType,
            object @delegate);

        /// <summary>
        /// Gets all the subscriptions for the event with delegates that take a single argument of type TValue
        /// </summary>
        /// <typeparam name="TValue">The type of the argument</typeparam>
        /// <returns>An enumerable of all the subscriptions</returns>
        IEnumerable<Action<TValue>> GetAllSubscriptions<TValue>();

        /// <summary>
        /// Gets all the subscriptions for the event with delegates that take a single argument of the specified valueType
        /// </summary>
        /// <param name="valueType">The type of the argument</param>
        /// <returns>An enumerable of all the subscriptions</returns>
        IEnumerable<object> GetAllSubscriptions(Type valueType);
    }
}