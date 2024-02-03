using HereticalSolutions.Repositories;

using HereticalSolutions.Delegates.Notifiers;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentRuntimeResourceManager
		: IRuntimeResourceManager,
		  IAsyncContainsRootResources,
		  IContainsDependencyResources,
		  ICleanUppable,
		  IDisposable
	{
		private readonly IRepository<int, string> rootResourceIDHashToID;

		private readonly IRepository<int, IReadOnlyResourceData> rootResourcesRepository;

		private readonly IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> rootResourceAddedNotifier;

		private readonly SemaphoreSlim semaphore;

		private readonly ILogger logger;

		public ConcurrentRuntimeResourceManager(
			IRepository<int, string> rootResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> rootResourcesRepository,
			IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> rootResourceAddedNotifier,
			SemaphoreSlim semaphore,
			ILogger logger = null)
		{
			this.rootResourceIDHashToID = rootResourceIDHashToID;

			this.rootResourcesRepository = rootResourcesRepository;

			this.rootResourceAddedNotifier = rootResourceAddedNotifier;

			this.semaphore = semaphore;

			this.logger = logger;
		}

		#region IRuntimeResourceManager

		#region IReadOnlyRuntimeResourceManager

		#region Has

		public bool HasRootResource(int rootResourceIDHash)
		{
			semaphore.Wait(); // Acquire the semaphore
			
			try
			{
				return rootResourcesRepository.Has(rootResourceIDHash);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		public bool HasRootResource(string rootResourceID)
		{
			return HasRootResource(rootResourceID.AddressToHash());
		}

		public bool HasResource(int[] resourcePathPartHashes)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathPartHashes[0],
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourcePathPartHashes);
		}

		public bool HasResource(string[] resourcePathParts)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourcePathParts);
		}

		#endregion

		#region Get

		public IReadOnlyResourceData GetRootResource(int rootResourceIDHash)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					rootResourceIDHash,
					out resource))
				{
					return null;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return resource;
		}

		public IReadOnlyResourceData GetRootResource(string rootResourceID)
		{
			return GetRootResource(rootResourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetResource(int[] resourcePathPartHashes)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathPartHashes[0],
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourcePathPartHashes))
				return null;

			return resource;
		}

		public IReadOnlyResourceData GetResource(string[] resourcePathParts)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourcePathParts))
				return null;

			return resource;
		}

		#endregion

		#region Try get

		public bool TryGetRootResource(
			int rootResourceIDHash,
			out IReadOnlyResourceData resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				return rootResourcesRepository.TryGet(
					rootResourceIDHash,
					out resource);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		public bool TryGetRootResource(
			string rootResourceID,
			out IReadOnlyResourceData resource)
		{
			return TryGetRootResource(
				rootResourceID.AddressToHash(),
				out resource);
		}

		public bool TryGetResource(
			int[] resourcePathPartHashes,
			out IReadOnlyResourceData resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathPartHashes[0],
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourcePathPartHashes);
		}

		public bool TryGetResource(
			string[] resourcePathParts,
			out IReadOnlyResourceData resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourcePathParts);
		}

		#endregion

		#region Get default

		public IResourceVariantData GetDefaultRootResource(int rootResourceIDHash)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					rootResourceIDHash,
					out resource))
				{
					return null;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return resource.DefaultVariant;
		}

		public IResourceVariantData GetDefaultRootResource(string rootResourceID)
		{
			return GetDefaultRootResource(rootResourceID.AddressToHash());
		}

		public IResourceVariantData GetDefaultResource(int[] resourcePathPartHashes)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathPartHashes[0],
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourcePathPartHashes))
				return null;

			return resource.DefaultVariant;
		}

		public IResourceVariantData GetDefaultResource(string[] resourcePathParts)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourcePathParts))
				return null;

			return resource.DefaultVariant;
		}

		#endregion

		#region Try get default

		public bool TryGetDefaultRootResource(
			int rootResourceIDHash,
			out IResourceVariantData resource)
		{
			IReadOnlyResourceData rootResource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					rootResourceIDHash,
					out rootResource))
				{
					resource = null;

					return false;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			resource = rootResource.DefaultVariant;

			return true;
		}

		public bool TryGetDefaultRootResource(
			string rootResourceID,
			out IResourceVariantData resource)
		{
			return TryGetDefaultRootResource(
				rootResourceID.AddressToHash(),
				out resource);
		}

		public bool TryGetDefaultResource(
			int[] resourcePathPartHashes,
			out IResourceVariantData resource)
		{
			IReadOnlyResourceData rootResource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathPartHashes[0],
					out rootResource))
				{
					resource = null;

					return false;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref rootResource,
				resourcePathPartHashes))
			{
				resource = null;

				return false;
			}

			resource = rootResource.DefaultVariant;

			return true;
		}

		public bool TryGetDefaultResource(
			string[] resourcePathParts,
			out IResourceVariantData resource)
		{
			IReadOnlyResourceData rootResource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out rootResource))
				{
					resource = null;

					return false;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref rootResource,
				resourcePathParts))
			{
				resource = null;

				return false;
			}

			resource = rootResource.DefaultVariant;

			return true;
		}

		#endregion

		#region All's

		public IEnumerable<int> RootResourceIDHashes
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return rootResourcesRepository.Keys;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public IEnumerable<string> RootResourceIDs
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return rootResourceIDHashToID.Values;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public IEnumerable<IReadOnlyResourceData> AllRootResources
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return rootResourcesRepository.Values;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		#endregion

		#endregion

		public async Task AddRootResource(
			IReadOnlyResourceData rootResource,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryAdd(
					rootResource.Descriptor.IDHash,
					rootResource))
				{
					progress?.Report(1f);

					return;
				}

				((IResourceData)rootResource).ParentResource = null;

				rootResourceIDHashToID.AddOrUpdate(
					rootResource.Descriptor.IDHash,
					rootResource.Descriptor.ID);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			progress?.Report(1f);
		}

		public async Task RemoveRootResource(
			int rootResourceIDHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			IReadOnlyResourceData resource;

			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					rootResourceIDHash,
					out resource))
				{
					progress?.Report(1f);

					return;
				}

				rootResourcesRepository.TryRemove(rootResourceIDHash);

				rootResourceIDHashToID.TryRemove(rootResourceIDHash);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (free)
			{
				progress?.Report(0.5f);

				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.5f,
					1f);

				await ((IResourceData)resource)
					.Clear(
						free,
						localProgress)
					.ThrowExceptions<ConcurrentRuntimeResourceManager>(logger);
			}

			progress?.Report(1f);
		}

		public async Task RemoveRootResource(
			string rootResourceID,
			bool free = true,
			IProgress<float> progress = null)
		{
			await RemoveRootResource(
				rootResourceID.AddressToHash(),
				free,
				progress)
				.ThrowExceptions<ConcurrentRuntimeResourceManager>(logger);
		}

		public async Task ClearAllRootResources(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IReadOnlyResourceData[] rootResourcesToFree;

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				rootResourcesToFree = rootResourcesRepository.Values.ToArray();

				rootResourceIDHashToID.Clear();

				rootResourcesRepository.Clear();
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (free)
			{
				int rootResourcesToFreeCount = rootResourcesToFree.Length;

				int totalStepsCount = rootResourcesToFreeCount + 1; //Clearing the repos counts as a step

				progress?.Report(1f / (float)totalStepsCount);

				for (int i = 0; i < rootResourcesToFreeCount; i++)
				{
					IResourceData rootResource = (IResourceData)rootResourcesToFree[i];

					IProgress<float> localProgress = progress.CreateLocalProgress(
						(1f / (float)totalStepsCount),
						1f,
						i,
						rootResourcesToFreeCount);

					await rootResource
						.Clear(
							free,
							localProgress)
						.ThrowExceptions<ConcurrentRuntimeResourceManager>(logger);

					progress?.Report((float)(i + 2) / (float)totalStepsCount); // +1 for clearing the repo, +1 because the step is actually finished
				}
			}

			progress?.Report(1f);
		}

		#endregion

		#region IAsyncContainsRootResources

		#region Get

		public async Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(int rootResourceIDHash)
		{
			Task<IReadOnlyResourceData> waitForNotificationTask;

			semaphore.Wait();

			//logger?.Log<ConcurrentResourceData>($"GetRootResourceWhenAvailable SEMAPHORE ACQUIRED");

			try
			{
				if (rootResourcesRepository.TryGet(
					rootResourceIDHash,
					out var result))
				{
					return result;
				}

				waitForNotificationTask = await rootResourceAddedNotifier
					.GetWaitForNotificationTask(rootResourceIDHash)
					.ThrowExceptions<Task<IReadOnlyResourceData>, ConcurrentRuntimeResourceManager>(logger);
			}
			finally
			{
				semaphore.Release();

				//logger?.Log<ConcurrentResourceData>($"GetRootResourceWhenAvailable SEMAPHORE RELEASED");
			}

			/*
			return await rootResourceAddedNotifier
				.GetValueWhenNotified(resourceIDHash)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
			*/

			//logger?.Log<ConcurrentResourceData>($"GetRootResourceWhenAvailable AWAITING INITIATED");

			var awaitedResult = await waitForNotificationTask
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			//logger?.Log<ConcurrentResourceData>($"GetRootResourceWhenAvailable AWAITING FINISHED");

			return awaitedResult;
		}

		public async Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(string rootResourceID)
		{
			return await GetRootResourceWhenAvailable(
				rootResourceID.AddressToHash())
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
		}

		public async Task<IReadOnlyResourceData> GetResourceWhenAvailable(int[] resourcePathPartHashes)
		{
			var result = await GetRootResourceWhenAvailable(
				resourcePathPartHashes[0])
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return await GetNestedResourceWhenAvailableRecursive(
				result,
				resourcePathPartHashes)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
		}

		public async Task<IReadOnlyResourceData> GetResourceWhenAvailable(string[] resourcePathParts)
		{
			var result = await GetRootResourceWhenAvailable(
				resourcePathParts[0])
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return await GetNestedResourceWhenAvailableRecursive(
				result,
				resourcePathParts)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
		}

		#endregion

		#region Get default

		public async Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(int rootResourceIDHash)
		{
			var rootResource = await GetRootResourceWhenAvailable(rootResourceIDHash)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return rootResource.DefaultVariant;
		}

		public async Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(string rootResourceID)
		{
			var rootResource = await GetRootResourceWhenAvailable(rootResourceID)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return rootResource.DefaultVariant;
		}

		public async Task<IResourceVariantData> GetDefaultResourceWhenAvailable(int[] resourcePathPartHashes)
		{
			var result = await GetRootResourceWhenAvailable(resourcePathPartHashes[0])
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			result = await GetNestedResourceWhenAvailableRecursive(
				result,
				resourcePathPartHashes)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return result.DefaultVariant;
		}

		public async Task<IResourceVariantData> GetDefaultResourceWhenAvailable(string[] resourcePathParts)
		{
			var result = await GetRootResourceWhenAvailable(resourcePathParts[0])
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			result = await GetNestedResourceWhenAvailableRecursive(
				result,
				resourcePathParts)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return result.DefaultVariant;
		}

		#endregion

		#endregion

		#region IContainsDependencyResources

		public async Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,
			IProgress<float> progress = null)
		{
			IReadOnlyResourceData dependencyResource = await GetDependencyResource(path)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(
					logger);

			IResourceVariantData dependencyVariantData = await ((IContainsDependencyResourceVariants)dependencyResource)
				.GetDependencyResourceVariant(variantID)
				.ThrowExceptions<IResourceVariantData, ConcurrentRuntimeResourceManager>(
					logger);

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
					.ThrowExceptions<ConcurrentRuntimeResourceManager>(
						logger);
			}

			progress?.Report(1f);

			return dependencyStorageHandle;
		}

		public async Task<IReadOnlyResourceData> GetDependencyResource(
			string path)
		{
			return await GetResourceWhenAvailable(
				path.SplitAddressBySeparator())
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(
					logger);
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (rootResourceIDHashToID is ICleanUppable)
				(rootResourceIDHashToID as ICleanUppable).Cleanup();

			if (rootResourcesRepository is ICleanUppable)
				(rootResourcesRepository as ICleanUppable).Cleanup();

			if (rootResourceAddedNotifier is ICleanUppable)
				(rootResourceAddedNotifier as ICleanUppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (rootResourceIDHashToID is IDisposable)
				(rootResourceIDHashToID as IDisposable).Dispose();

			if (rootResourcesRepository is IDisposable)
				(rootResourcesRepository as IDisposable).Dispose();

			if (rootResourceAddedNotifier is IDisposable)
				(rootResourceAddedNotifier as IDisposable).Dispose();
		}

		#endregion

		private bool GetNestedResourceRecursive(
			ref IReadOnlyResourceData currentData,
			int[] resourcePathPartHashes)
		{
			for (int i = 1; i < resourcePathPartHashes.Length; i++)
			{
				if (!currentData.TryGetNestedResource(
					resourcePathPartHashes[i],
					out var newCurrentData))
					return false;

				currentData = newCurrentData;
			}

			return true;
		}

		private bool GetNestedResourceRecursive(
			ref IReadOnlyResourceData currentData,
			string[] resourcePathParts)
		{
			for (int i = 1; i < resourcePathParts.Length; i++)
			{
				if (!currentData.TryGetNestedResource(
					resourcePathParts[i],
					out var newCurrentData))
					return false;

				currentData = newCurrentData;
			}

			return true;
		}

		private async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailableRecursive(
			IReadOnlyResourceData currentData,
			int[] resourcePathPartHashes)
		{
			for (int i = 1; i < resourcePathPartHashes.Length; i++)
			{
				IAsyncContainsNestedResources concurrentCurrentData = currentData as IAsyncContainsNestedResources;

				if (concurrentCurrentData == null)
					throw new Exception(
						logger.TryFormat<ConcurrentRuntimeResourceManager>(
							$"RESOURCE DATA {currentData.Descriptor.ID} IS NOT CONCURRENT"));

				currentData = await concurrentCurrentData
					.GetNestedResourceWhenAvailable(
						resourcePathPartHashes[i])
					.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
			}

			return currentData;
		}

		private async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailableRecursive(
			IReadOnlyResourceData currentData,
			string[] resourcePathParts)
		{
			for (int i = 1; i < resourcePathParts.Length; i++)
			{
				IAsyncContainsNestedResources concurrentCurrentData = currentData as IAsyncContainsNestedResources;

				if (concurrentCurrentData == null)
					throw new Exception(
						logger.TryFormat<ConcurrentRuntimeResourceManager>(
							$"RESOURCE DATA {currentData.Descriptor.ID} IS NOT CONCURRENT"));

				currentData = await concurrentCurrentData
					.GetNestedResourceWhenAvailable(
						resourcePathParts[i])
					.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
			}

			return currentData;
		}
	}
}