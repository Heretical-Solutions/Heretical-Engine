using System;
using System.Collections.Generic;

namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents an interface for an object repository.
    /// </summary>
    public interface IObjectRepository
    {
        /// <summary>
        /// Checks if the repository has a value of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns><c>true</c> if the repository has a value of type <typeparamref name="TValue"/>; otherwise, <c>false</c>.</returns>
        bool Has<TValue>();

        /// <summary>
        /// Checks if the repository has a value of the specified <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <returns><c>true</c> if the repository has a value of the specified <paramref name="valueType"/>; otherwise, <c>false</c>.</returns>
        bool Has(Type valueType);

        /// <summary>
        /// Adds a value of type <typeparamref name="TValue"/> to the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to add.</param>
        void Add<TValue>(TValue value);

        /// <summary>
        /// Adds a value of the specified <paramref name="valueType"/> to the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to add.</param>
        void Add(Type valueType, object value);

        /// <summary>
        /// Tries to add a value of type <typeparamref name="TValue"/> to the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to add.</param>
        /// <returns><c>true</c> if the value was successfully added; otherwise, <c>false</c>.</returns>
        bool TryAdd<TValue>(TValue value);

        /// <summary>
        /// Tries to add a value of the specified <paramref name="valueType"/> to the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to add.</param>
        /// <returns><c>true</c> if the value was successfully added; otherwise, <c>false</c>.</returns>
        bool TryAdd(Type valueType, object value);

        /// <summary>
        /// Updates a value of type <typeparamref name="TValue"/> in the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to update.</param>
        void Update<TValue>(TValue value);

        /// <summary>
        /// Updates a value of the specified <paramref name="valueType"/> in the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to update.</param>
        void Update(Type valueType, object value);

        /// <summary>
        /// Tries to update a value of type <typeparamref name="TValue"/> in the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to update.</param>
        /// <returns><c>true</c> if the value was successfully updated; otherwise, <c>false</c>.</returns>
        bool TryUpdate<TValue>(TValue value);

        /// <summary>
        /// Tries to update a value of the specified <paramref name="valueType"/> in the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to update.</param>
        /// <returns><c>true</c> if the value was successfully updated; otherwise, <c>false</c>.</returns>
        bool TryUpdate(Type valueType, object value);

        /// <summary>
        /// Adds or updates a value of type <typeparamref name="TValue"/> in the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to add or update.</param>
        void AddOrUpdate<TValue>(TValue value);

        /// <summary>
        /// Adds or updates a value of the specified <paramref name="valueType"/> in the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">The value to add or update.</param>
        void AddOrUpdate(Type valueType, object value);

        /// <summary>
        /// Gets the value of type <typeparamref name="TValue"/> from the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>The value of type <typeparamref name="TValue"/> if found; otherwise, the default value.</returns>
        TValue Get<TValue>();

        /// <summary>
        /// Gets the value of the specified <paramref name="valueType"/> from the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <returns>The value of the specified <paramref name="valueType"/> if found; otherwise, <c>null</c>.</returns>
        object Get(Type valueType);

        /// <summary>
        /// Tries to get the value of type <typeparamref name="TValue"/> from the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">When this method returns, contains the value of type <typeparamref name="TValue"/> if found; otherwise, the default value.</param>
        /// <returns><c>true</c> if the value was found; otherwise, <c>false</c>.</returns>
        bool TryGet<TValue>(out TValue value);

        /// <summary>
        /// Tries to get the value of the specified <paramref name="valueType"/> from the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="value">When this method returns, contains the value of the specified <paramref name="valueType"/> if found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the value was found; otherwise, <c>false</c>.</returns>
        bool TryGet(Type valueType, out object value);

        /// <summary>
        /// Removes the value of type <typeparamref name="TValue"/> from the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        void Remove<TValue>();

        /// <summary>
        /// Removes the value of the specified <paramref name="valueType"/> from the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        void Remove(Type valueType);

        /// <summary>
        /// Tries to remove the value of type <typeparamref name="TValue"/> from the repository.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns><c>true</c> if the value was removed; otherwise, <c>false</c>.</returns>
        bool TryRemove<TValue>();

        /// <summary>
        /// Tries to remove the value of the specified <paramref name="valueType"/> from the repository.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <returns><c>true</c> if the value was removed; otherwise, <c>false</c>.</returns>
        bool TryRemove(Type valueType);

        /// <summary>
        /// Gets the collection of keys in the repository.
        /// </summary>
        IEnumerable<Type> Keys { get; }
    }
}