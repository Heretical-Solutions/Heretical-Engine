using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

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


		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;

		public ConcurrentResourceData(
			ResourceDescriptor descriptor,
			IRepository<int, string> variantIDHashToID,
			IRepository<int, IResourceVariantData> variantsRepository,
			IRepository<int, string> nestedResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> nestedResourcesRepository,
			SemaphoreSlim semaphore,
			IFormatLogger logger)
		{
			Descriptor = descriptor;

			this.variantIDHashToID = variantIDHashToID;

			this.variantsRepository = variantsRepository;

			this.nestedResourceIDHashToID = nestedResourceIDHashToID;

			this.nestedResourcesRepository = nestedResourcesRepository;

			this.semaphore = semaphore;

			this.logger = logger;


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
				semaphore.Wait();

				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return defaultVariant;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public bool HasVariant(int variantIDHash)
		{
			semaphore.Wait();
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				return variantsRepository.Has(variantIDHash);
			}
			finally
			{
				semaphore.Release();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}
		}

		public bool HasVariant(string variantID)
		{
			return HasVariant(variantID.AddressToHash());
		}

		public IResourceVariantData GetVariant(int variantIDHash)
		{
			semaphore.Wait();
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

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
				semaphore.Release();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
			semaphore.Wait();
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				return variantsRepository.TryGet(variantIDHash, out variant);
			}
			finally
			{
				semaphore.Release();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
				semaphore.Wait();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return variantsRepository.Keys;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<string> VariantIDs
		{
			get
			{
				semaphore.Wait();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return variantIDHashToID.Values;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<IResourceVariantData> AllVariants
		{
			get
			{
				semaphore.Wait();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return variantsRepository.Values;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		#endregion

		#region IContainsNestedResources

		public IReadOnlyResourceData ParentResource
		{
			get
			{
				semaphore.Wait();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return parentResource;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
			set
			{
				semaphore.Wait();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					parentResource = value;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public bool IsRoot
		{
			get
			{
				semaphore.Wait();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return parentResource == null;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public bool HasNestedResource(int nestedResourceIDHash)
		{
			semaphore.Wait();
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				return nestedResourcesRepository.Has(nestedResourceIDHash);
			}
			finally
			{
				semaphore.Release();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}
		}

		public bool HasNestedResource(string nestedResourceID)
		{
			return HasNestedResource(nestedResourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetNestedResource(int nestedResourceIDHash)
		{
			semaphore.Wait();
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

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
				semaphore.Release();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
			semaphore.Wait();
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				return nestedResourcesRepository.TryGet(
					nestedResourceIDHash,
					out nestedResource);
			}
			finally
			{
				semaphore.Release();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
				semaphore.Wait();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return nestedResourcesRepository.Keys;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<string> NestedResourceIDs
		{
			get
			{
				semaphore.Wait();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return nestedResourceIDHashToID.Values;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<IReadOnlyResourceData> AllNestedResources
		{
			get
			{
				semaphore.Wait();
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return nestedResourcesRepository.Values;
				}
				finally
				{
					semaphore.Release();
					
					//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} ADDING VARIANT {variant.Descriptor.VariantID}");

			progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

			try
			{
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
			}
			finally
			{
				semaphore.Release(); 
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			if (allocate)
				await variant
					.StorageHandle
					.Allocate(progress)
					.ThrowExceptions<ConcurrentResourceData>(logger);

			progress?.Report(1f);
		}

		public async Task RemoveVariant(
			int variantHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			IResourceVariantData variant = null;

			progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

			try
			{
				if (!variantsRepository.TryGet(
					variantHash,
					out variant))
				{
					progress?.Report(1f);

					return;
				}

				variantIDHashToID.TryRemove(variantHash);

				variantsRepository.TryRemove(variantHash);

				UpdateDefaultVariant();
			}
			finally
			{
				semaphore.Release(); 
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			if (free)
				await variant
					.StorageHandle
					.Free(progress)
					.ThrowExceptions<ConcurrentResourceData>(logger);

			progress?.Report(1f);
		}

		public async Task RemoveVariant(
			string variantID,
			bool free = true,
			IProgress<float> progress = null)
		{
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} REMOVING VARIANT {variantID}");

			await RemoveVariant(
				variantID.AddressToHash(),
				free,
				progress)
				.ThrowExceptions<ConcurrentResourceData>(logger);
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

			await semaphore.WaitAsync(); 
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

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
							IProgress<float> localProgress = progress.CreateLocalProgress(
								0f,
								1f,
								counter,
								totalVariantsCount);
								
							await variant
								.StorageHandle
								.Free(localProgress)
								.ThrowExceptions<ConcurrentResourceData>(logger);
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
				semaphore.Release(); 
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");

				progress?.Report(1f);
			}
		}

		public async Task AddNestedResource(
			IReadOnlyResourceData nestedResource,
			IProgress<float> progress = null)
		{
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} ADDING NESTED RESOURCE {nestedResource.Descriptor.ID}");

			progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

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

				nestedResourceIDHashToID.AddOrUpdate(
					nestedResource.Descriptor.IDHash,
					nestedResource.Descriptor.ID);
			}
			finally
			{
				semaphore.Release(); 
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");

				progress?.Report(1f);
			}
		}

		public async Task RemoveNestedResource(
			int nestedResourceHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			IReadOnlyResourceData nestedResource;

			progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

			try
			{
				if (!nestedResourcesRepository.TryGet(
					nestedResourceHash,
					out nestedResource))
				{
					progress?.Report(1f);

					return;
				}

				((IResourceData)nestedResource).ParentResource = null;

				nestedResourceIDHashToID.TryRemove(nestedResourceHash);

				nestedResourcesRepository.TryRemove(nestedResourceHash);
			}
			finally
			{
				semaphore.Release(); 
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			if (free)
				await ((IResourceData)nestedResource)
					.Clear(
						free,
						progress)
					.ThrowExceptions<ConcurrentResourceData>(logger);

			progress?.Report(1f);
		}

		public async Task RemoveNestedResource(
			string nestedResourceID,
			bool free = true,
			IProgress<float> progress = null)
		{
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} REMOVING NESTED RESOURCE {nestedResourceID}");

			await RemoveNestedResource(
				nestedResourceID.AddressToHash(),
				free,
				progress)
				.ThrowExceptions<ConcurrentResourceData>(logger);
		}

		public async Task ClearAllNestedResources(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

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

						IProgress<float> localProgress = progress.CreateLocalProgress(
							0f,
							1f,
							counter,
							totalNestedResourcesCount);

						await ((IResourceData)nestedResource)
							.Clear(
								free,
								localProgress)
							.ThrowExceptions<ConcurrentResourceData>(logger);
					}

					counter++;

					progress?.Report((float)counter / (float)totalNestedResourcesCount);
				}

				nestedResourceIDHashToID.Clear();

				nestedResourcesRepository.Clear();
			}
			finally
			{
				semaphore.Release(); 
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");

				progress?.Report(1f);
			}
		}

		public async Task Clear(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

			try
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0f,
					0.5f);

				await ClearAllVariants(
					free,
					localProgress)
					.ThrowExceptions<ConcurrentResourceData>(logger);

				progress?.Report(0.5f);

				localProgress = progress.CreateLocalProgress(
					0.5f,
					1f);

				await ClearAllNestedResources(
					free,
					localProgress)
					.ThrowExceptions<ConcurrentResourceData>(logger);

				defaultVariant = null;

				ParentResource = null;
			}
			finally
			{
				semaphore.Release(); 
				
				//logger.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");

				progress?.Report(1f);
			}
		}

		#endregion
	}
}