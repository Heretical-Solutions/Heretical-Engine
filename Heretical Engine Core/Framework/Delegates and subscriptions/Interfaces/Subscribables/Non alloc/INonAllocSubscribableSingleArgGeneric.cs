namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscribableSingleArgGeneric<TValue>
        : INonAllocSubscribable
    {
        void Subscribe(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription);

        void Unsubscribe(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription);

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>>
                AllSubscriptions { get; }
    }
}