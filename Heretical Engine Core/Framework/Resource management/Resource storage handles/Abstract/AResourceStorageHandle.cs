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

			/*
			#region Get resource data

			var concurrentResourceManager = ((IAsyncContainsRootResources)context.RuntimeResourceManager);

			IReadOnlyResourceData dependencyResource;

			if (concurrentResourceManager != null)
			{
				dependencyResource = await concurrentResourceManager
					.GetResourceWhenAvailable(
						path.SplitAddressBySeparator())
					.ThrowExceptions<IReadOnlyResourceData>(
						GetType(),
						context.Logger);
			}
			else
			{
				dependencyResource = context.RuntimeResourceManager.GetResource(
					path.SplitAddressBySeparator());

				if (dependencyResource == null)
					context.Logger?.ThrowException(
						GetType(),
						$"RESOURCE {path} DOES NOT EXIST");
			}

			#endregion

			#region Get variant

			var concurrentDependencyResource = ((IAsyncContainsResourceVariants)dependencyResource);

			IResourceVariantData dependencyVariantData;

			if (concurrentDependencyResource != null)
			{
				if (string.IsNullOrEmpty(variantID))
				{
					dependencyVariantData = await concurrentDependencyResource
						.GetDefaultVariantWhenAvailable()
						.ThrowExceptions<IResourceVariantData>(
							GetType(),
							context.Logger);
				}
				else
				{
					dependencyVariantData = await concurrentDependencyResource
						.GetVariantWhenAvailable(
							variantID)
						.ThrowExceptions<IResourceVariantData>(
							GetType(),
							context.Logger);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(variantID))
				{
					dependencyVariantData = dependencyResource.DefaultVariant;
				}
				else
				{
					dependencyVariantData = dependencyResource.GetVariant(
						variantID);
				}

				if (dependencyVariantData == null)
					context.Logger?.ThrowException(
						GetType(),
						$"VARIANT {(string.IsNullOrEmpty(variantID) ? "NULL" : variantID)} DOES NOT EXIST");
			}

			#endregion

			progress?.Report(0.5f);

			var dependencyStorageHandle = dependencyVariantData.StorageHandle;

			if (!dependencyStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.5f,
					1f);

				await dependencyStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions(
						GetType(),
						context.Logger);
			}

			progress?.Report(1f);

			return dependencyStorageHandle;
			*/
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