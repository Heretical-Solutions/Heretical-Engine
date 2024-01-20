using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Broadcasting
{
    /// <summary>
    /// Represents a non-allocating broadcaster with a repository.
    /// Implements the <see cref="IPublisherSingleArg"/> and <see cref="INonAllocSubscribableSingleArg"/> interfaces.
    /// </summary>
    public class NonAllocBroadcasterWithRepository
        : IPublisherSingleArg,
          INonAllocSubscribableSingleArg
    {
        private readonly IReadOnlyObjectRepository broadcasterRepository;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAllocBroadcasterWithRepository"/> class.
        /// </summary>
        /// <param name="broadcasterRepository">The repository used to store broadcasters.</param>
        /// <param name="logger">The logger.</param>
        public NonAllocBroadcasterWithRepository(
            IReadOnlyObjectRepository broadcasterRepository,
            ILogger logger = null)
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
            var valueType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<NonAllocBroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (IPublisherSingleArgGeneric<TValue>)broadcasterObject;

            broadcaster.Publish(value);
        }

        /// <summary>
        /// Publishes a message of type <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The type of the message to be published.</param>
        /// <param name="value">The value to be published.</param>
        public void Publish(
            Type valueType,
            object value)
        {
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<NonAllocBroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

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
        public void Subscribe<TValue>(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription)
        {
            var valueType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<NonAllocBroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (INonAllocSubscribableSingleArgGeneric<TValue>)broadcasterObject;

            broadcaster.Subscribe(subscription);
        }

        /// <summary>
        /// Subscribes to messages of type <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The type of messages to subscribe to.</param>
        /// <param name="subscription">The subscription handler.</param>
        public void Subscribe(
            Type valueType,
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription)
        {
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<NonAllocBroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;

            broadcaster.Subscribe(valueType, subscription);
        }

        /// <summary>
        /// Unsubscribes from messages of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of messages to unsubscribe from.</typeparam>
        /// <param name="subscription">The subscription handler.</param>
        public void Unsubscribe<TValue>(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription)
        {
            var valueType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<NonAllocBroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (INonAllocSubscribableSingleArgGeneric<TValue>)broadcasterObject;

            broadcaster.Unsubscribe(subscription);
        }

        /// <summary>
        /// Unsubscribes from messages of type <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The type of messages to unsubscribe from.</param>
        /// <param name="subscription">The subscription handler.</param>
        public void Unsubscribe(
            Type valueType,
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription)
        {
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<NonAllocBroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;

            broadcaster.Unsubscribe(valueType, subscription);
        }

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>>
                INonAllocSubscribableSingleArg.GetAllSubscriptions<TValue>()
        {
            var valueType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<NonAllocBroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (INonAllocSubscribableSingleArgGeneric<TValue>)broadcasterObject;

            return broadcaster.AllSubscriptions;
        }

        public IEnumerable<ISubscription> GetAllSubscriptions(Type valueType)
        {
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<NonAllocBroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (INonAllocSubscribable)broadcasterObject;

            return broadcaster.AllSubscriptions;
        }

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>>
                INonAllocSubscribableSingleArg.AllSubscriptions
        {
            get
            {
                var result = new List<ISubscriptionHandler<
                    INonAllocSubscribableSingleArg,
                    IInvokableSingleArg>>();

                foreach (var key in broadcasterRepository.Keys)
                {
                    var broadcasterObject = broadcasterRepository.Get(key);

                    var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;

                    result.AddRange(broadcaster.AllSubscriptions);
                }

                return result;
            }
        }

        #region INonAllocSubscribable

        IEnumerable<ISubscription> INonAllocSubscribable.AllSubscriptions
        {
            get
            {
                List<ISubscription> result = new List<ISubscription>();

                foreach (var key in broadcasterRepository.Keys)
                {
                    var broadcasterObject = broadcasterRepository.Get(key);

                    var broadcaster = (INonAllocSubscribable)broadcasterObject;

                    result.AddRange(broadcaster.AllSubscriptions);
                }

                return result;
            }
        }

        public void UnsubscribeAll()
        {

        }

        #endregion

        #endregion
    }
}