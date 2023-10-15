
using System;

namespace HereticalSolutions.Messaging
{
    /// <summary>
    /// Represents a message sender.
    /// </summary>
    public interface IMessageSender
    {
        #region Pop

        /// <summary>
        /// Pops a message of the specified type from the sender's message queue.
        /// </summary>
        /// <param name="messageType">The type of the message to pop.</param>
        /// <param name="message">The popped message.</param>
        /// <returns>The message sender.</returns>
        IMessageSender PopMessage(Type messageType, out IMessage message);

        /// <summary>
        /// Pops a message of the specified type from the sender's message queue.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to pop.</typeparam>
        /// <param name="message">The popped message.</param>
        /// <returns>The message sender.</returns>
        IMessageSender PopMessage<TMessage>(out TMessage message) where TMessage : IMessage;

        #endregion

        #region Write

        /// <summary>
        /// Writes a formatted message to the message queue.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <returns>The message sender.</returns>
        IMessageSender Write(IMessage message, object[] args);

        /// <summary>
        /// Writes a formatted message of the specified type to the message queue.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to write.</typeparam>
        /// <param name="message">The message to write.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <returns>The message sender.</returns>
        IMessageSender Write<TMessage>(TMessage message, object[] args) where TMessage : IMessage;

        #endregion

        #region Send

        /// <summary>
        /// Sends a message to the recipient.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send(IMessage message);

        /// <summary>
        /// Sends a message of the specified type to the recipient.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to send.</typeparam>
        /// <param name="message">The message to send.</param>
        void Send<TMessage>(TMessage message) where TMessage : IMessage;

        /// <summary>
        /// Sends a message to the recipient immediately.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void SendImmediately(IMessage message);

        /// <summary>
        /// Sends a message of the specified type to the recipient immediately.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to send.</typeparam>
        /// <param name="message">The message to send.</param>
        void SendImmediately<TMessage>(TMessage message) where TMessage : IMessage;

        #endregion

        #region Deliver

        /// <summary>
        /// Delivers all the messages in the message sender's mailbox.
        /// </summary>
        void DeliverMessagesInMailbox();

        #endregion
    }
}
