using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryOpenGLAssetImporter : AssetImporter
	{
		public const string GEOMETRY_OPENGL_VARIANT_ID = "OpenGL geometry";

		public const int GEOMETRY_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly IReadOnlyResourceStorageHandle geometryRAMStorageHandle;

		private readonly GL cachedGL;

		public GeometryOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			IReadOnlyResourceStorageHandle geometryRAMStorageHandle,
			GL gl,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.geometryRAMStorageHandle = geometryRAMStorageHandle;

			cachedGL = gl;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, GeometryOpenGLAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = GEOMETRY_OPENGL_VARIANT_ID,
					VariantIDHash = GEOMETRY_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = GEOMETRY_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(GeometryOpenGL),
				},
				GeometryFactory.BuildGeometryOpenGLStorageHandle(
					geometryRAMStorageHandle,
					cachedGL),
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, GeometryOpenGLAssetImporter>(logger);

			progress?.Report(1f);

			return result;
		}
	}
}