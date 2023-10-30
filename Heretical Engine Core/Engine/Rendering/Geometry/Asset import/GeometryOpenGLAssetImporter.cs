using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;
using HereticalSolutions.HereticalEngine.Rendering.Factories;

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
			GL gl)
			: base(resourceManager)
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
					resourceID),
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
				progress);

			progress?.Report(1f);

			return result;
		}
	}
}