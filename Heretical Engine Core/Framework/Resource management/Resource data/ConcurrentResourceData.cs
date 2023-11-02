using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentResourceData
		: IResourceData
	{
		private readonly IRepository<int, string> variantIDHashToID;

		private readonly IRepository<int, IResourceVariantData> variantsRepository;

		private IResourceVariantData defaultVariant;


		private IReadOnlyResourceData parentResource;

		private readonly IRepository<int, string> nestedResourceIDHashToID;

		private readonly IRepository<int, IReadOnlyResourceData> nestedResourcesRepository;


		private readonly ReaderWriterLockSlim readWriteLock;

		public ConcurrentResourceData(
			ResourceDescriptor descriptor,
			IRepository<int, string> variantIDHashToID,
			IRepository<int, IResourceVariantData> variantsRepository,
			IRepository<int, string> nestedResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> nestedResourcesRepository,
			ReaderWriterLockSlim readWriteLock)
		{
			Descriptor = descriptor;

			this.variantIDHashToID = variantIDHashToID;

			this.variantsRepository = variantsRepository;

			this.nestedResourceIDHashToID = nestedResourceIDHashToID;

			this.nestedResourcesRepository = nestedResourcesRepository;

			this.readWriteLock = readWriteLock;


			defaultVariant = null;

			parentResource = null;
		}

		#region IResourceData

		#region IReadOnlyResourceData

		public ResourceDescriptor Descriptor { get; private set; }

		#region IContainsResourceVariants

		public IResourceVariantData DefaultVariant
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return defaultVariant;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public bool HasVariant(int variantIDHash)
		{
			readWriteLock.EnterReadLock();

			try
			{
				return variantsRepository.Has(variantIDHash);
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		public bool HasVariant(string variantID)
		{
			return HasVariant(variantID.AddressToHash());
		}

		public IResourceVariantData GetVariant(int variantIDHash)
		{
			readWriteLock.EnterReadLock();

			try
			{
				if (!variantsRepository.TryGet(
					variantIDHash,
					out var variant))
					return null;

				return variant;
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		public IResourceVariantData GetVariant(string variantID)
		{
			return GetVariant(variantID.AddressToHash());
		}

		public bool TryGetVariant(
			int variantIDHash,
			out IResourceVariantData variant)
		{
			readWriteLock.EnterReadLock();

			try
			{
				return variantsRepository.TryGet(variantIDHash, out variant);
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		public bool TryGetVariant(
			string variantID,
			out IResourceVariantData variant)
		{
			return TryGetVariant(variantID.AddressToHash(), out variant);
		}

		public IEnumerable<int> VariantIDHashes
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return variantsRepository.Keys;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public IEnumerable<string> VariantIDs
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return variantIDHashToID.Values;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public IEnumerable<IResourceVariantData> AllVariants
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return variantsRepository.Values;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		#endregion

		#region IContainsNestedResources

		public IReadOnlyResourceData ParentResource
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return parentResource;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
			set
			{
				readWriteLock.EnterWriteLock();

				try
				{
					parentResource = value;
				}
				finally
				{
					readWriteLock.ExitWriteLock();
				}
			}
		}

		public bool IsRoot
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return parentResource == null;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public bool HasNestedResource(int nestedResourceIDHash)
		{
			readWriteLock.EnterReadLock();

			try
			{
				return nestedResourcesRepository.Has(nestedResourceIDHash);
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		public bool HasNestedResource(string nestedResourceID)
		{
			return HasNestedResource(nestedResourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetNestedResource(int nestedResourceIDHash)
		{
			readWriteLock.EnterReadLock();

			try
			{
				if (!nestedResourcesRepository.TryGet(
					nestedResourceIDHash,
					out var nestedResource))
					return null;

				return nestedResource;
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		public IReadOnlyResourceData GetNestedResource(string nestedResourceID)
		{
			return GetNestedResource(nestedResourceID.AddressToHash());
		}

		public bool TryGetNestedResource(
			int nestedResourceIDHash,
			out IReadOnlyResourceData nestedResource)
		{
			readWriteLock.EnterReadLock();

			try
			{
				return nestedResourcesRepository.TryGet(
					nestedResourceIDHash,
					out nestedResource);
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		public bool TryGetNestedResource(
			string nestedResourceID,
			out IReadOnlyResourceData nestedResource)
		{
			return TryGetNestedResource(
				nestedResourceID.AddressToHash(),
				out nestedResource);
		}

		public IEnumerable<int> NestedResourceIDHashes
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return nestedResourcesRepository.Keys;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public IEnumerable<string> NestedResourceIDs
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return nestedResourceIDHashToID.Values;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public IEnumerable<IReadOnlyResourceData> AllNestedResources
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return nestedResourcesRepository.Values;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		#endregion


		#endregion

		public async Task AddVariant(
			IResourceVariantData variant,
			bool allocate = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
				if (!variantsRepository.TryAdd(
					variant.Descriptor.VariantIDHash,
					variant))
				{
					progress?.Report(1f);

					return;
				}

				variantIDHashToID.TryAdd(
					variant.Descriptor.VariantIDHash,
					variant.Descriptor.VariantID);

				UpdateDefaultVariant();

				if (allocate)
					await variant.StorageHandle.Allocate(progress);
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		public async Task RemoveVariant(
			int variantHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
				if (!variantsRepository.TryGet(
					variantHash,
					out var variant))
				{
					progress?.Report(1f);

					return;
				}

				variantIDHashToID.TryRemove(variantHash);

				variantsRepository.TryRemove(variantHash);

				UpdateDefaultVariant();

				if (free)
					await variant.StorageHandle.Free(progress);
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		public async Task RemoveVariant(
			string variantID,
			bool free = true,
			IProgress<float> progress = null)
		{
			await RemoveVariant(
				variantID.AddressToHash(),
				free,
				progress);
		}

		private void UpdateDefaultVariant()
		{
			defaultVariant = null;

			int topPriority = int.MinValue;

			foreach (var hashID in variantsRepository.Keys)
			{
				var currentVariant = variantsRepository.Get(hashID);

				var currentPriority = currentVariant.Descriptor.Priority;

				if (currentPriority > topPriority)
				{
					topPriority = currentPriority;

					defaultVariant = currentVariant;
				}
			}
		}

		public async Task ClearAllVariants(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
				int totalVariantsCount = variantsRepository.Count;

				int counter = 0;

				foreach (var key in variantsRepository.Keys)
				{
					if (variantsRepository.TryGet(
						key,
						out var variant))
					{
						if (free)
						{
							IProgress<float> localProgress = null;

							if (progress != null)
							{
								var localProgressInstance = new Progress<float>();

								localProgressInstance.ProgressChanged += (sender, value) =>
								{
									progress.Report((value + (float)counter) / (float)totalVariantsCount);
								};

								localProgress = localProgressInstance;
							}

							await variant.StorageHandle.Free(localProgress);
						}
					}

					counter++;

					progress?.Report((float)counter / (float)totalVariantsCount);
				}

				variantIDHashToID.Clear();

				variantsRepository.Clear();

				defaultVariant = null;
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		public async Task AddNestedResource(
			IReadOnlyResourceData nestedResource,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
				if (!nestedResourcesRepository.TryAdd(
					nestedResource.Descriptor.IDHash,
					nestedResource))
				{
					progress?.Report(1f);

					return;
				}

				((IResourceData)nestedResource).ParentResource = this;

				nestedResourceIDHashToID.TryAdd(
					nestedResource.Descriptor.IDHash,
					nestedResource.Descriptor.ID);
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		public async Task RemoveNestedResource(
			int nestedResourceHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
				if (!nestedResourcesRepository.TryGet(
					nestedResourceHash,
					out var nestedResource))
				{
					progress?.Report(1f);

					return;
				}

				((IResourceData)nestedResource).ParentResource = null;

				nestedResourceIDHashToID.TryRemove(nestedResourceHash);

				nestedResourcesRepository.TryRemove(nestedResourceHash);

				if (free)
					await ((IResourceData)nestedResource).Clear(
						free,
						progress);
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		public async Task RemoveNestedResource(
			string nestedResourceID,
			bool free = true,
			IProgress<float> progress = null)
		{
			await RemoveNestedResource(
				nestedResourceID.AddressToHash(),
				free,
				progress);
		}

		public async Task ClearAllNestedResources(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
				int totalNestedResourcesCount = nestedResourcesRepository.Count;

				int counter = 0;

				foreach (var key in nestedResourcesRepository.Keys)
				{
					if (!nestedResourcesRepository.TryGet(
						key,
						out var nestedResource))
					{
						((IResourceData)nestedResource).ParentResource = null;

						IProgress<float> localProgress = null;

						if (progress != null)
						{
							var localProgressInstance = new Progress<float>();

							localProgressInstance.ProgressChanged += (sender, value) =>
							{
								progress.Report((value + (float)counter) / (float)totalNestedResourcesCount);
							};

							localProgress = localProgressInstance;
						}

						await ((IResourceData)nestedResource).Clear(
							free,
							localProgress);
					}

					counter++;

					progress?.Report((float)counter / (float)totalNestedResourcesCount);
				}

				nestedResourceIDHashToID.Clear();

				nestedResourcesRepository.Clear();
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		public async Task Clear(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.5f);
					};

					localProgress = localProgressInstance;
				}

				await ClearAllVariants(
					free,
					localProgress);

				progress?.Report(0.5f);

				localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.5f + 0.5f);
					};

					localProgress = localProgressInstance;
				}

				await ClearAllNestedResources(
					free,
					localProgress);

				defaultVariant = null;

				ParentResource = null;
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		#endregion
	}
}