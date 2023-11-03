#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelOpenGLAssetImporter : AssetImporter
	{
		public const string MODEL_OPENGL_VARIANT_ID = "OpenGL model";

		public const int MODEL_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly IReadOnlyResourceStorageHandle modelRAMStorageHandle;

		public ModelOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			IReadOnlyResourceStorageHandle modelRAMStorageHandle,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.modelRAMStorageHandle = modelRAMStorageHandle;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, ModelOpenGLAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = MODEL_OPENGL_VARIANT_ID,
					VariantIDHash = MODEL_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = MODEL_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(ModelOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ModelFactory.BuildConcurrentModelOpenGLStorageHandle(
					resourceManager,
					modelRAMStorageHandle,
					logger),
#else
				ModelFactory.BuildModelOpenGLStorageHandle(
					resourceManager,
					textureRAMStorageHandle,
					logger),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, ModelOpenGLAssetImporter>(logger);

			progress?.Report(1f);

			return result;
		}
	}
}