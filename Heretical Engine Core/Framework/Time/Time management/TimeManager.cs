using HereticalSolutions.Repositories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Time
{
    public class TimeManager
        : ITimeManager,
          ISynchronizablesGenericArgRepository<float>,
          ISynchronizationProvidersRepository,
          ICleanUppable,
          IDisposable
    {
        private readonly IRepository<string, ISynchronizableGenericArg<float>> chronoRepository;

        private readonly IRuntimeTimer applicationRuntimeTimer;

        private readonly IPersistentTimer applicationPersistentTimer;

        public TimeManager(
            IRepository<string, ISynchronizableGenericArg<float>> chronoRepository,
            IRuntimeTimer applicationRuntimeTimer,
            IPersistentTimer applicationPersistentTimer)
        {
            this.chronoRepository = chronoRepository;

            this.applicationRuntimeTimer = applicationRuntimeTimer;

            this.applicationPersistentTimer = applicationPersistentTimer;
        }

        #region ITimeManager

        public IRuntimeTimer ApplicationRuntimeTimer { get => applicationRuntimeTimer; }

        public IPersistentTimer ApplicationPersistentTimer { get => applicationPersistentTimer; }

        #region ITickable

        public void Tick(float delta)
        {
            ((ITickable)applicationRuntimeTimer).Tick(delta);

            ((ITickable)applicationPersistentTimer).Tick(delta);

            foreach (var key in chronoRepository.Keys)
            {
                var synchronizable = chronoRepository.Get(key);

                synchronizable.Synchronize(delta);
            }
        }

        #endregion

        #endregion

        #region ISynchronizablesGenericArgRepository

        #region IReadOnlySynchronizablesGenericArgRepository

        public bool TryGetSynchronizable(
            string id,
            out ISynchronizableGenericArg<float> synchronizable)
        {
            return chronoRepository.TryGet(
                id,
                out synchronizable);
        }

        #endregion

        public void AddSynchronizable(ISynchronizableGenericArg<float> synchronizable)
        {
            chronoRepository.TryAdd(
                synchronizable.Descriptor.ID,
                synchronizable);
        }

        public void RemoveSynchronizable(string id)
        {
            if (!chronoRepository.TryGet(
                id,
                out var synchronizable))
                return;

            ((ISynchronizationProvider)synchronizable).UnsubscribeAll();

            chronoRepository.TryRemove(id);
        }

        public void RemoveSynchronizable(ISynchronizableGenericArg<float> synchronizable)
        {
            RemoveSynchronizable(synchronizable.Descriptor.ID);
        }

        public void RemoveAllSynchronizables()
        {
            foreach (var synchronizable in chronoRepository.Values)
            {
                ((ISynchronizationProvider)synchronizable).UnsubscribeAll();
            }

            chronoRepository.Clear();
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

        #region ICleanUppable

        public void Cleanup()
        {
            if (chronoRepository is ICleanUppable)
                (chronoRepository as ICleanUppable).Cleanup();

            if (applicationRuntimeTimer is ICleanUppable)
                (applicationRuntimeTimer as ICleanUppable).Cleanup();

            if (applicationPersistentTimer is ICleanUppable)
                (applicationPersistentTimer as ICleanUppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (chronoRepository is IDisposable)
                (chronoRepository as IDisposable).Dispose();

            if (applicationRuntimeTimer is IDisposable)
                (applicationRuntimeTimer as IDisposable).Dispose();

            if (applicationPersistentTimer is IDisposable)
                (applicationPersistentTimer as IDisposable).Dispose();
        }

        #endregion
    }
}