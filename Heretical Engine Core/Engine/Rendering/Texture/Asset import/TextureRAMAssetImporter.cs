using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureRAMAssetImporter : AAssetImporter
	{
		private string resourcePath;

		private FilePathSettings filePathSettings;

		private IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		public TextureRAMAssetImporter(
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
			FilePathSettings filePathSettings,
			IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer = null)
		{
			this.resourcePath = resourcePath;

			this.filePathSettings = filePathSettings;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<TextureRAMAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, TextureRAMAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_RAM_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_RAM_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.DEFAULT_PRIORIITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(Image<Rgba32>),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				TextureFactory.BuildConcurrentTextureRAMStorageHandle(
					filePathSettings,
					mainThreadCommandBuffer,
					runtimeResourceManager,
					loggerResolver),
#else
				TextureFactory.BuildTextureRAMStorageHandle(
					filePathSettings,
					mainThreadCommandBuffer,
					runtimeResourceManager,
					logger),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, TextureRAMAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<TextureRAMAssetImporter>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}

		public override void Cleanup()
		{
			resourcePath = null;

			filePathSettings = null;
		}
	}
}