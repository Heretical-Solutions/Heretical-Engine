using HereticalSolutions.Collections;

using HereticalSolutions.Delegates;

using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Pools;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging
{
    public class NonAllocMessageBus
        : INonAllocMessageSender,
          INonAllocMessageReceiver
    {
        private readonly NonAllocBroadcasterWithRepository broadcaster;

        private readonly IReadOnlyObjectRepository messageRepository;

        private readonly INonAllocDecoratedPool<IPoolElement<IMessage>> mailbox;

        private readonly IIndexable<IPoolElement<IPoolElement<IMessage>>> mailboxContentsAsIndexable;

        private readonly IFormatLogger logger;
        
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
            IIndexable<IPoolElement<IPoolElement<IMessage>>> mailboxContentsAsIndexable,
            IFormatLogger logger)
        {
            this.broadcaster = broadcaster;

            this.messageRepository = messageRepository;

            this.mailbox = mailbox;

            this.mailboxContentsAsIndexable = mailboxContentsAsIndexable;

            this.logger = logger;
        }

        #region IMessageSenderNonAlloc

        #region Pop
        
        public INonAllocMessageSender PopMessage(
            Type messageType,
            out IPoolElement<IMessage> message)
        {
            if (!messageRepository.TryGet(
                messageType,
                out object messagePoolObject))
                logger?.ThrowException<NonAllocMessageBus>(
                    $"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.Name}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;

            message = messagePool.Pop(null);

            return this;
        }

        public INonAllocMessageSender PopMessage<TMessage>(out IPoolElement<IMessage> message) where TMessage : IMessage
        {
            if (!messageRepository.TryGet(
                    typeof(TMessage),
                    out object messagePoolObject))
                logger?.ThrowException<NonAllocMessageBus>(
                    $"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).Name}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;

            message = messagePool.Pop(null);

            return this;
        }
        
        #endregion

        #region Write

        public INonAllocMessageSender Write(
            IPoolElement<IMessage> messageElement,
            object[] args)
        {
            if (messageElement == null)
                logger?.ThrowException<NonAllocMessageBus>(
                    $"INVALID MESSAGE");

            messageElement.Value.Write(args);

            return this;
        }

        public INonAllocMessageSender Write<TMessage>(
            IPoolElement<IMessage> messageElement,
            object[] args) where TMessage : IMessage
        {
            if (messageElement == null)
                logger?.ThrowException<NonAllocMessageBus>(
                    $"INVALID MESSAGE");

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

            if (!messageRepository.TryGet(
                messageType,
                out object messagePoolObject))
                logger?.ThrowException<NonAllocMessageBus>(
                    $"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.Name}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;

            messagePool.Push(message);
        }

        private void PushMessageToPool<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);

            if (!messageRepository.TryGet(
                messageType,
                out object messagePoolObject))
                logger?.ThrowException<NonAllocMessageBus>(
                    $"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).Name}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;

            messagePool.Push(message);
        }
        
        #endregion

        #region IMessageReceiverNonAlloc
        
        public void SubscribeTo<TMessage>(ISubscription subscription) where TMessage : IMessage
        {
            broadcaster.Subscribe<TMessage>(
                (ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TMessage>, IInvokableSingleArgGeneric<TMessage>>)
                    subscription);
        }
        
        public void SubscribeTo(
            Type messageType,
            ISubscription subscription)
        {
            broadcaster.Subscribe(
                messageType,
                (ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg>)
                    subscription);
        }

        public void UnsubscribeFrom<TMessage>(ISubscription subscription) where TMessage : IMessage
        {
            broadcaster.Unsubscribe<TMessage>(
                (ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TMessage>, IInvokableSingleArgGeneric<TMessage>>)
                    subscription);
        }
        
        public void UnsubscribeFrom(
            Type messageType,
            ISubscription subscription)
        {
            broadcaster.Unsubscribe(
                messageType,
                (ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg>)
                    subscription);
        }
        
        #endregion
    }
}