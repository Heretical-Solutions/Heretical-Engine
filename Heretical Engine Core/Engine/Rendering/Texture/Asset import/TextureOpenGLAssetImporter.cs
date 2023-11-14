#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGLAssetImporter : AssetImporter
	{
		public const string TEXTURE_OPENGL_VARIANT_ID = "OpenGL texture";

		public const int TEXTURE_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly IReadOnlyResourceStorageHandle textureRAMStorageHandle;

		private readonly TextureType textureType;

		private readonly GL cachedGL;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		public TextureOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			IReadOnlyResourceStorageHandle textureRAMStorageHandle,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger,
			TextureType textureType = TextureType.None)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.textureRAMStorageHandle = textureRAMStorageHandle;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

			this.textureType = textureType;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<TextureOpenGLAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, TextureOpenGLAssetImporter>(logger),
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
					textureRAMStorageHandle,
					textureType,
					cachedGL,
					mainThreadCommandBuffer,
					logger),
#else
				TextureFactory.BuildTextureOpenGLStorageHandle(
					textureRAMStorageHandle,
					textureType,
					cachedGL,
					mainThreadCommandBuffer,
					logger),
#endif					
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, TextureOpenGLAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<TextureOpenGLAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}