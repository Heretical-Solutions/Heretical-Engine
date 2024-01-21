namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizablesRepository
		: IReadOnlySynchronizablesRepository
	{
		void AddSynchronizable(ISynchronizableNoArgs synchronizable);

		void RemoveSynchronizable(string id);

		void RemoveSynchronizable(ISynchronizableNoArgs synchronizable);

		void RemoveAllSynchronizables();
	}
}