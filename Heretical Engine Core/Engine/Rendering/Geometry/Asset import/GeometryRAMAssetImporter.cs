using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryRAMAssetImporter : AssetImporter
	{
		public const string GEOMETRY_RAM_VARIANT_ID = "RAM geometry";

		public const int GEOMETRY_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly Geometry geometry;

		public GeometryRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			Geometry geometry,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.geometry = geometry;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, GeometryRAMAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = GEOMETRY_RAM_VARIANT_ID,
					VariantIDHash = GEOMETRY_RAM_VARIANT_ID.AddressToHash(),
					Priority = GEOMETRY_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(Geometry),
				},
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle(
					geometry),
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, GeometryRAMAssetImporter>(logger);

			progress?.Report(1f);

			return result;
		}
	}
}