#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshRAMAssetImporter : AssetImporter
	{
		public const string MESH_RAM_VARIANT_ID = "Mesh DTO";

		public const int MESH_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly MeshDTO mesh;

		public MeshRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			MeshDTO mesh,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.mesh = mesh;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<MeshRAMAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, MeshRAMAssetImporter>(logger),
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
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle(
					mesh),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle(
					mesh),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MeshRAMAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<MeshRAMAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}