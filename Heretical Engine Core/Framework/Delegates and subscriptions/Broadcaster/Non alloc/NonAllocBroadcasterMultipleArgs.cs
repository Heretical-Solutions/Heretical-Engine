using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates.Broadcasting
{
    /// <summary>
    /// A class that implements the IPublisherMultipleArgs and INonAllocSubscribableMultipleArgs interfaces.
    /// </summary>
    public class NonAllocBroadcasterMultipleArgs : IPublisherMultipleArgs, INonAllocSubscribableMultipleArgs
    {
        #region Subscriptions

        /// <summary>
        /// The pool of subscriptions.
        /// </summary>
        private readonly INonAllocDecoratedPool<IInvokableMultipleArgs> subscriptionsPool;

        /// <summary>
        /// The subscriptions stored as an indexable collection.
        /// </summary>
        private readonly IIndexable<IPoolElement<IInvokableMultipleArgs>> subscriptionsAsIndexable;

        /// <summary>
        /// The subscriptions stored in a fixed-size collection.
        /// </summary>
        private readonly IFixedSizeCollection<IPoolElement<IInvokableMultipleArgs>> subscriptionsWithCapacity;

        #endregion

        #region Buffer

        /// <summary>
        /// The buffer that holds the current subscriptions.
        /// </summary>
        private IInvokableMultipleArgs[] currentSubscriptionsBuffer;

        /// <summary>
        /// The count of valid entries in the current subscriptions buffer.
        /// </summary>
        private int currentSubscriptionsBufferCount = -1;

        #endregion

        /// <summary>
        /// A flag that indicates if a broadcast is currently in progress.
        /// </summary>
        private bool broadcastInProgress = false;

        /// <summary>
        /// Initializes a new instance of the NonAllocBroadcasterMultipleArgs class.
        /// </summary>
        /// <param name="subscriptionsPool">The pool of subscriptions.</param>
        /// <param name="subscriptionsContents">The contents of the subscriptions pool.</param>
        public NonAllocBroadcasterMultipleArgs(INonAllocDecoratedPool<IInvokableMultipleArgs> subscriptionsPool,
            INonAllocPool<IInvokableMultipleArgs> subscriptionsContents)
        {
            this.subscriptionsPool = subscriptionsPool;
            subscriptionsAsIndexable = (IIndexable<IPoolElement<IInvokableMultipleArgs>>)subscriptionsContents;
            subscriptionsWithCapacity = (IFixedSizeCollection<IPoolElement<IInvokableMultipleArgs>>)subscriptionsContents;
            currentSubscriptionsBuffer = new IInvokableMultipleArgs[subscriptionsWithCapacity.Capacity];
        }

        #region IPublisherMultipleArgs

        /// <summary>
        /// Subscribes to the broadcaster.
        /// </summary>
        /// <param name="subscription">The subscription handler.</param>
        public void Subscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription)
        {
            if (!subscription.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop(null);

            subscriptionElement.Value = ((ISubscriptionState<IInvokableMultipleArgs>)subscription).Invokable;

            subscription.Activate(this, subscriptionElement);
        }

        /// <summary>
        /// Unsubscribes from the broadcaster.
        /// </summary>
        /// <param name="subscription">The subscription handler.</param>
        public void Unsubscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription)
        {
            if (!subscription.ValidateTermination(this))
                return;

            var poolElement = ((ISubscriptionState<IInvokableMultipleArgs>)subscription).PoolElement;

            TryRemoveFromBuffer(poolElement);

            poolElement.Value = null;

            subscriptionsPool.Push(poolElement);

            subscription.Terminate();
        }

        /// <summary>
        /// Unsubscribes from the broadcaster.
        /// </summary>
        /// <param name="subscription">The subscription to unsubscribe.</param>
        public void Unsubscribe(IPoolElement<IInvokableMultipleArgs> subscription)
        {
            TryRemoveFromBuffer(subscription);

            subscription.Value = null;

            subscriptionsPool.Push(subscription);
        }

        // A private method used to remove a subscription from the current subscriptions buffer.
        private void TryRemoveFromBuffer(IPoolElement<IInvokableMultipleArgs> subscriptionElement)
        {
            if (!broadcastInProgress)
                return;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] == subscriptionElement.Value)
                {
                    currentSubscriptionsBuffer[i] = null;
                    return;
                }
            }
        }

        #endregion

        #region IPublisherMultipleArgs

        /// <summary>
        /// Publishes an event with the specified value.
        /// </summary>
        /// <param name="value">The value to publish.</param>
        public void Publish(object[] value)
        {
            ValidateBufferSize();

            currentSubscriptionsBufferCount = subscriptionsAsIndexable.Count;

            CopySubscriptionsToBuffer();

            InvokeSubscriptions(value);

            EmptyBuffer();
        }

        // A private method used to validate and resize the buffer if necessary.
        private void ValidateBufferSize()
        {
            if (currentSubscriptionsBuffer.Length < subscriptionsWithCapacity.Capacity)
                currentSubscriptionsBuffer = new IInvokableMultipleArgs[subscriptionsWithCapacity.Capacity];
        }

        // A private method used to copy the subscriptions to the buffer.
        private void CopySubscriptionsToBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = subscriptionsAsIndexable[i].Value;
        }

        // A private method used to invoke the subscriptions in the buffer.
        private void InvokeSubscriptions(object[] value)
        {
            broadcastInProgress = true;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null)
                    currentSubscriptionsBuffer[i].Invoke(value);
            }

            broadcastInProgress = false;
        }

        // A private method used to empty the buffer.
        private void EmptyBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = null;
        }

        #endregion
    }
}