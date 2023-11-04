#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshOpenGLAssetImporter : AssetImporter
	{
		public const string MESH_OPENGL_VARIANT_ID = "OpenGL mesh";

		public const int MESH_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly IReadOnlyResourceStorageHandle meshRAMStorageHandle;

		public MeshOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.meshRAMStorageHandle = meshRAMStorageHandle;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger.Log<MeshOpenGLAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, MeshOpenGLAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = MESH_OPENGL_VARIANT_ID,
					VariantIDHash = MESH_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = MESH_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MeshOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				MeshFactory.BuildConcurrentMeshOpenGLStorageHandle(
					resourceManager,
					meshRAMStorageHandle,
					logger),
#else
				MeshFactory.BuildMeshOpenGLStorageHandle(
					resourceManager,
					meshRAMStorageHandle,
					logger),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MeshOpenGLAssetImporter>(logger);

			progress?.Report(1f);

			logger.Log<MeshOpenGLAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}