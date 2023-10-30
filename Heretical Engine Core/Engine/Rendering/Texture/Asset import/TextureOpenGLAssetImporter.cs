using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGLAssetImporter : AssetImporter
	{
		public const string TEXTURE_OPENGL_VARIANT_ID = "OpenGL texture";

		public const int TEXTURE_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly TextureRAMStorageHandle textureRAMStorageHandle;

		private readonly TextureType textureType;

		private readonly GL cachedGL;

		public TextureOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			TextureRAMStorageHandle textureRAMStorageHandle,
			GL gl,
			TextureType textureType = TextureType.None)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.textureRAMStorageHandle = textureRAMStorageHandle;

			cachedGL = gl;

			this.textureType = textureType;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID),
				new ResourceVariantDescriptor()
				{
					VariantID = TEXTURE_OPENGL_VARIANT_ID,
					VariantIDHash = TEXTURE_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = TEXTURE_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(TextureOpenGL),
				},
				TextureFactory.BuildTextureOpenGLStorageHandle(
					textureRAMStorageHandle,
					textureType,
					cachedGL),
				true,
				progress);

			progress?.Report(1f);

			return result;
		}
	}
}