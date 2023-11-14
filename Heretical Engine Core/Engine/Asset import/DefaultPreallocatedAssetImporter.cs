#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public class DefaultPreallocatedAssetImporter<TAsset> : AssetImporter
	{
		private readonly string resourceID;

		private readonly TAsset preallocatedAsset;

		public DefaultPreallocatedAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			TAsset preallocatedAsset,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.preallocatedAsset = preallocatedAsset;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<DefaultPreallocatedAssetImporter<TAsset>>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, DefaultPreallocatedAssetImporter<TAsset>>(logger),
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
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle(
					preallocatedAsset),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle(
					preallocatedAsset),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, DefaultPreallocatedAssetImporter<TAsset>>(logger);

			progress?.Report(1f);

			logger?.Log<DefaultPreallocatedAssetImporter<TAsset>>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}