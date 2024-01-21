namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizationProvidersRepository
	{
		bool TryGetProvider(
			string id,
			out ISynchronizationProvider provider);
	}
}