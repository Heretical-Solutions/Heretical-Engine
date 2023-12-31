using HereticalSolutions.Repositories;

using HereticalSolutions.Synchronization;

namespace HereticalSolutions.Time
{
    public class TimeManager
        : ITimeManager,
          ISynchronizablesRepository,
          ISynchronizationProvidersRepository,
          ITickable
    {
        private readonly IRepository<string, ISynchronizableGeneric<float>> chronoRepository;

        private readonly IRuntimeTimer applicationActiveTimer;

        private readonly IPersistentTimer applicationPersistentTimer;

        public TimeManager(
            IRepository<string, ISynchronizableGeneric<float>> chronoRepository,
            IRuntimeTimer applicationActiveTimer,
            IPersistentTimer applicationPersistentTimer)
        {
            this.chronoRepository = chronoRepository;

            this.applicationActiveTimer = applicationActiveTimer;

            this.applicationPersistentTimer = applicationPersistentTimer;
        }

        #region ITimeManager

        public IRuntimeTimer ApplicationActiveTimer { get => applicationActiveTimer; }

        public IPersistentTimer ApplicationPersistentTimer { get => applicationPersistentTimer; }

        #endregion

        #region ISynchronizablesRepository

        public bool TryGetSynchronizable(
            string id,
            out ISynchronizableGeneric<float> synchronizable)
        {
            return chronoRepository.TryGet(
                id,
                out synchronizable);
        }

        public void AddSynchronizable(ISynchronizableGeneric<float> synchronizable)
        {
            chronoRepository.TryAdd(
                synchronizable.Descriptor.ID,
                synchronizable);
        }

        public void RemoveSynchronizable(string id)
        {
            chronoRepository.TryRemove(id);
        }

        public void RemoveSynchronizable(ISynchronizableGeneric<float> synchronizable)
        {
            chronoRepository.TryRemove(synchronizable.Descriptor.ID);
        }

        #endregion

        #region ISynchronizationProvidersRepository

        public bool TryGetProvider(
            string id,
            out ISynchronizationProvider provider)
        {
            provider = default;

            var result = chronoRepository.TryGet(
                id,
                out var synchronizable);

            if (result)
            {
                provider = (ISynchronizationProvider)synchronizable;
            }

            return result;
        }

        #endregion

        #region ITickable

        public void Tick(float delta)
        {
            ((ITickable)applicationActiveTimer).Tick(delta);

            ((ITickable)applicationPersistentTimer).Tick(delta);

            foreach (var key in chronoRepository.Keys)
            {
                var synchronizable = chronoRepository.Get(key);

                synchronizable.Synchronize(delta);
            }
        }

        #endregion
    }
}