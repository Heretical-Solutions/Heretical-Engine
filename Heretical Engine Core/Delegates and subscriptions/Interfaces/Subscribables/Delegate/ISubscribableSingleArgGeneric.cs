using System;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a subcribable object with a single argument of generic type.
    /// </summary>
    /// <typeparam name="TValue">The type of the argument.</typeparam>
    public interface ISubscribableSingleArgGeneric<TValue>
    {
        /// <summary>
        /// Subscribes an action delegate to the object.
        /// </summary>
        /// <param name="delegate">The delegate to subscribe.</param>
        void Subscribe(Action<TValue> @delegate);
        
        /// <summary>
        /// Unsubscribes an action delegate from the object.
        /// </summary>
        /// <param name="delegate">The delegate to unsubscribe.</param>
        void Unsubscribe(Action<TValue> @delegate);
    }
}