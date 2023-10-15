using System.Collections.Generic;

namespace HereticalSolutions.Repositories
{
	/// <summary>
	/// Represents an interface for a read-only repository.
	/// </summary>
	/// <typeparam name="TKey">The type of the repository key.</typeparam>
	/// <typeparam name="TValue">The type of the repository value.</typeparam>
	public interface IReadOnlyRepository<TKey, TValue>
	{
		/// <summary>
		/// Checks if the repository has a value associated with the specified <paramref name="key"/>.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns><c>true</c> if the repository has a value associated with the specified <paramref name="key"/>; otherwise, <c>false</c>.</returns>
		bool Has(TKey key);

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
		/// Gets the collection of keys in the repository.
		/// </summary>
		IEnumerable<TKey> Keys { get; }
	}
}