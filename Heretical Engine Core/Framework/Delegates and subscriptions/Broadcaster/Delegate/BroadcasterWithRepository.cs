using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Broadcasting
{
    public class BroadcasterWithRepository
        : IPublisherSingleArg,
          ISubscribableSingleArg,
          ICleanUppable,
          IDisposable
    {
        private readonly IReadOnlyObjectRepository broadcasterRepository;

        private readonly ILogger logger;

        public BroadcasterWithRepository(
            IReadOnlyObjectRepository broadcasterRepository,
            ILogger logger = null)
        {
            this.broadcasterRepository = broadcasterRepository;
            
            this.logger = logger;
        }

        #region IPublisherSingleArg

        public void Publish<TValue>(TValue value)
        {
            var valueType = typeof(TValue);
            
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<BroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (IPublisherSingleArgGeneric<TValue>)broadcasterObject;
            
            broadcaster.Publish(value);
        }
        
        public void Publish(
            Type valueType,
            object value)
        {
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<BroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (IPublisherSingleArg)broadcasterObject;
            
            broadcaster.Publish(valueType, value);
        }

        #endregion

        #region ISubscribableSingleArg
		
        public void Subscribe<TValue>(Action<TValue> @delegate)
        {
            var valueType = typeof(TValue);
            
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<BroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (ISubscribableSingleArgGeneric<TValue>)broadcasterObject;
            
            broadcaster.Subscribe(@delegate);
        }

        public void Subscribe(
            Type valueType,
            object @delegate)
        {
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<BroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (ISubscribableSingleArg)broadcasterObject;
            
            broadcaster.Subscribe(valueType, @delegate);
        }

        public void Unsubscribe<TValue>(Action<TValue> @delegate)
        {
            var valueType = typeof(TValue);
            
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<BroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (ISubscribableSingleArgGeneric<TValue>)broadcasterObject;
            
            broadcaster.Unsubscribe(@delegate);
        }

        public void Unsubscribe(
            Type valueType,
            object @delegate)
        {
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<BroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (ISubscribableSingleArg)broadcasterObject;
            
            broadcaster.Unsubscribe(valueType, @delegate);
        }

        public IEnumerable<Action<TValue>> GetAllSubscriptions<TValue>()
        {
            var valueType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<BroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (ISubscribableSingleArgGeneric<TValue>)broadcasterObject;

            return broadcaster.AllSubscriptions;
        }

        public IEnumerable<object> GetAllSubscriptions(Type valueType)
        {
            if (!broadcasterRepository.TryGet(
                    valueType,
                    out object broadcasterObject))
                throw new Exception(
                    logger.TryFormat<BroadcasterWithRepository>(
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));

            var broadcaster = (ISubscribable)broadcasterObject;

            return broadcaster.AllSubscriptions;
        }

        #region ISubscribable

        public IEnumerable<object> AllSubscriptions
        {
            get
            {
                List<object> result = new List<object>();

                foreach (var broadcaster in broadcasterRepository.Keys)
                {
                    var subscribable = (ISubscribable)broadcaster;

                    result.AddRange(subscribable.AllSubscriptions);
                }

                return result;
            }
        }

        public void UnsubscribeAll()
        {
            foreach (var broadcaster in broadcasterRepository.Keys)
            {
                var subscribable = (ISubscribable)broadcaster;

                subscribable.UnsubscribeAll();
            }
        }

        #endregion

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (broadcasterRepository is ICleanUppable)
                (broadcasterRepository as ICleanUppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (broadcasterRepository is IDisposable)
                (broadcasterRepository as IDisposable).Dispose();
        }

        #endregion
    }
}