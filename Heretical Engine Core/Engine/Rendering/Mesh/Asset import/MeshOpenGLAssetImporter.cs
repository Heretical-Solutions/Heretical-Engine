#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshOpenGLAssetImporter : AssetImporter
	{
		public const string MESH_OPENGL_VARIANT_ID = "OpenGL mesh";

		public const int MESH_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly string meshRAMPath;

		private readonly string meshRAMVariantID;

		public MeshOpenGLAssetImporter(
			string resourceID,
			string meshRAMPath,
			string meshRAMVariantID,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.meshRAMPath = meshRAMPath;

			this.meshRAMVariantID = meshRAMVariantID;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<MeshOpenGLAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, MeshOpenGLAssetImporter>(context.Logger),
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
					meshRAMPath,
					meshRAMVariantID,
					context),
#else
				MeshFactory.BuildMeshOpenGLStorageHandle(
					meshRAMPath,
					meshRAMVariantID,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MeshOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<MeshOpenGLAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}