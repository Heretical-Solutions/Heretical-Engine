using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Messaging;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AResourceStorageHandle<TResource>
	{
		protected readonly ApplicationContext context;


		protected bool allocated = false;

		protected TResource resource = default;

		public AResourceStorageHandle(
			ApplicationContext context)
		{
			this.context = context;


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
			return await ((IContainsDependencyResources)context.RuntimeResourceManager)
				.LoadDependency(
					path,
					variantID,
					progress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle>(
					GetType(),
					context.Logger);

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
			Action delegateToExecute)
		{
			var command = new MainThreadCommand(
				delegateToExecute);

			while (!context.MainThreadCommandBuffer.TryProduce(
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
			Func<Task> asyncDelegateToExecute)
		{
			var command = new MainThreadCommand(
				asyncDelegateToExecute);

			while (!context.MainThreadCommandBuffer.TryProduce(
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