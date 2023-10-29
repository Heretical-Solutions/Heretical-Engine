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
		private const string TEXTURE_OPENGL_RESOURCE_ID = "OpenGL texture";

		private readonly string resourceID;

		private readonly string variantID;

		private readonly TextureRAMStorageHandle textureRAMStorageHandle;

		private readonly TextureType textureType;

		private readonly GL cachedGL;

		public TextureOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			string variantID,
			TextureRAMStorageHandle textureRAMStorageHandle,
			GL gl,
			TextureType textureType = TextureType.None)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.variantID = variantID;

			this.textureRAMStorageHandle = textureRAMStorageHandle;

			cachedGL = gl;

			this.textureType = textureType;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				CreateNestedResourceData(
					resourceID,
					TEXTURE_OPENGL_RESOURCE_ID),
				new ResourceVariantDescriptor()
				{
					VariantID = variantID,
					VariantIDHash = variantID.AddressToHash(),
					Priority = 0,
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