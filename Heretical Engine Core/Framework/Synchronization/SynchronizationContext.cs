using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Synchronization
{
    public class SynchronizationContext
        : ISynchronizableNoArgs,
          ISynchronizationProvider,
          ICleanUppable,
          IDisposable
    {
        private readonly IReadOnlyObjectRepository metadata;

        private readonly IPublisherNoArgs pingerAsPublisher;

        private readonly INonAllocSubscribableNoArgs pingerAsSubscribable;


        private SynchronizationDescriptor descriptor;

        public SynchronizationContext(
            SynchronizationDescriptor descriptor,
            IReadOnlyObjectRepository metadata,
            IPublisherNoArgs pingerAsPublisher,
            INonAllocSubscribableNoArgs pingerAsSubscribable)
        {
            this.descriptor = descriptor;

            this.metadata = metadata;

            this.pingerAsPublisher = pingerAsPublisher;

            this.pingerAsSubscribable = pingerAsSubscribable;
        }

        #region ISynchronizableNoArgs

        #region ISynchronizable

        public SynchronizationDescriptor Descriptor { get => descriptor; }

        public IReadOnlyObjectRepository Metadata { get => metadata; }

        #endregion

        public void Synchronize()
        {
            if (metadata.Has<ITogglable>())
            {
                var togglable = metadata.Get<ITogglable>();

                if (!togglable.Active)
                {
                    return;
                }
            }

            pingerAsPublisher.Publish();
        }

        #endregion

        #region ISynchronizationProvider

        public void Subscribe(ISubscription subscription)
        {
            pingerAsSubscribable.Subscribe(
                subscription);
        }

        public void Unsubscribe(ISubscription subscription)
        {
            pingerAsSubscribable.Unsubscribe(
                subscription);
        }

        public void UnsubscribeAll()
        {
            pingerAsSubscribable.UnsubscribeAll();
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (metadata is ICleanUppable)
                (metadata as ICleanUppable).Cleanup();

            if (pingerAsPublisher is ICleanUppable)
                (pingerAsPublisher as ICleanUppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (metadata is IDisposable)
                (metadata as IDisposable).Dispose();

            if (pingerAsPublisher is IDisposable)
                (pingerAsPublisher as IDisposable).Dispose();
        }

        #endregion
    }
}