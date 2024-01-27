using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public class DefaultReadWriteAssetImporter<TAsset> : AAssetImporter
	{
		private string resourcePath;

		private TAsset readWriteAsset;

		public DefaultReadWriteAssetImporter(
			IRuntimeResourceManager runtimeResourceManager,
			ILoggerResolver loggerResolver = null,
			ILogger logger = null)
			: base(
				runtimeResourceManager,
				loggerResolver,
				logger)
		{
		}

		public void Initialize(
			string resourcePath,
			TAsset readWriteAsset)
		{
			this.resourcePath = resourcePath;

			this.readWriteAsset = readWriteAsset;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<DefaultReadWriteAssetImporter<TAsset>>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, DefaultReadWriteAssetImporter<TAsset>>(logger),
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
				ResourceManagementFactory.BuildConcurrentReadWriteResourceStorageHandle<TAsset>(
					readWriteAsset,
					runtimeResourceManager,
					loggerResolver),
#else
				ResourceManagementFactory.BuildReadWriteResourceStorageHandle<TAsset>(
					preallocatedAsset,
					runtimeResourceManager,
					loggerResolver),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, DefaultReadWriteAssetImporter<TAsset>>(logger);

			progress?.Report(1f);

			logger?.Log<DefaultReadWriteAssetImporter<TAsset>>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}

		public override void Cleanup()
		{
			resourcePath = null;

			readWriteAsset = default;
		}
	}
}