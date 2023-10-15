using System;

using HereticalSolutions.Logging;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Delegates.Broadcasting
{
    /// <summary>
    /// Represents a non-allocating broadcaster with a repository.
    /// Implements the <see cref="IPublisherSingleArg"/> and <see cref="INonAllocSubscribableSingleArg"/> interfaces.
    /// </summary>
    public class NonAllocBroadcasterWithRepository : IPublisherSingleArg, INonAllocSubscribableSingleArg
    {
        private readonly IReadOnlyObjectRepository broadcasterRepository;

        private readonly ISmartLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAllocBroadcasterWithRepository"/> class.
        /// </summary>
        /// <param name="broadcasterRepository">The repository used to store broadcasters.</param>
        /// <param name="logger">The logger.</param>
        public NonAllocBroadcasterWithRepository(
            IReadOnlyObjectRepository broadcasterRepository,
            ISmartLogger logger)
        {
            this.broadcasterRepository = broadcasterRepository;
            this.logger = logger;
        }

        #region IPublisherSingleArg

        /// <summary>
        /// Publishes a message of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of message to be published.</typeparam>
        /// <param name="value">The value to be published.</param>
        public void Publish<TValue>(TValue value)
        {
            var messageType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                    GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (IPublisherSingleArgGeneric<TValue>)broadcasterObject;

            broadcaster.Publish(value);
        }

        /// <summary>
        /// Publishes a message of type <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The type of the message to be published.</param>
        /// <param name="value">The value to be published.</param>
        public void Publish(Type valueType, object value)
        {
            var messageType = valueType;

            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                    GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (IPublisherSingleArg)broadcasterObject;

            broadcaster.Publish(valueType, value);
        }

        #endregion

        #region INonAllocSubscribableSingleArg

        /// <summary>
        /// Subscribes to messages of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of messages to subscribe to.</typeparam>
        /// <param name="subscription">The subscription handler.</param>
        public void Subscribe<TValue>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription)
        {
            var messageType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                    GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (INonAllocSubscribableSingleArgGeneric<TValue>)broadcasterObject;

            broadcaster.Subscribe(subscription);
        }

        /// <summary>
        /// Subscribes to messages of type <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The type of messages to subscribe to.</param>
        /// <param name="subscription">The subscription handler.</param>
        public void Subscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription)
        {
            var messageType = valueType;

            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                    GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;

            broadcaster.Subscribe(messageType, subscription);
        }

        /// <summary>
        /// Unsubscribes from messages of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of messages to unsubscribe from.</typeparam>
        /// <param name="subscription">The subscription handler.</param>
        public void Unsubscribe<TValue>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription)
        {
            var messageType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                    GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (INonAllocSubscribableSingleArgGeneric<TValue>)broadcasterObject;

            broadcaster.Unsubscribe(subscription);
        }

        /// <summary>
        /// Unsubscribes from messages of type <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The type of messages to unsubscribe from.</param>
        /// <param name="subscription">The subscription handler.</param>
        public void Unsubscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription)
        {
            var messageType = valueType;

            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;

            broadcaster.Unsubscribe(messageType, subscription);
        }

        #endregion
    }
}