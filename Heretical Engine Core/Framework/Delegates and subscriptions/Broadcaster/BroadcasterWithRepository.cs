using System;

using HereticalSolutions.Logging;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Delegates.Broadcasting
{
    /// <summary>
    /// A class that serves as a broadcaster with a repository.
    /// Implements the <see cref="IPublisherSingleArg"/> and <see cref="ISubscribableSingleArg"/> interfaces.
    /// </summary>
    public class BroadcasterWithRepository
        : IPublisherSingleArg,
          ISubscribableSingleArg
    {
        private readonly IReadOnlyObjectRepository broadcasterRepository;
        private readonly ISmartLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcasterWithRepository"/> class.
        /// </summary>
        /// <param name="broadcasterRepository">The repository of broadcasters.</param>
        /// <param name="logger">The logger to use for exception logging.</param>
        public BroadcasterWithRepository(
            IReadOnlyObjectRepository broadcasterRepository,
            ISmartLogger logger)
        {
            this.broadcasterRepository = broadcasterRepository;
            this.logger = logger;
        }

        #region IPublisherSingleArg

        /// <summary>
        /// Publishes a value to all subscribers.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to publish.</typeparam>
        /// <param name="value">The value to publish.</param>
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
        /// Publishes a value to all subscribers.
        /// </summary>
        /// <param name="valueType">The type of the value to publish.</param>
        /// <param name="value">The value to publish.</param>
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
            
            broadcaster.Publish(messageType, value);
        }

        #endregion

        #region ISubscribableSingleArg
		
        /// <summary>
        /// Subscribes a delegate to receive notifications when values of the specified type are published.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to subscribe to.</typeparam>
        /// <param name="delegate">The delegate to subscribe.</param>
        public void Subscribe<TValue>(Action<TValue> @delegate)
        {
            var messageType = typeof(TValue);
            
            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                    GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (ISubscribableSingleArgGeneric<TValue>)broadcasterObject;
            
            broadcaster.Subscribe(@delegate);
        }

        /// <summary>
        /// Subscribes a delegate to receive notifications when values of the specified type are published.
        /// </summary>
        /// <param name="valueType">The type of the value to subscribe to.</param>
        /// <param name="delegate">The delegate to subscribe.</param>
        public void Subscribe(Type valueType, object @delegate)
        {
            var messageType = valueType;
            
            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                    GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (ISubscribableSingleArg)broadcasterObject;
            
            broadcaster.Subscribe(valueType, @delegate);
        }

        /// <summary>
        /// Unsubscribes a delegate from receiving notifications when values of the specified type are published.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to unsubscribe from.</typeparam>
        /// <param name="delegate">The delegate to unsubscribe.</param>
        public void Unsubscribe<TValue>(Action<TValue> @delegate)
        {
            var messageType = typeof(TValue);
            
            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                    GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (ISubscribableSingleArgGeneric<TValue>)broadcasterObject;
            
            broadcaster.Unsubscribe(@delegate);
        }

        /// <summary>
        /// Unsubscribes a delegate from receiving notifications when values of the specified type are published.
        /// </summary>
        /// <param name="valueType">The type of the value to unsubscribe from.</param>
        /// <param name="delegate">The delegate to unsubscribe.</param>
        public void Unsubscribe(Type valueType, object @delegate)
        {
            var messageType = valueType;
            
            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                logger.Exception(
                    GetType(),
                    $"INVALID MESSAGE TYPE: \"{messageType.Name}\"");

            var broadcaster = (ISubscribableSingleArg)broadcasterObject;
            
            broadcaster.Unsubscribe(valueType, @delegate);
        }

        #endregion
    }
}