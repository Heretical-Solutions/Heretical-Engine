using HereticalSolutions.Delegates;

namespace HereticalSolutions.Synchronization
{
    public interface ISynchronizationProvider
    {
        void Subscribe(ISubscription subscription);

        void Unsubscribe(ISubscription subscription);

        void UnsubscribeAll();
    }
}