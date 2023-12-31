namespace HereticalSolutions.Delegates
{
    public interface ISubscribableNoArgs
        : ISubscribable
    {
        void Subscribe(Action @delegate);
        
        void Unsubscribe(Action @delegate);

        IEnumerable<Action> AllSubscriptions { get; }
    }
}