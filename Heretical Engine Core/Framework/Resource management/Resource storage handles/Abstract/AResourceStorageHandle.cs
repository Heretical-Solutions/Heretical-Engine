using HereticalSolutions.Collections.Managed;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AResourceStorageHandle<TResource>
	{
		protected readonly IRuntimeResourceManager runtimeResourceManager;

		protected readonly IFormatLogger logger;


		protected bool allocated = false;

		protected TResource resource = default;

		public AResourceStorageHandle(
			IRuntimeResourceManager runtimeResourceManager,
			IFormatLogger logger = null)
		{
			this.runtimeResourceManager = runtimeResourceManager;

			this.logger = logger;


			resource = default;

			allocated = false;
		}

		protected abstract Task<TResource> AllocateResource(
			IProgress<float> progress = null);

		protected abstract Task FreeResource(
			TResource resource,
			IProgress<float> progress = null);

		protected async Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,
			IProgress<float> progress = null)
		{
			return await ((IContainsDependencyResources)runtimeResourceManager)
				.LoadDependency(
					path,
					variantID,
					progress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle>(
					GetType(),
					logger);
		}

		protected async Task ExecuteOnMainThread(
			Action delegateToExecute,
			IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer)
		{
			var command = new MainThreadCommand(
				delegateToExecute);

			while (!mainThreadCommandBuffer.TryProduce(
				command))
			{
				await Task.Yield();
			}

			while (command.Status != ECommandStatus.DONE)
			{
				await Task.Yield();
			}
		}

		protected async Task ExecuteOnMainThread(
			Func<Task> asyncDelegateToExecute,
			IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer)
		{
			var command = new MainThreadCommand(
				asyncDelegateToExecute);

			while (!mainThreadCommandBuffer.TryProduce(
				command))
			{
				await Task.Yield();
			}

			while (command.Status != ECommandStatus.DONE)
			{
				await Task.Yield();
			}
		}
	}
}