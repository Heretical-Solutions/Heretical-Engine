#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryOpenGLAssetImporter : AAssetImporter
	{
		private string resourcePath;


		private string shaderOpenGLResourcePath;

		private string shaderOpenGLResourceVariantID;


		private string geometryRAMResourcePath;

		private string geometryRAMResourceVariantID;

		public GeometryOpenGLAssetImporter(
			ApplicationContext context)
			: base(
				context)
		{
		}

		public void Initialize(
			string resourcePath,

			string shaderOpenGLResourcePath,
			string shaderOpenGLResourceVariantID,

			string geometryRAMResourcePath,
			string geometryRAMResourceVariantID)
		{
			this.resourcePath = resourcePath;
			

			this.shaderOpenGLResourcePath = shaderOpenGLResourcePath;

			this.shaderOpenGLResourceVariantID = shaderOpenGLResourceVariantID;


			this.geometryRAMResourcePath = geometryRAMResourcePath;

			this.geometryRAMResourceVariantID = geometryRAMResourceVariantID;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<GeometryOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, GeometryOpenGLAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.NORMAL_PRIORIITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(GeometryOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				GeometryFactory.BuildConcurrentGeometryOpenGLStorageHandle(
					shaderOpenGLResourcePath,
					shaderOpenGLResourceVariantID,
					geometryRAMResourcePath,
					geometryRAMResourceVariantID,
					context),
#else
				GeometryFactory.BuildGeometryOpenGLStorageHandle(
					shaderOpenGLResourcePath,
					shaderOpenGLResourceVariantID,
					geometryRAMResourcePath,
					geometryRAMResourceVariantID,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, GeometryOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<GeometryOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}

		public override void Cleanup()
		{
			resourcePath = null;


			shaderOpenGLResourcePath = null;

			shaderOpenGLResourceVariantID = null;


			geometryRAMResourcePath = null;

			geometryRAMResourceVariantID = null;
		}
	}
}