#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

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

		private readonly IReadOnlyResourceStorageHandle shaderStorageHandle;

		private readonly GL cachedGL;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		public GeometryOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			IReadOnlyResourceStorageHandle geometryRAMStorageHandle,
			IReadOnlyResourceStorageHandle shaderStorageHandle,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.geometryRAMStorageHandle = geometryRAMStorageHandle;

			this.shaderStorageHandle = shaderStorageHandle;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<GeometryOpenGLAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

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
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				GeometryFactory.BuildConcurrentGeometryOpenGLStorageHandle(
					geometryRAMStorageHandle,
					shaderStorageHandle,
					cachedGL,
					mainThreadCommandBuffer,
					logger),
#else
				GeometryFactory.BuildGeometryOpenGLStorageHandle(
					geometryRAMStorageHandle,
					shaderStorageHandle,
					cachedGL,
					mainThreadCommandBuffer,
					logger),
#endif					
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, GeometryOpenGLAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<GeometryOpenGLAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}