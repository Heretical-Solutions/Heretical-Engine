#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshRAMAssetImporter : AssetImporter
	{
		public const string MESH_RAM_VARIANT_ID = "Mesh DTO";

		public const int MESH_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly MeshDTO mesh;

		public MeshRAMAssetImporter(
			string resourceID,
			MeshDTO mesh,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.mesh = mesh;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<MeshRAMAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, MeshRAMAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = MESH_RAM_VARIANT_ID,
					VariantIDHash = MESH_RAM_VARIANT_ID.AddressToHash(),
					Priority = MESH_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MaterialDTO),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<MeshDTO>(
					mesh,
					context),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<MeshDTO>(
					mesh,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MeshRAMAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<MeshRAMAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}