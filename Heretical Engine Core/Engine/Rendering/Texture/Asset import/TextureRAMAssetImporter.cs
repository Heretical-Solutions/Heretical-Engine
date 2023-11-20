#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureRAMAssetImporter : AssetImporter
	{
		public const string TEXTURE_RAM_VARIANT_ID = "RAM texture";

		public const int TEXTURE_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly FilePathSettings filePathSettings;

		public TextureRAMAssetImporter(
			string resourceID,
			FilePathSettings filePathSettings,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.filePathSettings = filePathSettings;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<TextureRAMAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, TextureRAMAssetImporter>(context.Logger),
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
					context),
#else
				TextureFactory.BuildTextureRAMStorageHandle(
					filePathSettings,
					context),
#endif					
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, TextureRAMAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<TextureRAMAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}