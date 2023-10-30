using HereticalSolutions.Persistence;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

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

		public ShaderOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string fullResourceID,
			ISerializer serializer,
			ISerializationArgument vertexShaderSerializationArgument,
			ISerializationArgument fragmentShaderSerializationArgument,
			GL gl)
			: base(resourceManager)
		{
			this.fullResourceID = fullResourceID;

			this.serializer = serializer;

			this.vertexShaderSerializationArgument = vertexShaderSerializationArgument;

			this.fragmentShaderSerializationArgument = fragmentShaderSerializationArgument;

			cachedGL = gl;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			serializer.Deserialize<ShaderSourceDTO>(
				vertexShaderSerializationArgument,
				out var vertexShaderSourceDTO);

			serializer.Deserialize<ShaderSourceDTO>(
				fragmentShaderSerializationArgument,
				out var fragmentShaderSourceDTO);

			var result = await AddAssetAsResourceVariant(
				GetOrCreateResourceData(fullResourceID),
				new ResourceVariantDescriptor()
				{
					VariantID = SHADER_OPENGL_VARIANT_ID,
					VariantIDHash = SHADER_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = SHADER_OPENGL_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(ShaderOpenGL)
				},
				ShaderFactory.BuildShaderOpenGLStorageHandle(
					vertexShaderSourceDTO.Text,
					fragmentShaderSourceDTO.Text,
					cachedGL),
				true,
				progress);

			progress?.Report(1f);

			return result;
		}
	}
}