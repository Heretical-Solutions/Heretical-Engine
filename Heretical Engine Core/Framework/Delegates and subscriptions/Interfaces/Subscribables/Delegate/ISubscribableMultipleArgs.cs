namespace HereticalSolutions.Delegates
{
    public interface ISubscribableMultipleArgs
        : ISubscribable
    {
        void Subscribe(Action<object[]> @delegate);
        
        void Unsubscribe(Action<object[]> @delegate);

        IEnumerable<Action<object[]>> AllSubscriptions { get; }
    }
}