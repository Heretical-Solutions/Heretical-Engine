#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialAssetDescriptorAssetImporter : AAssetImporter
	{
		private string resourcePath;

		private MaterialAssetDescriptor descriptor;

		public MaterialAssetDescriptorAssetImporter(
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
			MaterialAssetDescriptor descriptor)
		{
			this.resourcePath = resourcePath;

			this.descriptor = descriptor;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<MaterialAssetDescriptorAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, MaterialAssetDescriptorAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_ASSET_DESCRIPTOR_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_ASSET_DESCRIPTOR_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.DEFAULT_PRIORIITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MaterialAssetDescriptor),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<MaterialAssetDescriptor>(
					descriptor,
					runtimeResourceManager,
					loggerResolver),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<MaterialAssetDescriptor>(
					descriptor,
					runtimeResourceManager,
					loggerResolver),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MaterialAssetDescriptorAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<MaterialAssetDescriptorAssetImporter>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}

		public override void Cleanup()
		{
			resourcePath = null;

			descriptor = default;
		}
	}
}