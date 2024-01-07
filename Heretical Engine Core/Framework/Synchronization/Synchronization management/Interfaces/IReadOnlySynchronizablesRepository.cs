namespace HereticalSolutions.Synchronization
{
	public interface IReadOnlySynchronizablesRepository
	{
		bool TryGetSynchronizable(
			string id,
			out ISynchronizableNoArgs synchronizable);
	}
}