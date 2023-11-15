using System;
using System.Threading;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

using HereticalSolutions.Delegates.Notifiers;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentResourceData
		: IResourceData,
		  IAsyncContainsResourceVariants,
		  IAsyncContainsNestedResources
	{
		private readonly IRepository<int, string> variantIDHashToID;

		private readonly IRepository<int, IResourceVariantData> variantsRepository;

		private readonly IAsyncNotifierSingleArgGeneric<int, IResourceVariantData> variantAddedNotifier;

		private IResourceVariantData defaultVariant;


		private IReadOnlyResourceData parentResource;

		private readonly IRepository<int, string> nestedResourceIDHashToID;

		private readonly IRepository<int, IReadOnlyResourceData> nestedResourcesRepository;

		private readonly IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> nestedResourceAddedNotifier;


		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;

		public ConcurrentResourceData(
			ResourceDescriptor descriptor,

			IRepository<int, string> variantIDHashToID,
			IRepository<int, IResourceVariantData> variantsRepository,
			IAsyncNotifierSingleArgGeneric<int, IResourceVariantData> variantAddedNotifier,

			IRepository<int, string> nestedResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> nestedResourcesRepository,
			IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> nestedResourceAddedNotifier,

			SemaphoreSlim semaphore,

			IFormatLogger logger)
		{
			Descriptor = descriptor;


			this.variantIDHashToID = variantIDHashToID;

			this.variantsRepository = variantsRepository;

			this.variantAddedNotifier = variantAddedNotifier;


			this.nestedResourceIDHashToID = nestedResourceIDHashToID;

			this.nestedResourcesRepository = nestedResourcesRepository;

			this.nestedResourceAddedNotifier = nestedResourceAddedNotifier;


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

				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return defaultVariant;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public bool HasVariant(int variantIDHash)
		{
			semaphore.Wait();
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				return variantsRepository.Has(variantIDHash);
			}
			finally
			{
				semaphore.Release();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}
		}

		public bool HasVariant(string variantID)
		{
			return HasVariant(variantID.AddressToHash());
		}

		public IResourceVariantData GetVariant(int variantIDHash)
		{
			semaphore.Wait();
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

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
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				return variantsRepository.TryGet(variantIDHash, out variant);
			}
			finally
			{
				semaphore.Release();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return variantsRepository.Keys;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<string> VariantIDs
		{
			get
			{
				semaphore.Wait();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return variantIDHashToID.Values;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<IResourceVariantData> AllVariants
		{
			get
			{
				semaphore.Wait();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return variantsRepository.Values;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return parentResource;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
			set
			{
				semaphore.Wait();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					parentResource = value;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public bool IsRoot
		{
			get
			{
				semaphore.Wait();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return parentResource == null;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public bool HasNestedResource(int nestedResourceIDHash)
		{
			semaphore.Wait();
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				return nestedResourcesRepository.Has(nestedResourceIDHash);
			}
			finally
			{
				semaphore.Release();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}
		}

		public bool HasNestedResource(string nestedResourceID)
		{
			return HasNestedResource(nestedResourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetNestedResource(int nestedResourceIDHash)
		{
			semaphore.Wait();
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

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
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				return nestedResourcesRepository.TryGet(
					nestedResourceIDHash,
					out nestedResource);
			}
			finally
			{
				semaphore.Release();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return nestedResourcesRepository.Keys;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<string> NestedResourceIDs
		{
			get
			{
				semaphore.Wait();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return nestedResourceIDHashToID.Values;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<IReadOnlyResourceData> AllNestedResources
		{
			get
			{
				semaphore.Wait();
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

				try
				{
					return nestedResourcesRepository.Values;
				}
				finally
				{
					semaphore.Release();
					
					//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
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
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} ADDING VARIANT {variant.Descriptor.VariantID}");

			progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

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

				await variantAddedNotifier
					.Notify(
						variant.Descriptor.VariantIDHash,
						variant)
					.ThrowExceptions<ConcurrentResourceData>(logger);
			}
			finally
			{
				semaphore.Release(); 
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			if (allocate)
			{
				progress?.Report(0.5f);

				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.5f,
					1f);

				await variant
					.StorageHandle
					.Allocate(localProgress)
					.ThrowExceptions<ConcurrentResourceData>(logger);
			}
			
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
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

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
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			if (free)
			{
				progress?.Report(0.5f);

				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.5f,
					1f);

				await variant
					.StorageHandle
					.Free(localProgress)
					.ThrowExceptions<ConcurrentResourceData>(logger);
			}

			progress?.Report(1f);
		}

		public async Task RemoveVariant(
			string variantID,
			bool free = true,
			IProgress<float> progress = null)
		{
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} REMOVING VARIANT {variantID}");

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

			IResourceVariantData[] variantsToFree;

			await semaphore.WaitAsync(); 
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

			try
			{
				variantsToFree = variantsRepository.Values.ToArray();

				variantIDHashToID.Clear();

				variantsRepository.Clear();

				defaultVariant = null;
			}
			finally
			{
				semaphore.Release(); 
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			if (free)
			{
				int variantsToFreeCount = variantsToFree.Length;

				int totalStepsCount = variantsToFreeCount + 1; //Clearing the repos counts as a step

				progress?.Report(1f / (float)totalStepsCount);

				for (int i = 0; i < variantsToFree.Length; i++)
				{
					IProgress<float> localProgress = progress.CreateLocalProgress(
						(1f / (float)totalStepsCount),
						1f,
						i,
						variantsToFreeCount);

					await variantsToFree[i]
						.StorageHandle
						.Free(localProgress)
						.ThrowExceptions<ConcurrentResourceData>(logger);

					progress?.Report((float)(i + 2) / (float)totalStepsCount); // +1 for clearing the repo, +1 because the step is actually finished
				}
			}

			progress?.Report(1f);
		}

		public async Task AddNestedResource(
			IReadOnlyResourceData nestedResource,
			IProgress<float> progress = null)
		{
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} ADDING NESTED RESOURCE {nestedResource.Descriptor.ID}");

			progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

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

				await nestedResourceAddedNotifier
					.Notify(
						nestedResource.Descriptor.IDHash,
						nestedResource)
					.ThrowExceptions<ConcurrentResourceData>(logger);
			}
			finally
			{
				semaphore.Release(); 
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			progress?.Report(1f);
		}

		public async Task RemoveNestedResource(
			int nestedResourceHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			IReadOnlyResourceData nestedResource;

			progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

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
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			if (free)
			{
				progress?.Report(0.5f);

				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.5f,
					1f);

				await ((IResourceData)nestedResource)
					.Clear(
						free,
						localProgress)
					.ThrowExceptions<ConcurrentResourceData>(logger);
			}
			
			progress?.Report(1f);
		}

		public async Task RemoveNestedResource(
			string nestedResourceID,
			bool free = true,
			IProgress<float> progress = null)
		{
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} REMOVING NESTED RESOURCE {nestedResourceID}");

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

			IReadOnlyResourceData[] nestedResourcesToFree;

			await semaphore.WaitAsync(); 
			
			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED ASYNC");

			try
			{
				nestedResourcesToFree = nestedResourcesRepository.Values.ToArray();

				nestedResourceIDHashToID.Clear();

				nestedResourcesRepository.Clear();
			}
			finally
			{
				semaphore.Release(); 
				
				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			if (free)
			{
				int nestedResourcesToFreeCount = nestedResourcesToFree.Length;

				int totalStepsCount = nestedResourcesToFreeCount + 1; //Clearing the repos counts as a step

				progress?.Report(1f / (float)totalStepsCount);

				for (int i = 0; i < nestedResourcesToFreeCount; i++)
				{
					IResourceData nestedResource = (IResourceData)nestedResourcesToFree[i];

					nestedResource.ParentResource = null;

					IProgress<float> localProgress = progress.CreateLocalProgress(
						(1f / (float)totalStepsCount),
						1f,
						i,
						nestedResourcesToFreeCount);

					await nestedResource
						.Clear(
							free,
							localProgress)
						.ThrowExceptions<ConcurrentResourceData>(logger);

					progress?.Report((float)(i + 2) / (float)totalStepsCount); // +1 for clearing the repo, +1 because the step is actually finished
				}
			}

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
				.ThrowExceptions<ConcurrentResourceData>(logger);

			progress?.Report(0.5f);

			localProgress = progress.CreateLocalProgress(
				0.5f,
				1f);

			await ClearAllNestedResources(
				free,
				localProgress)
				.ThrowExceptions<ConcurrentResourceData>(logger);

			progress?.Report(1f);
		}

		#endregion

		#region IAsyncContainsResourceVariants

		public async Task<IResourceVariantData> GetDefaultVariantWhenAvailable()
		{
			Task<IResourceVariantData> waitForNotificationTask;

			semaphore.Wait();

			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				if (defaultVariant != null)
				{
					return defaultVariant;
				}

				waitForNotificationTask = await variantAddedNotifier
					.GetWaitForNotificationTask(-1, true)
					.ThrowExceptions<Task<IResourceVariantData>, ConcurrentResourceData>(logger);
			}
			finally
			{
				semaphore.Release();

				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			return await waitForNotificationTask
				.ThrowExceptions<IResourceVariantData, ConcurrentResourceData>(logger);

			/*
			return await variantAddedNotifier
				.GetValueWhenNotified(-1, true)
				.ThrowExceptions<IResourceVariantData, ConcurrentResourceData>(logger);
			*/
		}

		public async Task<IResourceVariantData> GetVariantWhenAvailable(
			int variantIDHash)
		{
			Task<IResourceVariantData> waitForNotificationTask;

			semaphore.Wait();

			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				if (variantsRepository.TryGet(
					variantIDHash,
					out var result))
				{
					return result;
				}

				waitForNotificationTask = await variantAddedNotifier
					.GetWaitForNotificationTask(variantIDHash)
					.ThrowExceptions<Task<IResourceVariantData>, ConcurrentResourceData>(logger);
			}
			finally
			{
				semaphore.Release();

				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			return await waitForNotificationTask
				.ThrowExceptions<IResourceVariantData, ConcurrentResourceData>(logger);

			/*
			return await variantAddedNotifier
				.GetValueWhenNotified(variantIDHash)
				.ThrowExceptions<IResourceVariantData, ConcurrentResourceData>(logger);
			*/
		}

		public async Task<IResourceVariantData> GetVariantWhenAvailable(
			string variantID)
		{
			return await GetVariantWhenAvailable(
				variantID.AddressToHash())
				.ThrowExceptions<IResourceVariantData, ConcurrentResourceData>(logger);
		}

		#endregion

		#region IAsyncContainsNestedResources

		public async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailable(
			int nestedResourceIDHash)
		{
			Task<IReadOnlyResourceData> waitForNotificationTask;

			semaphore.Wait();

			//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE ACQUIRED");

			try
			{
				if (nestedResourcesRepository.TryGet(
					nestedResourceIDHash,
					out var result))
				{
					return result;
				}

				waitForNotificationTask = await nestedResourceAddedNotifier
					.GetWaitForNotificationTask(nestedResourceIDHash)
					.ThrowExceptions<Task<IReadOnlyResourceData>, ConcurrentResourceData>(logger);
			}
			finally
			{
				semaphore.Release();

				//logger?.Log<ConcurrentResourceData>($"{Descriptor.ID} SEMAPHORE RELEASED");
			}

			/*
			return await nestedResourceAddedNotifier
				.GetValueWhenNotified(nestedResourceIDHash)
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentResourceData>(logger);
			*/

			return await waitForNotificationTask
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentResourceData>(logger);
		}

		public async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailable(
			string nestedResourceID)
		{
			return await GetNestedResourceWhenAvailable(
				nestedResourceID.AddressToHash())
				.ThrowExceptions<IReadOnlyResourceData, ConcurrentResourceData>(logger);
		}

		#endregion
	}
}