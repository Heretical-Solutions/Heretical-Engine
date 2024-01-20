using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Pools;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging
{
    public class MessageBus
        : IMessageSender,
          IMessageReceiver
    {
        private readonly BroadcasterWithRepository broadcaster;

        private readonly IReadOnlyObjectRepository messageRepository;

        private readonly Queue<IMessage> mailbox;

        private readonly ILogger logger;

        public MessageBus(
            BroadcasterWithRepository broadcaster,
            IReadOnlyObjectRepository messageRepository,
            Queue<IMessage> mailbox,
            ILogger logger = null)
        {
            this.broadcaster = broadcaster;

            this.messageRepository = messageRepository;

            this.mailbox = mailbox;

            this.logger = logger;
        }

        #region IMessageSender

        #region Pop

        public IMessageSender PopMessage(
            Type messageType,
            out IMessage message)
        {
            if (!messageRepository.TryGet(
                    messageType,
                    out object messagePoolObject))
                throw new Exception(
                    logger.TryFormat<MessageBus>(
                        $"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.Name}"));

            IPool<IMessage> messagePool = (IPool<IMessage>)messagePoolObject;

            message = messagePool.Pop();

            return this;
        }

        public IMessageSender PopMessage<TMessage>(out TMessage message) where TMessage : IMessage
        {
            if (!messageRepository.TryGet(
                    typeof(TMessage),
                    out object messagePoolObject))
                throw new Exception(
                    logger.TryFormat<MessageBus>(
                        $"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).Name}"));

            IPool<IMessage> messagePool = (IPool<IMessage>)messagePoolObject;

            message = (TMessage)messagePool.Pop();

            return this;
        }

        #endregion

        #region Write

        public IMessageSender Write(IMessage message, object[] args)
        {
            if (message == null)
                throw new Exception(
                    logger.TryFormat<MessageBus>(
                        $"INVALID MESSAGE"));

            message.Write(args);

            return this;
        }

        public IMessageSender Write<TMessage>(
            TMessage message,
            object[] args) where TMessage : IMessage
        {
            if (message == null)
                throw new Exception(
                    logger.TryFormat<MessageBus>(
                        $"INVALID MESSAGE"));

            message.Write(args);

            return this;
        }

        #endregion

        #region Send

        /// <summary>
        /// Sends the specified message to the mailbox for later delivery.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(IMessage message)
        {
            mailbox.Enqueue(message);
        }

        /// <summary>
        /// Sends the specified message to the mailbox for later delivery.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to send.</typeparam>
        /// <param name="message">The message to send.</param>
        public void Send<TMessage>(TMessage message) where TMessage : IMessage
        {
            mailbox.Enqueue(message);
        }

        /// <summary>
        /// Publishes the specified message immediately and pushes it back to the message pool.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        public void SendImmediately(IMessage message)
        {
            broadcaster.Publish(message.GetType(), message);

            PushMessageToPool(message);
        }

        /// <summary>
        /// Publishes the specified message immediately and pushes it back to the message pool.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        /// <param name="message">The message to publish.</param>
        public void SendImmediately<TMessage>(TMessage message) where TMessage : IMessage
        {
            broadcaster.Publish<TMessage>(message);

            PushMessageToPool<TMessage>(message);
        }

        #endregion

        #region Deliver

        /// <summary>
        /// Delivers all the messages in the mailbox.
        /// </summary>
        public void DeliverMessagesInMailbox()
        {
            int messagesToReceive = mailbox.Count;

            for (int i = 0; i < messagesToReceive; i++)
            {
                var message = mailbox.Dequeue();

                SendImmediately(message);
            }
        }

        #endregion

        private void PushMessageToPool(IMessage message)
        {
            var messageType = message.GetType();

            if (!messageRepository.TryGet(
                    messageType,
                    out object messagePoolObject))
                throw new Exception(
                    logger.TryFormat<MessageBus>(
                        $"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.Name}"));

            IPool<IMessage> messagePool = (IPool<IMessage>)messagePoolObject;

            messagePool.Push(message);
        }

        private void PushMessageToPool<TMessage>(TMessage message) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);

            if (!messageRepository.TryGet(
                    messageType,
                    out object messagePoolObject))
                throw new Exception(
                    logger.TryFormat<MessageBus>(
                        $"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).Name}"));

            IPool<IMessage> messagePool = (IPool<IMessage>)messagePoolObject;

            messagePool.Push(message);
        }

        #endregion

        #region IMessageReceiver

        /// <summary>
        /// Subscribes to messages of the specified type with the provided receiver delegate.
        /// </summary>
        /// <typeparam name="TMessage">The type of the messages to subscribe to.</typeparam>
        /// <param name="receiverDelegate">The receiver delegate that handles the messages.</param>
        public void SubscribeTo<TMessage>(Action<TMessage> receiverDelegate) where TMessage : IMessage
        {
            broadcaster.Subscribe<TMessage>(receiverDelegate);
        }

        /// <summary>
        /// Subscribes to messages of the specified type with the provided receiver delegate.
        /// </summary>
        /// <param name="messageType">The type of the messages to subscribe to.</param>
        /// <param name="receiverDelegate">The receiver delegate that handles the messages.</param>
        public void SubscribeTo(Type messageType, object receiverDelegate)
        {
            broadcaster.Subscribe(messageType, receiverDelegate);
        }

        /// <summary>
        /// Unsubscribes from messages of the specified type with the provided receiver delegate.
        /// </summary>
        /// <typeparam name="TMessage">The type of the messages to unsubscribe from.</typeparam>
        /// <param name="receiverDelegate">The receiver delegate to unsubscribe.</param>
        public void UnsubscribeFrom<TMessage>(Action<TMessage> receiverDelegate) where TMessage : IMessage
        {
            broadcaster.Unsubscribe<TMessage>(receiverDelegate);
        }

        /// <summary>
        /// Unsubscribes from messages of the specified type with the provided receiver delegate.
        /// </summary>
        /// <param name="messageType">The type of the messages to unsubscribe from.</param>
        /// <param name="receiverDelegate">The receiver delegate to unsubscribe.</param>
        public void UnsubscribeFrom(Type messageType, object receiverDelegate)
        {
            broadcaster.Unsubscribe(messageType, receiverDelegate);
        }

        #endregion
    }
}