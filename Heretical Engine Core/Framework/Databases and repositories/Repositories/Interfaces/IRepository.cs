namespace HereticalSolutions.Repositories
{
    public interface IRepository<TKey, TValue>
        : IReadOnlyRepository<TKey, TValue>
    {
        void Add(
            TKey key,
            TValue value);

        bool TryAdd(
            TKey key,
            TValue value);


        void Update(
            TKey key,
            TValue value);

        bool TryUpdate(
            TKey key,
            TValue value);


        void AddOrUpdate(
            TKey key,
            TValue value);       


        void Remove(TKey key);

        bool TryRemove(TKey key);


        void Clear();
    }
}