namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscribableNoArgs
        : INonAllocSubscribable
    {
        void Subscribe(ISubscription subscription);

        void Unsubscribe(ISubscription subscription);
    }
}