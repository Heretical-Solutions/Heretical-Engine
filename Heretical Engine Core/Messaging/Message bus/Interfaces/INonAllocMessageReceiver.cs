using System;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.Messaging
{
    /// <summary>
    /// Represents an interface for a non-allocating message receiver.
    /// </summary>
    public interface INonAllocMessageReceiver
    {
        /// <summary>
        /// Subscribes to receive messages of a specific type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="subscription">The subscription to add.</param>
        void SubscribeTo<TMessage>(ISubscription subscription) where TMessage : IMessage;
        
        /// <summary>
        /// Subscribes to receive messages of a specific type.
        /// </summary>
        /// <param name="messageType">The type of message to subscribe to.</param>
        /// <param name="subscription">The subscription to add.</param>
        void SubscribeTo(Type messageType, ISubscription subscription);
		
        /// <summary>
        /// Unsubscribes from receiving messages of a specific type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="subscription">The subscription to remove.</param>
        void UnsubscribeFrom<TMessage>(ISubscription subscription) where TMessage : IMessage;
        
        /// <summary>
        /// Unsubscribes from receiving messages of a specific type.
        /// </summary>
        /// <param name="messageType">The type of message to unsubscribe from.</param>
        /// <param name="subscription">The subscription to remove.</param>
        void UnsubscribeFrom(Type messageType, ISubscription subscription);
    }
}