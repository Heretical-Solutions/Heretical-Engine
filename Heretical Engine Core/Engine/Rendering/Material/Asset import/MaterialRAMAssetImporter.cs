#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialRAMAssetImporter : AssetImporter
	{
		public const string MATERIAL_RAM_VARIANT_ID = "Material DTO";

		public const int MATERIAL_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly MaterialDTO material;

		public MaterialRAMAssetImporter(
			string resourceID,
			MaterialDTO material,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.material = material;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<MaterialRAMAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, MaterialRAMAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = MATERIAL_RAM_VARIANT_ID,
					VariantIDHash = MATERIAL_RAM_VARIANT_ID.AddressToHash(),
					Priority = MATERIAL_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MaterialDTO),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<MaterialDTO>(
					material,
					context),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<MaterialDTO>(
					material,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MaterialRAMAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<MaterialRAMAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}