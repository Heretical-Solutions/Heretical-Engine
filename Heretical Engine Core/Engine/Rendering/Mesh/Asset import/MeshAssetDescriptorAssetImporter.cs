#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshAssetDescriptorAssetImporter : AAssetImporter
	{
		private string resourcePath;

		private MeshAssetDescriptor descriptor;

		public MeshAssetDescriptorAssetImporter(
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
			MeshAssetDescriptor descriptor)
		{
			this.resourcePath = resourcePath;

			this.descriptor = descriptor;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<MeshAssetDescriptorAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, MeshAssetDescriptorAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_RAM_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_RAM_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.DEFAULT_PRIORIITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MeshAssetDescriptor),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<MeshAssetDescriptor>(
					descriptor,
					runtimeResourceManager,
					loggerResolver),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<MeshAssetDescriptor>(
					descriptor,
					runtimeResourceManager,
					loggerResolver),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MeshAssetDescriptorAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<MeshAssetDescriptorAssetImporter>(
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