#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public class DefaultReadWriteAssetImporter<TAsset> : AssetImporter
	{
		private readonly string resourceID;

		private readonly TAsset readWriteAsset;

		public DefaultReadWriteAssetImporter(
			string resourceID,
			TAsset readWriteAsset,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.readWriteAsset = readWriteAsset;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<DefaultReadWriteAssetImporter<TAsset>>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, DefaultReadWriteAssetImporter<TAsset>>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = string.Empty,
					VariantIDHash = string.Empty.AddressToHash(),
					Priority = 0,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(TAsset),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentReadWriteResourceStorageHandle<TAsset>(
					readWriteAsset,
					context),
#else
				ResourceManagementFactory.BuildReadWriteResourceStorageHandle<TAsset>(
					preallocatedAsset,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, DefaultReadWriteAssetImporter<TAsset>>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<DefaultReadWriteAssetImporter<TAsset>>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}