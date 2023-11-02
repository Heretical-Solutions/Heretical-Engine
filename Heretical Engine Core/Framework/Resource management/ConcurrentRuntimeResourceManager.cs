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

		private readonly ReaderWriterLockSlim rwLock;

		public ConcurrentRuntimeResourceManager(
			IRepository<int, string> rootResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> rootResourcesRepository,
			ReaderWriterLockSlim rwLock)
		{
			this.rootResourceIDHashToID = rootResourceIDHashToID;

			this.rootResourcesRepository = rootResourcesRepository;

			this.rwLock = rwLock;
		}

		#region IRuntimeResourceManager

		#region IReadOnlyRuntimeResourceManager

		#region Has

		public bool HasRootResource(int resourceIDHash)
		{
			rwLock.EnterReadLock();
			
			try
			{
				return rootResourcesRepository.Has(resourceIDHash);
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		public bool HasRootResource(string resourceID)
		{
			return HasRootResource(resourceID.AddressToHash());
		}

		public bool HasResource(int[] resourceIDHashes)
		{
			IReadOnlyResourceData currentData;

			rwLock.EnterReadLock();

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHashes[0],
					out currentData))
					return false;
			}
			finally
			{
				rwLock.ExitReadLock();
			}

			return GetNestedResourceRecursive(
				ref currentData,
				resourceIDHashes);
		}

		public bool HasResource(string[] resourceIDs)
		{
			IReadOnlyResourceData currentData;

			rwLock.EnterReadLock();

			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDs[0].AddressToHash(),
					out currentData))
					return false;
			}
			finally
			{
				rwLock.ExitReadLock();
			}

			return GetNestedResourceRecursive(
				ref currentData,
				resourceIDs);
		}

		#endregion

		#region Get

		public IReadOnlyResourceData GetRootResource(int resourceIDHash)
		{
			rwLock.EnterReadLock();
			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHash,
					out var resource))
				{
					return null;
				}
				return resource;
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		public IReadOnlyResourceData GetRootResource(string resourceID)
		{
			return GetRootResource(resourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetResource(int[] resourceIDHashes)
		{
			if (!rootResourcesRepository.TryGet(
				resourceIDHashes[0],
				out var currentResource))
				return null;

			return GetNestedResourceRecursive(
				ref currentResource,
				resourceIDHashes);
		}

		public IReadOnlyResourceData GetResource(string[] resourceIDs)
		{
			if (!rootResourcesRepository.TryGet(
				resourceIDs[0].AddressToHash(),
				out var currentResource))
				return null;

			return GetNestedResourceRecursive(
				ref currentResource,
				resourceIDs);
		}

		#endregion

		#region Try get

		public bool TryGetRootResource(
			int resourceIDHash,
			out IReadOnlyResourceData resource)
		{
			rwLock.EnterReadLock();
			try
			{
				return rootResourcesRepository.TryGet(
					resourceIDHash,
					out resource);
			}
			finally
			{
				rwLock.ExitReadLock();
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
			if (!rootResourcesRepository.TryGet(
				resourceIDHashes[0],
				out resource))
				return false;

			return GetNestedResourceRecursive(
				ref resource,
				resourceIDHashes);
		}

		public bool TryGetResource(
			string[] resourceIDs,
			out IReadOnlyResourceData resource)
		{
			if (!rootResourcesRepository.TryGet(
				resourceIDs[0].AddressToHash(),
				out resource))
				return false;

			return GetNestedResourceRecursive(
				ref resource,
				resourceIDs);
		}

		#endregion

		#region Get default

		public IResourceVariantData GetDefaultRootResource(int resourceIDHash)
		{
			rwLock.EnterReadLock();
			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHash,
					out var resource))
				{
					return null;
				}
				return resource.DefaultVariant;
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		public IResourceVariantData GetDefaultRootResource(string resourceID)
		{
			return GetDefaultRootResource(resourceID.AddressToHash());
		}

		public IResourceVariantData GetDefaultResource(int[] resourceIDHashes)
		{
			if (!rootResourcesRepository.TryGet(
				resourceIDHashes[0],
				out var currentResource))
				return null;

			return GetNestedResourceRecursive(
				ref currentResource,
				resourceIDHashes);
		}

		public IResourceVariantData GetDefaultResource(string[] resourceIDs)
		{
			if (!rootResourcesRepository.TryGet(
				resourceIDs[0].AddressToHash(),
				out var currentResource))
				return null;

			return GetNestedResourceRecursive(
				ref currentResource,
				resourceIDs);
		}

		#endregion

		#region Try get default

		public bool TryGetDefaultRootResource(
			int resourceIDHash,
			out IResourceVariantData resource)
		{
			rwLock.EnterReadLock();
			try
			{
				if (!rootResourcesRepository.TryGet(
					resourceIDHash,
					out var rootResource))
				{
					resource = null;
					return false;
				}
				resource = rootResource.DefaultVariant;
				return true;
			}
			finally
			{
				rwLock.ExitReadLock();
			}
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
			if (!rootResourcesRepository.TryGet(
				resourceIDHashes[0],
				out var currentResource))
			{
				resource = null;
				return false;
			}

			return GetNestedResourceRecursive(
				ref currentResource,
				resourceIDHashes);
		}

		public bool TryGetDefaultResource(
			string[] resourceIDs,
			out IResourceVariantData resource)
		{
			if (!rootResourcesRepository.TryGet(
				resourceIDs[0].AddressToHash(),
				out var currentResource))
			{
				resource = null;
				return false;
			}

			return GetNestedResourceRecursive(
				ref currentResource,
				resourceIDs);
		}

		#endregion

		#region All's

		public IEnumerable<int> RootResourceIDHashes
		{
			get
			{
				rwLock.EnterReadLock();
				try
				{
					return rootResourcesRepository.Keys;
				}
				finally
				{
					rwLock.ExitReadLock();
				}
			}
		}

		public IEnumerable<string> RootResourceIDs
		{
			get
			{
				rwLock.EnterReadLock();
				try
				{
					return rootResourceIDHashToID.Values;
				}
				finally
				{
					rwLock.ExitReadLock();
				}
			}
		}

		public IEnumerable<IReadOnlyResourceData> AllRootResources
		{
			get
			{
				rwLock.EnterReadLock();
				try
				{
					return rootResourcesRepository.Values;
				}
				finally
				{
					rwLock.ExitReadLock();
				}
			}
		}

		#endregion

		#endregion

		public async Task AddRootResource(
			IReadOnlyResourceData resource,
			IProgress<float> progress = null)
		{
			rwLock.EnterWriteLock();
			try
			{
				progress?.Report(0f);

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

				progress?.Report(1f);
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		public async Task RemoveRootResource(
			int idHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			rwLock.EnterWriteLock();
			try
			{
				progress?.Report(0f);

				if (!rootResourcesRepository.TryGet(idHash, out var resource))
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

				progress?.Report(1f);
			}
			finally
			{
				rwLock.ExitWriteLock();
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
			rwLock.EnterWriteLock();
			try
			{
				progress?.Report(0f);
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
				progress?.Report(1f);
			}
			finally
			{
				rwLock.ExitWriteLock();
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