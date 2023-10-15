using System;

namespace HereticalSolutions.Messaging
{
	/// <summary>
    /// Represents an interface for receiving messages.
    /// </summary>
	public interface IMessageReceiver
	{
		/// <summary>
        /// Subscribes to a specific message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="receiverDelegate">The delegate that handles the received message.</param>
		void SubscribeTo<TMessage>(Action<TMessage> receiverDelegate) where TMessage : IMessage;
		
        /// <summary>
        /// Subscribes to a specific message type.
        /// </summary>
        /// <param name="messageType">The type of message to subscribe to.</param>
        /// <param name="receiverDelegate">The delegate that handles the received message.</param>
		void SubscribeTo(Type messageType, object receiverDelegate);

		/// <summary>
        /// Unsubscribes from a specific message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="receiverDelegate">The delegate that handles the received message.</param>
		void UnsubscribeFrom<TMessage>(Action<TMessage> receiverDelegate) where TMessage : IMessage;
		
        /// <summary>
        /// Unsubscribes from a specific message type.
        /// </summary>
        /// <param name="messageType">The type of message to unsubscribe from.</param>
        /// <param name="receiverDelegate">The delegate that handles the received message.</param>
		void UnsubscribeFrom(Type messageType, object receiverDelegate);
	}
}