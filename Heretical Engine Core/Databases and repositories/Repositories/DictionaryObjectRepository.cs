using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents a dictionary-based object repository.
    /// </summary>
    public class DictionaryObjectRepository : IObjectRepository, IReadOnlyObjectRepository, ICloneableObjectRepository
    {
        private readonly IRepository<Type, object> database;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryObjectRepository"/> class with the specified <paramref name="database"/>.
        /// </summary>
        /// <param name="database">The repository database.</param>
        public DictionaryObjectRepository(IRepository<Type, object> database)
        {
            this.database = database;
        }

        #region IObjectRepository

        #region IReadOnlyObjectRepository

        /// <summary>
        /// Checks if the repository contains a value of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns><c>true</c> if the repository contains a value of type <typeparamref name="TValue"/>; otherwise, <c>false</c>.</returns>
        public bool Has<TValue>()
        {
            return database.Has(typeof(TValue));
        }

        /// <summary>
        /// Checks if the repository contains a value of the specified <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <returns><c>true</c> if the repository contains a value of the specified <paramref name="valueType"/>; otherwise, <c>false</c>.</returns>
        public bool Has(Type valueType)
        {
            return database.Has(valueType);
        }

        /// <summary>
        /// Gets the value of type <typeparamref name="TValue"/> from the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>The value of type <typeparamref name="TValue"/> from the repository.</returns>
        public TValue Get<TValue>()
        {
            return (TValue)database.Get(typeof(TValue));
        }

        /// <summary>
        /// Gets the value of the specified <paramref name="valueType"/> from the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <returns>The value of the specified <paramref name="valueType"/> from the repository.</returns>
        public object Get(Type valueType)
        {
            return database.Get(valueType);
        }

        /// <summary>
        /// Tries to get the value of type <typeparamref name="TValue"/> from the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">When this method returns, contains the value of type <typeparamref name="TValue"/> from the repository, if the value exists; otherwise, the default value for the type.</param>
        /// <returns><c>true</c> if the repository contains a value of type <typeparamref name="TValue"/>; otherwise, <c>false</c>.</returns>
        public bool TryGet<TValue>(out TValue value)
        {
            value = default(TValue);
            
            bool result = database.TryGet(typeof(TValue), out object valueObject);

            if (result)
                value = (TValue)valueObject;

            return result;
        }

        /// <summary>
        /// Tries to get the value of the specified <paramref name="valueType"/> from the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">When this method returns, contains the value of the specified <paramref name="valueType"/> from the repository, if the value exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the repository contains a value of the specified <paramref name="valueType"/>; otherwise, <c>false</c>.</returns>
        public bool TryGet(Type valueType, out object value)
        {
            return database.TryGet(valueType, out value);
        }

        /// <summary>
        /// Gets the keys of the repository.
        /// </summary>
        public IEnumerable<Type> Keys => database.Keys;

        #endregion

        /// <summary>
        /// Adds a value of type <typeparamref name="TValue"/> to the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to add.</param>
        public void Add<TValue>(TValue value)
        {
            database.Add(typeof(TValue), value);
        }

        /// <summary>
        /// Adds a value of the specified <paramref name="valueType"/> to the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to add.</param>
        public void Add(Type valueType, object value)
        {
            database.AddOrUpdate(valueType, value);
        }

        /// <summary>
        /// Tries to add a value of type <typeparamref name="TValue"/> to the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to add.</param>
        /// <returns><c>true</c> if the value was added successfully; otherwise, <c>false</c>.</returns>
        public bool TryAdd<TValue>(TValue value)
        {
            if (Has<TValue>())
                return false;
            
            Add<TValue>(value);

            return true;
        }

        /// <summary>
        /// Tries to add a value of the specified <paramref name="valueType"/> to the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to add.</param>
        /// <returns><c>true</c> if the value was added successfully; otherwise, <c>false</c>.</returns>
        public bool TryAdd(Type valueType, object value)
        {
            if (Has(valueType))
                return false;
            
            Add(valueType, value);

            return true;
        }

        /// <summary>
        /// Updates the value of type <typeparamref name="TValue"/> in the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to update.</param>
        public void Update<TValue>(TValue value)
        {
            database.Update(typeof(TValue), value);
        }

        /// <summary>
        /// Updates the value of the specified <paramref name="valueType"/> in the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to update.</param>
        public void Update(Type valueType, object value)
        {
            database.Update(valueType, value);
        }

        /// <summary>
        /// Tries to update the value of type <typeparamref name="TValue"/> in the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to update.</param>
        /// <returns><c>true</c> if the value was updated successfully; otherwise, <c>false</c>.</returns>
        public bool TryUpdate<TValue>(TValue value)
        {
            if (!Has<TValue>())
                return false;
            
            Update<TValue>(value);

            return true;
        }

        /// <summary>
        /// Tries to update the value of the specified <paramref name="valueType"/> in the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to update.</param>
        /// <returns><c>true</c> if the value was updated successfully; otherwise, <c>false</c>.</returns>
        public bool TryUpdate(Type valueType, object value)
        {
            if (!Has(valueType))
                return false;
            
            Update(valueType, value);

            return true;
        }

        /// <summary>
        /// Adds or updates the value of type <typeparamref name="TValue"/> in the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to add or update.</param>
        public void AddOrUpdate<TValue>(TValue value)
        {
            database.AddOrUpdate(typeof(TValue), value);
        }

        /// <summary>
        /// Adds or updates the value of the specified <paramref name="valueType"/> in the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to add or update.</param>
        public void AddOrUpdate(Type valueType, object value)
        {
            database.AddOrUpdate(valueType, value);
        }

        /// <summary>
        /// Removes the value of type <typeparamref name="TValue"/> from the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        public void Remove<TValue>()
        {
            database.Remove(typeof(TValue));
        }

        /// <summary>
        /// Removes the value of the specified <paramref name="valueType"/> from the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        public void Remove(Type valueType)
        {
            database.Remove(valueType);
        }

        /// <summary>
        /// Tries to remove the value of type <typeparamref name="TValue"/> from the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns><c>true</c> if the value was removed successfully; otherwise, <c>false</c>.</returns>
        public bool TryRemove<TValue>()
        {
            if (!Has<TValue>())
                return false;
            
            Remove<TValue>();

            return true;
        }

        /// <summary>
        /// Tries to remove the value of the specified <paramref name="valueType"/> from the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <returns><c>true</c> if the value was removed successfully; otherwise, <c>false</c>.</returns>
        public bool TryRemove(Type valueType)
        {
            if (!Has(valueType))
                return false;
            
            Remove(valueType);

            return true;
        }

        #endregion

        #region ICloneableObjectRepository

        /// <summary>
        /// Creates a shallow copy of the repository.
        /// </summary>
        /// <returns>A shallow copy of the repository.</returns>
        public IObjectRepository Clone()
        {
            return RepositoriesFactory.CloneDictionaryObjectRepository(database);
        }

        #endregion
    }
}