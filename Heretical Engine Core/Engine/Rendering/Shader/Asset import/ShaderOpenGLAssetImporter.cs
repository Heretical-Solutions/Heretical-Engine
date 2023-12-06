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
		private readonly string resourcePath;

		private readonly ISerializer serializer;

		private readonly ISerializationArgument vertexShaderSerializationArgument;

		private readonly ISerializationArgument fragmentShaderSerializationArgument;

		public ShaderOpenGLAssetImporter(
			string resourcePath,
			ISerializer serializer,
			ISerializationArgument vertexShaderSerializationArgument,
			ISerializationArgument fragmentShaderSerializationArgument,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourcePath = resourcePath;

			this.serializer = serializer;

			this.vertexShaderSerializationArgument = vertexShaderSerializationArgument;

			this.fragmentShaderSerializationArgument = fragmentShaderSerializationArgument;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<ShaderOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			serializer.Deserialize<ShaderSourceDTO>(
				vertexShaderSerializationArgument,
				out var vertexShaderSourceDTO);

			serializer.Deserialize<ShaderSourceDTO>(
				fragmentShaderSerializationArgument,
				out var fragmentShaderSourceDTO);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(resourcePath)
					.ThrowExceptions<IResourceData, ShaderOpenGLAssetImporter>(context.Logger),
				new ResourceVariantDescriptor
				{
					VariantID = AssetImportConstants.ASSET_SHADER_OPENGL_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_SHADER_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.DEFAULT_PRIORIITY,
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
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}
	}
}