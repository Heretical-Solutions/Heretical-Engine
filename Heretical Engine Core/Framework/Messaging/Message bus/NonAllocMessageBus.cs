using System;
using HereticalSolutions.Collections;
using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Broadcasting;
using HereticalSolutions.Pools;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Messaging
{
    /// <summary>
    /// Represents a non-allocating message bus that can send and receive messages.
    /// </summary>
    public class NonAllocMessageBus : INonAllocMessageSender, INonAllocMessageReceiver
    {
        private readonly NonAllocBroadcasterWithRepository broadcaster;
        private readonly IReadOnlyObjectRepository messageRepository;
        private readonly INonAllocDecoratedPool<IPoolElement<IMessage>> mailbox;
        private readonly IIndexable<IPoolElement<IPoolElement<IMessage>>> mailboxContentsAsIndexable;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NonAllocMessageBus"/> class.
        /// </summary>
        /// <param name="broadcaster">The broadcaster used to publish messages.</param>
        /// <param name="messageRepository">The repository that stores message pools.</param>
        /// <param name="mailbox">The pool of message elements.</param>
        /// <param name="mailboxContentsAsIndexable">The indexable collection of message elements.</param>
        public NonAllocMessageBus(
            NonAllocBroadcasterWithRepository broadcaster,
            IReadOnlyObjectRepository messageRepository,
            INonAllocDecoratedPool<IPoolElement<IMessage>> mailbox,
            IIndexable<IPoolElement<IPoolElement<IMessage>>> mailboxContentsAsIndexable)
        {
            this.broadcaster = broadcaster;
            this.messageRepository = messageRepository;
            this.mailbox = mailbox;
            this.mailboxContentsAsIndexable = mailboxContentsAsIndexable;
        }

        #region IMessageSenderNonAlloc

        #region Pop
        
        /// <summary>
        /// Retrieves a message of the specified type from the message pool.
        /// </summary>
        /// <param name="messageType">The type of the message to retrieve.</param>
        /// <param name="message">The retrieved message.</param>
        /// <returns>The message sender instance.</returns>
        /// <exception cref="Exception">Thrown when the specified message type is invalid for the message bus.</exception>
        public INonAllocMessageSender PopMessage(Type messageType, out IPoolElement<IMessage> message)
        {
            if (!messageRepository.TryGet(messageType, out object messagePoolObject))
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.ToString()}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;

            message = messagePool.Pop(null);

            return this;
        }

        /// <summary>
        /// Retrieves a message of the specified type from the message pool.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to retrieve.</typeparam>
        /// <param name="message">The retrieved message.</param>
        /// <returns>The message sender instance.</returns>
        /// <exception cref="Exception">Thrown when the specified message type is invalid for the message bus.</exception>
        public INonAllocMessageSender PopMessage<TMessage>(out IPoolElement<IMessage> message) where TMessage : IMessage
        {
            if (!messageRepository.TryGet(typeof(TMessage), out object messagePoolObject))
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).ToString()}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;

            message = messagePool.Pop(null);

            return this;
        }
        
        #endregion

        #region Write

        /// <summary>
        /// Writes data to the specified message.
        /// </summary>
        /// <param name="messageElement">The message element to write to.</param>
        /// <param name="args">The data to write to the message.</param>
        /// <returns>The message sender instance.</returns>
        /// <exception cref="Exception">Thrown when the message element is invalid.</exception>
        public INonAllocMessageSender Write(IPoolElement<IMessage> messageElement, object[] args)
        {
            if (messageElement == null)
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE");

            messageElement.Value.Write(args);

            return this;
        }

        /// <summary>
        /// Writes data to the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to write to.</typeparam>
        /// <param name="messageElement">The message element to write to.</param>
        /// <param name="args">The data to write to the message.</param>
        /// <returns>The message sender instance.</returns>
        /// <exception cref="Exception">Thrown when the message element is invalid.</exception>
        public INonAllocMessageSender Write<TMessage>(IPoolElement<IMessage> messageElement, object[] args) where TMessage : IMessage
        {
            if (messageElement == null)
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE");

            messageElement.Value.Write(args);

            return this;
        }
        
        #endregion

        #region Send

        /// <summary>
        /// Sends a message to the message bus.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(IPoolElement<IMessage> message)
        {
            var messageElement = mailbox.Pop(null);

            messageElement.Value = message;
        }

        /// <summary>
        /// Sends a message to the message bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to send.</typeparam>
        /// <param name="message">The message to send.</param>
        public void Send<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage
        {
            var messageElement = mailbox.Pop(null);

            messageElement.Value = message;
        }

        /// <summary>
        /// Sends a message to the message bus and immediately publishes it to subscribers.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendImmediately(IPoolElement<IMessage> message)
        {
            broadcaster.Publish(message.Value.GetType(), message.Value);

            PushMessageToPool(message);
        }

        /// <summary>
        /// Sends a message to the message bus and immediately publishes it to subscribers.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to send.</typeparam>
        /// <param name="message">The message to send.</param>
        public void SendImmediately<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage
        {
            broadcaster.Publish<TMessage>((TMessage)message.Value);

            PushMessageToPool<TMessage>(message);
        }

        #endregion

        #region Deliver
        
        /// <summary>
        /// Sends all messages in the mailbox to subscribers immediately.
        /// </summary>
        public void DeliverMessagesInMailbox()
        {
            int messagesToReceive = mailboxContentsAsIndexable.Count;

            for (int i = 0; i < messagesToReceive; i++)
            {
                var message = mailboxContentsAsIndexable[0];

                SendImmediately(message.Value);
                
                mailbox.Push(message);
            }
        }

        #endregion
        
        private void PushMessageToPool(IPoolElement<IMessage> message)
        {
            var messageType = message.Value.GetType();

            if (!messageRepository.TryGet(messageType, out object messagePoolObject))
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.ToString()}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;

            messagePool.Push(message);
        }

        private void PushMessageToPool<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);

            if (!messageRepository.TryGet(messageType, out object messagePoolObject))
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).ToString()}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;

            messagePool.Push(message);
        }
        
        #endregion

        #region IMessageReceiverNonAlloc
        
        /// <summary>
        /// Subscribes to messages of the specified type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the messages to subscribe to.</typeparam>
        /// <param name="subscription">The subscription to add.</param>
        public void SubscribeTo<TMessage>(ISubscription subscription) where TMessage : IMessage
        {
            broadcaster.Subscribe<TMessage>((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TMessage>, IInvokableSingleArgGeneric<TMessage>>)subscription);
        }
        
        /// <summary>
        /// Subscribes to messages of the specified type.
        /// </summary>
        /// <param name="messageType">The type of the messages to subscribe to.</param>
        /// <param name="subscription">The subscription to add.</param>
        public void SubscribeTo(Type messageType, ISubscription subscription)
        {
            broadcaster.Subscribe(messageType, (ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg>)subscription);
        }

        /// <summary>
        /// Unsubscribes from messages of the specified type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the messages to unsubscribe from.</typeparam>
        /// <param name="subscription">The subscription to remove.</param>
        public void UnsubscribeFrom<TMessage>(ISubscription subscription) where TMessage : IMessage
        {
            broadcaster.Unsubscribe<TMessage>((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TMessage>, IInvokableSingleArgGeneric<TMessage>>)subscription);
        }
        
        /// <summary>
        /// Unsubscribes from messages of the specified type.
        /// </summary>
        /// <param name="messageType">The type of the messages to unsubscribe from.</param>
        /// <param name="subscription">The subscription to remove.</param>
        public void UnsubscribeFrom(Type messageType, ISubscription subscription)
        {
            broadcaster.Unsubscribe(messageType, (ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg>)subscription);
        }
        
        #endregion
    }
}