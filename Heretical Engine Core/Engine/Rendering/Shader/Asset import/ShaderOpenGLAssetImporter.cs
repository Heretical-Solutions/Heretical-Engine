using HereticalSolutions.Persistence;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ShaderOpenGLAssetImporter : AssetImporter
	{
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

			this.cachedGL = gl;
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
					VariantID = string.Empty,
					VariantIDHash = string.Empty.AddressToHash(),
					Priority = 0,
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