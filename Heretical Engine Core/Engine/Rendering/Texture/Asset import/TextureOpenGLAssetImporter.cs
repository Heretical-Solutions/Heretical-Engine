#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGLAssetImporter : AssetImporter
	{
		private readonly string resourcePath;


		private readonly string textureRAMResourcePath;

		private readonly string textureRAMResourceVariantID;


		private readonly string textureDescriptorResourcePath;

		private readonly string textureDescriptorResourceVariantID;


		public TextureOpenGLAssetImporter(
			string resourcePath,

			string textureRAMResourcePath,
			string textureRAMResourceVariantID,
			
			string textureDescriptorResourcePath,
			string textureDescriptorResourceVariantID,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourcePath = resourcePath;


			this.textureRAMResourcePath = textureRAMResourcePath;

			this.textureRAMResourceVariantID = textureRAMResourceVariantID;


			this.textureDescriptorResourcePath = textureDescriptorResourcePath;

			this.textureDescriptorResourceVariantID = textureDescriptorResourceVariantID;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<TextureOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, TextureOpenGLAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.NORMAL_PRIORIITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(TextureOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				TextureFactory.BuildConcurrentTextureOpenGLStorageHandle(
					textureRAMResourcePath,
					textureRAMResourceVariantID,
					textureDescriptorResourcePath,
					textureDescriptorResourceVariantID,
					context),
#else
				TextureFactory.BuildTextureOpenGLStorageHandle(
					textureRAMResourcePath,
					textureRAMResourceVariantID,
					textureDescriptorResourcePath,
					textureDescriptorResourceVariantID,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, TextureOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<TextureOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}
	}
}