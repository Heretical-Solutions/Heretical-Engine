using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Synchronization
{
    public class SynchronizationContext
        : ISynchronizableNoArgs,
          ISynchronizationProvider
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

        //TODO: Add UnsubscribeAll
        #endregion
    }
}