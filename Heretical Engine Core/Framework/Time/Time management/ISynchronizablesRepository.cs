using HereticalSolutions.Synchronization;

namespace HereticalSolutions.Time
{
	public interface ISynchronizablesRepository
	{
		bool TryGetSynchronizable(
			string id,
			out ISynchronizableGeneric<float> synchronizable);

		void AddSynchronizable(
			ISynchronizableGeneric<float> synchronizable);

		void RemoveSynchronizable(string id);

		void RemoveSynchronizable(ISynchronizableGeneric<float> synchronizable);
	}
}