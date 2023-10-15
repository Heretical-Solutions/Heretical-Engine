using HereticalSolutions.Collections;
using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates.Pinging
{
    /// <summary>
    /// Represents a class that can publish events without arguments and can have non-allocating subscribers.
    /// </summary>
    public class NonAllocPinger : IPublisherNoArgs, INonAllocSubscribableNoArgs
    {
        #region Subscriptions

        /// <summary>
        /// The pool of subscriptions.
        /// </summary>
        private readonly INonAllocDecoratedPool<IInvokableNoArgs> subscriptionsPool;

        /// <summary>
        /// The subscriptions as an indexable for fast access.
        /// </summary>
        private readonly IIndexable<IPoolElement<IInvokableNoArgs>> subscriptionsAsIndexable;

        /// <summary>
        /// The subscriptions with capacity for tracking the count.
        /// </summary>
        private readonly IFixedSizeCollection<IPoolElement<IInvokableNoArgs>> subscriptionsWithCapacity;

        #endregion

        #region Buffer

        /// <summary>
        /// The buffer to store current subscriptions.
        /// </summary>
        private IInvokableNoArgs[] currentSubscriptionsBuffer;

        /// <summary>
        /// The count of elements in the buffer.
        /// </summary>
        private int currentSubscriptionsBufferCount = -1;

        #endregion

        /// <summary>
        /// Flag that indicates if a ping is in progress.
        /// </summary>
        private bool pingInProgress = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAllocPinger"/> class.
        /// </summary>
        /// <param name="subscriptionsPool">The pool of subscriptions.</param>
        /// <param name="subscriptionsContents">The contents of the subscriptions.</param>
        public NonAllocPinger(
            INonAllocDecoratedPool<IInvokableNoArgs> subscriptionsPool,
            INonAllocPool<IInvokableNoArgs> subscriptionsContents)
        {
            this.subscriptionsPool = subscriptionsPool;

            subscriptionsAsIndexable = (IIndexable<IPoolElement<IInvokableNoArgs>>)subscriptionsContents;

            subscriptionsWithCapacity =
                (IFixedSizeCollection<IPoolElement<IInvokableNoArgs>>)subscriptionsContents;

            currentSubscriptionsBuffer = new IInvokableNoArgs[subscriptionsWithCapacity.Capacity];
        }

        #region INonAllocSubscribableNoArgs

        /// <summary>
        /// Subscribes to the event.
        /// </summary>
        /// <param name="subscription">The subscription to be added.</param>
        public void Subscribe(ISubscription subscription)
        {
            var subscriptionHandler = (ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>)subscription;

            if (!subscriptionHandler.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop(null);

            subscriptionElement.Value = ((ISubscriptionState<IInvokableNoArgs>)subscription).Invokable;

            subscriptionHandler.Activate(this, subscriptionElement);
        }

        /// <summary>
        /// Unsubscribes from the event.
        /// </summary>
        /// <param name="subscription">The subscription to be removed.</param>
        public void Unsubscribe(ISubscription subscription)
        {
            var subscriptionHandler = (ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>)subscription;

            if (!subscriptionHandler.ValidateTermination(this))
                return;

            var poolElement = ((ISubscriptionState<IInvokableNoArgs>)subscription).PoolElement;

            TryRemoveFromBuffer(poolElement);

            poolElement.Value = null;

            subscriptionsPool.Push(poolElement);

            subscriptionHandler.Terminate();
        }

        /// <summary>
        /// Unsubscribes from the event.
        /// </summary>
        /// <param name="subscription">The subscription to be removed.</param>
        public void Unsubscribe(IPoolElement<IInvokableNoArgs> subscription)
        {
            TryRemoveFromBuffer(subscription);

            subscription.Value = null;

            subscriptionsPool.Push(subscription);
        }

        /// <summary>
        /// Tries to remove a subscription from the buffer if a ping is in progress.
        /// </summary>
        /// <param name="subscriptionElement">The subscription element to be removed.</param>
        private void TryRemoveFromBuffer(IPoolElement<IInvokableNoArgs> subscriptionElement)
        {
            if (!pingInProgress)
                return;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                if (currentSubscriptionsBuffer[i] == subscriptionElement.Value)
                {
                    currentSubscriptionsBuffer[i] = null;

                    return;
                }
        }

        #endregion

        #region IPublisherNoArgs

        /// <summary>
        /// Publishes the event to all subscribers.
        /// </summary>
        public void Publish()
        {
            ValidateBufferSize();

            currentSubscriptionsBufferCount = subscriptionsAsIndexable.Count;

            CopySubscriptionsToBuffer();

            InvokeSubscriptions();

            EmptyBuffer();
        }

        /// <summary>
        /// Validates the size of the buffer and resizes if necessary.
        /// </summary>
        private void ValidateBufferSize()
        {
            if (currentSubscriptionsBuffer.Length < subscriptionsWithCapacity.Capacity)
                currentSubscriptionsBuffer = new IInvokableNoArgs[subscriptionsWithCapacity.Capacity];
        }

        /// <summary>
        /// Copies the subscriptions to the buffer.
        /// </summary>
        private void CopySubscriptionsToBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = subscriptionsAsIndexable[i].Value;
        }

        /// <summary>
        /// Invokes the subscriptions in the buffer.
        /// </summary>
        private void InvokeSubscriptions()
        {
            pingInProgress = true;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null)
                    currentSubscriptionsBuffer[i].Invoke();
            }

            pingInProgress = false;
        }

        /// <summary>
        /// Empties the subscription buffer.
        /// </summary>
        private void EmptyBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = null;
        }

        #endregion
    }
}