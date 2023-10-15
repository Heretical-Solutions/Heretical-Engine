using System;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Messaging
{
    public interface INonAllocMessageSender
    {
        #region Pop

        /// <summary>
        /// Pop a message of the given type from the message pool.
        /// </summary>
        /// <param name="messageType">The type of the message to pop.</param>
        /// <param name="message">The popped message.</param>
        /// <returns>The message sender.</returns>
        INonAllocMessageSender PopMessage(Type messageType, out IPoolElement<IMessage> message);

        /// <summary>
        /// Pop a message of the given type from the message pool.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to pop.</typeparam>
        /// <param name="message">The popped message.</param>
        /// <returns>The message sender.</returns>
        INonAllocMessageSender PopMessage<TMessage>(out IPoolElement<IMessage> message) where TMessage : IMessage;

        #endregion

        #region Write

        /// <summary>
        /// Write data to the specified message element.
        /// </summary>
        /// <param name="messageElement">The message element to write data to.</param>
        /// <param name="args">The data to write.</param>
        /// <returns>The message sender.</returns>
        INonAllocMessageSender Write(IPoolElement<IMessage> messageElement, object[] args);

        /// <summary>
        /// Write data to the specified message element of the specified type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message element.</typeparam>
        /// <param name="messageElement">The message element to write data to.</param>
        /// <param name="args">The data to write.</param>
        /// <returns>The message sender.</returns>
        INonAllocMessageSender Write<TMessage>(IPoolElement<IMessage> messageElement, object[] args) where TMessage : IMessage;

        #endregion

        #region Send

        /// <summary>
        /// Send the specified message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send(IPoolElement<IMessage> message);

        /// <summary>
        /// Send the specified message of the specified type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message to send.</param>
        void Send<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage;

        /// <summary>
        /// Send the specified message immediately.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void SendImmediately(IPoolElement<IMessage> message);

        /// <summary>
        /// Send the specified message of the specified type immediately.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message to send.</param>
        void SendImmediately<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage;

        #endregion

        #region Deliver

        /// <summary>
        /// Deliver all messages in the mailbox.
        /// </summary>
        void DeliverMessagesInMailbox();

        #endregion
    }
}