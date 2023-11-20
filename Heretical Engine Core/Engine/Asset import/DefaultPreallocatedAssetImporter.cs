#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public class DefaultPreallocatedAssetImporter<TAsset> : AssetImporter
	{
		private readonly string resourceID;

		private readonly TAsset preallocatedAsset;

		public DefaultPreallocatedAssetImporter(
			string resourceID,
			TAsset preallocatedAsset,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.preallocatedAsset = preallocatedAsset;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<DefaultPreallocatedAssetImporter<TAsset>>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, DefaultPreallocatedAssetImporter<TAsset>>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = string.Empty,
					VariantIDHash = string.Empty.AddressToHash(),
					Priority = 0,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(TAsset),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<TAsset>(
					preallocatedAsset,
					context),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<TAsset>(
					preallocatedAsset,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, DefaultPreallocatedAssetImporter<TAsset>>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<DefaultPreallocatedAssetImporter<TAsset>>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}