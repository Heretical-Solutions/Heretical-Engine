using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Broadcasting
{
    public class NonAllocBroadcasterGeneric<TValue>
        : IPublisherSingleArgGeneric<TValue>,
          IPublisherSingleArg,
          INonAllocSubscribableSingleArgGeneric<TValue>,
          INonAllocSubscribableSingleArg
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

        public NonAllocBroadcasterGeneric(
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

        #region INonAllocSubscribableSingleArgGeneric

        public void Subscribe(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription)
        {
            if (!subscription.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop();

            subscriptionElement.Value = ((ISubscriptionState<ISubscription>)subscription).Invokable;

            subscription.Activate(this, subscriptionElement);
        }

        public void Unsubscribe(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription)
        {
            if (!subscription.ValidateTermination(this))
                return;

            var poolElement = ((ISubscriptionState<IInvokableSingleArgGeneric<TValue>>)subscription).PoolElement;

            TryRemoveFromBuffer(poolElement);

            poolElement.Value = null;

            subscriptionsPool.Push(poolElement);

            subscription.Terminate();
        }

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>>
                INonAllocSubscribableSingleArgGeneric<TValue>.AllSubscriptions
        {
            get
            {
                var allSubscriptions = new ISubscriptionHandler<
                    INonAllocSubscribableSingleArgGeneric<TValue>,
                    IInvokableSingleArgGeneric<TValue>>
                    [subscriptionsAsIndexable.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = (ISubscriptionHandler<
                        INonAllocSubscribableSingleArgGeneric<TValue>,
                        IInvokableSingleArgGeneric<TValue>>)
                        subscriptionsAsIndexable[i].Value;

                return allSubscriptions;
            }
        }

        #endregion

        #region INonAllocSubscribableSingleArg

        public void Subscribe<TArgument>(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TArgument>,
                IInvokableSingleArgGeneric<TArgument>>
                subscription)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (subscription)
            {
                case ISubscriptionHandler<
                    INonAllocSubscribableSingleArgGeneric<TValue>,
                    IInvokableSingleArgGeneric<TValue>> tValueSubscription:

                    Subscribe(tValueSubscription);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormat<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
            }
        }

        public void Subscribe(
            Type valueType,
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (subscription)
            {
                case ISubscriptionHandler<
                    INonAllocSubscribableSingleArgGeneric<TValue>,
                    IInvokableSingleArgGeneric<TValue>>
                    tValueSubscription:

                    Subscribe(tValueSubscription);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormat<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
            }
        }

        /// <param name="subscription">The subscription handler to unsubscribe.</param>
        public void Unsubscribe<TArgument>(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TArgument>,
                IInvokableSingleArgGeneric<TArgument>>
                subscription)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (subscription)
            {
                case ISubscriptionHandler<
                    INonAllocSubscribableSingleArgGeneric<TValue>,
                    IInvokableSingleArgGeneric<TValue>>
                    tValueSubscription:

                    Unsubscribe(tValueSubscription);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormat<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
            }
        }

        public void Unsubscribe(
            Type valueType,
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (subscription)
            {
                case ISubscriptionHandler<
                    INonAllocSubscribableSingleArgGeneric<TValue>,
                    IInvokableSingleArgGeneric<TValue>>
                    tValueSubscription:

                    Unsubscribe(tValueSubscription);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormat<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
            }
        }

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>>
                INonAllocSubscribableSingleArg.GetAllSubscriptions<TValue>()
        {
            var allSubscriptions = new ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                [subscriptionsAsIndexable.Count];

            for (int i = 0; i < allSubscriptions.Length; i++)
                allSubscriptions[i] = (ISubscriptionHandler<
                    INonAllocSubscribableSingleArgGeneric<TValue>,
                    IInvokableSingleArgGeneric<TValue>>)
                    subscriptionsAsIndexable[i].Value;

            return allSubscriptions;
        }

        public IEnumerable<ISubscription> GetAllSubscriptions(Type valueType)
        {
            ISubscription[] allSubscriptions = new ISubscription[subscriptionsAsIndexable.Count];

            for (int i = 0; i < allSubscriptions.Length; i++)
                allSubscriptions[i] = subscriptionsAsIndexable[i].Value;

            return allSubscriptions;
        }

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>>
                INonAllocSubscribableSingleArg.AllSubscriptions
        {
            get
            {
                var allSubscriptions = new ISubscriptionHandler<
                    INonAllocSubscribableSingleArg,
                    IInvokableSingleArg>
                    [subscriptionsAsIndexable.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = (ISubscriptionHandler<
                        INonAllocSubscribableSingleArg,
                        IInvokableSingleArg>)
                        subscriptionsAsIndexable[i].Value;

                return allSubscriptions;
            }
        }

        #endregion

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
            {
                var subscription = (ISubscriptionHandler<
                    INonAllocSubscribableSingleArgGeneric<TValue>,
                    IInvokableSingleArgGeneric<TValue>>)
                    subscriptionsAsIndexable[0];

                Unsubscribe(subscription);
            }
        }

        #endregion

        #region IPublisherSingleArgGeneric

        public void Publish(TValue value)
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

        #region IPublisherSingleArg

        public void Publish<TArgument>(TArgument value)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (value)
            {
                case TValue tValue:

                    Publish(tValue);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormat<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
            }
        }

        public void Publish(Type valueType, object value)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (value)
            {
                case TValue tValue:

                    Publish(tValue);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormat<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
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

        private void InvokeSubscriptions(TValue value)
        {
            broadcastInProgress = true;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null)
                {
                    var subscriptionState = (ISubscriptionState<IInvokableSingleArgGeneric<TValue>>)currentSubscriptionsBuffer[i];

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