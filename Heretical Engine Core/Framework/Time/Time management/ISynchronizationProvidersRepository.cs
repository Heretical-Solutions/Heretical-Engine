using HereticalSolutions.Synchronization;

namespace HereticalSolutions.Time
{
	public interface ISynchronizationProvidersRepository
	{
		bool TryGetProvider(
			string id,
			out ISynchronizationProvider provider);
	}
}