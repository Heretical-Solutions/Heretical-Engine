#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public class DefaultReadWriteAssetImporter<TAsset> : AssetImporter
	{
		private readonly string resourceID;

		private readonly TAsset readWriteAsset;

		public DefaultReadWriteAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			TAsset readWriteAsset,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.readWriteAsset = readWriteAsset;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger.Log<DefaultReadWriteAssetImporter<TAsset>>(
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
				ResourceManagementFactory.BuildConcurrentReadWriteResourceStorageHandle(
					readWriteAsset),
#else
				ResourceManagementFactory.BuildReadWriteResourceStorageHandle(
					preallocatedAsset),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, DefaultPreallocatedAssetImporter<TAsset>>(logger);

			progress?.Report(1f);

			logger.Log<DefaultReadWriteAssetImporter<TAsset>>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}