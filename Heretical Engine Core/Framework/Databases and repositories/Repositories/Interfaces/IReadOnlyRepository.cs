namespace HereticalSolutions.Repositories
{
	/// <summary>
	/// Represents a read-only repository that provides methods to access and retrieve values based on a key
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the repository.</typeparam>
	/// <typeparam name="TValue">The type of the values in the repository.</typeparam>
	public interface IReadOnlyRepository<TKey, TValue>
	{
		/// <summary>
		/// Determines whether the repository contains a value with the specified key
		/// </summary>
		/// <param name="key">The key to locate in the repository.</param>
		/// <returns><c>true</c> if the repository contains a value with the specified key; otherwise, <c>false</c>.</returns>
		bool Has(TKey key);

		/// <summary>
		/// Gets the value associated with the specified key
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <returns>The value associated with the specified key.</returns>
		TValue Get(TKey key);

		/// <summary>
		/// Tries to get the value associated with the specified key
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
		/// <returns><c>true</c> if the repository contains a value with the specified key; otherwise, <c>false</c>.</returns>
		bool TryGet(
			TKey key,
			out TValue value);

		/// <summary>
		/// Gets the number of key-value pairs contained in the repository
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets an enumerable collection that contains the keys in the repository
		/// </summary>
		IEnumerable<TKey> Keys { get; }

		/// <summary>
		/// Gets an enumerable collection that contains the values in the repository
		/// </summary>
		IEnumerable<TValue> Values { get; }
	}
}