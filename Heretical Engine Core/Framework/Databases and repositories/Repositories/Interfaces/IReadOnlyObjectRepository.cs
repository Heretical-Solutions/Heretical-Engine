namespace HereticalSolutions.Repositories
{
    public interface IReadOnlyObjectRepository
    {
        bool Has<TValue>();

        bool Has(Type valueType);


        TValue Get<TValue>();

        object Get(Type valueType);


        bool TryGet<TValue>(out TValue value);

        bool TryGet(
            Type valueType,
            out object value);


        int Count { get; }

        IEnumerable<Type> Keys { get; }

        IEnumerable<object> Values { get; }
    }
}