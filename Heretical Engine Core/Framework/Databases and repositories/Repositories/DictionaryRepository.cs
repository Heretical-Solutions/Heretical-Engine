using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Repositories
{
    public class DictionaryRepository<TKey, TValue> :
        IRepository<TKey, TValue>,
        IClonableRepository<TKey, TValue>,
        ICleanUppable,
        IDisposable
    {
        private readonly Dictionary<TKey, TValue> database;

        public DictionaryRepository(
            Dictionary<TKey, TValue> database)
        {
            this.database = database;
        }

        #region IRepository

        #region IReadOnlyRepository

        public bool Has(TKey key)
        {
            return database.ContainsKey(key);
        }

        public TValue Get(TKey key)
        {
            return database[key];
        }

        public bool TryGet(
            TKey key,
            out TValue value)
        {
            return database.TryGetValue(
                key,
                out value);
        }

        public int Count { get { return database.Count; } }

        public IEnumerable<TKey> Keys { get { return database.Keys; } }

        public IEnumerable<TValue> Values { get { return database.Values; } }

        #endregion

        public void Add(
            TKey key,
            TValue value)
        {
            database.Add(
                key,
                value);
        }

        public bool TryAdd(
            TKey key,
            TValue value)
        {
            return database.TryAdd(
                key,
                value);
        }

        public void Update(
            TKey key,
            TValue value)
        {
            database[key] = value;
        }

        public bool TryUpdate(
            TKey key,
            TValue value)
        {
            if (!Has(key))
                return false;

            Update(
                key,
                value);

            return true;
        }

        public void AddOrUpdate(
            TKey key,
            TValue value)
        {
            if (Has(key))
                Update(
                    key,
                    value);
            else
                Add(
                    key,
                    value);
        }

        public void Remove(TKey key)
        {
            database.Remove(key);
        }

        public bool TryRemove(TKey key)
        {
            if (!Has(key))
                return false;

            Remove(key);

            return true;
        }


        public void Clear()
        {
            database.Clear();
        }

        #endregion

        #region IClonableRepository

        public IRepository<TKey, TValue> Clone()
        {
            return RepositoriesFactory.CloneDictionaryRepository(database);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (typeof(ICleanUppable).IsAssignableFrom(typeof(TValue)))
            {
                foreach (var value in database.Values)
                    (value as ICleanUppable).Cleanup();
            }

            if (typeof(IDisposable).IsAssignableFrom(typeof(TValue)))
            {
                foreach (var value in database.Values)
                    (value as IDisposable).Dispose();
            }

            Clear();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Cleanup();

            if (database is IDisposable)
                (database as IDisposable).Dispose();
        }

        #endregion
    }
}