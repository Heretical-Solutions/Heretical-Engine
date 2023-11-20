#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.Persistence;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

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

		public ShaderOpenGLAssetImporter(
			string fullResourceID,
			ISerializer serializer,
			ISerializationArgument vertexShaderSerializationArgument,
			ISerializationArgument fragmentShaderSerializationArgument,
			ApplicationContext context)
			: base(
				context)
		{
			this.fullResourceID = fullResourceID;

			this.serializer = serializer;

			this.vertexShaderSerializationArgument = vertexShaderSerializationArgument;

			this.fragmentShaderSerializationArgument = fragmentShaderSerializationArgument;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<ShaderOpenGLAssetImporter>(
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
					.ThrowExceptions<IResourceData, ShaderOpenGLAssetImporter>(context.Logger),
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
					context),
#else
				ShaderFactory.BuildShaderOpenGLStorageHandle(
					vertexShaderSourceDTO.Text,
					fragmentShaderSourceDTO.Text,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, ShaderOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<ShaderOpenGLAssetImporter>(
				$"IMPORTING {fullResourceID} FINISHED");

			return result;
		}
	}
}