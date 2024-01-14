#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public abstract class AAssetImporter
	{
		protected readonly IRuntimeResourceManager runtimeResourceManager;

		protected readonly IFormatLogger logger;

		public AAssetImporter(
			IRuntimeResourceManager runtimeResourceManager,
			IFormatLogger logger = null)
		{
			this.runtimeResourceManager = runtimeResourceManager;

			this.logger = logger;
		}

		public virtual async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			throw new NotImplementedException();
		}

		public virtual void Cleanup()
		{
		}

		protected virtual async Task<IResourceData> GetOrCreateResourceData(
			string fullResourcePath)
		{
			string[] resourcePathParts = fullResourcePath.SplitAddressBySeparator();

			IReadOnlyResourceData currentData = null;

			if (!runtimeResourceManager.TryGetRootResource(
				resourcePathParts[0],
				out currentData))
			{
				var descriptor = new ResourceDescriptor()
				{
					ID = resourcePathParts[0],

					IDHash = resourcePathParts[0].AddressToHash(),

					FullPath = resourcePathParts[0]
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

				await runtimeResourceManager.AddRootResource(
					currentData)
					.ThrowExceptions(
						GetType(),
						logger);
			}

			for (int i = 1; i < resourcePathParts.Length; i++)
			{
				IReadOnlyResourceData newCurrentData;

				if (currentData.TryGetNestedResource(
					resourcePathParts[i],
					out newCurrentData))
				{
					currentData = newCurrentData;
				}
				else
				{
					var descriptor = new ResourceDescriptor()
					{
						ID = resourcePathParts[i],

						IDHash = resourcePathParts[i].AddressToHash(),

						//TODO: check if works correctly
						FullPath = resourcePathParts.PartialAddress(i)
					};

					newCurrentData =
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
			string parentResourcePath,
			string nestedResourceID)
		{
			var parent = await GetOrCreateResourceData(
				parentResourcePath)
				.ThrowExceptions(
					GetType(),
					logger);

			var descriptor = new ResourceDescriptor()
			{
				ID = nestedResourceID,

				IDHash = nestedResourceID.AddressToHash(),

				FullPath = $"{parentResourcePath}/{nestedResourceID}"
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
				resourceStorageHandle,
				resourceData);

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

		protected async Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,
			IProgress<float> progress = null)
		{
			return await ((IContainsDependencyResources)runtimeResourceManager)
				.LoadDependency(
					path,
					variantID,
					progress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle>(
					GetType(),
					logger);
		}
	}
}