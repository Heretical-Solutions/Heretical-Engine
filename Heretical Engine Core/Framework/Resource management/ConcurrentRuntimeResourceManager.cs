using System;
using System.Threading;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentRuntimeResourceManager : IRuntimeResourceManager
	{
		private readonly IRepository<int, string> rootResourceIDHashToID;

		private readonly IRepository<int, IReadOnlyResourceData> rootResourcesRepository;

		private readonly SemaphoreSlim semaphore;

		public ConcurrentRuntimeResourceManager(
			IRepository<int, string> rootResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> rootResourcesRepository,
			SemaphoreSlim semaphore)
		{
			this.rootResourceIDHashToID = rootResourceIDHashToID;

			this.rootResourcesRepository = rootResourcesRepository;

			this.semaphore = semaphore;
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

				progress?.Report(1f);
			}
		}

		public async Task RemoveRootResource(
			int idHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!rootResourcesRepository.TryGet(
					idHash,
					out var resource))
				{
					progress?.Report(1f);

					return;
				}

				rootResourcesRepository.TryRemove(idHash);

				rootResourceIDHashToID.TryRemove(idHash);

				if (free)
					await ((IResourceData)resource).Clear(
						free,
						progress);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		public async Task RemoveRootResource(
			string resourceID,
			bool free = true,
			IProgress<float> progress = null)
		{
			await RemoveRootResource(
				resourceID.AddressToHash(),
				free,
				progress);
		}

		public async Task ClearAllRootResources(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				int totalRootResourcesCount = rootResourcesRepository.Count;

				int counter = 0;

				foreach (var key in rootResourcesRepository.Keys)
				{
					if (rootResourcesRepository.TryGet(key, out var rootResource))
					{
						IProgress<float> localProgress = null;

						if (progress != null)
						{
							var localProgressInstance = new Progress<float>();

							localProgressInstance.ProgressChanged += (sender, value) =>
							{
								progress.Report((value + (float)counter) / (float)totalRootResourcesCount);
							};

							localProgress = localProgressInstance;
						}

						await ((IResourceData)rootResource).Clear(
							free,
							localProgress);
					}

					counter++;

					progress?.Report((float)counter / (float)totalRootResourcesCount);
				}

				rootResourceIDHashToID.Clear();

				rootResourcesRepository.Clear();
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
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
	}
}