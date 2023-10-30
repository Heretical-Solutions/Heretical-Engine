using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

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
			IReadOnlyResourceStorageHandle meshRAMStorageHandle)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.meshRAMStorageHandle = meshRAMStorageHandle;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				GetOrCreateResourceData(
					resourceID),
				new ResourceVariantDescriptor()
				{
					VariantID = MESH_OPENGL_VARIANT_ID,
					VariantIDHash = MESH_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = MESH_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MeshOpenGL),
				},
				MeshFactory.BuildMeshOpenGLStorageHandle(
					resourceManager,
					meshRAMStorageHandle),
				true,
				progress);

			progress?.Report(1f);

			return result;
		}
	}
}