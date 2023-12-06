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
		private readonly string resourcePath;

		private readonly FilePathSettings filePathSettings;

		public TextureRAMAssetImporter(
			string resourcePath,
			FilePathSettings filePathSettings,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourcePath = resourcePath;

			this.filePathSettings = filePathSettings;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<TextureRAMAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, TextureRAMAssetImporter>(context.Logger),
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
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}
	}
}