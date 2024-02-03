using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Synchronization
{
	public class SynchronizationManager
		: ISynchronizationManager,
		  ISynchronizablesRepository,
		  ISynchronizationProvidersRepository,
		  ICleanUppable,
		  IDisposable
	{
		private readonly IRepository<string, ISynchronizableNoArgs> synchroRepository;

		public SynchronizationManager(
			IRepository<string, ISynchronizableNoArgs> synchroRepository)
		{
			this.synchroRepository = synchroRepository;
		}

		#region ISynchronizablesRepository

		#region IReadOnlySynchronizablesRepository

		public bool TryGetSynchronizable(
			string id,
			out ISynchronizableNoArgs synchronizable)
		{
			return synchroRepository.TryGet(
				id,
				out synchronizable);
		}

		#endregion

		public void AddSynchronizable(ISynchronizableNoArgs synchronizable)
		{
			synchroRepository.TryAdd(
				synchronizable.Descriptor.ID,
				synchronizable);
		}

		public void RemoveSynchronizable(string id)
		{
			if (!synchroRepository.TryGet(
				id,
				out var synchronizable))
				return;

			((ISynchronizationProvider)synchronizable).UnsubscribeAll();

			synchroRepository.TryRemove(id);
		}

		public void RemoveSynchronizable(ISynchronizableNoArgs synchronizable)
		{
			RemoveSynchronizable(synchronizable.Descriptor.ID);
		}

		public void RemoveAllSynchronizables()
		{
			foreach (var synchronizable in synchroRepository.Values)
			{
				((ISynchronizationProvider)synchronizable).UnsubscribeAll();
			}

			synchroRepository.Clear();
		}

		#endregion

		#region ISynchronizationProvidersRepository

		public bool TryGetProvider(
			string id,
			out ISynchronizationProvider provider)
		{
			provider = default;

			var result = synchroRepository.TryGet(
				id,
				out var synchronizable);

			if (result)
			{
				provider = (ISynchronizationProvider)synchronizable;
			}

			return result;
		}

		#endregion

		#region ISynchronizationManager

		public void SynchronizeAll(string id)
		{
			if (synchroRepository.TryGet(
				id,
				out var synchronizable))
			{
				synchronizable.Synchronize();
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (synchroRepository is ICleanUppable)
				(synchroRepository as ICleanUppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (synchroRepository is IDisposable)
				(synchroRepository as IDisposable).Dispose();
		}

		#endregion
	}
}