#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Persistence;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ShaderOpenGLAssetImporter : AssetImporter
	{
		public const string SHADER_OPENGL_VARIANT_ID = "OpenGL shader";

		public const int SHADER_OPENGL_PRIORITY = 0;

		private readonly string fullResourceID;

		private readonly ISerializer serializer;

		private readonly ISerializationArgument vertexShaderSerializationArgument;

		private readonly ISerializationArgument fragmentShaderSerializationArgument;

		private readonly GL cachedGL;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		public ShaderOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string fullResourceID,
			ISerializer serializer,
			ISerializationArgument vertexShaderSerializationArgument,
			ISerializationArgument fragmentShaderSerializationArgument,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.fullResourceID = fullResourceID;

			this.serializer = serializer;

			this.vertexShaderSerializationArgument = vertexShaderSerializationArgument;

			this.fragmentShaderSerializationArgument = fragmentShaderSerializationArgument;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger.Log<ShaderOpenGLAssetImporter>(
				$"IMPORTING {fullResourceID} INITIATED");

			progress?.Report(0f);

			serializer.Deserialize<ShaderSourceDTO>(
				vertexShaderSerializationArgument,
				out var vertexShaderSourceDTO);

			serializer.Deserialize<ShaderSourceDTO>(
				fragmentShaderSerializationArgument,
				out var fragmentShaderSourceDTO);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(fullResourceID)
					.ThrowExceptions<IResourceData, ShaderOpenGLAssetImporter>(logger),
				new ResourceVariantDescriptor
				{
					VariantID = SHADER_OPENGL_VARIANT_ID,
					VariantIDHash = SHADER_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = SHADER_OPENGL_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(ShaderOpenGL)
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ShaderFactory.BuildConcurrentShaderOpenGLStorageHandle(
					vertexShaderSourceDTO.Text,
					fragmentShaderSourceDTO.Text,
					cachedGL,
					mainThreadCommandBuffer,
					logger),
#else
				ShaderFactory.BuildShaderOpenGLStorageHandle(
					vertexShaderSourceDTO.Text,
					fragmentShaderSourceDTO.Text,
					cachedGL,
					mainThreadCommandBuffer,
					logger),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, ShaderOpenGLAssetImporter>(logger);

			progress?.Report(1f);

			logger.Log<ShaderOpenGLAssetImporter>(
				$"IMPORTING {fullResourceID} FINISHED");

			return result;
		}
	}
}