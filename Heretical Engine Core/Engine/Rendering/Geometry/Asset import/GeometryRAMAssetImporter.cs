#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryRAMAssetImporter : AssetImporter
	{
		public const string GEOMETRY_RAM_VARIANT_ID = "RAM geometry";

		public const int GEOMETRY_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly Geometry geometry;

		public GeometryRAMAssetImporter(
			string resourceID,
			Geometry geometry,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.geometry = geometry;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<GeometryRAMAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, GeometryRAMAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = GEOMETRY_RAM_VARIANT_ID,
					VariantIDHash = GEOMETRY_RAM_VARIANT_ID.AddressToHash(),
					Priority = GEOMETRY_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(Geometry),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<Geometry>(
					geometry,
					context),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<Geometry>(
					geometry,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, GeometryRAMAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<GeometryRAMAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}