using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshRAMAssetImporter : AssetImporter
	{
		private const string MESH_RAM_VARIANT_ID = "RAM mesh";

		private const int MESH_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly Mesh mesh;

		public MeshRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			Mesh mesh)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.mesh = mesh;
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
					VariantID = MESH_RAM_VARIANT_ID,
					VariantIDHash = MESH_RAM_VARIANT_ID.AddressToHash(),
					Priority = MESH_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(Mesh),
				},
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle(
					mesh),
				true,
				progress);

			progress?.Report(1f);

			return result;
		}
	}
}