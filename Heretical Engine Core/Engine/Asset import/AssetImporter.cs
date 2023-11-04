#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public abstract class AssetImporter
	{
		protected readonly IRuntimeResourceManager resourceManager;

		protected readonly IFormatLogger logger;

		public AssetImporter(
			IRuntimeResourceManager resourceManager,
			IFormatLogger logger)
		{
			this.resourceManager = resourceManager;

			this.logger = logger;
		}

		public virtual async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			throw new NotImplementedException();
		}

		protected virtual async Task<IResourceData> GetOrCreateResourceData(
			string fullResourceID)
		{
			string[] resourceIDs = fullResourceID.SplitAddressBySeparator();

			IReadOnlyResourceData currentData = null;

			if (resourceManager.HasRootResource(resourceIDs[0]))
			{
				currentData = resourceManager.GetRootResource(resourceIDs[0]);
			}
			else
			{
				var descriptor = new ResourceDescriptor()
				{
					ID = resourceIDs[0],
					IDHash = resourceIDs[0].AddressToHash()
				};

				currentData =
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
					ResourceManagementFactory.BuildConcurrentResourceData(
						descriptor,
						logger);
#else
					ResourceManagementFactory.BuildResourceData(
						descriptor,
						logger);
#endif

				await resourceManager.AddRootResource(
					currentData)
					.ThrowExceptions(
						GetType(),
						logger);
			}

			for (int i = 1; i < resourceIDs.Length; i++)
			{
				if (currentData.HasNestedResource(resourceIDs[i]))
				{
					currentData = currentData.GetNestedResource(resourceIDs[i]);
				}
				else
				{
					var descriptor = new ResourceDescriptor()
					{
						ID = resourceIDs[i],

						IDHash = resourceIDs[i].AddressToHash()
					};

					IReadOnlyResourceData newCurrentData =
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
						ResourceManagementFactory.BuildConcurrentResourceData(
							descriptor,
							logger);
#else						
						ResourceManagementFactory.BuildResourceData(
							descriptor,
							logger);
#endif

					await ((IResourceData)currentData)
						.AddNestedResource(
							newCurrentData)
						.ThrowExceptions(
							GetType(),
							logger);

					currentData = newCurrentData;
				}
			}

			return (IResourceData)currentData;
		}

		protected virtual async Task<IResourceData> GetOrCreateNestedResourceData(
			string fullResourceID,
			string nestedResourceID)
		{
			var parent = await GetOrCreateResourceData(
				fullResourceID)
				.ThrowExceptions(
					GetType(),
					logger);

			var descriptor = new ResourceDescriptor()
			{
				ID = nestedResourceID,

				IDHash = nestedResourceID.AddressToHash()
			};

			IReadOnlyResourceData child =
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentResourceData(
					descriptor,
					logger);
#else				
				ResourceManagementFactory.BuildResourceData(
					descriptor,
					logger);
#endif

			await parent
				.AddNestedResource(
					child)
				.ThrowExceptions(
					GetType(),
					logger);

			return (IResourceData)child;
		}

		protected virtual async Task<IResourceVariantData> AddAssetAsResourceVariant(
			IResourceData resourceData,
			ResourceVariantDescriptor variantDescriptor,
			IReadOnlyResourceStorageHandle resourceStorageHandle,
			bool allocate = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var variantData = ResourceManagementFactory.BuildResourceVariantData(
				variantDescriptor,
				resourceStorageHandle);

			await resourceData
				.AddVariant(
					variantData,
					allocate,
					progress)
				.ThrowExceptions(
					GetType(),
					logger);

			progress?.Report(1f);

			return variantData;
		}
	}
}