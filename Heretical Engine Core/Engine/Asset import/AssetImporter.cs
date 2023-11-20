#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public abstract class AssetImporter
	{
		protected readonly ApplicationContext context;

		public AssetImporter(
			ApplicationContext context)
		{
			this.context = context;
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

			if (!context.RuntimeResourceManager.TryGetRootResource(
				resourceIDs[0],
				out currentData))
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
						context.Logger);
#else
					ResourceManagementFactory.BuildResourceData(
						descriptor,
						logger);
#endif

				await context.RuntimeResourceManager.AddRootResource(
					currentData)
					.ThrowExceptions(
						GetType(),
						context.Logger);
			}

			for (int i = 1; i < resourceIDs.Length; i++)
			{
				IReadOnlyResourceData newCurrentData;

				if (currentData.TryGetNestedResource(
					resourceIDs[i],
					out newCurrentData))
				{
					currentData = newCurrentData;
				}
				else
				{
					var descriptor = new ResourceDescriptor()
					{
						ID = resourceIDs[i],

						IDHash = resourceIDs[i].AddressToHash()
					};

					newCurrentData =
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
						ResourceManagementFactory.BuildConcurrentResourceData(
							descriptor,
							context.Logger);
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
							context.Logger);

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
					context.Logger);

			var descriptor = new ResourceDescriptor()
			{
				ID = nestedResourceID,

				IDHash = nestedResourceID.AddressToHash()
			};

			IReadOnlyResourceData child =
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentResourceData(
					descriptor,
					context.Logger);
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
					context.Logger);

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
					context.Logger);

			progress?.Report(1f);

			return variantData;
		}
	}
}