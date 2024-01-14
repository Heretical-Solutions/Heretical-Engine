#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialPrototypeDescriptorAssetImporter : AAssetImporter
	{
		private string resourcePath;

		private MaterialPrototypeDescriptor descriptor;

		public MaterialPrototypeDescriptorAssetImporter(
			IRuntimeResourceManager runtimeResourceManager,
			ILoggerResolver loggerResolver = null,
			IFormatLogger logger = null)
			: base(
				runtimeResourceManager,
				loggerResolver,
				logger)
		{
		}

		public void Initialize(
			string resourcePath,
			MaterialPrototypeDescriptor descriptor)
		{
			this.resourcePath = resourcePath;

			this.descriptor = descriptor;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<MaterialPrototypeDescriptorAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, MaterialPrototypeDescriptorAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_PROTOTYPE_DESCRIPTOR_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_PROTOTYPE_DESCRIPTOR_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.DEFAULT_PRIORIITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MaterialPrototypeDescriptor),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<MaterialPrototypeDescriptor>(
					descriptor,
					runtimeResourceManager,
					loggerResolver),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<MaterialPrototypeDescriptor>(
					descriptor,
					runtimeResourceManager,
					loggerResolver),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MaterialPrototypeDescriptorAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<MaterialPrototypeDescriptorAssetImporter>(
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