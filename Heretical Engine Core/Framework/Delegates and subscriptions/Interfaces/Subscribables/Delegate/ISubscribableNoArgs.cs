using System;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Interface for subscribing to and unsubscribing from an action with no arguments.
    /// </summary>
    public interface ISubscribableNoArgs
    {
        /// <summary>
        /// Subscribes to the specified delegate.
        /// </summary>
        /// <param name="delegate">The delegate to subscribe to.</param>
        void Subscribe(Action @delegate);
        
        /// <summary>
        /// Unsubscribes from the specified delegate.
        /// </summary>
        /// <param name="delegate">The delegate to unsubscribe from.</param>
        void Unsubscribe(Action @delegate);
    }
}