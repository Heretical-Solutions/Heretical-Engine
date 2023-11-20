#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGLAssetImporter : AssetImporter
	{
		public const string TEXTURE_OPENGL_VARIANT_ID = "OpenGL texture";

		public const int TEXTURE_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly string textureRAMPath;

		private readonly string textureRAMVariantID;

		private readonly TextureType textureType;

		public TextureOpenGLAssetImporter(
			string resourceID,
			string textureRAMPath,
			string textureRAMVariantID,
			TextureType textureType,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.textureRAMPath = textureRAMPath;

			this.textureRAMVariantID = textureRAMVariantID;

			this.textureType = textureType;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<TextureOpenGLAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, TextureOpenGLAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = TEXTURE_OPENGL_VARIANT_ID,
					VariantIDHash = TEXTURE_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = TEXTURE_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(TextureOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				TextureFactory.BuildConcurrentTextureOpenGLStorageHandle(
					textureRAMPath,
					textureRAMVariantID,
					textureType,
					context),
#else
				TextureFactory.BuildTextureOpenGLStorageHandle(
					textureRAMPath,
					textureRAMVariantID,
					textureType,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, TextureOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<TextureOpenGLAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}