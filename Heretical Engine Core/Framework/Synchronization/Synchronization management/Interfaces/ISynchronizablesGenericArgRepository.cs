namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizablesGenericArgRepository<TDelta>
		: IReadOnlySynchronizablesGenericArgRepository<TDelta>
	{
		void AddSynchronizable(ISynchronizableGenericArg<TDelta> synchronizable);

		void RemoveSynchronizable(string id);

		void RemoveSynchronizable(ISynchronizableGenericArg<TDelta> synchronizable);

		void RemoveAllSynchronizables();
	}
}