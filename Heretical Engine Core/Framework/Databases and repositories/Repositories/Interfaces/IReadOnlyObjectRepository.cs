using System;
using System.Collections.Generic;

namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents an interface for a read-only object repository.
    /// </summary>
    public interface IReadOnlyObjectRepository
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
        /// Gets the collection of keys in the repository.
        /// </summary>
        IEnumerable<Type> Keys { get; }
    }
}