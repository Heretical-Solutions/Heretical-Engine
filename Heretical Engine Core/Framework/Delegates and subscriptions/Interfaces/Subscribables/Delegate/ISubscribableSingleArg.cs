namespace HereticalSolutions.Delegates
{
    public interface ISubscribableSingleArg
        : ISubscribable
    {
        void Subscribe<TValue>(Action<TValue> @delegate);

        void Subscribe(Type valueType, object @delegate);

        void Unsubscribe<TValue>(Action<TValue> @delegate);
        
        void Unsubscribe(Type valueType, object @delegate);

        IEnumerable<Action<TValue>> GetAllSubscriptions<TValue>();

        IEnumerable<object> GetAllSubscriptions(Type valueType);
    }
}