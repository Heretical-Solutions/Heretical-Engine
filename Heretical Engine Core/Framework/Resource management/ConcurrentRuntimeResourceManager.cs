using System;
using System.Threading;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

using HereticalSolutions.Delegates.Notifiers;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentRuntimeResourceManager
		: IRuntimeResourceManager,
		  IAsyncContainsRootResources,
		  IContainsDependencyResources
	{
		private readonly IRepository<int, string> rootResourceIDHashToID;

		private readonly IRepository<int, IReadOnlyResourceData> rootResourcesRepository;

		private readonly IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> rootResourceAddedNotifier;

		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;

		public ConcurrentRuntimeResourceManager(
			IRepository<int, string> rootResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> rootResourcesRepository,
			IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> rootResourceAddedNotifier,
			SemaphoreSlim semaphore,
			IFormatLogger logger)
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

		public bool HasRootResource(int resourceIDHash)
		{
			semaphore.Wait(); // Acquire the semaphore
			
			try
			{
				return rootResourcesRepository.Has(resourceIDHash);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		public bool HasRootResource(string resourceID)
		{
			return HasRootResource(resourceID.AddressToHash());
		}

		public bool HasResource(int[] resourceIDHashes)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHashes[0],
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourceIDHashes);
		}

		public bool HasResource(string[] resourceIDs)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDs[0].AddressToHash(),
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourceIDs);
		}

		#endregion

		#region Get

		public IReadOnlyResourceData GetRootResource(int resourceIDHash)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHash,
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

		public IReadOnlyResourceData GetRootResource(string resourceID)
		{
			return GetRootResource(resourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetResource(int[] resourceIDHashes)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHashes[0],
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourceIDHashes))
				return null;

			return resource;
		}

		public IReadOnlyResourceData GetResource(string[] resourceIDs)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDs[0].AddressToHash(),
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourceIDs))
				return null;

			return resource;
		}

		#endregion

		#region Try get

		public bool TryGetRootResource(
			int resourceIDHash,
			out IReadOnlyResourceData resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				return rootResourcesRepository.TryGet(
					resourceIDHash,
					out resource);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		public bool TryGetRootResource(
			string resourceID,
			out IReadOnlyResourceData resource)
		{
			return TryGetRootResource(
				resourceID.AddressToHash(),
				out resource);
		}

		public bool TryGetResource(
			int[] resourceIDHashes,
			out IReadOnlyResourceData resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHashes[0],
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourceIDHashes);
		}

		public bool TryGetResource(
			string[] resourceIDs,
			out IReadOnlyResourceData resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDs[0].AddressToHash(),
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourceIDs);
		}

		#endregion

		#region Get default

		public IResourceVariantData GetDefaultRootResource(int resourceIDHash)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHash,
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

		public IResourceVariantData GetDefaultRootResource(string resourceID)
		{
			return GetDefaultRootResource(resourceID.AddressToHash());
		}

		public IResourceVariantData GetDefaultResource(int[] resourceIDHashes)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHashes[0],
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourceIDHashes))
				return null;

			return resource.DefaultVariant;
		}

		public IResourceVariantData GetDefaultResource(string[] resourceIDs)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDs[0].AddressToHash(),
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourceIDs))
				return null;

			return resource.DefaultVariant;
		}

		#endregion

		#region Try get default

		public bool TryGetDefaultRootResource(
			int resourceIDHash,
			out IResourceVariantData resource)
		{
			IReadOnlyResourceData rootResource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHash,
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
			string resourceID,
			out IResourceVariantData resource)
		{
			return TryGetDefaultRootResource(
				resourceID.AddressToHash(),
				out resource);
		}

		public bool TryGetDefaultResource(
			int[] resourceIDHashes,
			out IResourceVariantData resource)
		{
			IReadOnlyResourceData rootResource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHashes[0],
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
				resourceIDHashes))
			{
				resource = null;

				return false;
			}

			resource = rootResource.DefaultVariant;

			return true;
		}

		public bool TryGetDefaultResource(
			string[] resourceIDs,
			out IResourceVariantData resource)
		{
			IReadOnlyResourceData rootResource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDs[0].AddressToHash(),
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
				resourceIDs))
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
			IReadOnlyResourceData resource,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryAdd(
					resource.Descriptor.IDHash,
					resource))
				{
					progress?.Report(1f);

					return;
				}

				((IResourceData)resource).ParentResource = null;

				rootResourceIDHashToID.AddOrUpdate(
					resource.Descriptor.IDHash,
					resource.Descriptor.ID);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			progress?.Report(1f);
		}

		public async Task RemoveRootResource(
			int idHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			IReadOnlyResourceData resource;

			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					idHash,
					out resource))
				{
					progress?.Report(1f);

					return;
				}

				rootResourcesRepository.TryRemove(idHash);

				rootResourceIDHashToID.TryRemove(idHash);
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
			string resourceID,
			bool free = true,
			IProgress<float> progress = null)
		{
			await RemoveRootResource(
				resourceID.AddressToHash(),
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

		public async Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(int resourceIDHash)
		{
			Task<IReadOnlyResourceData> waitForNotificationTask;

			semaphore.Wait();

			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				if (rootResourcesRepository.TryGet(
					resourceIDHash,
					out var result))
				{
					return result;
				}

				waitForNotificationTask = await rootResourceAddedNotifier
					.GetWaitForNotificationTask(resourceIDHash)
					.ThrowExceptions<Task<IReadOnlyResourceData>, ConcurrentRuntimeResourceManager>(logger);
			}
			finally
			{
				semaphore.Release();

				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			/*
			return await rootResourceAddedNotifier
				.GetValueWhenNotified(resourceIDHash)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
			*/

			return await waitForNotificationTask
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
		}

		public async Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(string resourceID)
		{
			return await GetRootResourceWhenAvailable(
				resourceID.AddressToHash())
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
		}

		public async Task<IReadOnlyResourceData> GetResourceWhenAvailable(int[] resourceIDHashes)
		{
			var result = await GetRootResourceWhenAvailable(
				resourceIDHashes[0])
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return await GetNestedResourceWhenAvailableRecursive(
				result,
				resourceIDHashes)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
		}

		public async Task<IReadOnlyResourceData> GetResourceWhenAvailable(string[] resourceIDs)
		{
			var result = await GetRootResourceWhenAvailable(
				resourceIDs[0])
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return await GetNestedResourceWhenAvailableRecursive(
				result,
				resourceIDs)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
		}

		#endregion

		#region Get default

		public async Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(int resourceIDHash)
		{
			var rootResource = await GetRootResourceWhenAvailable(resourceIDHash)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return rootResource.DefaultVariant;
		}

		public async Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(string resourceID)
		{
			var rootResource = await GetRootResourceWhenAvailable(resourceID)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return rootResource.DefaultVariant;
		}

		public async Task<IResourceVariantData> GetDefaultResourceWhenAvailable(int[] resourceIDHashes)
		{
			var result = await GetRootResourceWhenAvailable(resourceIDHashes[0])
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			result = await GetNestedResourceWhenAvailableRecursive(
				result,
				resourceIDHashes)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			return result.DefaultVariant;
		}

		public async Task<IResourceVariantData> GetDefaultResourceWhenAvailable(string[] resourceIDs)
		{
			var result = await GetRootResourceWhenAvailable(resourceIDs[0])
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);

			result = await GetNestedResourceWhenAvailableRecursive(
				result,
				resourceIDs)
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

		private bool GetNestedResourceRecursive(
			ref IReadOnlyResourceData currentData,
			int[] resourceIDHashes)
		{
			for (int i = 1; i < resourceIDHashes.Length; i++)
			{
				if (!currentData.TryGetNestedResource(
					resourceIDHashes[i],
					out var newCurrentData))
					return false;

				currentData = newCurrentData;
			}

			return true;
		}

		private bool GetNestedResourceRecursive(
			ref IReadOnlyResourceData currentData,
			string[] resourceIDs)
		{
			for (int i = 1; i < resourceIDs.Length; i++)
			{
				if (!currentData.TryGetNestedResource(
					resourceIDs[i],
					out var newCurrentData))
					return false;

				currentData = newCurrentData;
			}

			return true;
		}

		private async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailableRecursive(
			IReadOnlyResourceData currentData,
			int[] resourceIDHashes)
		{
			for (int i = 1; i < resourceIDHashes.Length; i++)
			{
				IAsyncContainsNestedResources concurrentCurrentData = currentData as IAsyncContainsNestedResources;

				if (concurrentCurrentData == null)
					logger.ThrowException<ConcurrentRuntimeResourceManager>(
						$"RESOURCE DATA {currentData.Descriptor.ID} IS NOT CONCURRENT");

				currentData = await concurrentCurrentData
					.GetNestedResourceWhenAvailable(
						resourceIDHashes[i])
					.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
			}

			return currentData;
		}

		private async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailableRecursive(
			IReadOnlyResourceData currentData,
			string[] resourceIDs)
		{
			for (int i = 1; i < resourceIDs.Length; i++)
			{
				IAsyncContainsNestedResources concurrentCurrentData = currentData as IAsyncContainsNestedResources;

				if (concurrentCurrentData == null)
					logger.ThrowException<ConcurrentRuntimeResourceManager>(
						$"RESOURCE DATA {currentData.Descriptor.ID} IS NOT CONCURRENT");

				currentData = await concurrentCurrentData
					.GetNestedResourceWhenAvailable(
						resourceIDs[i])
					.ThrowExceptions<IReadOnlyResourceData, ConcurrentRuntimeResourceManager>(logger);
			}

			return currentData;
		}
	}
}