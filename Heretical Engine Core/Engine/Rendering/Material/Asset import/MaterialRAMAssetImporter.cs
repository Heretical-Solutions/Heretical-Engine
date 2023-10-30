using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialRAMAssetImporter : AssetImporter
	{
		public const string MATERIAL_RAM_VARIANT_ID = "Material DTO";

		public const int MATERIAL_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly MaterialDTO material;

		public MaterialRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			MaterialDTO material)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.material = material;
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
					VariantID = MATERIAL_RAM_VARIANT_ID,
					VariantIDHash = MATERIAL_RAM_VARIANT_ID.AddressToHash(),
					Priority = MATERIAL_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MaterialDTO),
				},
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle(
					material),
				true,
				progress);

			progress?.Report(1f);

			return result;
		}
	}
}