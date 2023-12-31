using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates.Pinging
{
    public class NonAllocPinger
        : IPublisherNoArgs,
          INonAllocSubscribableNoArgs
    {
        #region Subscriptions

        private readonly INonAllocDecoratedPool<ISubscription> subscriptionsPool;

        private readonly IIndexable<IPoolElement<ISubscription>> subscriptionsAsIndexable;

        private readonly IFixedSizeCollection<IPoolElement<ISubscription>> subscriptionsWithCapacity;

        #endregion

        #region Buffer

        private ISubscription[] currentSubscriptionsBuffer;

        private int currentSubscriptionsBufferCount = -1;

        #endregion

        private bool pingInProgress = false;

        public NonAllocPinger(
            INonAllocDecoratedPool<ISubscription> subscriptionsPool,
            INonAllocPool<ISubscription> subscriptionsContents)
        {
            this.subscriptionsPool = subscriptionsPool;

            subscriptionsAsIndexable = (IIndexable<IPoolElement<ISubscription>>)subscriptionsContents;

            subscriptionsWithCapacity =
                (IFixedSizeCollection<IPoolElement<ISubscription>>)subscriptionsContents;

            currentSubscriptionsBuffer = new ISubscription[subscriptionsWithCapacity.Capacity];
        }

        #region INonAllocSubscribableNoArgs

        public void Subscribe(ISubscription subscription)
        {
            var subscriptionHandler = (ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>)subscription;

            if (!subscriptionHandler.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop();

            subscriptionElement.Value = subscription;

            subscriptionHandler.Activate(this, subscriptionElement);
        }

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

        public void Unsubscribe(IPoolElement<ISubscription> subscription)
        {
            TryRemoveFromBuffer(subscription);

            subscription.Value = null;

            subscriptionsPool.Push(subscription);
        }

        #region INonAllocSubscribable

        public IEnumerable<ISubscription> AllSubscriptions
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
                Unsubscribe(subscriptionsAsIndexable[0]);
        }

        #endregion

        #endregion

        private void TryRemoveFromBuffer(IPoolElement<ISubscription> subscriptionElement)
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

        #region IPublisherNoArgs

        public void Publish()
        {
            //If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
            //should NOT be changed during the invokation
            //That's why we'll copy the subscriptions array to buffer and invoke it from there

            ValidateBufferSize();

            currentSubscriptionsBufferCount = subscriptionsAsIndexable.Count;

            CopySubscriptionsToBuffer();

            InvokeSubscriptions();

            EmptyBuffer();
        }

        #endregion

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

        private void InvokeSubscriptions()
        {
            pingInProgress = true;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null)
                {
                    var subscriptionState = (ISubscriptionState<IInvokableNoArgs>)currentSubscriptionsBuffer[i];

                    subscriptionState.Invokable.Invoke();
                }
            }

            pingInProgress = false;
        }

        private void EmptyBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = null;
        }
    }
}