using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AConcurrentReadOnlyResourceStorageHandle<TResource>
		: IReadOnlyResourceStorageHandle
	{
		protected readonly SemaphoreSlim semaphore;

		protected readonly ApplicationContext context;


		protected bool allocated = false;

		protected TResource resource = default;

		public AConcurrentReadOnlyResourceStorageHandle(
			SemaphoreSlim semaphore,
			ApplicationContext context)
		{
			this.semaphore = semaphore;

			this.context = context;


			resource = default;

			allocated = false;
		}

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return allocated;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (allocated)
				{
					progress?.Report(1f);

					return;
				}

				context.Logger?.Log(
					GetType(),
					$"ALLOCATING");

				resource = await AllocateResource(
					progress)
					.ThrowExceptions(
						GetType(),
						context.Logger);

				allocated = true;

				context.Logger?.Log(
					GetType(),
					$"ALLOCATED");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		public async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					progress?.Report(1f);

					return;
				}

				context.Logger?.Log(
					GetType(),
					$"FREEING");

				await FreeResource(
					resource,
					progress)
					.ThrowExceptions(
						GetType(),
						context.Logger);

				resource = default;

				allocated = false;

				context.Logger?.Log(
					GetType(),
					$"FREE");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					if (!allocated)
						context.Logger?.ThrowException(
							GetType(),
							"RESOURCE IS NOT ALLOCATED");

					return resource;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
					context.Logger?.ThrowException(
						GetType(),
						"RESOURCE IS NOT ALLOCATED");

				return (TValue)(object)resource;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion

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