namespace HereticalSolutions.Repositories
{
	public interface IReadOnlyRepository<TKey, TValue>
	{
		bool Has(TKey key);

		TValue Get(TKey key);

		bool TryGet(
			TKey key,
			out TValue value);

		int Count { get; }

		IEnumerable<TKey> Keys { get; }

		IEnumerable<TValue> Values { get; }
	}
}