using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;
using HereticalSolutions.HereticalEngine.Rendering.Factories;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshOpenGLAssetImporter : AssetImporter
	{
		private const string MESH_OPENGL_RESOURCE_ID = "OpenGL mesh";

		private readonly string resourceID;

		private readonly string variantID;

		private readonly IReadOnlyResourceStorageHandle meshRAMStorageHandle;

		private readonly GL cachedGL;

		public MeshOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			string variantID,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			GL gl)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.variantID = variantID;

			this.meshRAMStorageHandle = meshRAMStorageHandle;

			cachedGL = gl;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				CreateNestedResourceData(
					resourceID,
					MESH_OPENGL_RESOURCE_ID),
				new ResourceVariantDescriptor()
				{
					VariantID = variantID,
					VariantIDHash = variantID.AddressToHash(),
					Priority = 0,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.GPU,
					ResourceType = typeof(MeshOpenGL),
				},
				MeshFactory.BuildMeshOpenGLStorageHandle(
					meshRAMStorageHandle,
					cachedGL),
				true,
				progress);

			progress?.Report(1f);

			return result;
		}
	}
}