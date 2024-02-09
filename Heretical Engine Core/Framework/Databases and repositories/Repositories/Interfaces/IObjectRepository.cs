namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents an interface for a generic object repository that provides methods for adding, updating, removing, and clearing objects
    /// </summary>
    public interface IObjectRepository
        : IReadOnlyObjectRepository
    {
        /// <summary>
        /// Adds a value of type <typeparamref name="TValue"/> to the repository
        /// </summary>
        /// <typeparam name="TValue">The type of the value to add.</typeparam>
        /// <param name="value">The value to add.</param>
        void Add<TValue>(TValue value);

        /// <summary>
        /// Adds an object of the specified <paramref name="valueType"/> to the repository
        /// </summary>
        /// <param name="valueType">The type of the object to add.</param>
        /// <param name="value">The object to add.</param>
        void Add(
            Type valueType,
            object value);

        /// <summary>
        /// Tries to add a value of type <typeparamref name="TValue"/> to the repository
        /// </summary>
        /// <typeparam name="TValue">The type of the value to add.</typeparam>
        /// <param name="value">The value to add.</param>
        /// <returns><c>true</c> if the value was added successfully; otherwise, <c>false</c>.</returns>
        bool TryAdd<TValue>(TValue value);

        /// <summary>
        /// Tries to add an object of the specified <paramref name="valueType"/> to the repository
        /// </summary>
        /// <param name="valueType">The type of the object to add.</param>
        /// <param name="value">The object to add.</param>
        /// <returns><c>true</c> if the object was added successfully; otherwise, <c>false</c>.</returns>
        bool TryAdd(
            Type valueType,
            object value);

        /// <summary>
        /// Updates a value of type <typeparamref name="TValue"/> in the repository
        /// </summary>
        /// <typeparam name="TValue">The type of the value to update.</typeparam>
        /// <param name="value">The value to update.</param>
        void Update<TValue>(TValue value);

        /// <summary>
        /// Updates an object of the specified <paramref name="valueType"/> in the repository
        /// </summary>
        /// <param name="valueType">The type of the object to update.</param>
        /// <param name="value">The object to update.</param>
        void Update(
            Type valueType,
            object value);

        /// <summary>
        /// Tries to update a value of type <typeparamref name="TValue"/> in the repository
        /// </summary>
        /// <typeparam name="TValue">The type of the value to update.</typeparam>
        /// <param name="value">The value to update.</param>
        /// <returns><c>true</c> if the value was updated successfully; otherwise, <c>false</c>.</returns>
        bool TryUpdate<TValue>(TValue value);

        /// <summary>
        /// Tries to update an object of the specified <paramref name="valueType"/> in the repository
        /// </summary>
        /// <param name="valueType">The type of the object to update.</param>
        /// <param name="value">The object to update.</param>
        /// <returns><c>true</c> if the object was updated successfully; otherwise, <c>false</c>.</returns>
        bool TryUpdate(
            Type valueType,
            object value);

        /// <summary>
        /// Adds a value of type <typeparamref name="TValue"/> to the repository if it does not exist, or updates it if it already exists
        /// </summary>
        /// <typeparam name="TValue">The type of the value to add or update.</typeparam>
        /// <param name="value">The value to add or update.</param>
        void AddOrUpdate<TValue>(TValue value);

        /// <summary>
        /// Adds an object of the specified <paramref name="valueType"/> to the repository if it does not exist, or updates it if it already exists
        /// </summary>
        /// <param name="valueType">The type of the object to add or update.</param>
        /// <param name="value">The object to add or update.</param>
        void AddOrUpdate(
            Type valueType,
            object value);

        /// <summary>
        /// Removes all values of type <typeparamref name="TValue"/> from the repository
        /// </summary>
        /// <typeparam name="TValue">The type of the values to remove.</typeparam>
        void Remove<TValue>();

        /// <summary>
        /// Removes all objects of the specified <paramref name="valueType"/> from the repository
        /// </summary>
        /// <param name="valueType">The type of the objects to remove.</param>
        void Remove(Type valueType);

        /// <summary>
        /// Tries to remove all values of type <typeparamref name="TValue"/> from the repository
        /// </summary>
        /// <typeparam name="TValue">The type of the values to remove.</typeparam>
        /// <returns><c>true</c> if the values were removed successfully; otherwise, <c>false</c>.</returns>
        bool TryRemove<TValue>();

        /// <summary>
        /// Tries to remove all objects of the specified <paramref name="valueType"/> from the repository
        /// </summary>
        /// <param name="valueType">The type of the objects to remove.</param>
        /// <returns><c>true</c> if the objects were removed successfully; otherwise, <c>false</c>.</returns>
        bool TryRemove(Type valueType);

        /// <summary>
        /// Removes all values and objects from the repository
        /// </summary>
        void Clear();
    }
}