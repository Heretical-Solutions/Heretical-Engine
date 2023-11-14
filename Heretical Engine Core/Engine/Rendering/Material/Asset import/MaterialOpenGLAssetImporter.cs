#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialOpenGLAssetImporter : AssetImporter
	{
		public const string MATERIAL_OPENGL_VARIANT_ID = "OpenGL material";

		public const int MATERIAL_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly IReadOnlyResourceStorageHandle materialRAMStorageHandle;

		public MaterialOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			IReadOnlyResourceStorageHandle materialRAMStorageHandle,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.materialRAMStorageHandle = materialRAMStorageHandle;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<MaterialOpenGLAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, MaterialOpenGLAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = MATERIAL_OPENGL_VARIANT_ID,
					VariantIDHash = MATERIAL_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = MATERIAL_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MaterialOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				MaterialFactory.BuildConcurrentMaterialOpenGLStorageHandle(
					resourceManager,
					materialRAMStorageHandle,
					logger),
#else
				MaterialFactory.BuildMaterialOpenGLStorageHandle(
					resourceManager,
					materialRAMStorageHandle,
					logger),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MaterialOpenGLAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<MaterialOpenGLAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}