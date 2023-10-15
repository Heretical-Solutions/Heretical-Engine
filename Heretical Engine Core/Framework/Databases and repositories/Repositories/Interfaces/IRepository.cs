using System.Collections.Generic;

namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents an interface for a repository.
    /// </summary>
    /// <typeparam name="TKey">The type of the repository key.</typeparam>
    /// <typeparam name="TValue">The type of the repository value.</typeparam>
    public interface IRepository<TKey, TValue>
    {
        /// <summary>
        /// Checks if the repository has a value associated with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><c>true</c> if the repository has a value associated with the specified <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        bool Has(TKey key);

        /// <summary>
        /// Adds a value associated with the specified <paramref name="key"/> to the repository.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Tries to add a value associated with the specified <paramref name="key"/> to the repository.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns><c>true</c> if the value was added; otherwise, <c>false</c>.</returns>
        bool TryAdd(TKey key, TValue value);

        /// <summary>
        /// Updates the value associated with the specified <paramref name="key"/> in the repository.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="value">The value to update.</param>
        void Update(TKey key, TValue value);

        /// <summary>
        /// Tries to update the value associated with the specified <paramref name="key"/> in the repository.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="value">The value to update.</param>
        /// <returns><c>true</c> if the value was updated; otherwise, <c>false</c>.</returns>
        bool TryUpdate(TKey key, TValue value);

        /// <summary>
        /// Adds or updates a value associated with the specified <paramref name="key"/> in the repository.
        /// </summary>
        /// <param name="key">The key to add or update.</param>
        /// <param name="value">The value to add or update.</param>
        void AddOrUpdate(TKey key, TValue value);       

        /// <summary>
        /// Gets the value associated with the specified <paramref name="key"/> from the repository.
        /// </summary>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <returns>The value associated with the specified <paramref name="key"/> if found; otherwise, the default value for the value type.</returns>
        TValue Get(TKey key);

        /// <summary>
        /// Tries to get the value associated with the specified <paramref name="key"/> from the repository.
        /// </summary>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified <paramref name="key"/> if found; otherwise, the default value for the value type.</param>
        /// <returns><c>true</c> if the value was found; otherwise, <c>false</c>.</returns>
        bool TryGet(TKey key, out TValue value);

        /// <summary>
        /// Removes the value associated with the specified <paramref name="key"/> from the repository.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        void Remove(TKey key);

        /// <summary>
        /// Tries to remove the value associated with the specified <paramref name="key"/> from the repository.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns><c>true</c> if the value was removed; otherwise, <c>false</c>.</returns>
        bool TryRemove(TKey key);

        /// <summary>
        /// Gets the collection of keys in the repository.
        /// </summary>
        IEnumerable<TKey> Keys { get; }
    }
}