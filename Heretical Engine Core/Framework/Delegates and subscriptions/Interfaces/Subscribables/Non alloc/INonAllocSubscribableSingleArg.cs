namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscribableSingleArg
        : INonAllocSubscribable
    {
        void Subscribe<TValue>(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription);
        
        void Subscribe(
            Type valueType,
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription);

        void Unsubscribe<TValue>(
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>
                subscription);
        
        void Unsubscribe(
            Type valueType,
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription);

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArgGeneric<TValue>,
                IInvokableSingleArgGeneric<TValue>>>
                GetAllSubscriptions<TValue>();

        IEnumerable<ISubscription> GetAllSubscriptions(Type valueType);

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>>
                AllSubscriptions { get; }
    }
}