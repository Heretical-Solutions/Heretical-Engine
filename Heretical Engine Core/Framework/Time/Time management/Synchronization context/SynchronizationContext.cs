using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents a synchronization context.
    /// </summary>
    public class SynchronizationContext : ISynchronizable, ISynchronizationProvider
    {
        /// <summary>
        /// Gets or sets the synchronization context descriptor.
        /// </summary>
        public SynchronizationContextDescriptor Descriptor { get; private set; }

        private readonly IPublisherSingleArgGeneric<float> broadcasterAsPublisher;
        private readonly INonAllocSubscribableSingleArgGeneric<float> broadcasterAsSubscribable;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContext"/> class.
        /// </summary>
        /// <param name="descriptor">The synchronization context descriptor.</param>
        /// <param name="broadcasterAsPublisher">The publisher as a broadcaster.</param>
        /// <param name="broadcasterAsSubscribable">The subscriber as a broadcaster.</param>
        public SynchronizationContext(SynchronizationContextDescriptor descriptor, IPublisherSingleArgGeneric<float> broadcasterAsPublisher,
            INonAllocSubscribableSingleArgGeneric<float> broadcasterAsSubscribable)
        {
            Descriptor = descriptor;
            this.broadcasterAsPublisher = broadcasterAsPublisher;
            this.broadcasterAsSubscribable = broadcasterAsSubscribable;
        }

        #region ISynchronizable

        /// <summary>
        /// Synchronizes the context.
        /// </summary>
        /// <param name="delta">The time delta.</param>
        public void Synchronize(float delta)
        {
            if (Descriptor.CanBeToggled && !Descriptor.Active)
                return;

            if (Descriptor.CanScale)
                broadcasterAsPublisher.Publish(delta * Descriptor.Scale);
            else
                broadcasterAsPublisher.Publish(delta);
        }

        #endregion

        #region ISynchronizationProvider

        /// <summary>
        /// Subscribes to the synchronization context.
        /// </summary>
        /// <param name="subscription">The subscription to add.</param>
        public void Subscribe(ISubscription subscription)
        {
            broadcasterAsSubscribable.Subscribe((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<float>,
                IInvokableSingleArgGeneric<float>>)subscription);
        }

        /// <summary>
        /// Unsubscribes from the synchronization context.
        /// </summary>
        /// <param name="subscription">The subscription to remove.</param>
        public void Unsubscribe(ISubscription subscription)
        {
            broadcasterAsSubscribable.Unsubscribe((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<float>,
                IInvokableSingleArgGeneric<float>>)subscription);
        }

        //TODO: Add UnsubscribeAll
        #endregion
    }
}