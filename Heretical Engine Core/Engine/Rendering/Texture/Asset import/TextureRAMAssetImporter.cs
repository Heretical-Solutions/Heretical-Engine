#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureRAMAssetImporter : AssetImporter
	{
		public const string TEXTURE_RAM_VARIANT_ID = "RAM texture";

		public const int TEXTURE_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly FilePathSettings filePathSettings;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		public TextureRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			FilePathSettings filePathSettings,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.filePathSettings = filePathSettings;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger.Log<TextureRAMAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, TextureRAMAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = TEXTURE_RAM_VARIANT_ID,
					VariantIDHash = TEXTURE_RAM_VARIANT_ID.AddressToHash(),
					Priority = TEXTURE_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(Image<Rgba32>),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				TextureFactory.BuildConcurrentTextureRAMStorageHandle(
					filePathSettings,
					mainThreadCommandBuffer,
					logger),
#else
				TextureFactory.BuildTextureRAMStorageHandle(
					filePathSettings,
					mainThreadCommandBuffer,
					logger),
#endif					
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, TextureRAMAssetImporter>(logger);

			progress?.Report(1f);

			logger.Log<TextureRAMAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}