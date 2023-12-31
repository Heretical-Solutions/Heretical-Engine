using HereticalSolutions.Delegates;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization
{
    public class SynchronizationContext
        : ISynchronizable,
          ISynchronizationProvider
    {
        private SynchronizationDescriptor descriptor;

        private readonly IPublisherNoArgs pingerAsPublisher;

        private readonly INonAllocSubscribableNoArgs pingerAsSubscribable;

        private readonly IFormatLogger logger;

        public SynchronizationContext(
            SynchronizationDescriptor descriptor,
            IPublisherNoArgs pingerAsPublisher,
            INonAllocSubscribableNoArgs pingerAsSubscribable,
            IFormatLogger logger)
        {
            this.descriptor = descriptor;

            this.pingerAsPublisher = pingerAsPublisher;

            this.pingerAsSubscribable = pingerAsSubscribable;

            this.logger = logger;
        }

        #region ISynchronizable

        public SynchronizationDescriptor Descriptor { get => descriptor; }

        public void Toggle(bool active)
        {
            if (!descriptor.CanBeToggled)
            {
                logger?.ThrowException<SynchronizationContext>(
                    "SYNCHRONIZATION IS TOGGLED WHILE ITS CONSTRAINED NOT TO BE ABLE TO BE TOGGLED");
            }

            descriptor.Active = active;
        }

        public void Synchronize()
        {
            if (!descriptor.CanBeToggled && !descriptor.Active)
            {
                logger?.ThrowException<SynchronizationContext>(
                    "SYNCHRONIZATION IS TOGGLED WHILE ITS CONSTRAINED NOT TO BE ABLE TO BE TOGGLED");
            }

            if (!descriptor.Active)
                return;

            if (descriptor.CanScale)
            {
                logger?.ThrowException<SynchronizationContext>(
                    "THIS SYNCHRONIZATION IS NOT GENERIC, IT CANNOT BE SCALED");
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