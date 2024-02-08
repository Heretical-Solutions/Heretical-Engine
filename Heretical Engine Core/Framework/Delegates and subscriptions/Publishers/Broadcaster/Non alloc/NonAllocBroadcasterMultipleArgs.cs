using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Broadcasting
{
    public class NonAllocBroadcasterMultipleArgs
        : IPublisherMultipleArgs,
          INonAllocSubscribableMultipleArgs,
          ICleanUppable,
          IDisposable
    {
        #region Subscriptions

        private readonly INonAllocDecoratedPool<ISubscription> subscriptionsPool;

        private readonly IIndexable<IPoolElement<ISubscription>> subscriptionsAsIndexable;

        private readonly IFixedSizeCollection<IPoolElement<ISubscription>> subscriptionsWithCapacity;

        #endregion

        private readonly ILogger logger;

        #region Buffer

        private ISubscription[] currentSubscriptionsBuffer;

        private int currentSubscriptionsBufferCount = -1;

        #endregion

        private bool broadcastInProgress = false;

        public NonAllocBroadcasterMultipleArgs(
            INonAllocDecoratedPool<ISubscription> subscriptionsPool,
            INonAllocPool<ISubscription> subscriptionsContents,
            ILogger logger = null)
        {
            this.subscriptionsPool = subscriptionsPool;

            this.logger = logger;

            subscriptionsAsIndexable = (IIndexable<IPoolElement<ISubscription>>)subscriptionsContents;

            subscriptionsWithCapacity = (IFixedSizeCollection<IPoolElement<ISubscription>>)subscriptionsContents;

            currentSubscriptionsBuffer = new ISubscription[subscriptionsWithCapacity.Capacity];
        }

        #region INonAllocSubscribableMultipleArgs

        public void Subscribe(
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>
                subscription)
        {
            if (!subscription.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop();

            var subscriptionState = (ISubscriptionState<IInvokableMultipleArgs>)subscription;

            subscriptionElement.Value = (ISubscription)subscriptionState;

            subscription.Activate(this, subscriptionElement);

            logger?.Log<NonAllocBroadcasterMultipleArgs>(
                $"SUBSCRIPTION ADDED: {subscriptionElement.Value.GetHashCode()}");
        }

        public void Unsubscribe(
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>
                subscription)
        {
            if (!subscription.ValidateTermination(this))
                return;

            var poolElement = ((ISubscriptionState<IInvokableMultipleArgs>)subscription).PoolElement;

            TryRemoveFromBuffer(poolElement);

            var previousValue = poolElement.Value;

            poolElement.Value = null;

            subscriptionsPool.Push(poolElement);

            subscription.Terminate();

            logger?.Log<NonAllocBroadcasterMultipleArgs>(
                $"SUBSCRIPTION REMOVED: {previousValue.GetHashCode()}");
        }

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>>
                INonAllocSubscribableMultipleArgs.AllSubscriptions
        {
            get
            {
                var allSubscriptions = new ISubscriptionHandler<
                    INonAllocSubscribableMultipleArgs,
                    IInvokableMultipleArgs>
                    [subscriptionsAsIndexable.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = (ISubscriptionHandler<
                        INonAllocSubscribableMultipleArgs,
                        IInvokableMultipleArgs>)
                        subscriptionsAsIndexable[i].Value;

                return allSubscriptions;
            }
        }

        #region INonAllocSubscribable

        IEnumerable<ISubscription> INonAllocSubscribable.AllSubscriptions
        {
            get
            {
                ISubscription[] allSubscriptions = new ISubscription[subscriptionsAsIndexable.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = subscriptionsAsIndexable[i].Value;

                return allSubscriptions;
            }
        }

        public void UnsubscribeAll()
        {
            while (subscriptionsAsIndexable.Count > 0)
            {
                var subscription = (ISubscriptionHandler<
                    INonAllocSubscribableMultipleArgs,
                    IInvokableMultipleArgs>)
                    subscriptionsAsIndexable[0].Value;

                Unsubscribe(subscription);
            }
        }

        #endregion

        #endregion

        #region IPublisherMultipleArgs

        public void Publish(object[] value)
        {
            //If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
            //should NOT be changed during the invokation
            //That's why we'll copy the subscriptions array to buffer and invoke it from there

            ValidateBufferSize();

            currentSubscriptionsBufferCount = subscriptionsAsIndexable.Count;

            CopySubscriptionsToBuffer();

            InvokeSubscriptions(value);

            EmptyBuffer();
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (subscriptionsPool is ICleanUppable)
                (subscriptionsPool as ICleanUppable).Cleanup();

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null
                    && currentSubscriptionsBuffer[i] is ICleanUppable)
                {
                    (currentSubscriptionsBuffer[i] as ICleanUppable).Cleanup();
                }
            }

            EmptyBuffer();

            currentSubscriptionsBufferCount = -1;

            broadcastInProgress = false;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (subscriptionsPool is IDisposable)
                (subscriptionsPool as IDisposable).Dispose();

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null
                    && currentSubscriptionsBuffer[i] is IDisposable)
                {
                    (currentSubscriptionsBuffer[i] as IDisposable).Dispose();
                }
            }
        }

        #endregion

        private void TryRemoveFromBuffer(IPoolElement<ISubscription> subscriptionElement)
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

        private void ValidateBufferSize()
        {
            if (currentSubscriptionsBuffer.Length < subscriptionsWithCapacity.Capacity)
                currentSubscriptionsBuffer = new ISubscription[subscriptionsWithCapacity.Capacity];
        }

        private void CopySubscriptionsToBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = subscriptionsAsIndexable[i].Value;
        }

        private void InvokeSubscriptions(object[] value)
        {
            broadcastInProgress = true;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null)
                {
                    var subscriptionState = (ISubscriptionState<IInvokableMultipleArgs>)currentSubscriptionsBuffer[i];

                    subscriptionState.Invokable.Invoke(value);
                }
            }

            broadcastInProgress = false;
        }

        private void EmptyBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = null;
        }
    }
}