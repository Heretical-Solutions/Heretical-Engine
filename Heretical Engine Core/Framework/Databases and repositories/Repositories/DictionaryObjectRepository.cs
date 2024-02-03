using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Repositories
{
    public class DictionaryObjectRepository
        : IObjectRepository,
          ICloneableObjectRepository,
          ICleanUppable,
          IDisposable
    {
        private readonly IRepository<Type, object> database;

        public DictionaryObjectRepository(
            IRepository<Type, object> database)
        {
            this.database = database;
        }

        #region IObjectRepository

        #region IReadOnlyObjectRepository

        public bool Has<TValue>()
        {
            return database.Has(typeof(TValue));
        }

        public bool Has(Type valueType)
        {
            return database.Has(valueType);
        }


        public TValue Get<TValue>()
        {
            return (TValue)database.Get(typeof(TValue));
        }

        public object Get(Type valueType)
        {
            return database.Get(valueType);
        }


        public bool TryGet<TValue>(out TValue value)
        {
            value = default(TValue);
            
            bool result = database.TryGet(
                typeof(TValue),
                out object valueObject);

            if (result)
                value = (TValue)valueObject;

            return result;
        }

        public bool TryGet(
            Type valueType,
            out object value)
        {
            return database.TryGet(
                valueType,
                out value);
        }


        public int Count { get { return database.Count; } }

        public IEnumerable<Type> Keys => database.Keys;

        public IEnumerable<object> Values => database.Values;

        #endregion

        public void Add<TValue>(TValue value)
        {
            database.Add(
                typeof(TValue),
                value);
        }

        public void Add(
            Type valueType,
            object value)
        {
            database.AddOrUpdate(
                valueType,
                value);
        }


        public bool TryAdd<TValue>(TValue value)
        {
            if (Has<TValue>())
                return false;
            
            Add<TValue>(value);

            return true;
        }

        public bool TryAdd(
            Type valueType,
            object value)
        {
            if (Has(valueType))
                return false;
            
            Add(
                valueType,
                value);

            return true;
        }


        public void Update<TValue>(TValue value)
        {
            database.Update(
                typeof(TValue),
                value);
        }

        public void Update(
            Type valueType,
            object value)
        {
            database.Update(
                valueType,
                value);
        }


        public bool TryUpdate<TValue>(TValue value)
        {
            if (!Has<TValue>())
                return false;
            
            Update<TValue>(value);

            return true;
        }

        public bool TryUpdate(
            Type valueType,
            object value)
        {
            if (!Has(valueType))
                return false;
            
            Update(
                valueType,
                value);

            return true;
        }


        public void AddOrUpdate<TValue>(TValue value)
        {
            database.AddOrUpdate(typeof(TValue), value);
        }

        public void AddOrUpdate(
            Type valueType,
            object value)
        {
            database.AddOrUpdate(
                valueType,
                value);
        }


        public void Remove<TValue>()
        {
            database.Remove(typeof(TValue));
        }

        public void Remove(Type valueType)
        {
            database.Remove(valueType);
        }


        public bool TryRemove<TValue>()
        {
            if (!Has<TValue>())
                return false;
            
            Remove<TValue>();

            return true;
        }

        public bool TryRemove(Type valueType)
        {
            if (!Has(valueType))
                return false;
            
            Remove(valueType);

            return true;
        }


        public void Clear()
        {
            database.Clear();
        }

        #endregion

        #region ICloneableObjectRepository

        public IObjectRepository Clone()
        {
            return RepositoriesFactory.CloneDictionaryObjectRepository(database);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            foreach (var value in database.Values)
            {
                if (value is ICleanUppable)
                    (value as ICleanUppable).Cleanup();
            }

            foreach (var value in database.Values)
            {
                if (value is IDisposable)
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