namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents a generic repository interface that provides basic CRUD operations for a collection of key-value pairs
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the repository.</typeparam>
    /// <typeparam name="TValue">The type of the values in the repository.</typeparam>
    public interface IRepository<TKey, TValue>
        : IReadOnlyRepository<TKey, TValue>
    {
        /// <summary>
        /// Adds a new key-value pair to the repository
        /// </summary>
        /// <param name="key">The key of the pair to add.</param>
        /// <param name="value">The value of the pair to add.</param>
        void Add(
            TKey key,
            TValue value);

        /// <summary>
        /// Tries to add a new key-value pair to the repository
        /// </summary>
        /// <param name="key">The key of the pair to add.</param>
        /// <param name="value">The value of the pair to add.</param>
        /// <returns><c>true</c> if the pair was added successfully; otherwise, <c>false</c>.</returns>
        bool TryAdd(
            TKey key,
            TValue value);

        /// <summary>
        /// Updates the value of an existing key-value pair in the repository
        /// </summary>
        /// <param name="key">The key of the pair to update.</param>
        /// <param name="value">The new value of the pair.</param>
        void Update(
            TKey key,
            TValue value);

        /// <summary>
        /// Tries to update the value of an existing key-value pair in the repository
        /// </summary>
        /// <param name="key">The key of the pair to update.</param>
        /// <param name="value">The new value of the pair.</param>
        /// <returns><c>true</c> if the pair was updated successfully; otherwise, <c>false</c>.</returns>
        bool TryUpdate(
            TKey key,
            TValue value);

        /// <summary>
        /// Adds a new key-value pair to the repository or updates the value of an existing pair
        /// </summary>
        /// <param name="key">The key of the pair to add or update.</param>
        /// <param name="value">The value of the pair to add or update.</param>
        void AddOrUpdate(
            TKey key,
            TValue value);

        /// <summary>
        /// Removes a key-value pair from the repository
        /// </summary>
        /// <param name="key">The key of the pair to remove.</param>
        void Remove(TKey key);

        /// <summary>
        /// Tries to remove a key-value pair from the repository
        /// </summary>
        /// <param name="key">The key of the pair to remove.</param>
        /// <returns><c>true</c> if the pair was removed successfully; otherwise, <c>false</c>.</returns>
        bool TryRemove(TKey key);

        /// <summary>
        /// Removes all key-value pairs from the repository
        /// </summary>
        void Clear();
    }
}