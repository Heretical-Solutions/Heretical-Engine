using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Logging;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureAssetDescriptorAssetImporter : AAssetImporter
	{
		private string resourcePath;

		private string textureName;

		private TextureType textureType;

		public TextureAssetDescriptorAssetImporter(
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
			string textureName,
			TextureType textureType)
		{
			this.resourcePath = resourcePath;

			this.textureName = textureName;

			this.textureType = textureType;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<TextureAssetDescriptorAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, TextureAssetDescriptorAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_ASSET_DESCRIPTOR_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_ASSET_DESCRIPTOR_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.DEFAULT_PRIORIITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(TextureAssetDescriptor),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<TextureAssetDescriptor>(
					TextureFactory.BuildTextureAssetDescriptor(
						textureName,
						textureType),
					runtimeResourceManager,
					loggerResolver),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<TextureAssetDescriptor>(
					TextureFactory.BuildTextureAssetDescriptor(
						textureName,
						textureType),
					runtimeResourceManager,
					logger),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, TextureAssetDescriptorAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<TextureAssetDescriptorAssetImporter>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}

		public override void Cleanup()
		{
			resourcePath = null;

			textureName = null;

			textureType = default;
		}
	}
}