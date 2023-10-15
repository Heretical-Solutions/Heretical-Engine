using System;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for subscribing and unsubscribing delegates with multiple arguments.
    /// </summary>
    public interface ISubscribableMultipleArgs
    {
        /// <summary>
        /// Subscribes a delegate with multiple arguments.
        /// </summary>
        /// <param name="@delegate">The delegate to subscribe.</param>
        void Subscribe(Action<object[]> @delegate);
        
        /// <summary>
        /// Unsubscribes a previously subscribed delegate with multiple arguments.
        /// </summary>
        /// <param name="@delegate">The delegate to unsubscribe.</param>
        void Unsubscribe(Action<object[]> @delegate);
    }
}