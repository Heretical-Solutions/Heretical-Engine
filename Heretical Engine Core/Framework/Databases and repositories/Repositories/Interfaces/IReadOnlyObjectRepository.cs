namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents a read-only object repository
    /// </summary>
    public interface IReadOnlyObjectRepository
    {
        /// <summary>
        /// Determines whether the repository contains an object of the specified type
        /// </summary>
        /// <typeparam name="TValue">The type of the object.</typeparam>
        /// <returns><c>true</c> if the repository contains an object of the specified type; otherwise, <c>false</c>.</returns>
        bool Has<TValue>();

        /// <summary>
        /// Determines whether the repository contains an object of the specified type
        /// </summary>
        /// <param name="valueType">The type of the object.</param>
        /// <returns><c>true</c> if the repository contains an object of the specified type; otherwise, <c>false</c>.</returns>
        bool Has(Type valueType);

        /// <summary>
        /// Gets the object of the specified type from the repository
        /// </summary>
        /// <typeparam name="TValue">The type of the object.</typeparam>
        /// <returns>The object of the specified type.</returns>
        TValue Get<TValue>();

        /// <summary>
        /// Gets the object of the specified type from the repository
        /// </summary>
        /// <param name="valueType">The type of the object.</param>
        /// <returns>The object of the specified type.</returns>
        object Get(Type valueType);

        /// <summary>
        /// Tries to get the object of the specified type from the repository
        /// </summary>
        /// <typeparam name="TValue">The type of the object.</typeparam>
        /// <param name="value">When this method returns, contains the object of the specified type if it is found; otherwise, the default value for the type.</param>
        /// <returns><c>true</c> if the object of the specified type is found; otherwise, <c>false</c>.</returns>
        bool TryGet<TValue>(out TValue value);

        /// <summary>
        /// Tries to get the object of the specified type from the repository
        /// </summary>
        /// <param name="valueType">The type of the object.</param>
        /// <param name="value">When this method returns, contains the object of the specified type if it is found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the object of the specified type is found; otherwise, <c>false</c>.</returns>
        bool TryGet(
            Type valueType,
            out object value);

        /// <summary>
        /// Gets the number of objects in the repository
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the collection of object types in the repository
        /// </summary>
        IEnumerable<Type> Keys { get; }

        /// <summary>
        /// Gets the collection of objects in the repository
        /// </summary>
        IEnumerable<object> Values { get; }
    }
}