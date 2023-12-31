namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscribableMultipleArgs
        : INonAllocSubscribable
    {
        void Subscribe(
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>
                subscription);

        void Unsubscribe(
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>
                subscription);

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>>
                AllSubscriptions { get; }
    }
}