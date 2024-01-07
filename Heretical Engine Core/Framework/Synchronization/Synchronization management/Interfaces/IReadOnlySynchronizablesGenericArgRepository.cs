namespace HereticalSolutions.Synchronization
{
	public interface IReadOnlySynchronizablesGenericArgRepository<TDelta>
	{
		bool TryGetSynchronizable(
			string id,
			out ISynchronizableGenericArg<TDelta> synchronizable);
	}
}