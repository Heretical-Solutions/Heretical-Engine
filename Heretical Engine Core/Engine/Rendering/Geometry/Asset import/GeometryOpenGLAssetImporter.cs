#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryOpenGLAssetImporter : AssetImporter
	{
		public const string GEOMETRY_OPENGL_VARIANT_ID = "OpenGL geometry";

		public const int GEOMETRY_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly string shaderOpenGLPath;

		private readonly string shaderOpenGLVariantID;

		private readonly string geometryRAMPath;

		private readonly string geometryRAMVariantID;

		public GeometryOpenGLAssetImporter(
			string resourceID,
			string shaderOpenGLPath,
			string shaderOpenGLVariantID,
			string geometryRAMPath,
			string geometryRAMVariantID,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;
			
			this.shaderOpenGLPath = shaderOpenGLPath;

			this.shaderOpenGLVariantID = shaderOpenGLVariantID;

			this.geometryRAMPath = geometryRAMPath;

			this.geometryRAMVariantID = geometryRAMVariantID;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<GeometryOpenGLAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, GeometryOpenGLAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = GEOMETRY_OPENGL_VARIANT_ID,
					VariantIDHash = GEOMETRY_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = GEOMETRY_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(GeometryOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				GeometryFactory.BuildConcurrentGeometryOpenGLStorageHandle(
					shaderOpenGLPath,
					shaderOpenGLVariantID,
					geometryRAMPath,
					geometryRAMVariantID,
					context),
#else
				GeometryFactory.BuildGeometryOpenGLStorageHandle(
					shaderOpenGLPath,
					shaderOpenGLVariantID,
					geometryRAMPath,
					geometryRAMVariantID,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, GeometryOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<GeometryOpenGLAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}