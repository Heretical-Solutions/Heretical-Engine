namespace HereticalSolutions.Delegates
{
    public interface ISubscribableSingleArgGeneric<TValue>
        : ISubscribable
    {
        void Subscribe(Action<TValue> @delegate);
        
        void Unsubscribe(Action<TValue> @delegate);

        IEnumerable<Action<TValue>> AllSubscriptions { get; }
    }
}