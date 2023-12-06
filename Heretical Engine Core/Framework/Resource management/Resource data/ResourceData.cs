using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	/// <summary>
	/// Represents resource data that can be read and modified.
	/// </summary>
	public class ResourceData
		: IResourceData,
		  IContainsDependencyResourceVariants
	{
		private readonly IRepository<int, string> variantIDHashToID;

		private readonly IRepository<int, IResourceVariantData> variantsRepository;

		private IResourceVariantData defaultVariant;


		private readonly IRepository<int, string> nestedResourceIDHashToID;

		private readonly IRepository<int, IReadOnlyResourceData> nestedResourcesRepository;


		private readonly IFormatLogger logger;


		public ResourceData(
			ResourceDescriptor descriptor,
			IRepository<int, string> variantIDHashToID,
			IRepository<int, IResourceVariantData> variantsRepository,
			IRepository<int, string> nestedResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> nestedResourcesRepository,
			IFormatLogger logger)
		{
			Descriptor = descriptor;
			
			this.variantIDHashToID = variantIDHashToID;

			this.variantsRepository = variantsRepository;

			this.nestedResourceIDHashToID = nestedResourceIDHashToID;

			this.nestedResourcesRepository = nestedResourcesRepository;

			this.logger = logger;


			defaultVariant = null;

			ParentResource = null;
		}

		#region IResourceData

		#region IReadOnlyResourceData

		/// <summary>
		/// Gets the descriptor of the resource.
		/// </summary>
		public ResourceDescriptor Descriptor { get; private set; }

		#region IContainsResourceVariants

		/// <summary>
		/// Gets the default variant data of the resource.
		/// </summary>
		public IResourceVariantData DefaultVariant => defaultVariant;

		public bool HasVariant(int variantIDHash)
		{
			return variantsRepository.Has(variantIDHash);
		}

		public bool HasVariant(string variantID)
		{
			return HasVariant(variantID.AddressToHash());
		}

		/// <summary>
		/// Gets the variant data of the resource based on the variant ID hash.
		/// </summary>
		/// <param name="variantIDHash">The hash of the variant ID.</param>
		/// <returns>The variant data associated with the specified variant ID hash.</returns>
		public IResourceVariantData GetVariant(int variantIDHash)
		{
			if (!variantsRepository.TryGet(
				variantIDHash,
				out var variant))
				return null;

			return variant;
		}

		/// <summary>
		/// Gets the variant data of the resource based on the variant ID.
		/// </summary>
		/// <param name="variantID">The ID of the variant.</param>
		/// <returns>The variant data associated with the specified variant ID.</returns>
		public IResourceVariantData GetVariant(string variantID)
		{
			return GetVariant(variantID.AddressToHash());
		}

		public bool TryGetVariant(
			int variantIDHash,
			out IResourceVariantData variant)
		{
			return variantsRepository.TryGet(
				variantIDHash,
				out variant);
		}

		public bool TryGetVariant(
			string variantID,
			out IResourceVariantData variant)
		{
			return TryGetVariant(
				variantID.AddressToHash(),
				out variant);
		}

		/// <summary>
		/// Gets the variant hashes available for the resource.
		/// </summary>
		public IEnumerable<int> VariantIDHashes => variantsRepository.Keys;

		public IEnumerable<string> VariantIDs => variantIDHashToID.Values;

		public IEnumerable<IResourceVariantData> AllVariants => variantsRepository.Values;

		#endregion

		#region IContainsNestedResources

		public IReadOnlyResourceData ParentResource { get; set; }

		public bool IsRoot { get => ParentResource == null; }

		public bool HasNestedResource(int nestedResourceIDHash)
		{
			return nestedResourcesRepository.Has(nestedResourceIDHash);
		}

		public bool HasNestedResource(string nestedResourceID)
		{
			return HasNestedResource(nestedResourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetNestedResource(int nestedResourceIDHash)
		{
			if (!nestedResourcesRepository.TryGet(
				nestedResourceIDHash,
				out var nestedResource))
				return null;

			return nestedResource;
		}

		public IReadOnlyResourceData GetNestedResource(string nestedResourceID)
		{
			return GetNestedResource(nestedResourceID.AddressToHash());
		}

		public bool TryGetNestedResource(
			int nestedResourceIDHash,
			out IReadOnlyResourceData nestedResource)
		{
			return nestedResourcesRepository.TryGet(
				nestedResourceIDHash,
				out nestedResource);
		}

		public bool TryGetNestedResource(
			string nestedResourceID,
			out IReadOnlyResourceData nestedResource)
		{
			return TryGetNestedResource(
				nestedResourceID.AddressToHash(),
				out nestedResource);
		}

		public IEnumerable<int> NestedResourceIDHashes => nestedResourcesRepository.Keys;

		public IEnumerable<string> NestedResourceIDs => nestedResourceIDHashToID.Values;

		public IEnumerable<IReadOnlyResourceData> AllNestedResources => nestedResourcesRepository.Values;

		#endregion


		#endregion

		/// <summary>
		/// Adds a variant to the resource.
		/// </summary>
		/// <param name="variant">The variant data to add.</param>
		/// <param name="progress">An optional progress reporter for tracking the add operation.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public async Task AddVariant(
			IResourceVariantData variant,
			bool allocate = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!variantsRepository.TryAdd(
				variant.Descriptor.VariantIDHash,
				variant))
			{
				progress?.Report(1f);

				return;
			}

			variantIDHashToID.AddOrUpdate(
				variant.Descriptor.VariantIDHash,
				variant.Descriptor.VariantID);

			UpdateDefaultVariant();

			if (allocate)
				await variant
					.StorageHandle
					.Allocate(progress)
					.ThrowExceptions<ResourceData>(logger);

			progress?.Report(1f);
		}

		public async Task RemoveVariant(
			int variantIDHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!variantsRepository.TryGet(
				variantIDHash,
				out var variant))
			{
				progress?.Report(1f);

				return;
			}

			variantIDHashToID.TryRemove(variantIDHash);

			variantsRepository.TryRemove(variantIDHash);

			UpdateDefaultVariant();

			if (free)
				await variant
					.StorageHandle
					.Free(progress)
					.ThrowExceptions<ResourceData>(logger);

			progress?.Report(1f);
		}

		public async Task RemoveVariant(
			string variantID,
			bool free = true,
			IProgress<float> progress = null)
		{
			await RemoveVariant(
				variantID.AddressToHash(),
				free,
				progress)
				.ThrowExceptions<ResourceData>(logger);
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
						IProgress<float> localProgress = progress.CreateLocalProgress(
							0f,
							1f,
							counter,
							totalVariantsCount);
					
						await variant
							.StorageHandle
							.Free(localProgress)
							.ThrowExceptions<ResourceData>(logger);
					}
				}

				counter++;

				progress?.Report((float)counter / (float)totalVariantsCount);
			}

			variantIDHashToID.Clear();

			variantsRepository.Clear();

			defaultVariant = null;

			progress?.Report(1f);
		}

		public async Task AddNestedResource(
			IReadOnlyResourceData nestedResource,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!nestedResourcesRepository.TryAdd(
				nestedResource.Descriptor.IDHash,
				nestedResource))
			{
				progress?.Report(1f);

				return;
			}

			((IResourceData)nestedResource).ParentResource = this;

			nestedResourceIDHashToID.AddOrUpdate(
				nestedResource.Descriptor.IDHash,
				nestedResource.Descriptor.ID);

			progress?.Report(1f);
		}

		public async Task RemoveNestedResource(
			int nestedResourceIDHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!nestedResourcesRepository.TryGet(
				nestedResourceIDHash,
				out var nestedResource))
			{
				progress?.Report(1f);

				return;
			}

			((IResourceData)nestedResource).ParentResource = null;

			nestedResourceIDHashToID.TryRemove(nestedResourceIDHash);

			nestedResourcesRepository.TryRemove(nestedResourceIDHash);

			if (free)
				await ((IResourceData)nestedResource)
					.Clear(
						free,
						progress)
					.ThrowExceptions<ResourceData>(logger);

			progress?.Report(1f);
		}

		public async Task RemoveNestedResource(
			string nestedResourceID,
			bool free = true,
			IProgress<float> progress = null)
		{
			await RemoveNestedResource(
				nestedResourceID.AddressToHash(),
				free,
				progress)
				.ThrowExceptions<ResourceData>(logger);
		}

		public async Task ClearAllNestedResources(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			int totalNestedResourcesCount = nestedResourcesRepository.Count;

			int counter = 0;

			foreach (var key in nestedResourcesRepository.Keys)
			{
				if (!nestedResourcesRepository.TryGet(
					key,
					out var nestedResource))
				{
					((IResourceData)nestedResource).ParentResource = null;

					IProgress<float> localProgress = progress.CreateLocalProgress(
						0f,
						1f,
						counter,
						totalNestedResourcesCount);

					await ((IResourceData)nestedResource)
						.Clear(
							free,
							localProgress)
						.ThrowExceptions<ResourceData>(logger);
				}

				counter++;

				progress?.Report((float)counter / (float)totalNestedResourcesCount);
			}

			nestedResourceIDHashToID.Clear();

			nestedResourcesRepository.Clear();

			progress?.Report(1f);
		}

		public async Task Clear(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.5f);

			await ClearAllVariants(
				free,
				localProgress)
				.ThrowExceptions<ResourceData>(logger);

			progress?.Report(0.5f);

			localProgress = progress.CreateLocalProgress(
				0.5f,
				1f);

			await ClearAllNestedResources(
				free,
				localProgress)
				.ThrowExceptions<ResourceData>(logger);

			defaultVariant = null;

			ParentResource = null;

			progress?.Report(1f);
		}

		#endregion
	
		#region IContainsDependencyResourceVariants

		public async Task<IResourceVariantData> GetDependencyResourceVariant(string variantID = null)
		{
			IResourceVariantData dependencyResourceVariant = null;

			if (string.IsNullOrEmpty(variantID))
			{
				dependencyResourceVariant = defaultVariant;
			}
			else
			{
				dependencyResourceVariant = GetVariant(
					variantID);
			}

			if (dependencyResourceVariant == null)
				logger?.ThrowException<ResourceData>(
					$"VARIANT {(string.IsNullOrEmpty(variantID) ? "NULL" : variantID)} DOES NOT EXIST");

			return dependencyResourceVariant;
		}

		#endregion
	}
}