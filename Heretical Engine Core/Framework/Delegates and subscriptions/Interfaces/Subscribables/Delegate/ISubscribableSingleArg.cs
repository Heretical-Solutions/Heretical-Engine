using System;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for subscribing to and unsubscribing from events with a single argument.
    /// </summary>
    public interface ISubscribableSingleArg
    {
        /// <summary>
        /// Subscribes to the event with the specified delegate.
        /// </summary>
        /// <typeparam name="TValue">The type of the delegate's argument.</typeparam>
        /// <param name="delegate">The delegate to be subscribed.</param>
        void Subscribe<TValue>(Action<TValue> @delegate);

        /// <summary>
        /// Subscribes to the event with the specified delegate.
        /// </summary>
        /// <param name="valueType">The type of the delegate's argument.</param>
        /// <param name="delegate">The delegate to be subscribed.</param>
        void Subscribe(Type valueType, object @delegate);

        /// <summary>
        /// Unsubscribes from the event with the specified delegate.
        /// </summary>
        /// <typeparam name="TValue">The type of the delegate's argument.</typeparam>
        /// <param name="delegate">The delegate to be unsubscribed.</param>
        void Unsubscribe<TValue>(Action<TValue> @delegate);
        
        /// <summary>
        /// Unsubscribes from the event with the specified delegate.
        /// </summary>
        /// <param name="valueType">The type of the delegate's argument.</param>
        /// <param name="delegate">The delegate to be unsubscribed.</param>
        void Unsubscribe(Type valueType, object @delegate);
    }
}