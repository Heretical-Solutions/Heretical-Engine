using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshRAMAssetImporter : AssetImporter
	{
		private const string MESH_RAM_RESOURCE_ID = "RAM mesh";

		private readonly string resourceID;

		private readonly string variantID;

		private readonly Mesh mesh;

		public MeshRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			string variantID,
			Mesh mesh)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.variantID = variantID;

			this.mesh = mesh;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				CreateNestedResourceData(
					resourceID,
					MESH_RAM_RESOURCE_ID),
				new ResourceVariantDescriptor()
				{
					VariantID = variantID,
					VariantIDHash = variantID.AddressToHash(),
					Priority = 0,
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