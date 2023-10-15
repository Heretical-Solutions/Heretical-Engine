using System.Collections.Generic;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Generic repository implementation using a dictionary as the underlying data store.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the repository.</typeparam>
    /// <typeparam name="TValue">The type of the values in the repository.</typeparam>
    public class DictionaryRepository<TKey, TValue> :
        IRepository<TKey, TValue>,
        IReadOnlyRepository<TKey, TValue>,
        IClonableRepository<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> database;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryRepository{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="database">The dictionary to use as the underlying data store.</param>
        public DictionaryRepository(Dictionary<TKey, TValue> database)
        {
            this.database = database;
        }

        #region IRepository

        #region IReadOnlyRepository

        /// <summary>
        /// Determines whether the repository contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the repository.</param>
        /// <returns><c>true</c> if the repository contains an element with the specified key; otherwise, <c>false</c>.</returns>
        public bool Has(TKey key)
        {
            return database.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public TValue Get(TKey key)
        {
            return database[key];
        }

        /// <summary>
        /// Tries to get the value associated with the specified key from the repository.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the value type.</param>
        /// <returns><c>true</c> if the repository contains an element with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGet(TKey key, out TValue value)
        {
            return database.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the repository.
        /// </summary>
        public IEnumerable<TKey> Keys { get { return database.Keys; } }

        #endregion

        /// <summary>
        /// Adds a key-value pair to the repository.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(TKey key, TValue value)
        {
            database.Add(key, value);
        }

        /// <summary>
        /// Tries to add the specified key-value pair to the repository.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns><c>true</c> if the key-value pair was added successfully; otherwise, <c>false</c>.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (Has(key))
                return false;

            Add(key, value);

            return true;
        }

        /// <summary>
        /// Updates the value associated with the specified key in the repository.
        /// </summary>
        /// <param name="key">The key of the element to update.</param>
        /// <param name="value">The new value of the element.</param>
        public void Update(TKey key, TValue value)
        {
            database[key] = value;
        }

        /// <summary>
        /// Tries to update the value associated with the specified key in the repository.
        /// </summary>
        /// <param name="key">The key of the element to update.</param>
        /// <param name="value">The new value of the element.</param>
        /// <returns><c>true</c> if the key-value pair was updated successfully; otherwise, <c>false</c>.</returns>
        public bool TryUpdate(TKey key, TValue value)
        {
            if (!Has(key))
                return false;

            Update(key, value);

            return true;
        }

        /// <summary>
        /// Adds a key-value pair to the repository if the key does not already exist, or updates the value associated with the key if it does exist.
        /// </summary>
        /// <param name="key">The key of the element to add or update.</param>
        /// <param name="value">The value of the element to add or update.</param>
        public void AddOrUpdate(TKey key, TValue value)
        {
            if (Has(key))
                Update(key, value);
            else
                Add(key, value);
        }

        /// <summary>
        /// Removes the element with the specified key from the repository.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public void Remove(TKey key)
        {
            database.Remove(key);
        }

        /// <summary>
        /// Tries to remove the element with the specified key from the repository.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element was removed successfully; otherwise, <c>false</c>.</returns>
        public bool TryRemove(TKey key)
        {
            if (!Has(key))
                return false;

            Remove(key);

            return true;
        }

        #endregion

        #region IClonableRepository

        /// <summary>
        /// Creates a new instance of the repository that is a clone of the current instance.
        /// </summary>
        /// <returns>A new instance of the repository that is a clone of the current instance.</returns>
        public IRepository<TKey, TValue> Clone()
        {
            return RepositoriesFactory.CloneDictionaryRepository(database);
        }

        #endregion
    }
}